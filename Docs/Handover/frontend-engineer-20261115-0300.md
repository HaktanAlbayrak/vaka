# Devir notu: frontend-engineer, Faz 2 tur 2 (ön tur düzeltmesi)

Önceki not: Archive/frontend-engineer-20261115-0200.md.

## Düzeltmeler (B2)
- `Styles/app.css` tablo modu (xl ve üstü): `.table-stack td` `overflow-wrap: break-word` (en küçük içerik genişliğini düşürmez; sığan veri kırılmaz, sütunlar sözcük genişliğine göre boyutlanır). Kart modu (`max-xl`) `anywhere` aynen kalır.
- `.table-stack .chip` istisna listesine eklendi. xl ve üstünde `.btn`, `.badge`, `.chip` `white-space: nowrap` (Süper admin, Yalnız görüntüleme, SKT geçmiş, Kullanılabilir tek satır). Kart modunda nowrap yok, imza değişmez.
- `Machines/Form.cshtml`: kaldırılmış makinede "Hat değiştirilemez." ipucu gösterilmez.

## Öz denetim
`npm run build:css` tamam; `dotnet build -warnaserror -o .qa/fe/build` 0 hata; §4 grep (transition-all, outline-none, onclick, h-screen, dash) 0. Tarayıcıda ölçülmedi.
Risk: gerçekten sığmayan 64 karakterlik lot no gibi veride `break-word` min-content'i düşürmediğinden tablo 1280-1366'da taşabilir (QA'nın `normal` denemesindeki gibi); taşma sarmalayıcıda kaydırma olarak kalır. Bu istenmiyorsa hücreye `max-width` verilir, QA ölçümüne göre.

## G11 beklenen imzası
Değişmedi (kart modu kuralları aynı, yalnız `max-xl` dışı davranış değişti).

## Backend'den beklenen/eksik
Yok.

## Ek tur: taşma gerilemesi (üçüncü sürüm)
- `.cell-text` ve `cqw` sınırı kaldırıldı (sığan değeri de kırıyordu), `.panel` `container-type` geri alındı.
- Ray yanı tablolarda (`.table-stack-rail`, xl ve üstü) tek uzun sütun: `td.cell-long` `width: 100%; white-space: normal; overflow-wrap: anywhere`; diğer tüm td `white-space: nowrap` (içerik genişliğine kadar daralır). Uzun sütun kalan alanı alır, değer sığıyorsa kırılmaz, sığmıyorsa yalnız o hücrede kırılır, tablo panel genişliğini aşmaz. Hat kodu nowrap (en çok 16 karakter).
- Uygulandı: `Materials/Details` Lot no, `Facilities/Details` hat Adı. Kart modu (`max-xl`) kuralları ve imzası değişmedi.
- Ölçülmedi. Risk: nowrap sütunların toplamı (rayla 592 px panelde) panele sığmazsa taşma olur; mevcut sütunlarda (kod 16 karakter, rozet, düğme) beklenmiyor.
- Öz denetim: build:css tamam, `dotnet build -warnaserror` 0 uyarı 0 hata. §4 grep kalıpları görünümlerde değişmedi (yalnız sınıf ekleme/çıkarma); tam tabloyu önceki turda QA 0 ihlal buldu.
