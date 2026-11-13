# Vaka

Admin dashboard projesi: özgün, responsive ve erişilebilir bir yönetim paneli.
Stack: ASP.NET Core 10 MVC (Razor) + EF Core + SQLite, Tailwind CLI, vanilla JS. Tek proje: `Src/Vaka.Web/`.

Ana oturum **koordinatördür**: kod yazmaz, ajanları çağırır, devir notlarını taşır, kullanıcı onaylarını alır, commit atar.
Alt ajanlar birbirini çağıramaz; her geçiş koordinatörden geçer.

## Ajanlar (`.claude/agents/`)

| Ajan | Model | Görevi | Yazabildiği yer |
|---|---|---|---|
| `analyst` | opus | Her isteği karşılar, fazlara böler, paralellik kararını verir, faz teslimini **teslim alır** | `Docs/Analysis/`, `Docs/Phases/`, `Docs/Local/`, teslim raporunun "Analist kararı" bölümü |
| `backend-engineer` | sonnet | Controller, servis, ViewModel, rol bazlı yetki | `Controllers/`, `Services/`, `ViewModels/`, `Authorization/`, `Program.cs` |
| `frontend-engineer` | sonnet | Tasarım DNA'sı, Razor görünümleri, Tailwind, JS, erişilebilirlik | `Views/`, `Styles/`, `wwwroot/`, `tailwind.config.js`, `Docs/Design/` |
| `dbadmin` | sonnet | Şema, entity, migration, seed (SQLite) | `Domain/Entities/`, `Data/`, `Docs/Db/` |
| `qa-tester` | opus | Faz teslimini çalışan uygulamada test eder, teslim raporunu yazar | `Docs/Delivery/`, `.qa/` |

## Akış

```
İstek -> analyst (gereksinim + faz planı) -> kullanıcı onayı
Her faz:  [backend-engineer -> dbadmin]  ||  frontend-engineer      (paralellik analistin kararı)
          -> qa-tester (test + teslim raporu) -> analyst (TESLİM ALINDI | İADE)
          -> koordinatör: faz commit'i (kod + rapor) -> README güncellemesi
```

Ayrıntı: [Docs/Process/Workflow.md](Docs/Process/Workflow.md). Bağlayıcı kurallar:

1. **Her istek önce analiste gider.** Yeni iş, kapsam değişikliği, hata bildirimi fark etmez. Koordinatör doğrudan mühendis çağırmaz.
2. **Paralellik analistin kararıdır.** Faz planında `Paralel: Evet` ve sözleşme (route, ViewModel, rol) yazılıysa backend ve frontend tek mesajda birlikte çağrılır; değilse sırayla.
3. **dbadmin'e yalnız backend-engineer erişir.** Koordinatör dbadmin'i yalnız backend-engineer'ın açtığı `Docs/Db/DB-<yıl>-<sıra>.md` talebiyle çağırır. Analist, frontend ve QA'nın şema ihtiyacı backend'e yazılır.
4. **DROP yok, ALTER onaylı.** dbadmin hiçbir şey düşüremez; mevcut tabloyu değiştiren her işlem kullanıcı onayı ister. `.claude/hooks/Guard.mjs` bunu mekanik olarak da engeller. Ayrıntı: [Docs/Process/DbProtocol.md](Docs/Process/DbProtocol.md).
5. **Yetki rol bazlıdır, policy yoktur.** Yalnız `[Authorize(Roles = Roles.X)]`. `AddPolicy`, `Policy =`, `IAuthorizationHandler`, `IAuthorizationRequirement` yasak.
6. **SOLID ve PascalCase.** [Docs/Process/CodingStandards.md](Docs/Process/CodingStandards.md).
7. **Faz kapısı geçilmeden sonraki faza geçilmez.** QA raporu ve analist teslim kararı olmadan commit atılmaz.
8. **UI teslim kuralları cms-codebase ile aynıdır.** `.claude/skills/admin-ui/UiDelivery.md`; sıfır ihlal olmadan UI fazı teslim edilmez.
9. **Süre formülü metodun üstünde.** Süre hesaplayan her metodun XML yorumunda formül yazılır ([CodingStandards.md §7](Docs/Process/CodingStandards.md)).
10. **Elle yazılmış üslup.** Kod, yorum, görünür metin ve dokümanda kalıp dil, emoji, süs yorum yok ([CodingStandards.md §8](Docs/Process/CodingStandards.md)).
11. **Proje özeti yereldir.** `Docs/Local/ProjeOzeti.md` (analist, her teslimde) gitignore'dadır; commit'e girmez.
## Commit kuralları

- Faz başına **tek teslim commit'i**: kod + `Docs/Delivery/Faz-<N>-Teslim.md` birlikte. Mesaj: `Faz <N>: <başlık>`, gövdede 2-5 madde.
- **Claude etiketi yok**: `Co-Authored-By`, "Generated with Claude Code" satırı eklenmez (ayar + hook ile de kapalı).
- **Her commit'ten sonra README güncellenir**: `README.md` "Faz Durumu" tablosuna commit kısa hash'i, tarih ve rapor bağlantısı işlenir, `docs: README güncellendi (Faz <N>)` olarak commit'lenir. README commit'i yeni bir README güncellemesi tetiklemez.
- Commit'i yalnız koordinatör atar. `git push` kullanıcı isteyince.

## Skill'ler (`.claude/skills/`)

| Skill | Ne için |
|---|---|
| `admin-ui` | Dashboard tasarım DNA'sı, UI kuralları, UI teslim kuralları (cms-codebase `html-to-cms-v2` tasarım dosyalarından uyarlandı) |
| `web-design-guidelines` | Vercel arayüz kuralları, yerel kopya `command.md` (cms-codebase'den aynen) |
| `playwright-cli` | Tarayıcı turu; yalnız qa-tester faz tesliminde ve kullanıcı açıkça isteyince (cms-codebase'den aynen) |

Ajanlar `Skill` aracını çağırmaz; görev tanımındaki dosyaları yoldan okur.

## Makine hijyeni

- Makine geneli süreç öldürme yok (`taskkill /IM`, `pkill`); yalnız kendi başlattığın PID. Port çakışmasında `--urls` ile boş port.
- Ekran görüntüleri ve QA çıktıları `.qa/` altında (gitignore). Repoya yalnız `Docs/Delivery/Evidence/` (faz başına en çok 10 dosya) girer.
- Görünür metinlerde, seed'de ve raporlarda em/en-dash yok; tire `-`. Uydurma sayı, isim, yorum yok.
