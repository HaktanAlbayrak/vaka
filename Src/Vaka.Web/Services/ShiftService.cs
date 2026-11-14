using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class ShiftService : IShiftService
{
    private readonly VakaDbContext _db;
    private readonly IDurationCalculator _calculator;

    public ShiftService(VakaDbContext db, IDurationCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    public async Task<ShiftListViewModel> ListAsync(int? facilityId, CancellationToken cancellationToken)
    {
        var facilities = await _db.Facilities
            .AsNoTracking()
            .OrderBy(facility => facility.Code)
            .Select(facility => new FacilityOptionViewModel { Id = facility.Id, Name = facility.Name })
            .ToArrayAsync(cancellationToken);

        var selected = facilities.FirstOrDefault(facility => facility.Id == facilityId) ?? facilities.FirstOrDefault();
        var model = new ShiftListViewModel { FacilityId = selected?.Id ?? 0, Facilities = facilities };
        if (selected is null)
        {
            return model;
        }

        var shifts = await _db.Shifts
            .AsNoTracking()
            .Where(shift => shift.FacilityId == selected.Id)
            .OrderBy(shift => shift.StartTime)
            .ToListAsync(cancellationToken);

        model.Items = shifts
            .Select(shift => new ShiftListItemViewModel
            {
                Id = shift.Id,
                Name = shift.Name,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                EndsNextDay = _calculator.EndsNextDay(shift.StartTime, shift.EndTime),
                IsActive = shift.IsActive
            })
            .ToArray();
        return model;
    }

    public async Task<LookupResult<ShiftFormViewModel>> GetFormAsync(int? id, int? facilityId, CancellationToken cancellationToken)
    {
        if (id is not null)
        {
            var form = await _db.Shifts
                .AsNoTracking()
                .Where(shift => shift.Id == id)
                .Select(shift => new ShiftFormViewModel
                {
                    Id = shift.Id,
                    FacilityId = shift.FacilityId,
                    FacilityName = shift.Facility.Name,
                    Name = shift.Name,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    IsActive = shift.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

            return form is null ? LookupResult<ShiftFormViewModel>.Missing() : LookupResult<ShiftFormViewModel>.Found(form);
        }

        var facility = await _db.Facilities
            .AsNoTracking()
            .Where(item => item.Id == facilityId)
            .Select(item => new ShiftFormViewModel { FacilityId = item.Id, FacilityName = item.Name })
            .FirstOrDefaultAsync(cancellationToken);

        return facility is null ? LookupResult<ShiftFormViewModel>.Missing() : LookupResult<ShiftFormViewModel>.Found(facility);
    }

    public async Task<CommandResult> CreateAsync(int facilityId, ShiftRequest request, CancellationToken cancellationToken)
    {
        var facility = await _db.Facilities.AsNoTracking().FirstOrDefaultAsync(item => item.Id == facilityId, cancellationToken);
        if (facility is null)
        {
            return CommandResult.NotFound();
        }

        var errors = new List<FieldError>();
        if (!facility.IsActive)
        {
            errors.Add(new FieldError(string.Empty, "Pasif tesise vardiya eklenemez."));
        }

        var shift = new Shift { FacilityId = facilityId };
        await ApplyAsync(shift, request, errors, cancellationToken);
        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        _db.Shifts.Add(shift);
        await _db.SaveChangesAsync(cancellationToken);
        return CommandResult.Success(facilityId);
    }

    public async Task<CommandResult> UpdateAsync(int id, ShiftRequest request, CancellationToken cancellationToken)
    {
        var shift = await _db.Shifts.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (shift is null)
        {
            return CommandResult.NotFound();
        }

        var errors = new List<FieldError>();
        await ApplyAsync(shift, request, errors, cancellationToken);
        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return CommandResult.Success(shift.FacilityId);
    }

    /// <summary>K-38: doğrulanan değerler vardiyaya yazılır; kayıt aktifse aynı tesisteki diğer aktif vardiyalarla çakışma denetlenir (<see cref="IDurationCalculator.DoShiftsOverlap"/>).</summary>
    private async Task ApplyAsync(Shift shift, ShiftRequest request, List<FieldError> errors, CancellationToken cancellationToken)
    {
        var name = DefinitionRules.CheckName(request.Name, "Vardiya adı", DefinitionRules.ShiftNameMaxLength, errors);
        if (request.StartTime is null)
        {
            errors.Add(new FieldError("StartTime", "Başlangıç saatini girin."));
        }

        if (request.EndTime is null)
        {
            errors.Add(new FieldError("EndTime", "Bitiş saatini girin."));
        }

        if (request.StartTime is { } start && request.EndTime is { } end)
        {
            if (start == end)
            {
                errors.Add(new FieldError("EndTime", "Bitiş saati başlangıç saatiyle aynı olamaz."));
            }
            else if (request.IsActive)
            {
                var candidate = new ShiftWindow(shift.Id, start, end);
                var others = await _db.Shifts
                    .AsNoTracking()
                    .Where(other => other.FacilityId == shift.FacilityId && other.IsActive && other.Id != shift.Id)
                    .ToListAsync(cancellationToken);

                var conflict = others.FirstOrDefault(other => _calculator.DoShiftsOverlap(candidate, new ShiftWindow(other.Id, other.StartTime, other.EndTime)));
                if (conflict is not null)
                {
                    errors.Add(new FieldError(string.Empty, $"Bu aralık \"{conflict.Name}\" vardiyasıyla çakışıyor."));
                }
            }
        }

        if (errors.Count > 0)
        {
            return;
        }

        shift.Name = name;
        shift.StartTime = request.StartTime!.Value;
        shift.EndTime = request.EndTime!.Value;
        shift.IsActive = request.IsActive;
    }
}
