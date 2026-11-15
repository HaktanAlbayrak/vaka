using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class ProcessedCommands : IProcessedCommands
{
    private readonly VakaDbContext _db;

    public ProcessedCommands(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<CommandResult?> ReplayAsync(Guid commandId, int userId, CancellationToken cancellationToken)
    {
        var key = commandId.ToString("D");
        var stored = await _db.ProcessedCommands
            .AsNoTracking()
            .Where(item => item.CommandId == key)
            .Select(item => new { item.UserId, item.ResultJson })
            .FirstOrDefaultAsync(cancellationToken);

        if (stored is null)
        {
            return null;
        }

        if (stored.UserId != userId)
        {
            return CommandResult.Rejected(string.Empty, "Bu işlem kimliği başka bir oturuma ait. Sayfayı yenileyin.");
        }

        using var document = JsonDocument.Parse(stored.ResultJson);
        return CommandResult.Success(document.RootElement.GetProperty("recordId").GetInt32());
    }

    public void Remember(Guid commandId, string commandType, int userId, int recordId, DateTime createdAtUtc) =>
        _db.ProcessedCommands.Add(new ProcessedCommand
        {
            CommandId = commandId.ToString("D"),
            CommandType = commandType,
            UserId = userId,
            ResultJson = JsonSerializer.Serialize(new { recordId }),
            CreatedAtUtc = createdAtUtc
        });
}
