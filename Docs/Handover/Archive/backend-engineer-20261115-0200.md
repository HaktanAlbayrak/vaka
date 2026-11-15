DEVİR: backend-engineer -> koordinatör (qa-tester)   Faz: 2 tur 2 (backend tamam)   Dosya: Docs/Handover/backend-engineer-20261115-0200.md

Yapıldı:
- `MachineFormViewModel.RetiredAtLocal?` ve `MachineRetireViewModel.RetiredAtLocal?` (PL-43); GET edit ve retire GET'te dolar (UTC -> `ITurkeyClock.ToLocal`).
- B1: `ErrorController.Status([FromRoute] int code)`; 400-599 dışı 404. Gövde bağlaması yalnız rota değerine inmiyordu: eski imza `code` değerini form gövdesinden de alıyordu, re-execute'ta POST gövdesi korunur. `[FromRoute]` ile gövde artık bağlanmıyor, başka çözüm gerekmedi.
- B5: `MachineService.UpdateAsync` kaldırılmış makinede "Kullanımdan kaldırılmış makine değiştirilemez." (alan adsız, ad dahil hiçbir şey kaydedilmez). `MachineRetirement` yapıcıya `ITurkeyClock` aldı (DI otomatik).
- B7: `POST /machines/create` `refresh=true` ise `ModelState.Clear()`, kayıt yok, alan hatası yok; form gönderilen tesisin aktif hatlarıyla çizilir, kod, ad korunur, hat seçimi gönderilen hat o tesisin hattıysa korunur. GET `/machines/create?facilityId...` duruyor (adres paylaşımı, token taşımaz).
- B9: `includeId` kalktı (grep 0). `MachineService.cs` 269 satır: liste, form, ekleme, düzenleme ve seçenek listesi tek sorumluluk alanı (makine okuma/yazma); bölünürse yalnız seçenek metotları ayrılır, ama iki dosyada aynı DbContext sorgusu tekrarlanacağı için bölünmedi.
- B10: tesis seçilmemişken hat etiketi `T1-H1 - Hazırlık hattı`; tesis seçiliyken ve `/machines/lines` JSON'u `H1 - Hazırlık hattı` (değişmedi).

Doğrulama (.qa/be/g.db = f2.db kopyası, port 5347, kapatıldı; `dotnet build -warnaserror -o .qa/be/build` 0 uyarı 0 hata; §4 grep ve dash/region grep 0):
- Antiforgery'siz POST, gövdeler `Name=Y`, `Code=T9&Name=Y`, `Code=200`, `code=404`: Faz 2'nin 13 POST'u (facilities, lines, machines, materials, shifts, material-lots create/edit; machines retire) ve Faz 1'in 11'i (login, first-login, change-password, change-pin, users create/edit/deactivate/activate/reset-pin/reset-password; logout bilinçli antiforgery dışı, K11a) hepsi 400. Not: oturumlu istek; logout gövdede 302.
- `/error/0`, `/error/200`, `/error/999` 404; `/error/403` 403, `/error/404` 404, `/error/500` 500, `/error` 500.
- Refresh: 200, kod RF1 ve ad korundu, hat seçenekleri tesis 1'in H1, H2'si, kayıt oluşmadı (liste 0 satır).
- Kaldırılmış makine: edit GET `14.11.2026 18:40`, edit POST 200 ve neden metni, ad değişmedi; retire GET tarih dolu. `/machines` tesis seçilmemişken `T1-H1 - Hazırlık hattı`.

Frontend için: refresh düğmesi aynı forma `name="refresh" value="true"` ile POST eder, ayrı alan gerekmez. Retired formda devre dışı select'ler post edilmez (FacilityId/LineId 0 gelir); sunucu retired kontrolünü önce yapar, sorun yok.
DB talebi: yok.
Açık: önceki notun maddeleri değişmedi.
