using System.ComponentModel.DataAnnotations;

namespace Vaka.Web.ViewModels.Account;

public sealed class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Mevcut parolayı girin.")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni parolayı girin.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni parolayı tekrar girin.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Parolalar aynı değil.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
