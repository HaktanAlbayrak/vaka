using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Services;

public sealed class ProcedureVersionReader : IProcedureVersionReader
{
    public const string MediaAccept = "image/jpeg,image/png,image/webp,video/mp4";

    private readonly VakaDbContext _db;
    private readonly ISystemSettingsReader _settings;
    private readonly IDurationCalculator _calculator;
    private readonly ITurkeyClock _clock;
    private readonly IProcedureLinks _links;

    public ProcedureVersionReader(
        VakaDbContext db,
        ISystemSettingsReader settings,
        IDurationCalculator calculator,
        ITurkeyClock clock,
        IProcedureLinks links)
    {
        _db = db;
        _settings = settings;
        _calculator = calculator;
        _clock = clock;
        _links = links;
    }

    public async Task<LookupResult<ProcedureDraftViewModel>> GetDraftAsync(int id, CancellationToken cancellationToken)
    {
        var head = await _db.ProcedureVersions
            .AsNoTracking()
            .Where(version => version.Id == id)
            .Select(version => new
            {
                version.Id,
                version.Status,
                version.CleaningTypeId,
                TypeName = version.CleaningType.Name,
                version.CleaningType.Kind,
                version.VersionNo,
                version.MaterialRequired,
                version.DocumentRef
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (head is null)
        {
            return LookupResult<ProcedureDraftViewModel>.Missing();
        }

        if (head.Status != ProcedureVersionStatus.Draft)
        {
            return LookupResult<ProcedureDraftViewModel>.Refused();
        }

        var phases = await ReadPhasesAsync(id, cancellationToken);
        var blockers = DraftRules.GetPublishBlockers(
            _calculator,
            head.Kind == CleaningTypeKind.Planned,
            phases.Select(phase => (phase.OrderNo, phase.Steps.Count, phase.MinDurationSeconds)).ToArray());

        return LookupResult<ProcedureDraftViewModel>.Found(new ProcedureDraftViewModel
        {
            Id = head.Id,
            CleaningTypeId = head.CleaningTypeId,
            CleaningTypeName = head.TypeName,
            Kind = head.Kind,
            KindLabel = ProcedureLabels.Kind(head.Kind),
            VersionNo = head.VersionNo,
            MaterialRequired = head.MaterialRequired,
            DocumentRef = head.DocumentRef,
            Phases = phases.Select((phase, phaseIndex) => new DraftPhaseViewModel
            {
                Id = phase.Id,
                OrderNo = phase.OrderNo,
                Title = phase.Title,
                MinDurationMinutes = phase.MinDurationSeconds is { } seconds ? _calculator.SecondsToMinutes(seconds) : null,
                CanMoveUp = phaseIndex > 0,
                CanMoveDown = phaseIndex < phases.Count - 1,
                Steps = phase.Steps.Select((step, stepIndex) => new DraftStepViewModel
                {
                    Id = step.Id,
                    OrderNo = step.OrderNo,
                    Title = step.Title,
                    Description = step.Description,
                    CanMoveUp = stepIndex > 0,
                    CanMoveDown = stepIndex < phase.Steps.Count - 1,
                    Media = ToMedia(step.Media)
                }).ToArray()
            }).ToArray(),
            PublishBlockers = blockers,
            MaxPhotoSizeMb = await _settings.GetValueAsync(SettingKeys.MaxPhotoMb, cancellationToken),
            MaxVideoSizeMb = await _settings.GetValueAsync(SettingKeys.MaxVideoMb, cancellationToken),
            MediaAccept = MediaAccept
        });
    }

    public async Task<LookupResult<ProcedureVersionViewModel>> GetVersionAsync(int id, CancellationToken cancellationToken)
    {
        var head = await _db.ProcedureVersions
            .AsNoTracking()
            .Where(version => version.Id == id)
            .Select(version => new
            {
                version.Id,
                version.Status,
                version.CleaningTypeId,
                TypeName = version.CleaningType.Name,
                version.CleaningType.Kind,
                version.VersionNo,
                version.PublishedAtUtc,
                PublishedBy = version.PublishedBy == null ? null : version.PublishedBy.FullName,
                version.DocumentRef,
                version.MaterialRequired
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (head is null)
        {
            return LookupResult<ProcedureVersionViewModel>.Missing();
        }

        if (head.Status == ProcedureVersionStatus.Draft)
        {
            return LookupResult<ProcedureVersionViewModel>.Refused();
        }

        var phases = await ReadPhasesAsync(id, cancellationToken);
        return LookupResult<ProcedureVersionViewModel>.Found(new ProcedureVersionViewModel
        {
            Id = head.Id,
            CleaningTypeId = head.CleaningTypeId,
            CleaningTypeName = head.TypeName,
            Kind = head.Kind,
            KindLabel = ProcedureLabels.Kind(head.Kind),
            VersionNo = head.VersionNo,
            Status = head.Status,
            StatusLabel = ProcedureLabels.Status(head.Status),
            PublishedAtLocal = head.PublishedAtUtc is { } published ? _clock.ToLocal(published) : null,
            PublishedByName = head.PublishedBy ?? ProcedureLabels.SetupData,
            DocumentRef = head.DocumentRef,
            MaterialRequired = head.MaterialRequired,
            Phases = phases.Select(phase => new VersionPhaseViewModel
            {
                OrderNo = phase.OrderNo,
                Title = phase.Title,
                MinDurationMinutes = phase.MinDurationSeconds is { } seconds ? _calculator.SecondsToMinutes(seconds) : null,
                Steps = phase.Steps.Select(step => new VersionStepViewModel
                {
                    OrderNo = step.OrderNo,
                    Title = step.Title,
                    Description = step.Description,
                    Media = ToMedia(step.Media)
                }).ToArray()
            }).ToArray()
        });
    }

    private async Task<List<PhaseRow>> ReadPhasesAsync(int versionId, CancellationToken cancellationToken)
    {
        var rows = await _db.ProcedurePhases
            .AsNoTracking()
            .Where(phase => phase.ProcedureVersionId == versionId)
            .OrderBy(phase => phase.OrderNo)
            .Select(phase => new
            {
                phase.Id,
                phase.OrderNo,
                phase.Title,
                phase.MinDurationSeconds,
                Steps = phase.Steps
                    .OrderBy(step => step.OrderNo)
                    .Select(step => new
                    {
                        step.Id,
                        step.OrderNo,
                        step.Title,
                        step.Description,
                        Media = step.Media
                            .OrderBy(media => media.Id)
                            .Select(media => new MediaRow(media.Id, media.MediaType, media.OriginalFileName))
                            .ToList()
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return rows.Select(row => new PhaseRow(
            row.Id,
            row.OrderNo,
            row.Title,
            row.MinDurationSeconds,
            row.Steps.Select(step => new StepRow(step.Id, step.OrderNo, step.Title, step.Description, step.Media)).ToList())).ToList();
    }

    private StepMediaViewModel[] ToMedia(IEnumerable<MediaRow> media) =>
        media.Select(item => new StepMediaViewModel
        {
            Id = item.Id,
            MediaType = item.MediaType,
            MediaTypeLabel = ProcedureLabels.MediaType(item.MediaType),
            OriginalFileName = item.OriginalFileName,
            Url = _links.Media(item.Id)
        }).ToArray();

    private sealed record MediaRow(int Id, StepMediaType MediaType, string OriginalFileName);

    private sealed record StepRow(int Id, int OrderNo, string Title, string? Description, List<MediaRow> Media);

    private sealed record PhaseRow(int Id, int OrderNo, string Title, int? MinDurationSeconds, List<StepRow> Steps);
}
