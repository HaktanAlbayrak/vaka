namespace Vaka.Web.Services;

public enum CommandStatus
{
    Succeeded,
    Rejected,
    Forbidden,
    NotFound,
    PinLocked
}

public sealed record FieldError(string Field, string Message);

public sealed class CommandResult
{
    private static readonly FieldError[] NoErrors = [];

    private CommandResult(CommandStatus status, IReadOnlyList<FieldError> errors, int? entityId)
    {
        Status = status;
        Errors = errors;
        EntityId = entityId;
    }

    public CommandStatus Status { get; }

    /// <summary>Alan adı boşsa hata alana bağlı olmayan iş kuralı reddidir.</summary>
    public IReadOnlyList<FieldError> Errors { get; }

    public int? EntityId { get; }

    public bool Succeeded => Status == CommandStatus.Succeeded;

    public static CommandResult Success(int? entityId = null) => new(CommandStatus.Succeeded, NoErrors, entityId);

    public static CommandResult Rejected(string field, string message) =>
        new(CommandStatus.Rejected, [new FieldError(field, message)], null);

    public static CommandResult Rejected(IReadOnlyList<FieldError> errors) =>
        new(CommandStatus.Rejected, errors, null);

    public static CommandResult Forbidden() => new(CommandStatus.Forbidden, NoErrors, null);

    public static CommandResult NotFound() => new(CommandStatus.NotFound, NoErrors, null);

    public static CommandResult PinLocked(string field = "") =>
        new(CommandStatus.PinLocked, [new FieldError(field, "PIN kilitlendi.")], null);

    public static CommandResult FromPin(PinCheckStatus status, string field = "") => status switch
    {
        PinCheckStatus.Wrong => Rejected(field, "PIN hatalı."),
        PinCheckStatus.Locked => Rejected(field, "PIN kilitli. PIN'inizi sıfırlatmak için yöneticinize başvurun."),
        PinCheckStatus.NotSet => Rejected(field, "PIN belirlenmemiş."),
        PinCheckStatus.LockedNow => PinLocked(field),
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Kabul edilen PIN hata üretmez.")
    };
}

public sealed record LookupResult<T>(CommandStatus Status, T? Value) where T : class
{
    public static LookupResult<T> Found(T value) => new(CommandStatus.Succeeded, value);

    public static LookupResult<T> Missing() => new(CommandStatus.NotFound, null);

    public static LookupResult<T> OutOfScope() => new(CommandStatus.Forbidden, null);
}
