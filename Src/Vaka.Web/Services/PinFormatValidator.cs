namespace Vaka.Web.Services;

public sealed class PinFormatValidator : IPinFormatValidator
{
    private readonly ISystemSettingsReader _settings;

    public PinFormatValidator(ISystemSettingsReader settings)
    {
        _settings = settings;
    }

    public async Task<string?> ValidateNewPinAsync(string? pin, CancellationToken cancellationToken)
    {
        var length = await _settings.GetValueAsync(SettingKeys.PinLength, cancellationToken);

        if (string.IsNullOrEmpty(pin))
        {
            return "PIN girin.";
        }

        if (!pin.All(char.IsAsciiDigit))
        {
            return "PIN yalnız rakamlardan oluşmalı.";
        }

        if (pin.Length != length)
        {
            return $"PIN {length} haneli olmalı.";
        }

        if (pin.All(digit => digit == pin[0]))
        {
            return "PIN'in tüm haneleri aynı olamaz.";
        }

        if (IsSequence(pin, 1) || IsSequence(pin, -1))
        {
            return "PIN ardışık rakamlardan oluşamaz.";
        }

        return null;
    }

    private static bool IsSequence(string pin, int step)
    {
        for (var index = 1; index < pin.Length; index++)
        {
            if (pin[index] - pin[index - 1] != step)
            {
                return false;
            }
        }

        return true;
    }
}
