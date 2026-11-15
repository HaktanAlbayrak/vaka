using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordListViewModel
{
    public RecordListItemViewModel[] Items { get; set; } = [];

    public RecordListFilterViewModel Filter { get; set; } = new();

    public SelectOptionViewModel[] Facilities { get; set; } = [];

    public SelectOptionViewModel[] Lines { get; set; } = [];

    public SelectOptionViewModel[] Machines { get; set; } = [];

    /// <summary>Id temizlik türü değeri (<c>CleaningTypeKind</c>), Label görünür adı.</summary>
    public SelectOptionViewModel[] KindOptions { get; set; } = [];

    /// <summary>Id kayıt durumu değeri (<c>RecordStatus</c>), Label görünür adı.</summary>
    public SelectOptionViewModel[] StatusOptions { get; set; } = [];

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int Total { get; set; }

    /// <summary>En az 1; sayfalama sunucuda, ekran yalnız bağlantı çizer.</summary>
    public int PageCount { get; set; }
}
