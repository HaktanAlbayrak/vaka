using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class StepLocator : IStepLocator
{
    private readonly VakaDbContext _db;

    public StepLocator(VakaDbContext db)
    {
        _db = db;
    }

    public Task<int?> GetRecordIdAsync(int stepId, CancellationToken cancellationToken) => _db.GetRecordIdAsync(stepId, cancellationToken);
}
