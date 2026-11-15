# Vaka

Üretim tesislerinde makine temizliğinin dijital kaydı. Temizlik, versiyonlu bir prosedüre bağlı olarak adım adım ve sunucu saatiyle
kaydedilir; kayıt sonradan değiştirilemez ve denetimde kanıt olarak kullanılır. Operatör sahada tablet ya da telefonla çalışır,
yönetici tanımları, talepleri ve raporları yönetir.

| Katman | Teknoloji |
|---|---|
| Backend | ASP.NET Core 10 MVC, EF Core, cookie kimlik doğrulama, rol bazlı `[Authorize(Roles)]` (policy yok) |
| Veritabanı | SQLite |
| Frontend | Razor, Tailwind CLI, vanilla JS (self-host varlıklar) |

## Faz Durumu

Her faz kendi commit'iyle teslim edilir; teslim raporu aynı commit'te `Docs/Delivery/` altına girer.

| Faz | Başlık | Durum | Tarih | Commit | Teslim raporu |
|---|---|---|---|---|---|
| 0 | Analiz ve faz planı | Tamamlandı | 2026-11-13 | 9b8eb67 | - (analiz fazı, QA yok) |
| T | Tasarım DNA'sı ve kabuk prototipi | Tamamlandı | 2026-11-13 | 947be60 | [Faz-T-Teslim.md](Docs/Delivery/Faz-T-Teslim.md) |
| 1 | Giriş, PIN, kullanıcılar ve Süper admin | Tamamlandı | 2026-11-14 | 38d0b4e | [Faz-1-Teslim.md](Docs/Delivery/Faz-1-Teslim.md) |
| 2 | Tesis yapısı, vardiya ve malzeme | Tamamlandı | 2026-11-14 | c3a4fd7 | [Faz-2-Teslim.md](Docs/Delivery/Faz-2-Teslim.md) |
| 3 | Temizlik tipi ve prosedür versiyonu | Tamamlandı | 2026-11-15 | e867695 | [Faz-3-Teslim.md](Docs/Delivery/Faz-3-Teslim.md) |
| 4 | Kayıt açma ve makine kilidi | Sırada | | | |
| 5 | Adım yürütme ve mola | Bekliyor | | | |
| 6 | Kayıt listesi, detay ve denetim çıktısı, bütünlük, müdahale | Bekliyor | | | |
| 6A | Acil müdahale, süre ve efor raporu, malzeme sorgusu | Bekliyor | | | |
| 7 | Talep, beklemeye alma, yönetici panosu, adım içi mola, görevli PIN onayı | Bekliyor (doküman kalemlerinden sonra) | | | |
| 8 | Bildirim ve düzeltme | Bekliyor (doküman kalemlerinden sonra) | | | |
| 9 | Grafik, CSV, sistem ayarları ve son regresyon | Bekliyor (doküman kalemlerinden sonra) | | | |

## Son Değişiklikler

| Tarih | Commit | Değişiklik |
|---|---|---|
| 2026-11-14 | c3a4fd7 | Faz 2: tesis, hat, makine, vardiya, malzeme ve lot tanımları; Salı takvimine göre faz planı güncellendi |
| 2026-11-14 | a3f6ec3 | Süreç: UI fazlarında tam testten önce QA yerleşim ön turu (rapor yok, tur sayılmaz) |
| 2026-11-14 | 38d0b4e | Faz 1: giriş, ilk girişte parola ve PIN, PIN kilidi, kullanıcı yönetimi, Süper admin |
| 2026-11-13 | 947be60 | Faz T: onaylı tasarım DNA'sı, uygulama iskeleti ve beş prototip sayfa |
| 2026-11-13 | 9b8eb67 | Faz 0: analiz, kusur listesi, gereksinimler ve onaylı faz planı |

## Çalıştırma

İlk kurulumda veritabanı migration ile oluşturulur; uygulama açılışta migration çalıştırmaz.

```bash
dotnet tool install --global dotnet-ef     # bir kez
dotnet ef database update --project Src/Vaka.Web
dotnet run --project Src/Vaka.Web          # Development, http://localhost:5080
```

Olay zinciri özet anahtarı (Faz 4 ve sonrası) repoda ve `appsettings*.json`'da tutulmaz. Development'ta bir kez
user-secrets'a yazılır; anahtar yoksa ya da çözülünce 32 bayttan kısaysa uygulama başlamaz. Üretimde aynı değer
`EventChain__HmacKey` ortam değişkeniyle verilir. Geliştirme anahtarıyla yazılmış zincir üretim anahtarıyla doğrulanmaz.

```bash
dotnet user-secrets set "EventChain:HmacKey" "$(openssl rand -base64 32)" --project Src/Vaka.Web
```

Seed tek bir Süper admin hesabı içerir (sicil no `0001`). Geliştirme parolası depoda tutulmaz, ekipten alınır;
ilk girişte parola değiştirilir ve PIN belirlenir. Canlı ortama geçmeden önce bu hesabın parolası değiştirilmiş olmalıdır.

Derlenmiş CSS, yazı tipleri ve ikonlar depodadır. Stil değiştirilirse:

```bash
npm ci --prefix Src/Vaka.Web
npm run build:css --prefix Src/Vaka.Web    # yazı tipi ve ikon için: build:assets
```

Prototip sayfaları yalnız Development ortamında açılır: `/prototype/my-work`, `/prototype/run`, `/prototype/new-record`,
`/prototype/records`, `/prototype/dashboard`.

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
  Analysis/                  Brief.md, Kusur.md, Requirements.md (analist)
  Phases/                    PhasePlan.md, Decisions.md
  Design/Dna.md              Tasarım DNA kartı
  Db/                        DbChanges.md indeksi + DB-<yıl>-<sıra>.md kayıtları
  Delivery/                  Faz-<N>-Teslim.md raporları, Evidence/
  Handover/                  Ajan devir notları
Src/Vaka.Web/                Uygulama (Controllers, Views, Styles, wwwroot)
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
