# Vaka

Rol bazlı yetkilendirmeli admin dashboard: özgün tasarım, responsive ve erişilebilir arayüz.

| Katman | Teknoloji |
|---|---|
| Backend | ASP.NET Core 10 MVC, EF Core, cookie kimlik doğrulama, rol bazlı `[Authorize(Roles)]` (policy yok) |
| Veritabanı | SQLite |
| Frontend | Razor, Tailwind CLI, vanilla JS (self-host varlıklar) |

## Faz Durumu

Her faz kendi commit'iyle teslim edilir; teslim raporu aynı commit'te `Docs/Delivery/` altına girer.

| Faz | Başlık | Durum | Tarih | Commit | Teslim raporu |
|---|---|---|---|---|---|
| - | Proje yapısı ve Claude yönetimi | Hazır | 2026-11-13 | - | - |
| 0 | Analiz ve faz planı | Bekliyor | | | |

## Son Değişiklikler

| Tarih | Commit | Değişiklik |
|---|---|---|

## Çalıştırma

Uygulama iskeleti ilk geliştirme fazında oluşturulur. O zamana kadar bu bölüm boştur; faz tesliminde güncellenir.

Gereksinimler: .NET SDK 10, Node.js 20+, QA için `@playwright/cli` (`npm i -g @playwright/cli`).

## Depo Yapısı

```
CLAUDE.md                    Koordinatör kuralları (ajan akışı, commit kuralları)
.claude/
  agents/                    analyst, backend-engineer, frontend-engineer, dbadmin, qa-tester
  skills/admin-ui/           Tasarım DNA'sı, UI kuralları, UI teslim kuralları
  skills/web-design-guidelines/, playwright-cli/
  hooks/Guard.mjs            Commit etiketi, DROP ve ALTER denetimi
  settings.json
Docs/
  Process/                   Workflow, CodingStandards, DbProtocol
  Analysis/                  Requirements.md (analist)
  Phases/                    PhasePlan.md, Decisions.md
  Design/Dna.md              Tasarım DNA kartı
  Db/                        DbChanges.md indeksi + DB-<yıl>-<sıra>.md kayıtları
  Delivery/                  Faz-<N>-Teslim.md raporları, Evidence/
  Handover/                  Ajan devir notları
Src/Vaka.Web/                Uygulama (ilk geliştirme fazında oluşur)
```

## Geliştirme Süreci

1. Her istek `analyst` ajanına gider; gereksinimler ve faz planı çıkar, kullanıcı onaylar.
2. Fazda `backend-engineer` ve `frontend-engineer` analistin kararına göre paralel ya da sırayla çalışır. Veritabanı işi yalnız
   `backend-engineer` talebiyle `dbadmin`'e gider (DROP yok, ALTER onaylı).
3. `qa-tester` fazı çalışan uygulamada test eder ve sabit biçimli teslim raporunu yazar.
4. `analyst` raporu inceler: TESLİM ALINDI ya da İADE.
5. Teslim alınan faz tek commit'le işlenir, ardından bu README güncellenir.

Ayrıntı: [Docs/Process/Workflow.md](Docs/Process/Workflow.md) · [CodingStandards.md](Docs/Process/CodingStandards.md) ·
[DbProtocol.md](Docs/Process/DbProtocol.md) · [Teslim raporu biçimi](Docs/Delivery/_Template.md)
