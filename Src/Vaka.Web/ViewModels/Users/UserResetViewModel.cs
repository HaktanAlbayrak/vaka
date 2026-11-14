using System.ComponentModel.DataAnnotations;

namespace Vaka.Web.ViewModels.Users;

public sealed class UserResetViewModel
{
    [DataType(DataType.Password)]
    public string? TemporaryPassword { get; set; }

    public string? Pin { get; set; }
}
