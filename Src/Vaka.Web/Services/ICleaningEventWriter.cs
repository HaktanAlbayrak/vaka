namespace Vaka.Web.Services;

public sealed record EventDraft(int RecordId, string EventType, object Payload, int? ActorUserId, Guid? CommandId, DateTime OccurredAtUtc);

public interface ICleaningEventWriter
{
    /// <summary>
    /// Kaydın zincirine olayı ekler ve kaydeder: sıra = son sıra + 1; özet = HMAC(önceki özet, kanonik olay) (BR-32).
    /// Çağıran yazma kilitli işlemin içinde olmalıdır; olay durum değişikliğiyle aynı işlemde onaylanır.
    /// </summary>
    Task AppendAsync(EventDraft draft, CancellationToken cancellationToken);
}
