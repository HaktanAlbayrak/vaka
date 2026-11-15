using System.Collections.Concurrent;

namespace Vaka.Web.Authorization;

public sealed class SessionRevocations : ISessionRevocations
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _revoked = new();
    private readonly TimeProvider _time;

    public SessionRevocations(TimeProvider time)
    {
        _time = time;
    }

    public void Revoke(string sessionId, int sessionMinutes)
    {
        var now = _time.GetUtcNow();
        foreach (var entry in _revoked.Where(entry => entry.Value <= now))
        {
            _revoked.TryRemove(entry.Key, out _);
        }

        _revoked[sessionId] = now.AddMinutes(sessionMinutes);
    }

    public bool IsRevoked(string sessionId) =>
        _revoked.TryGetValue(sessionId, out var until) && until > _time.GetUtcNow();
}
