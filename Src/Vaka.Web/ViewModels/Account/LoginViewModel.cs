using System.ComponentModel.DataAnnotations;

namespace Vaka.Web.ViewModels.Account;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Sicil no girin.")]
    public string RegistryNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parola girin.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; set; }

    public string? StatusMessage { get; set; }
}
