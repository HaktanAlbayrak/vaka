using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public static class StepQueries
{
    /// <summary>Adımın kayıt kimliği; adım yoksa boş. Zaman aşımını uygulamak için işlemden önce çağrılır.</summary>
    public static Task<int?> GetRecordIdAsync(this VakaDbContext db, int stepId, CancellationToken cancellationToken) =>
        db.RecordSteps
            .AsNoTracking()
            .Where(step => step.Id == stepId)
            .Select(step => (int?)step.Phase.CleaningRecordId)
            .FirstOrDefaultAsync(cancellationToken);

    /// <summary>Adımın kaydını faz, adım ve görevlileriyle izlemeli yükler; yazma işlemleri içindir. Adım yoksa boş.</summary>
    public static Task<CleaningRecord?> LoadRecordForStepAsync(this VakaDbContext db, int stepId, CancellationToken cancellationToken) =>
        db.CleaningRecords
            .Include(record => record.Phases).ThenInclude(phase => phase.ProcedurePhase)
            .Include(record => record.Phases).ThenInclude(phase => phase.Steps).ThenInclude(step => step.ProcedureStep)
            .Include(record => record.Phases).ThenInclude(phase => phase.Steps).ThenInclude(step => step.Participants).ThenInclude(participant => participant.WorkInterval)
            .AsSplitQuery()
            .FirstOrDefaultAsync(record => record.Phases.Any(phase => phase.Steps.Any(step => step.Id == stepId)), cancellationToken);

    /// <summary>Kaydın sıralı adımları: faz sırası, sonra adım sırası (BR-14). Faz ve adım yüklenmiş olmalıdır.</summary>
    public static List<RecordStep> OrderedSteps(this CleaningRecord record) =>
        [.. record.Phases
            .OrderBy(phase => phase.ProcedurePhase.OrderNo)
            .SelectMany(phase => phase.Steps.OrderBy(step => step.ProcedureStep.OrderNo))];

    /// <summary>Kaydın tüm molalarının aralıkları; açık mola bitişi boştur (hesaplayıcı kapanmamış molayı sayar ya da atlar).</summary>
    public static async Task<List<BreakSpan>> GetBreakSpansAsync(this VakaDbContext db, int recordId, CancellationToken cancellationToken)
    {
        var rows = await db.RecordBreaks
            .AsNoTracking()
            .Where(item => item.CleaningRecordId == recordId)
            .Select(item => new { item.StartedAtUtc, item.EndedAtUtc })
            .ToListAsync(cancellationToken);
        return [.. rows.Select(row => UtcMoments.Span(row.StartedAtUtc, row.EndedAtUtc))];
    }
}
