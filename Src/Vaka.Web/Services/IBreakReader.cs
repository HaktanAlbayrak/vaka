using Vaka.Web.ViewModels.Breaks;

namespace Vaka.Web.Services;

public interface IBreakReader
{
    /// <summary>Kişinin açık molası var mı (BR-39 kişi kilidi); molada olan kişinin yazma istekleri reddedilir.</summary>
    Task<bool> IsOnBreakAsync(int userId, CancellationToken cancellationToken);

    /// <summary>Kişinin açık molasının mola ekranı modeli; açık mola yoksa boş. <c>CommandId</c> her çizimde yenidir.</summary>
    Task<CurrentBreakViewModel?> GetCurrentAsync(int userId, CancellationToken cancellationToken);
}

public static class BreakGuard
{
    /// <summary>Molada olan kişi için <see cref="CommandResult.OnBreak"/>, değilse boş döner.</summary>
    public static async Task<CommandResult?> CheckAsync(this IBreakReader breaks, int userId, CancellationToken cancellationToken) =>
        await breaks.IsOnBreakAsync(userId, cancellationToken) ? CommandResult.OnBreak() : null;
}
