using System.ComponentModel.DataAnnotations;

namespace Vaka.Web.ViewModels.Account;

public sealed class LoginViewModel
{
    public LoginMode Mode { get; set; } = LoginMode.Pin;

    [Required(ErrorMessage = "Sicil no girin.")]
    public string RegistryNo { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [StringLength(256)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [StringLength(256)]
    public string? Pin { get; set; }

    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; set; }

    public string? StatusMessage { get; set; }

    public DateTime? LockedUntilLocal { get; set; }
}
