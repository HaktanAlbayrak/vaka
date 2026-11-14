namespace Vaka.Web.ViewModels.Users;

public sealed class UserListViewModel
{
    public UserListItemViewModel[] Items { get; set; } = [];

    public string? Query { get; set; }

    public UserStatusFilter Status { get; set; } = UserStatusFilter.Active;

    public int Page { get; set; } = 1;

    public int PageSize { get; set; }

    public int Total { get; set; }

    public string? StatusMessage { get; set; }
}
