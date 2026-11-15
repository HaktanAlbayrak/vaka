using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Vaka.Web.Services;

public static class ConstraintViolations
{
    private const int SqliteConstraint = 19;

    /// <summary>
    /// Kayıt açarken kısmi benzersiz index ihlali mi: makine kilidi (BR-23), sahip kilidi (BR-37) ya da kayıt no. Ayrım SQLite iletisindeki
    /// kolon adından yapılır, ileti dili değerlendirilmez. Eşzamanlı istekte kaybeden taraf bu ihlali alır.
    /// </summary>
    public static bool IsRecordOpenConflict(DbUpdateException exception) =>
        exception.InnerException is SqliteException { SqliteErrorCode: SqliteConstraint } inner
        && (inner.Message.Contains("CleaningRecords.MachineId", StringComparison.Ordinal)
            || inner.Message.Contains("CleaningRecords.OwnerUserId", StringComparison.Ordinal)
            || inner.Message.Contains("CleaningRecords.RecordNo", StringComparison.Ordinal));
}
