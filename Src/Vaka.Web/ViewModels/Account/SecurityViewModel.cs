namespace Vaka.Web.ViewModels.Account;

public sealed class SecurityViewModel
{
    public int PinLength { get; set; }

    public string? StatusMessage { get; set; }

    public ChangePasswordViewModel Password { get; set; } = new();

    public ChangePinViewModel Pin { get; set; } = new();
}
