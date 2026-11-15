using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class WriteTransactionRunner : IWriteTransactionRunner
{
    private const int SqliteBusy = 5;
    private const int SqliteLocked = 6;

    private readonly VakaDbContext _db;

    public WriteTransactionRunner(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<CommandResult> RunAsync(Func<CancellationToken, Task<CommandResult>> action, CancellationToken cancellationToken)
    {
        try
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            _db.ChangeTracker.Clear();

            var result = await action(cancellationToken);
            if (result.Succeeded || result.IsPinFailure)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return result;
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode is SqliteBusy or SqliteLocked)
        {
            return CommandResult.Rejected(string.Empty, "Sistem şu an meşgul. İşlemi birkaç saniye sonra yeniden deneyin.");
        }
    }
}
