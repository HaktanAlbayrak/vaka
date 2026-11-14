# Frontend devir notu: Faz 1

## Değişenler
- Görünümler (hepsi sözleşmedeki `@model` tipleriyle): `Views/Account/Login`, `FirstLogin`, `Security`, `AccessDenied`; `Views/Home/Index`; `Views/Users/Index`, `Form`, `Deactivate`.
- Kabuk (`Views/Shared/_Layout.cshtml`) üç kipte: tam (ray, alt çubuk, çekmece), odak (`ViewData["Focus"] = true`, ilk giriş: üst çubukta kişi + her boyutta "Kullanıcı değiştir"), genel (oturumsuz: marka, bağlantı, tema). Demo kişi yalnız `ViewData["DemoRole"]` set eden prototip görünümlerinde ve oturumsuzken çıkar (T-B16: `/error`, `/error/403`, `/error/404`, `/account/access-denied`, `/account/login` genel kipte).
- Kişi adı `User.Identity.Name`, sicil no `VakaClaimTypes.RegistryNo`, rol metni `Roles.Label`. Menü: Ana sayfa (`/`, Süper admin-yalnız hariç), Kullanıcılar (`/users`, Manager veya SuperAdmin), Parola ve PIN (`/account/security`). Bağlantısız kalem yok (K8b). Prototip bağlantısı gerçek oturumda yok (K14).
- "Kullanıcı değiştir": `POST /account/logout`, `asp-antiforgery="true"` token'lı form (`_SwitchUser`).
- Yeni ortak parçalar: `_Field`, `_FormSummary`, `_Pagination` (prototip Records da bunu kullanıyor), `_AccessDenied` (Account/AccessDenied ve Error/Forbidden ortak), `_SwitchUser`, `_TopbarTools`. `_Badge`'e `user.*` rozetleri. `_PinInput` hata metnini `Error` verilmezse `ModelState[Name]`'den alır.
- JS: `RevealToggle.js` (parola göster/gizle), `FormBusy.js` (gönderimde düğme kapanır, liste süzgecinde iskelet), `UnsavedGuard.js` (kullanıcı formunda kaydedilmemiş değişiklik uyarısı); `App.js` bağlandı, ilk hatalı alan yoksa form özetine odak.
- `Styles/app.css`: `panel-tag`, `disclosure`, `switch-btn-show`, `page-public`, tablo hücresi `xl:h-14`. `Styles/BuildAssets.mjs`: 8 ikon eklendi (house, key-round, user-plus, pencil, user-check, user-x, rotate-ccw, save); sprite yeniden üretildi.
- T-B21: `Dna.md` veri dili satırı uygulamayla aynı (768 px altı 2, 768-1279 px 4 sütunlu kart; tablo satırı 1280 px ve üstünde en az 56 px). Bileşen listesi ve kabuk kipleri eklendi.

## Dört durum
- Giriş, ilk giriş, Güvenlik, Form, Pasifleştirme: dolu + alan hatası + form özeti (`role="alert"`, ilk hatalı alana odak). Kullanıcı listesi: dolu, boş (süzgeçli ve süzgeçsiz iki metin), yükleniyor (süzgeç gönderiminde ve sayfa bağlantısında iskelet + "Yükleniyor…"), hata (sunucu hatasında `/error` sayfası, sonraki adım düğmeli).
- Liste hata durumu ayrı ViewModel alanı gerektirmedi: sunucu hatası genel hata sayfasına düşer.

## Öz denetim
- `dotnet build Src/Vaka.Web -warnaserror` 0 uyarı 0 hata; `npm run build:css` tamam.
- Controller'lar henüz yok; görünümleri çalıştırmak için kapsam dışı geçici bir kopyada (scratchpad, repo dışı) sahte denetleyiciyle 20 görünüm durumu çizildi: hepsi 200 (403 sayfaları 403), her birinde `h1` 1, em/en-dash 0, dış kaynak 0. Oturumsuz hata ve giriş sayfalarında demo kişi ve `prototype` metni 0. Süper admin menüsünde yalnız Kullanıcılar ve Parola ve PIN. Süreç yalnız kendi PID'iyle kapatıldı.
- `UiDelivery §4` grep tablosu Views, Styles, wwwroot/js üzerinde 0 ihlal.
- Görsel tur yapılmadı (QA).

## QA ölçmesi gereken hücreler
- 360, 768, 1024, 1440, 1920: `/account/login`, `/account/first-login` (iki mod), `/account/security`, `/users` (dolu, boş, uzun ad, 23+ kayıt), `/users/create`, `/users/edit/{id}` (rol kilitli, pasif hesap, hata), `/users/deactivate/{id}` (açık iş, engelli), `/account/access-denied`, `/error/404`.
- 768-1279 px liste: dört sütunlu kart; 1280 px: tablo, satır en az 56 px.
- 1024 altı: üst çubukta "Kullanıcı değiştir" tek dokunuş; ilk giriş ekranında 1440'ta da görünür.
- Kullanıcı formunda sayfadan çıkış uyarısı; PIN alanı belirleme modunda `maxlength` = `PinLength`, doğrulama modunda sınır yok.
- Çevrimdışı: tüm POST düğmeleri `data-requires-online`.

## DNA uygunluğu (kart maddeleri)
- Kenarlık 2 px, köşe 2 px, tek accent, gölge ve gradient yok: korundu.
- Tablo veri dili (kart dönüşümü, rozet ikon + metin + kenarlık): korundu; kullanıcı durumları metin + ikonlu rozet.
- Dokunma hedefi en az 44 px; birincil formlarda `btn-lg` 56 px.
- Sayfa iskeleti ana kolon + sağ bağlam rayı: ilk giriş, kullanıcı formu, pasifleştirme.
- Boş: kesik çizgili kutu + eylem; hata: düz hata kenarlıklı kutu + sonraki adım; yükleniyor: durağan iskelet.
- Sapma: yok. Eklenen: `disclosure` (details) ile sıfırlama/etkinleştirme bölümleri; hareket yalnız 150 ms `transform`.

## Backend'den beklenen/eksik
- Yanlış PIN reddi (sıfırlama, etkinleştirme, pasifleştirme) boş anahtarla gelirse form özetinde görünür. Alanın altında göstermek için `ResetPin.Pin`, `ResetPassword.Pin`, `Activate.Pin`, `Pin` anahtarıyla vermeniz yeterli; görünüm her iki durumu da işler.
- `UserFormViewModel` pasifleştirme engelinin nedenini taşımıyor (kendi hesabı mı, son Süper admin mi); görünüm genel cümle gösteriyor. İsterseniz `DeactivateBlockReason?` ekleyin, ben bağlarım.
- K11a: oturumu düşmüş kişinin `POST /account/logout` isteği antiforgery token'ı eski kimlikle ürettiği için 400 verebilir; `Logout` action'ında token doğrulamasını gevşetmek (örn. `[IgnoreAntiforgeryToken]`) ya da kimliksiz doğrulama gerekir.
- Login POST başarısızlığında `ErrorMessage` ve boş anahtarlı ModelState hatası birlikte dolarsa iki kez görünür; yalnız biri yeterli.
- Alan adları sözleşmeyle aynı; uyuşmazlık yok.
