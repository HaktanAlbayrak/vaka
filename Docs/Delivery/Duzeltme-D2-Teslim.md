# Düzeltme D-2 Teslim Raporu: Görevli panelinin metinleri

> Faz dışı düzeltme (PL-106, PL-74 düzeni). D-1'in ayrı teslim raporu yok (commit `0552f38`); bu kayıt `_Template.md`'nin kısa halidir. Yalnız analistin hedefli ölçüm şartı (PhasePlan "Düzeltme D-2") ölçüldü.

| Alan | Değer |
|---|---|
| Faz | D-2 (faz dışı düzeltme) |
| Tarih | 2026-11-17 |
| Tur | 1 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `43dd95f`; değişen dosya yalnız `Views/Records/_Participants.cshtml`, kopyası `.qa/d2/src` repodakiyle bayt bayt aynı). Devir notu `Docs/Handover/frontend-engineer-20261117-0055.md` |
| Ortam | LOCAL, Release build, port 5921 (Production), DB: `.qa/` kopyası. `.qa/d2/qa.db` = `.qa/f6fqa/qa.db` dosyasının SQLite yedekleme API kopyası. Chromium (Playwright 1.61). Gerçek `App_Data/vaka.db`'ye ve PID 20552'ye dokunulmadı; kendi PID'im (35384) kapatıldı |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ |
| Analist kararı | TESLİM ALINDI (2026-11-17, PL-107) |

## 1. Kapsam

`_Participants.cshtml` metinleri değişti. Backend, şema ve sunucu iletileri değişmedi.

| Yer | Yeni metin |
|---|---|
| Panel özeti | "Yardımcı ekle", ikon `user-plus` |
| JS kapalı arama etiketi | "Yardımcı ara" |
| Seçim kutusu etiketi ve erişilebilir adı | "Yardımcı ara" |
| İpucu | "...listeden yardımcıyı seçin." |
| Adım sürerken ipucu eki | "Eklenen yardımcı kendi PIN'iyle katılınca çalışmaya başlar." |
| JS kapalı `legend` | "Yardımcı seç" |
| Gönder düğmesi | "Yardımcıyı ekle" |

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| D2-1 | Bekleyen ve devam eden adımda: özet "Yardımcı ekle", combobox erişilebilir adı "Yardımcı ara", düğme "Yardımcıyı ekle"; JS kapalı yolda "Yardımcı ara" ve "Yardımcı seç" | GEÇTİ | `Evidence/D-2/D2Olcum.txt`. Kayıt 39 adım 168, önce Hazır sonra Devam ediyor; iki durumda da bütün hücrelerde: özet "Yardımcı ekle" (ikon `user-plus`), combobox etiketi "Yardımcı ara". `getByRole("combobox", { name: "Yardımcı ara" })` 1 öğe buluyor. İpucu "Ad ya da sicil no parçası yazın, listeden yardımcıyı seçin."; devam eden adımda eki "Eklenen yardımcı kendi PIN'iyle katılınca çalışmaya başlar." var, bekleyende yok. Düğme "Yardımcıyı ekle". JS kapalıyken arama etiketi "Yardımcı ara" ve legend "Yardımcı seç" görünür, combobox gizli; JS açıkken tersi. Değişmeyenler yerinde: "Görevliler" listesi, "Kişileri ara", "Bir kişi seçin." |
| D2-2 | 320 (%200), 360 ve 1440 px, açık ve koyu: taşma ve yatay kaydırma yok, G12 kusuru 0, konsol 0 | GEÇTİ | İki adım durumunda JS açık 3 genişlik x 2 tema, JS kapalı 360 x 2 tema; toplam 16 hücre. `hScroll` false, viewport dışı öğe 0. Panel gövdesi dışına çıkan öğe 0; etiket ile alan solu aynı. Özet 161x44 px; panel düğmeleri 44 px ve üstü. Konsol 0 (`D2Olcum.txt`, `PanelDevamJsAcik360.png`, `PanelBekleyenJsKapali360.png`) |
| D2-3 | Gerileme: klavyeyle yardımcı ekleme (Faz 6C K16) ve yardımcının katılması (Faz 6E K3) | GEÇTİ | 360 px, yalnız klavye: "Yardımcı ekle" özetine 6. Tab'da ulaşıldı (odak halkası var). Seçim kutusunun erişilebilir adı "Yardımcı ara". "2971" yazıldı, ArrowDown ile "Yeni Operatör Deneme, Sicil 2971" seçildi, Enter, PIN, Enter. Olay `ParticipantAdded`. Ardından kişi kartında "Adıma katıl: Yeni Operatör Deneme" ile yardımcının PIN'i girildi: kart "Çalışıyor". Olay `ParticipantJoined`: yapan 63, `SessionUserId` 4. Aralık açıldı. Sayfada h1 1, kaydırma yok, 44 px altı hedef 0, konsol 0 |
| D2-4 | Değişen dosyada grep tablosu 0 | GEÇTİ | `UiDelivery.md §4` kalıpları (transition-all, outline-none, h-screen, onclick, CDN, jquery, em/en-dash, ham renk, `<img`, `...`) `_Participants.cshtml`'de 0 eşleşme. `user-plus` sembolü sprite'ta var. Release derlemesi 0 uyarı, 0 hata; sunucu günlüğünde istisna 0 |

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | 0 uyarı, 0 hata (`.qa/d2/src`) |
| CSS | `npm run build:css` | Değişmedi (yalnız metin ve mevcut ikon) |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | Kapsam dışı (C# değişmedi) |
| Adlandırma (PascalCase) | örneklem denetimi | Kapsam dışı (yeni dosya yok) |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal |
| Mekanik eval | `UiDelivery.md §3` | 16 hücrede panel ölçümü beklenen değerlerde; sayfa eval h1 1, `hScroll` false, 44 px altı hedef 0, dash yok, konsol 0 |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Yeni migration yok |

## 4. Yetki matrisi

yok (route ve rol değişmedi)

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| Run, bekleyen adım paneli | Temiz; JS kapalı temiz | Ölçülmedi | Ölçülmedi | Temiz | Ölçülmedi | Temiz | Ölçülmedi | 320 temiz | - |
| Run, devam eden adım paneli | Temiz; JS kapalı temiz | Ölçülmedi | Ölçülmedi | Temiz | Ölçülmedi | Temiz | Ölçülmedi | 320 temiz | Özete Tab, seçim kutusu, ok tuşu, Enter, PIN; odak halkası var |

Tasarım DNA'sı uygunluğu: UYGUN (ikon Lucide sprite'ından `user-plus`; metin sahanın dilinde, dash yok; görsel düzen değişmedi).

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| - | - | yok | - | - | - | - |

## 7. Geçenler

D2-1 - D2-4; değişmeyen metinler ("Görevliler", "Kişileri ara", "Bir kişi seçin.", kart düğmeleri) yerinde.

## 8. Kapsam dışı / yapılamayan

768, 1024, 1920 px ve reduced-motion ölçülmedi; analistin şartı 320 (%200), 360 ve 1440 px. Değişiklik yalnız metin.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| - | yok | - | - |

## 10. Analist kararı

```
D-2: TESLİM ALINDI
Tarih: 2026-11-17
Gerekçe: Ölçüm şartı 4/4, bulgu 0; 16 hücrede taşma ve konsol 0, Faz 6C K16 ve Faz 6E K3 gerilemesi yok. Kendim okudum:
  _Participants.cshtml:204-205 özet "Yardımcı ekle" ve user-plus ikonu, :210 ve :224 "Yardımcı ara", :232 ipucu, :258 "Yardımcı seç",
  :300 "Yardımcıyı ekle"; metinler PL-106 tablosuyla aynı.
Açık kalem: yok.
```
