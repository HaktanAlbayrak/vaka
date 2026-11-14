# Devir notu: frontend-engineer, Faz 1 tur 3 (B7)

## Yapılan
- `Styles/app.css`: `.table-stack .stack-lead` (`max-md:order-first`) eklendi; `wwwroot/css/app.css` yeniden derlendi.
- `Views/Users/Index.cshtml`: Ad soyad hücresine `stack-lead` sınıfı eklendi.
- Etki: 768 altında kart Ad soyad (tam satır), sonra Sicil no + Roller, Durum + İşlem. Boşluk yok. 768 ve üstünde sınıf etkisiz (768-1279 ve 1280+ değişmedi).
- Prototip Records: dokunulmadı. Kayıt no etiketsiz kart başlığı olarak tek başına 1. satırda (PL-28, QA T2 1a GEÇTİ); `stack-lead` kullanmaz.

## Sıra notu
`order` yalnız görsel sırayı değiştirir. DOM sırası Sicil no, Ad soyad, ... kaldı (xl tablo sütunları ve başlıklarla uyum için). Kartta odaklanabilir tek öğe İşlem bağlantısı olduğundan odak sırası görsel sıradan sapmaz. Ekran okuyucu 768 altında Sicil no'yu Ad soyad'dan önce okur; hücre etiketleri ("Sicil no", "Ad soyad") yazılı olduğundan anlam bozulmaz.

## Öz denetim
- `npm run build:css`: başarılı; derlenen CSS'te `.stack-lead{order:-9999}` var.
- `dotnet build Src/Vaka.Web -warnaserror -o .qa/fe/build`: 0 uyarı, 0 hata.
- em/en-dash: değişen dosyalarda 0.
- Ölçülmedi: 360/320/768/1280 px'te tarayıcı yerleşimi (görsel tur qa-tester'da). Beklenen: `Ad soyad:2@1 Sicil no:1@2 Roller:1@2 Durum:1@3 İşlem:1@3`.

## Backend'den beklenen/eksik
Yok.
