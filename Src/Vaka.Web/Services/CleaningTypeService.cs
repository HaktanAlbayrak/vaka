using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Services;

public sealed class CleaningTypeService : ICleaningTypeService
{
    private const string DuplicateCode = "Bu tip kodu kayıtlı.";

    private readonly VakaDbContext _db;
    private readonly ISystemSettingsReader _settings;
    private readonly IDurationCalculator _calculator;
    private readonly ICleaningTypeBlockProvider _blocks;
    private readonly IProcedureLinks _links;
    private readonly ITurkeyClock _clock;

    public CleaningTypeService(
        VakaDbContext db,
        ISystemSettingsReader settings,
        IDurationCalculator calculator,
        ICleaningTypeBlockProvider blocks,
        IProcedureLinks links,
        ITurkeyClock clock)
    {
        _db = db;
        _settings = settings;
        _calculator = calculator;
        _blocks = blocks;
        _links = links;
        _clock = clock;
    }

    public async Task<CleaningTypeListViewModel> ListAsync(CleaningTypeKind? kind, bool showInactive, CancellationToken cancellationToken)
    {
        var types = _db.CleaningTypes.AsNoTracking().Where(type => showInactive || type.IsActive);
        if (kind is not null)
        {
            types = types.Where(type => type.Kind == kind);
        }

        var rows = await types
            .OrderBy(type => type.Code)
            .Select(type => new
            {
                type.Id,
                type.Code,
                type.Name,
                type.Kind,
                type.IsActive,
                Current = type.Versions.Where(version => version.Status == ProcedureVersionStatus.Current).Select(version => (int?)version.VersionNo).FirstOrDefault(),
                Draft = type.Versions.Where(version => version.Status == ProcedureVersionStatus.Draft).Select(version => (int?)version.VersionNo).FirstOrDefault(),
                Machines = type.MachineLinks.Count()
            })
            .ToListAsync(cancellationToken);

        return new CleaningTypeListViewModel
        {
            Items = rows.Select(row => new CleaningTypeListItemViewModel
            {
                Id = row.Id,
                Code = row.Code,
                Name = row.Name,
                Kind = row.Kind,
                KindLabel = ProcedureLabels.Kind(row.Kind),
                IsActive = row.IsActive,
                CurrentVersionNo = row.Current,
                DraftVersionNo = row.Draft,
                MachineCount = row.Machines
            }).ToArray(),
            Kind = kind,
            KindOptions = ProcedureLabels.KindOptions(),
            ShowInactive = showInactive
        };
    }

    public async Task<LookupResult<CleaningTypeFormViewModel>> GetFormAsync(
        int? id,
        int viewerUserId,
        bool withBlockingRecords,
        CancellationToken cancellationToken)
    {
        var maxLock = await _settings.GetValueAsync(SettingKeys.MaxLockMinutes, cancellationToken);
        if (id is null)
        {
            return LookupResult<CleaningTypeFormViewModel>.Found(new CleaningTypeFormViewModel
            {
                KindOptions = ProcedureLabels.KindOptions(),
                MaxLockMinutes = maxLock
            });
        }

        var stored = await _db.CleaningTypes
            .AsNoTracking()
            .Where(type => type.Id == id)
            .Select(type => new { type.Id, type.Code, type.Name, type.Kind, type.DefaultLockMinutes, type.IsActive })
            .FirstOrDefaultAsync(cancellationToken);

        if (stored is null)
        {
            return LookupResult<CleaningTypeFormViewModel>.Missing();
        }

        return LookupResult<CleaningTypeFormViewModel>.Found(new CleaningTypeFormViewModel
        {
            Id = stored.Id,
            Code = stored.Code,
            Name = stored.Name,
            Kind = stored.Kind,
            KindLabel = ProcedureLabels.Kind(stored.Kind),
            KindOptions = ProcedureLabels.KindOptions(),
            KindLocked = true,
            DefaultLockMinutes = stored.DefaultLockMinutes,
            MaxLockMinutes = maxLock,
            IsActive = stored.IsActive,
            BlockingRecords = withBlockingRecords ? await _blocks.ListAsync(stored.Id, viewerUserId, cancellationToken) : []
        });
    }

    public async Task<LookupResult<CleaningTypeDetailViewModel>> GetDetailAsync(int id, CancellationToken cancellationToken)
    {
        var type = await _db.CleaningTypes
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new
            {
                item.Id,
                item.Code,
                item.Name,
                item.Kind,
                item.DefaultLockMinutes,
                item.IsActive,
                Machines = item.MachineLinks.Count(),
                Versions = item.Versions
                    .OrderByDescending(version => version.VersionNo)
                    .Select(version => new
                    {
                        version.Id,
                        version.VersionNo,
                        version.Status,
                        version.PublishedAtUtc,
                        PublishedBy = version.PublishedBy == null ? null : version.PublishedBy.FullName,
                        version.DocumentRef
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (type is null)
        {
            return LookupResult<CleaningTypeDetailViewModel>.Missing();
        }

        var draft = type.Versions.FirstOrDefault(version => version.Status == ProcedureVersionStatus.Draft);
        return LookupResult<CleaningTypeDetailViewModel>.Found(new CleaningTypeDetailViewModel
        {
            Id = type.Id,
            Code = type.Code,
            Name = type.Name,
            Kind = type.Kind,
            KindLabel = ProcedureLabels.Kind(type.Kind),
            DefaultLockMinutes = type.DefaultLockMinutes,
            IsActive = type.IsActive,
            MachineCount = type.Machines,
            CanMapMachines = type.Kind == CleaningTypeKind.Planned,
            Versions = type.Versions.Select(version => new ProcedureVersionItemViewModel
            {
                Id = version.Id,
                VersionNo = version.VersionNo,
                Status = version.Status,
                StatusLabel = ProcedureLabels.Status(version.Status),
                PublishedAtLocal = version.PublishedAtUtc is { } published ? _clock.ToLocal(published) : null,
                PublishedByName = version.Status == ProcedureVersionStatus.Draft ? null : version.PublishedBy ?? ProcedureLabels.SetupData,
                DocumentRef = version.DocumentRef,
                Url = _links.Version(version.Id, version.Status)
            }).ToArray(),
            DraftId = draft?.Id,
            CanCreateDraft = draft is null
        });
    }

    public async Task<CommandResult> CreateAsync(CleaningTypeRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<FieldError>();
        var code = DefinitionRules.CheckCode(request.Code, "Tip kodu", DefinitionRules.CodeMaxLength, true, errors);
        var name = DefinitionRules.CheckName(request.Name, "Tip adı", DefinitionRules.NameMaxLength, errors);
        if (request.Kind is null)
        {
            errors.Add(new FieldError("Kind", "Tür seçin."));
        }
        else
        {
            await CheckLockMinutesAsync(request.Kind.Value, request.DefaultLockMinutes, errors, cancellationToken);
        }

        if (errors.Count == 0 && await _db.CleaningTypes.AnyAsync(type => type.Code == code, cancellationToken))
        {
            errors.Add(new FieldError("Code", DuplicateCode));
        }

        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        var type = new CleaningType
        {
            Code = code,
            Name = name,
            Kind = request.Kind!.Value,
            DefaultLockMinutes = request.Kind == CleaningTypeKind.Planned ? request.DefaultLockMinutes : null,
            IsActive = request.IsActive
        };
        _db.CleaningTypes.Add(type);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected("Code", DuplicateCode);
        }

        return CommandResult.Success(type.Id);
    }

    public async Task<CommandResult> UpdateAsync(int id, int viewerUserId, CleaningTypeRequest request, CancellationToken cancellationToken)
    {
        var type = await _db.CleaningTypes.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (type is null)
        {
            return CommandResult.NotFound();
        }

        var errors = new List<FieldError>();
        var code = DefinitionRules.CheckCode(request.Code, "Tip kodu", DefinitionRules.CodeMaxLength, true, errors);
        var name = DefinitionRules.CheckName(request.Name, "Tip adı", DefinitionRules.NameMaxLength, errors);
        if (request.Kind is not null && request.Kind != type.Kind)
        {
            errors.Add(new FieldError("Kind", "Tür kayıttan sonra değiştirilemez."));
        }

        await CheckLockMinutesAsync(type.Kind, request.DefaultLockMinutes, errors, cancellationToken);
        if (errors.Count == 0
            && !string.Equals(code, type.Code, StringComparison.OrdinalIgnoreCase)
            && await _db.CleaningTypes.AnyAsync(other => other.Code == code && other.Id != id, cancellationToken))
        {
            errors.Add(new FieldError("Code", DuplicateCode));
        }

        if (errors.Count == 0 && type.IsActive && !request.IsActive
            && (await _blocks.ListAsync(id, viewerUserId, cancellationToken)).Length > 0)
        {
            errors.Add(new FieldError("IsActive", "Aktif ya da beklemedeki kaydı olan tip pasifleştirilemez."));
        }

        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        type.Code = code;
        type.Name = name;
        type.DefaultLockMinutes = type.Kind == CleaningTypeKind.Planned ? request.DefaultLockMinutes : null;
        type.IsActive = request.IsActive;
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected("Code", DuplicateCode);
        }

        return CommandResult.Success(id);
    }

    private async Task CheckLockMinutesAsync(CleaningTypeKind kind, int? minutes, List<FieldError> errors, CancellationToken cancellationToken)
    {
        const string field = "DefaultLockMinutes";
        if (kind == CleaningTypeKind.Emergency)
        {
            if (minutes is not null)
            {
                errors.Add(new FieldError(field, "Acil tipte kilit süresi girilmez."));
            }

            return;
        }

        var max = await _settings.GetValueAsync(SettingKeys.MaxLockMinutes, cancellationToken);
        if (minutes is null)
        {
            errors.Add(new FieldError(field, "Varsayılan kilit süresini girin."));
        }
        else if (!_calculator.IsLockWithinLimit(minutes.Value, max))
        {
            errors.Add(new FieldError(field, $"Varsayılan kilit süresi 1 ile {max} dakika arasında olmalı."));
        }
    }
}
