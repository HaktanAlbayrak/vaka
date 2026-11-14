# Frontend devir notu: Faz 1 bağlama

## Değişen
- `Views/Users/Form.cshtml`: pasifleştirme engeli `Model.DeactivateBlockReason` ile gösterilir, null ise eski genel cümle.

## Kontroller (kod okuma, çalıştırma yok)
- Giriş nedenleri: controller `StatusMessage`'ı `SessionEndReason.Message(reason)` ile dolduruyor (expired, inactive, pin-locked, signed-out dört metin var); Login görünümü bunu `ViewData["StatusMessage"]` ile kabuktaki bilgi satırına veriyor. Ek iş gerekmedi.
- Başarı iletisi: Login, Security, Users/Index, Users/Form `StatusMessage`'ı gösteriyor. Controller TempData'sı Security ve Users/Index/Form'a yazılıyor; Home/FirstLogin'de gösterim yok (yönlendirme hedefi Index ya da Security olduğu için gerekmedi).
- Parola/PIN: `_Field` çağrılarında parola alanları null değer alıyor; `_PinInput` `value` basmıyor.
- PIN anahtarları: `Pin`, `ResetPin.Pin`, `ResetPassword.Pin`, `Activate.Pin` (ayrıca `Pin.CurrentPin/NewPin/ConfirmPin`, `NewPin`, `ConfirmPin`) görünümde var; `_PinInput` hatayı `ModelState[Name]`'den alan altında gösterir.

## Öz denetim
- `dotnet build Src/Vaka.Web -warnaserror` ve em/en-dash taraması sonuçları: 0 uyarı, 0 hata, 0 dash. CSS değişmedi. Uygulama çalıştırılmadı.

## Backend'den beklenen/eksik
- Yok.
