using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public interface IMachineRetirement
{
    Task<LookupResult<MachineRetireViewModel>> GetAsync(int id, int viewerUserId, CancellationToken cancellationToken);

    /// <summary>BR-35: engelleyen kayıt varsa ya da makine zaten kaldırılmışsa reddedilir. Makine silinmez; <c>IsActive</c> false, kaldırılma anı yazılır.</summary>
    Task<CommandResult> RetireAsync(int id, int viewerUserId, CancellationToken cancellationToken);
}
