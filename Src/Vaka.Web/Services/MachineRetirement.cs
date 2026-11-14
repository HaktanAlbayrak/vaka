using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class MachineRetirement : IMachineRetirement
{
    private readonly VakaDbContext _db;
    private readonly IMachineBlockProvider _blocks;
    private readonly TimeProvider _time;
    private readonly ITurkeyClock _clock;

    public MachineRetirement(VakaDbContext db, IMachineBlockProvider blocks, TimeProvider time, ITurkeyClock clock)
    {
        _db = db;
        _blocks = blocks;
        _time = time;
        _clock = clock;
    }

    public async Task<LookupResult<MachineRetireViewModel>> GetAsync(int id, int viewerUserId, CancellationToken cancellationToken)
    {
        var machine = await _db.Machines
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new { item.Id, item.Code, item.Name, item.RetiredAtUtc })
            .FirstOrDefaultAsync(cancellationToken);

        if (machine is null)
        {
            return LookupResult<MachineRetireViewModel>.Missing();
        }

        var blocking = await _blocks.ListAsync(id, viewerUserId, cancellationToken);
        return LookupResult<MachineRetireViewModel>.Found(new MachineRetireViewModel
        {
            MachineId = machine.Id,
            Code = machine.Code,
            Name = machine.Name,
            BlockingRecords = blocking,
            CanRetire = machine.RetiredAtUtc is null && blocking.Length == 0,
            RetiredAtLocal = machine.RetiredAtUtc is { } retired ? _clock.ToLocal(retired) : null
        });
    }

    public async Task<CommandResult> RetireAsync(int id, int viewerUserId, CancellationToken cancellationToken)
    {
        var machine = await _db.Machines.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (machine is null)
        {
            return CommandResult.NotFound();
        }

        if (machine.RetiredAtUtc is not null)
        {
            return CommandResult.Rejected(string.Empty, "Makine zaten kullanımdan kaldırılmış.");
        }

        if ((await _blocks.ListAsync(id, viewerUserId, cancellationToken)).Length > 0)
        {
            return CommandResult.Rejected(string.Empty, "Aktif ya da beklemedeki kaydı olan makine kullanımdan kaldırılamaz.");
        }

        machine.IsActive = false;
        machine.RetiredAtUtc = _time.GetUtcNow().UtcDateTime;
        await _db.SaveChangesAsync(cancellationToken);
        return CommandResult.Success(id);
    }
}
