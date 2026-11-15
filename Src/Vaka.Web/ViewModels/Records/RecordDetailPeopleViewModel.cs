namespace Vaka.Web.ViewModels.Records;

public sealed class RecordDetailPeopleViewModel
{
    public string OwnerName { get; set; } = string.Empty;

    public string OwnerRegistryNo { get; set; } = string.Empty;

    public string? FirstStepStartedByName { get; set; }

    /// <summary>Kayıt sahibi dışında herhangi bir adımın görevlisi olmuş kişiler.</summary>
    public RecordHelperViewModel[] Helpers { get; set; } = [];
}
