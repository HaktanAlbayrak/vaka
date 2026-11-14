# Frontend devir notu: Faz 1 tur 2 (B1, B6)

## Değişen
- `Styles/app.css`: `.table-stack .stack-wide` artık `max-xl:col-span-2`. 768 altı kart 2 sütun, yani tam satır; 768-1279 px kart 4 sütun, yani en çok iki sütun. Anahtar sınıf adı aynı. `.switch-btn` devre dışı biçimi eklendi (`disabled:cursor-not-allowed disabled:opacity-50`). `wwwroot/css/app.css` `npm run build:css` ile yeniden üretildi.
- `Views/Users/Index.cshtml`: `stack-wide` yalnız Ad soyad hücresinde; Roller, Durum, İşlem hücrelerinden kalktı.
- `Views/Prototype/Records.cshtml` (PL-28 kontrolü): kayıt no ve Durum hücrelerinden `stack-wide` kalktı, yalnız Makine (tek uzun metin) kaldı. Başka table-stack kullanımı yok.
- `Views/Shared/_SwitchUser.cshtml`: iki gerçek `POST /account/logout` düğmesi (üst bar, ray ve çekmece) `data-requires-online` taşıyor. Kapsam notu: demo kipindeki `<a>` bağlantısı (`demoSwitchHref`, yalnız prototip) etkilenmedi; `disabled` bağlantıda çalışmaz.
- `Views/Shared/_Layout.cshtml:202`: şerit metni "Bağlantı yok. Kayıt, onay ve kullanıcı değiştirme yapılamaz. Bağlantı gelince bu uyarı kapanır, işlemi yeniden yapın."

## Ölçülen
- `npm run build:css`: başarılı. Üretilen CSS'te `stack-wide{grid-column:span 2/span 2}` var.
- Görünüm, Styles ve JS'te em/en-dash 0. UiDelivery §4 grep (`transition-all`, `outline-none`, `h-screen`, `onclick=`, `cdn.`, `@latest`) Views ve Styles/app.css'te 0.

## Ölçülmedi, QA ölçecek
- `dotnet build -warnaserror`: tamamlanamadı. `bin` altındaki `Vaka.Web.exe` başka süreç tarafından kilitli (benim süreçlerim değil); ayrı çıktı klasörüyle denemede de backend'in yarım `UserAdministration.cs` değişikliği (`SqliteLocked` tanımsız) ve eşzamanlı derleme dosya kilidi hata verdi. Hatalar frontend dosyalarında değil; Razor derleme sonucu temiz doğrulanamadı.
- Hücre genişlikleri (768/1024'te Ad soyad en çok iki sütun, diğerleri bir; 360'ta dört hücre ikişer yan yana; 108 karakterli ad kırılması; 1440 tablo ve 56 px satır) tarayıcıda ölçülmedi. Dna.md ile uyum yalnız CSS kuralından çıkarım.
- Çevrimdışıyken üç düğmenin devre dışı kalması ve bağlantı gelince açılması tarayıcıda ölçülmedi; mekanizma mevcut `ConnectionStatus.js` (`data-requires-online`), JS değişmedi. Odak kipinde ("bar-always") ve rail/drawer görünümleri ayrıca ölçülmeli.

## Backend'den beklenen/eksik
- Yok. Derleme için backend düzeltmeleri tamamlanınca `dotnet build -warnaserror` yeniden koşulmalı.
