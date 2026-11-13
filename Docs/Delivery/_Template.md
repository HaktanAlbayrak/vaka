# Faz <N> Teslim Raporu: <başlık>

| Alan | Değer |
|---|---|
| Faz | <N> |
| Tarih | <yyyy-mm-dd> |
| Tur | 1 |
| Test edilen | `<branch>` @ `<kısa hash ya da "çalışma ağacı">` |
| Ortam | LOCAL, Release build, port <port>, DB: `.qa/` kopyası |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ / KALDI |
| Analist kararı | BEKLİYOR |

## 1. Kapsam

<Faz planından: bu fazda teslim edilen iş kalemleri, tek satırlık liste.>

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | | | <komut çıktısı, dosya:satır ya da Evidence yolu> |

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | |
| CSS | `npm run build:css` | |
| Test | `dotnet test` | |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme |
| Adlandırma (PascalCase) | örneklem denetimi | |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal |
| Mekanik eval | `UiDelivery.md §3` | beklenen değerler |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | |

## 4. Yetki matrisi

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| | 302 | 403 | 200 | 200 |

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| | | | | | | | | | |

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri): <madde madde UYGUN / SAPMA>

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | | | | | | AÇIK |

## 7. Geçenler

<kısa liste>

## 8. Kapsam dışı / yapılamayan

<madde ve nedeni>

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|

## 10. Analist kararı

```
FAZ <N>: TESLİM ALINDI | İADE
Tarih: <yyyy-mm-dd>
Gerekçe: <kabul kriterleri ve bulgulara göre 1-3 satır>
İade maddeleri: <bulgu no, sahibi>   Yeniden denetim şartı: <...>
```
