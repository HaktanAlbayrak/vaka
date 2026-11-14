using System.ComponentModel.DataAnnotations;

namespace Vaka.Web.ViewModels.Account;

public sealed class FirstLoginViewModel
{
    public string FullName { get; set; } = string.Empty;

    public bool MustChangePassword { get; set; }

    public bool MustSetPin { get; set; }

    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Parolalar aynı değil.")]
    public string? ConfirmPassword { get; set; }

    public string? NewPin { get; set; }

    [Compare(nameof(NewPin), ErrorMessage = "PIN'ler aynı değil.")]
    public string? ConfirmPin { get; set; }

    public int PinLength { get; set; }
}
