using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.WebUtilities;

namespace Vaka.Web.Services;

/// <summary>
/// Yükleme action'ında istek gövdesi ve çok parçalı form sınırı ayardan türetilir: azami fotoğraf ve video MB değerlerinin büyüğü
/// (<see cref="IDurationCalculator.GetUploadBodyLimitBytes"/>). Antiforgery denetimi formu okumadan önce çalışsın diye sırası ondan küçüktür (attribute Order = 900).
/// Beyan edilen uzunluk sınırı aşıyorsa istek 413 ile reddedilir; hata sayfasını durum kodu yönlendirmesi çizer.
/// </summary>
public sealed class UploadLimitFilter : IAsyncAuthorizationFilter
{
    private readonly ISystemSettingsReader _settings;
    private readonly IDurationCalculator _calculator;

    public UploadLimitFilter(ISystemSettingsReader settings, IDurationCalculator calculator)
    {
        _settings = settings;
        _calculator = calculator;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var photo = await _settings.GetValueAsync(SettingKeys.MaxPhotoMb, context.HttpContext.RequestAborted);
        var video = await _settings.GetValueAsync(SettingKeys.MaxVideoMb, context.HttpContext.RequestAborted);
        var limit = _calculator.GetUploadBodyLimitBytes(photo, video);

        var request = context.HttpContext.Request;
        if (request.ContentLength > limit)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status413PayloadTooLarge);
            return;
        }

        var bodyFeature = context.HttpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();
        if (bodyFeature is { IsReadOnly: false })
        {
            bodyFeature.MaxRequestBodySize = limit;
        }

        context.HttpContext.Features.Set<IFormFeature>(new FormFeature(request, new FormOptions { MultipartBodyLengthLimit = limit }));
    }
}
