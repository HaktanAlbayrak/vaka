using System.ComponentModel.DataAnnotations;

namespace Vaka.Web.ViewModels.Account;

public sealed class ChangePinViewModel
{
    [Required(ErrorMessage = "Mevcut PIN'i girin.")]
    public string CurrentPin { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni PIN'i girin.")]
    public string NewPin { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni PIN'i tekrar girin.")]
    [Compare(nameof(NewPin), ErrorMessage = "PIN'ler aynı değil.")]
    public string ConfirmPin { get; set; } = string.Empty;
}
