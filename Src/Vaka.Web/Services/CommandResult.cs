namespace Vaka.Web.Services;

public enum CommandStatus
{
    Succeeded,
    Rejected,
    Forbidden,
    NotFound,
    PinLocked,
    NotEditable
}

public sealed record FieldError(string Field, string Message);

public sealed class CommandResult
{
    private static readonly FieldError[] NoErrors = [];

    private CommandResult(CommandStatus status, IReadOnlyList<FieldError> errors, int? entityId, bool isPinFailure = false)
    {
        Status = status;
        Errors = errors;
        EntityId = entityId;
        IsPinFailure = isPinFailure;
    }

    public CommandStatus Status { get; }

    /// <summary>Alan adı boşsa hata alana bağlı olmayan iş kuralı reddidir.</summary>
    public IReadOnlyList<FieldError> Errors { get; }

    public int? EntityId { get; }

    public bool Succeeded => Status == CommandStatus.Succeeded;

    /// <summary>PIN reddi: hatalı deneme sayacı yazıldığı için işlem geri alınmaz, yalnız sayaç kalır (BR-48).</summary>
    public bool IsPinFailure { get; }

    public static CommandResult Success(int? entityId = null) => new(CommandStatus.Succeeded, NoErrors, entityId);

    public static CommandResult Rejected(string field, string message) =>
        new(CommandStatus.Rejected, [new FieldError(field, message)], null);

    public static CommandResult Rejected(IReadOnlyList<FieldError> errors) =>
        new(CommandStatus.Rejected, errors, null);

    /// <summary>Düzenleyici işlemlerinde reddin hangi versiyonda olduğu <see cref="EntityId"/> ile taşınır; yeniden çizim bunu kullanır.</summary>
    public static CommandResult Rejected(int entityId, IReadOnlyList<FieldError> errors) =>
        new(CommandStatus.Rejected, errors, entityId);

    public static CommandResult Rejected(int entityId, string field, string message) =>
        new(CommandStatus.Rejected, [new FieldError(field, message)], entityId);

    public static CommandResult Forbidden() => new(CommandStatus.Forbidden, NoErrors, null);

    /// <summary>Yayınlanmış versiyona yönelik düzenleme isteği (BR-04); <see cref="EntityId"/> versiyon kimliğidir.</summary>
    public static CommandResult NotEditable(int versionId) =>
        new(CommandStatus.NotEditable, [new FieldError(string.Empty, "Yayınlanmış versiyon değiştirilemez. Değişiklik için yeni taslak açın.")], versionId);

    public static CommandResult NotFound() => new(CommandStatus.NotFound, NoErrors, null);

    public static CommandResult PinLocked(string field = "") =>
        new(CommandStatus.PinLocked, [new FieldError(field, "PIN kilitlendi.")], null, isPinFailure: true);

    public static CommandResult FromPin(PinCheckStatus status, string field = "") => status switch
    {
        PinCheckStatus.Wrong => PinRejected(field, "PIN hatalı."),
        PinCheckStatus.Locked => PinRejected(field, "PIN kilitli. PIN'inizi sıfırlatmak için yöneticinize başvurun."),
        PinCheckStatus.NotSet => PinRejected(field, "PIN belirlenmemiş."),
        PinCheckStatus.LockedNow => PinLocked(field),
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Kabul edilen PIN hata üretmez.")
    };

    private static CommandResult PinRejected(string field, string message) =>
        new(CommandStatus.Rejected, [new FieldError(field, message)], null, isPinFailure: true);
}

public sealed record LookupResult<T>(CommandStatus Status, T? Value) where T : class
{
    public static LookupResult<T> Found(T value) => new(CommandStatus.Succeeded, value);

    public static LookupResult<T> Missing() => new(CommandStatus.NotFound, null);

    /// <summary>Kayıt var ama bu ekranda açılmaz (ör. acil tipin makine eşlemesi, taslak olmayan versiyonun düzenleyicisi).</summary>
    public static LookupResult<T> Refused() => new(CommandStatus.Rejected, null);

    public static LookupResult<T> OutOfScope() => new(CommandStatus.Forbidden, null);
}
