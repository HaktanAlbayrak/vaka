using Vaka.Web.Domain.Entities;
using Vaka.Web.Services;

namespace Vaka.Web.Data.Seed;

public static class SettingSeed
{
    public static readonly SystemSetting[] Rows =
    {
        new() { Id = 1, Key = SettingKeys.SessionMinutes, Value = 30, Unit = "dakika" },
        new() { Id = 2, Key = SettingKeys.PinLength, Value = 4, Unit = "hane" },
        new() { Id = 3, Key = SettingKeys.PinAttemptLimit, Value = 5, Unit = "deneme" }
    };
}
