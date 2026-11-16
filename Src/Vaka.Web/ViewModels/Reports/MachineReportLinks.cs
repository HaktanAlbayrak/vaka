namespace Vaka.Web.ViewModels.Reports;

public static class MachineReportLinks
{
    public static string For(int machineId, DateOnly? from, DateOnly? to, Services.CleaningTypeKind? kind)
    {
        var url = $"/reports/machine?MachineId={machineId}";
        if (from is { } start)
        {
            url += $"&From={start:yyyy-MM-dd}";
        }

        if (to is { } end)
        {
            url += $"&To={end:yyyy-MM-dd}";
        }

        return kind is { } value ? url + $"&Kind={(int)value}" : url;
    }
}
