using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class ProcedureVersionPublisher : IProcedureVersionPublisher
{
    private const int SqliteBusy = 5;
    private const int SqliteLocked = 6;
    private const string DraftExists = "Bu tipin taslağı zaten var.";

    private readonly VakaDbContext _db;
    private readonly IPinService _pins;
    private readonly IDurationCalculator _calculator;
    private readonly TimeProvider _time;

    public ProcedureVersionPublisher(VakaDbContext db, IPinService pins, IDurationCalculator calculator, TimeProvider time)
    {
        _db = db;
        _pins = pins;
        _calculator = calculator;
        _time = time;
    }

    public async Task<CommandResult> CreateDraftAsync(int cleaningTypeId, CancellationToken cancellationToken)
    {
        if (!await _db.CleaningTypes.AnyAsync(type => type.Id == cleaningTypeId, cancellationToken))
        {
            return CommandResult.NotFound();
        }

        var versions = await _db.ProcedureVersions
            .AsNoTracking()
            .Where(version => version.CleaningTypeId == cleaningTypeId)
            .Select(version => new { version.VersionNo, version.Status })
            .ToListAsync(cancellationToken);

        if (versions.Any(version => version.Status == ProcedureVersionStatus.Draft))
        {
            return CommandResult.Rejected(cleaningTypeId, string.Empty, DraftExists);
        }

        var current = await _db.ProcedureVersions
            .AsNoTracking()
            .Include(version => version.Phases.OrderBy(phase => phase.OrderNo))
            .ThenInclude(phase => phase.Steps.OrderBy(step => step.OrderNo))
            .ThenInclude(step => step.Media)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                version => version.CleaningTypeId == cleaningTypeId && version.Status == ProcedureVersionStatus.Current,
                cancellationToken);

        var draft = new ProcedureVersion
        {
            CleaningTypeId = cleaningTypeId,
            VersionNo = versions.Count == 0 ? 1 : versions.Max(version => version.VersionNo) + 1,
            Status = ProcedureVersionStatus.Draft,
            MaterialRequired = current?.MaterialRequired ?? false,
            DocumentRef = current?.DocumentRef
        };

        foreach (var phase in current?.Phases ?? [])
        {
            draft.Phases.Add(new ProcedurePhase
            {
                OrderNo = phase.OrderNo,
                Title = phase.Title,
                MinDurationSeconds = phase.MinDurationSeconds,
                Steps = phase.Steps.Select(step => new ProcedureStep
                {
                    OrderNo = step.OrderNo,
                    Title = step.Title,
                    Description = step.Description,
                    Media = step.Media.Select(media => new StepMedia
                    {
                        MediaType = media.MediaType,
                        Sha256 = media.Sha256,
                        OriginalFileName = media.OriginalFileName,
                        ContentType = media.ContentType,
                        SizeBytes = media.SizeBytes
                    }).ToList()
                }).ToList()
            });
        }

        _db.ProcedureVersions.Add(draft);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected(cleaningTypeId, string.Empty, DraftExists);
        }

        return CommandResult.Success(draft.Id);
    }

    public async Task<PublishOutcome> PublishAsync(int versionId, int actorId, string? pin, CancellationToken cancellationToken)
    {
        var version = await _db.ProcedureVersions
            .AsNoTracking()
            .Where(item => item.Id == versionId)
            .Select(item => new { item.Id, item.Status, item.CleaningTypeId, item.VersionNo, item.CleaningType.Kind })
            .FirstOrDefaultAsync(cancellationToken);

        if (version is null)
        {
            return Failed(CommandResult.NotFound());
        }

        if (version.Status != ProcedureVersionStatus.Draft)
        {
            return Failed(CommandResult.NotEditable(versionId));
        }

        var blockers = await GetBlockersAsync(versionId, version.Kind, cancellationToken);
        if (blockers.Length > 0)
        {
            return Failed(CommandResult.Rejected(versionId, blockers.Select(text => new FieldError(string.Empty, text)).ToArray()));
        }

        if (string.IsNullOrEmpty(pin))
        {
            return Failed(CommandResult.Rejected(versionId, "Pin", "PIN girin."));
        }

        try
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            _db.ChangeTracker.Clear();

            var status = await _pins.VerifyAsync(actorId, pin, cancellationToken);
            if (status != PinCheckStatus.Accepted)
            {
                await transaction.CommitAsync(cancellationToken);
                return Failed(CommandResult.FromPin(status, "Pin"));
            }

            var result = await SwapCurrentAsync(versionId, version.CleaningTypeId, actorId, cancellationToken);
            if (!result.Succeeded)
            {
                return Failed(result);
            }

            await transaction.CommitAsync(cancellationToken);
            return new PublishOutcome(CommandResult.Success(version.CleaningTypeId), version.CleaningTypeId, version.VersionNo);
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode is SqliteBusy or SqliteLocked)
        {
            return Failed(CommandResult.Rejected(versionId, string.Empty, "Sistem şu an meşgul. İşlemi birkaç saniye sonra yeniden deneyin."));
        }
    }

    private async Task<CommandResult> SwapCurrentAsync(int versionId, int cleaningTypeId, int actorId, CancellationToken cancellationToken)
    {
        var draft = await _db.ProcedureVersions.FirstAsync(item => item.Id == versionId, cancellationToken);
        if (draft.Status != ProcedureVersionStatus.Draft)
        {
            return CommandResult.NotEditable(versionId);
        }

        var previous = await _db.ProcedureVersions
            .FirstOrDefaultAsync(item => item.CleaningTypeId == cleaningTypeId && item.Status == ProcedureVersionStatus.Current, cancellationToken);

        try
        {
            if (previous is not null)
            {
                previous.Status = ProcedureVersionStatus.Archived;
                await _db.SaveChangesAsync(cancellationToken);
            }

            draft.Status = ProcedureVersionStatus.Current;
            draft.PublishedAtUtc = _time.GetUtcNow().UtcDateTime;
            draft.PublishedByUserId = actorId;
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected(versionId, string.Empty, "Versiyon yayınlanamadı. Sayfayı yenileyip yeniden deneyin.");
        }

        return CommandResult.Success(versionId);
    }

    private async Task<string[]> GetBlockersAsync(int versionId, CleaningTypeKind kind, CancellationToken cancellationToken)
    {
        var phases = await _db.ProcedurePhases
            .AsNoTracking()
            .Where(phase => phase.ProcedureVersionId == versionId)
            .Select(phase => new { phase.OrderNo, StepCount = phase.Steps.Count(), phase.MinDurationSeconds })
            .ToListAsync(cancellationToken);

        return DraftRules.GetPublishBlockers(
            _calculator,
            kind == CleaningTypeKind.Planned,
            phases.Select(phase => (phase.OrderNo, phase.StepCount, phase.MinDurationSeconds)).ToArray());
    }

    private static PublishOutcome Failed(CommandResult result) => new(result, 0, 0);
}
