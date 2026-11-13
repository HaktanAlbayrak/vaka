# Kod Standartları

> Okur: backend-engineer, frontend-engineer, dbadmin; qa-tester bu dosyaya göre denetler.

## 1. Adlandırma (PascalCase)

| Öğe | Kural | Örnek |
|---|---|---|
| Klasör ve dosya (C#, Razor, doküman) | PascalCase | `Services/UserService.cs`, `Views/Shared/_SidebarNav.cshtml`, `Docs/Phases/PhasePlan.md` |
| Namespace, sınıf, record, struct, enum, enum üyesi | PascalCase | `Vaka.Web.Services`, `OrderStatus.Completed` |
| Arayüz | `I` + PascalCase | `IUserService` |
| Metot, property, event, sabit | PascalCase | `GetActiveUsersAsync`, `Roles.Admin` |
| Asenkron metot | `Async` soneki | `SaveAsync` |
| Parametre, yerel değişken | camelCase | `userId` |
| Özel alan | `_camelCase` | `_dbContext` |
| Tablo ve kolon | PascalCase, tablo çoğul | `Users.CreatedDate` |
| Controller / action | PascalCase; URL küçük harf kebab (`LowercaseUrls`) | `UsersController.Edit` -> `/users/edit/5` |
| ViewModel | `...ViewModel` soneki | `DashboardSummaryViewModel` |
| JS modül dosyası ve sınıf | PascalCase | `wwwroot/js/SidebarNav.js`, `class SidebarNav` |
| JS fonksiyon ve değişken | camelCase | `toggleSidebar` |
| CSS özel sınıf ve değişken | kebab-case (web standardı, Tailwind ile uyum) | `.stat-tile`, `--radius-card` |

Araçların zorunlu kıldığı adlar istisnadır: `.claude/`, `wwwroot/`, `package.json`, `tailwind.config.js`, `appsettings.json`, `node_modules/`.
Kısaltmalar kelime gibi yazılır: `HtmlHelper`, `UserId`, `DbContext` (iki harfliler hariç: `IO`).

## 2. SOLID

- **S (tek sorumluluk):** Controller yalnız HTTP akışını yönetir (model bağlama, yetki, servis çağrısı, görünüm). İş kuralı servistedir. Bir sınıf tek nedenden değişir; 200 satırı geçen servis bölünmeye adaydır.
- **O (açık/kapalı):** Davranış çeşitleniyorsa `switch` büyütmek yerine arayüz + yeni uygulama. Var olan sınıf yeni durum için düzenlenmez, genişletilir.
- **L (yerine geçme):** Arayüzü uygulayan sınıf sözleşmeyi daraltmaz; `NotImplementedException` atan üye yoktur.
- **I (arayüz ayrımı):** Küçük, amaca özel arayüzler (`IUserReader`, `IUserWriter` gerekirse); tüketicinin kullanmadığı üye arayüzde durmaz.
- **D (bağımlılığın tersine çevrilmesi):** Controller ve servisler somut sınıfa değil arayüze bağlanır; hepsi constructor injection ile gelir. `new XService()` ve servis bulucu (`IServiceProvider.GetService`) yok.

## 3. Backend katmanları

Katmanlar: `Controllers -> Services (arayüz + uygulama) -> Data (EF Core DbContext)`.

Yasak: CQRS, MediatR, generic repository / unit of work sarmalayıcıları (DbContext zaten bunlardır), mikroservis, mesaj kuyruğu, AutoMapper,
özel ara katman çerçeveleri. Eşleme elle ya da küçük `ToViewModel()` uzantılarıyla yapılır.

Zorunlu: `async/await` + `CancellationToken`, okuma sorgularında `AsNoTracking()` ve projeksiyon (`Select`), sayfalama sunucuda,
`ModelState` doğrulaması + POST'ta antiforgery, entity doğrudan görünüme verilmez (ViewModel), sır `appsettings.json`'a yazılmaz.

## 4. Yetkilendirme: yalnız rol

```csharp
public static class Roles
{
    public const string Admin = "Admin";
    public const string Editor = "Editor";
    public const string Viewer = "Viewer";
    public const string AdminOrEditor = Admin + "," + Editor;
}

[Authorize(Roles = Roles.Admin)]
public sealed class UsersController : Controller { }
```

- Rol adları yalnız `Authorization/Roles.cs` sabitlerinden gelir; sihirli metin yok. Rol listesi analistin `Requirements.md` matrisinden.
- Görünümde: `@if (User.IsInRole(Roles.Admin))`. Menü ve butonlar role göre gizlenir, ama asıl koruma her zaman action'dadır.
- **Yasak:** `AddPolicy`, `[Authorize(Policy = ...)]`, `IAuthorizationHandler`, `IAuthorizationRequirement`, `AuthorizationPolicyBuilder`, claim tabanlı özel kontrol.
- Kimlik: cookie authentication; yetkisiz erişim 403 sayfası, oturumsuz erişim giriş sayfasına 302.
- **Sahiplik iş kuralıdır, yetki politikası değildir** (K-30, 2026-11-13 onaylı). "Bu kaydı yalnız sahibi ya da görevlisi değiştirir" gibi
  veriye bağlı kurallar servis metodunda veri sorgusuyla kontrol edilir. Controller oturumdaki kullanıcının `UserId` değerini okuyup servise
  parametre olarak geçirir; servis kuralı ihlal eden isteği iş hatası olarak döner, controller bunu 403 ya da form hatasına çevirir.
  Bu kullanım "claim tabanlı özel kontrol" yasağına girmez. Yasak olan, sahipliği `AddPolicy`, `IAuthorizationHandler` ya da
  claim'e özel yetki kuralıyla çözmektir.

QA denetimi (sıfır eşleşme beklenir):

```bash
grep -rnE "AddPolicy|Policy\s*=|IAuthorizationHandler|IAuthorizationRequirement|AuthorizationPolicyBuilder" Src --include=*.cs --include=*.cshtml
grep -rnE "\[Authorize\(Roles\s*=\s*\"" Src --include=*.cs        # sihirli rol metni
```

`[Authorize]` ya da `[AllowAnonymous]` taşımayan controller bulgu sayılır.

## 5. Frontend kodu

Razor, Tailwind ve JS kuralları `.claude/skills/admin-ui/UiRules.md` dosyasındadır. Özet: Tailwind CLI derlemesi, token tabanlı tema,
vanilla JS modülleri (`defer`), jQuery/GSAP/CDN yok, varlıklar self-host, `transition: all` yok.

## 6. Öz denetim (her mühendis, devirden önce)

```bash
dotnet build Src/Vaka.Web -warnaserror        # 0 hata, 0 uyarı
npm run build:css --prefix Src/Vaka.Web       # frontend
dotnet test                                   # test projesi varsa
```

## 7. Süre hesaplayan fonksiyonlar: formül üstte yazılır

Süre üreten ya da süreye göre karar veren her metot (brüt süre, net süre, mola toplamı, faz minimum kontrolü, efor, kilit ve zaman aşımı)
XML yorumunda formülü açık yazar. Formül kodu okumadan anlaşılmalı; uç durumlar `<remarks>` içinde.

```csharp
/// <summary>
/// Kaydın net çalışma süresi.
/// Brüt = son adım bitişi - ilk adım başlangıcı
/// Net  = Brüt - Σ (mola bitişi - mola başlangıcı)
/// </summary>
/// <remarks>Bitmemiş adım ve kapanmamış mola hesaba girmez.</remarks>
public TimeSpan CalculateNetDuration(CleaningRecord record) { ... }
```

- Formül iş kuralı kimliğiyle eşleşir (ör. `BR-20`); kural değişirse yorum aynı commit'te değişir.
- Süre hesapları tek bir serviste toplanır (ör. `IDurationCalculator`); controller ya da görünümde süre aritmetiği yapılmaz.
- QA bu metotları okuyarak denetler: formül satırı yoksa ya da kodla uyuşmuyorsa ORTA bulgu.

## 8. Üslup: kod ve metin elle yazılmış gibi durur

Kod, yorum, görünür metin, doküman ve commit mesajları için:

- Yorum yalnız "neden"i anlatır. Kodu tekrar eden yorum (`// Kullanıcıyı kaydet`) yazılmaz. İstisna §7 formül yorumları ve public arayüz özetleri.
- Bölüm ayıran süs yorumları (`// ===== Helpers =====`), `#region`, boş `<summary>` yok.
- Emoji yok: kodda, görünür metinde, dokümanda, commit'te.
- Kalıp ve pazarlama dili yok: "kapsamlı", "güçlü", "sorunsuz", "modern ve şık", "robust", "seamless", "comprehensive".
  Görünür metin kısa ve sahanın diliyle yazılır ("Adımı başlat", "Mola ver").
- İçi boş adlar yok: `Helper`, `Utils`, `Manager`, `Processor`, `DataService`. Adlar alan dilinden gelir (`CleaningRecord`, `ProcedureVersion`).
- "İleride lazım olur" diye yazılmış arayüz, ayar, parametre ya da her satırı saran `try/catch` yok. Kullanılmayan kod teslimde kalmaz.
- README ve dokümanlarda rozet yığını, abartılı başlık ve tekrar eden özet paragrafları yok.
- Arayüzdeki şablon görünüm yasakları `.claude/skills/admin-ui/DesignDna.md §4` dosyasındadır.
