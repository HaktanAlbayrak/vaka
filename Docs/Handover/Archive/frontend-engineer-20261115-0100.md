# Devir notu: frontend-engineer, Faz 2 yerleşim ön turu düzeltmesi

Önceki not: Archive/frontend-engineer-20261114-2359.md. G11 imzaları değişmedi; PL-39 istisnasındaki 1024-1279 beklentisi (`Ad:2@1 Kod:1@2 Makine:1@2 Durum:1@3 İşlem:1@3`) artık CSS ile sağlanıyor.

## Düzeltmeler (yalnız `Styles/app.css`, `wwwroot/css/app.css` derlendi)
1. Hat tablosu 1024-1279: `.table-stack.table-stack-rail .stack-lead` için `lg:max-xl:order-first` eklendi. Ad ilk satıra geçer. `max-md:order-first` ile birlikte 768-1023 etkilenmez. Lot tablosunda `stack-lead` yok, değişmedi.
2. Kod hücresi taşması: `.table-stack td` için `max-xl:min-w-0` (grid öğesi içerik genişliğine kilitlenmesin) ve `overflow-wrap: anywhere`. Ortak kural olduğundan tesis, makine, malzeme, lot, kullanıcı (sicil no) tüm kart tablolarını kapsar; görünüm dosyası değişmedi.
3. 360 başlık taşması: `.page-title` ve `.page-lead` için `overflow-wrap: anywhere`. `_Layout` sarmalayıcısında `min-w-0` zaten vardı. Tüm sayfalara uygulanır.

## Öz denetim
- `npm run build:css` tamam; derlenen CSS'te kural var. `dotnet build -warnaserror -o .qa/fe/build`: 0 uyarı, 0 hata.
- `app.css` üzerinde em/en-dash, `transition: all`, `outline-none`, `h-screen`, `onclick=` grep: 0.
- **Ölçülmedi:** tarayıcıda 360/768/1024 yerleşimi ve taşma; QA yeniden ölçmeli. `anywhere` xl tablo modunda da etkili, sütun genişliğini düşürmesi olası ama kod hücreleri tek satırda kalmalı; 1440'ta QA bakmalı.

## Backend'den beklenen/eksik
Yok. /machines/edit/30 (kaldırılmış makine düzenlenebiliyor) karar dışı, dokunulmadı.
