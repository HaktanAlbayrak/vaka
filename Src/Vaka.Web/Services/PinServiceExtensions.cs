namespace Vaka.Web.Services;

public static class PinServiceExtensions
{
    /// <summary>PIN kabul edilirse boş, aksi halde <c>Pin</c> alanına bağlı ret döner. Boş PIN sayaç artırmaz.</summary>
    public static async Task<CommandResult?> ConfirmAsync(this IPinService pins, int actorId, string? pin, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(pin))
        {
            return CommandResult.Rejected("Pin", "PIN girin.");
        }

        var status = await pins.VerifyAsync(actorId, pin, cancellationToken);
        return status == PinCheckStatus.Accepted ? null : CommandResult.FromPin(status, "Pin");
    }
}
