using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class MaterialEntryResolver : IMaterialEntryResolver
{
    private readonly ILotCatalog _lots;

    public MaterialEntryResolver(ILotCatalog lots)
    {
        _lots = lots;
    }

    public async Task<MaterialEntryViewModel[]> ResolveAsync(IReadOnlyList<MaterialEntryViewModel> posted, MaterialEdits? edits, CancellationToken cancellationToken)
    {
        var entries = posted.Select(item => (item.MaterialLotId, item.ExpiredReason)).ToList();
        if (edits is not null)
        {
            if (edits.RemoveIndex is { } index && index >= 0 && index < entries.Count)
            {
                entries.RemoveAt(index);
            }

            if (edits.AddLotId is { } lotId && entries.All(item => item.MaterialLotId != lotId))
            {
                entries.Add((lotId, edits.AddExpiredReason));
            }
        }

        var lots = (await _lots.GetAsync(entries.Select(item => item.MaterialLotId).ToArray(), cancellationToken))
            .ToDictionary(lot => lot.Id);

        return [.. entries.Where(item => lots.ContainsKey(item.MaterialLotId)).Select(item =>
        {
            var lot = lots[item.MaterialLotId];
            return new MaterialEntryViewModel
            {
                MaterialLotId = lot.Id,
                ExpiredReason = item.ExpiredReason,
                MaterialCode = lot.MaterialCode,
                MaterialName = lot.MaterialName,
                LotNo = lot.LotNo,
                ExpiryDate = lot.ExpiryDate,
                IsExpired = lot.IsExpired
            };
        })];
    }
}
