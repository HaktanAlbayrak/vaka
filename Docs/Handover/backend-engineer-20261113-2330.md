DEVİR: backend-engineer -> frontend-engineer   Faz: T   Dosya: Docs/Handover/backend-engineer-20261113-2330.md

Yapıldı:
- `Src/Vaka.Web/Vaka.Web.csproj` (net10.0, Nullable, ImplicitUsings) ve `Program.cs`: MVC, LowercaseUrls, kebab URL (`Routing/KebabCaseParameterTransformer.cs` + `RouteTokenTransformerConvention`; attribute route kullanan controller'lar `[Route("[controller]/[action]")]` yazar), statik dosya, Production'da `/error` istisna sayfası, `UseStatusCodePagesWithReExecute("/error/{0}")`. Kimlik doğrulama, veritabanı ve Migrate() yok. Çözüm dosyası eklenmedi, gerek yok.
- `Authorization/Roles.cs`: Operator, Manager, Auditor, SuperAdmin; ManagerOrAuditor, ManagerOrSuperAdmin, RecordReaders, Any (yalnız tekil sabitlerden kurulu).
- `Controllers/PrototypeController.cs` (`[AllowAnonymous]`): MyWork, Run, NewRecord, Records, Dashboard. `OnActionExecuting` Development dışında `NotFound()` verir (PL-10).
- `Controllers/ErrorController.cs` (`[AllowAnonymous]`): `/error` (500, `Failure`), `/error/{code}` (403 `Forbidden`, 404 `NotFound`, diğer `Failure`).
- `Properties/launchSettings.json` (http://localhost:5080, Development), `appsettings.json` (yalnız Logging ve AllowedHosts).

Doğrulama:
- `dotnet build Src/Vaka.Web -warnaserror`: 0 uyarı, 0 hata.
- Development, 127.0.0.1:5187: beş prototip route'u 200; Production: beşi de 404. `/error` 500, `/error/403` 403 döner. Süreç kapatıldı.
- CodingStandards §4 grep'leri 0 eşleşme.

Sonrakinin bilmesi gerekenler:
- Çalıştırma: `dotnet run --project Src/Vaka.Web` (Development, 5080) ya da `--urls http://127.0.0.1:<port>`. Production denemesi: `ASPNETCORE_ENVIRONMENT=Production`.
- Route'lar: `/prototype/my-work`, `/prototype/run`, `/prototype/new-record`, `/prototype/records`, `/prototype/dashboard`. Action'lar ViewModel'siz `View()` döner; demo veri görünümde ya da frontend'in ekleyeceği statik model olarak tutulur (ViewModel gerekirse devir notuyla bana yaz).
- Layout yolu `Views/Shared/_Layout.cshtml`; `_ViewStart` bunu atar. Hata görünümleri `Views/Error/Forbidden`, `NotFound`, `Failure`.
- Yer tutucu dosyalar, frontend tarafından değiştirilecek: `Views/_ViewImports.cshtml` (Vaka.Web.Authorization eklidir, `Roles` görünümde kullanılabilir), `Views/_ViewStart.cshtml`, `Views/Shared/_Layout.cshtml` (boş iskelet), `Views/Prototype/*.cshtml` (5 adet, tek başlık), `Views/Error/*.cshtml` (3 adet).
- Kimlik doğrulama yok: `User.Identity.Name` ve `User.IsInRole` bu fazda boş döner; kabuk menüsü ve kişi adı demo değerle çizilir.
- `wwwroot/` yok; frontend oluşturur. `Routing/` klasörü (kebab dönüştürücü) bende kalır.

DB talebi: yok
Açık/ertelenen:
- `appsettings.Development.json` eklenmedi; gerekirse sonraki faz.
- `Program.cs` içinde `UseAuthentication`/`UseAuthorization` yok; Faz 1'de cookie kimlik doğrulama ile eklenir.
