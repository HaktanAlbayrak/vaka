DEVİR: frontend-engineer -> koordinatör   Faz: 6F (FE 4, 6)   Dosya: Docs/Handover/frontend-engineer-20261117-0055.md

Yapıldı:
- `Views/Account/Login.cshtml`: üstte "Giriş yöntemi" gezinmesi (`.login-mode`): "PIN ile" ve "Parola ile" iki bağlantı, `/account/login?mode=pin|password` (+ `returnUrl`), geçerli olan `aria-current="page"`. JS gerekmez, GET ile çalışır. Gizli alan `Mode`. PIN modunda `RegistryNo` (`autocomplete="off"`) ve `_PinInput` (`Pin`, numeric, off); form `autocomplete="off"`; alanlar GET'te boş. Parola modunda eski iki alan. Altta mod başına açıklama: PIN modunda "PIN ile giriş operatör hesapları içindir; Yönetici, Denetçi ve Süper admin parola ile girer. İlk girişinizi parola ile yapın." Lead mod başına değişir.
- Kilit: `Model.LockedUntilLocal` doluysa danger bildirimi "Çok sayıda hatalı giriş. Giriş HH:mm saatine kadar kapalı; yöneticiniz erken açabilir." (saat mono, `role="alert"`, `data-form-summary`). Kilit varken `ErrorMessage` çizilmez (kilit metni önceliklidir). Aksi halde `ErrorMessage` mevcut bildirim.
- `Views/Users/Form.cshtml` (E-04): hesap durumu paneline `LoginLockedUntilLocal` doluysa "Giriş kilitli" rozeti ve "Giriş HH:mm saatine kadar kapalı, süre dolunca kendiliğinden açılır." metni; kilit yok ve `LoginFailedCount > 0` ise sayaç satırı. `CanUnlockLogin` doluysa "Giriş kilidini aç" açılır paneli: form `/users/unlock-login/{Id}`, PIN alanı `UnlockLogin.Pin` (kimlik `UnlockLogin-Pin`); hata sonrası yol `/unlock-login/` içerirse panel açık gelir (diğer panellerle aynı).
- `Styles/app.css`: `.login-mode`, `.login-mode-item` (ikili, 44 px yüksekliğinde, seçili 2 px accent kenarlık). `wwwroot/css/app.css` yeniden üretildi.
- `Styles/BuildAssets.mjs`: ikon listesine `hash`, `lock-open` (`npm run build:assets`, `wwwroot/lib/icons/Icons.svg` yeniden üretildi). Dosya sınırında açıkça yok, ikon kaynağı olduğu için dokunuldu.
- B4 (K14) `wwwroot/js/FocusFirst.js`: `checkVisibility` yoksa `getClientRects().length > 0` ve kapalı `details` içinde (özet dışında) olmama koşuluyla eleme.

Dört durum: dolu, hata (`ErrorMessage` ya da kilit metni, alan hataları alan altında), yükleniyor (`data-busy-form`). Boş durum yok (form).

Kabul kriterleri (FE payı): K1/K8 mod seçimi, boş alanlar, `autocomplete="off"`, PIN `inputmode="numeric"` (çıktıda sayıldı: 3 off, 1 numeric); K3/K4 kilit metni ve E-04 açma düğmesi; K11 ölçüldü (aşağıda); K14 ölçüldü.

Ölçüm düzeni: kendi derlemem `.qa/f6f/bin`, kaynak kopyası `.qa/f6f/src` (repoya girmez), DB `.qa/f6e2/qa.db` (+wal) kopyası `.qa/f6f/qa.db`, port 5851, Playwright betikleri `.qa/f6f/*.mjs`. Backend servisleri ve migration henüz hazır olmadığından kopyada iki sahte var: `AccountController` POST'ta sicil `9999` kilit (`LockedUntilLocal`), PIN modunda sabit "Sicil no ya da PIN hatalı."; `UserDirectory` kullanıcı formunda `CanUnlockLogin = true`, kilit 14 dk sonra, sayaç 5. Gerçek PIN girişi, kilit oluşması, açma POST'u ve çerez ölçülemedi.
- Giriş (PIN, parola, hata, kilit durumları) ve E-04 (tüm `details` açık), 320, 360, 768, 1280, 1440 px x açık ve koyu (40 satır): sayfa yatay kaydırması 0, `main` içinde viewport dışına taşan öğe 0, 44 px altı dokunma hedefi 0. 320 px'te iki rozet yan yana sığıyor.
- Konsol hatası 0 (giriş turu). E-04 turunda 10 hata satırı yalnız betiğin yetkisiz kimlikleri yoklarken aldığı 403 kaynak yanıtlarıdır, sayfa hatası değil.
- K14 (`.qa/f6f/k.mjs`, 360 px): `Element.prototype.checkVisibility` silinmiş ve silinmemiş iki bağlamda aynı sonuç: boş gönderimde odak `RegistryNo`; yanlış PIN ve yanlış parolada odak hata bildiriminde; Run (`/records/run/10`, 2006) yanlış PIN sonrası odak `pin-finish-38`. TypeError 0, konsol 0.
- JS kapalı akış (mod bağlantıları düz GET) elle ölçülmedi, işaretleme gereği çalışır; G12 hizalama turu qa-tester'a.

G11 imzası (ölçüldü, 320-1440 px'in hepsinde tek sütun, sol kenar sabit):
- Giriş, PIN modu: Yöntem:1@1, Sicil no:1@2, PIN:1@3, Giriş yap:1@4. Hata ya da kilit bildirimi Yöntem'den sonra, alanlardan önce gelir (Bildirim:1@2, Sicil no:1@3, PIN:1@4, Giriş yap:1@5). Parola modu aynı, PIN yerine Parola.
- E-04 durum paneli: Rozetler:1@1 ve 2@1 (320 ve 1440'ta ölçüldü, yan yana), Kilit metni:1@2.
- E-04 Giriş kilidini aç paneli: Açıklama:1@1, PIN:1@2, Düğme:1@3.
(G11 planda "uygulanmaz"; imza yine de kayıt için verildi.)

Doğrulama: `npm run build:css` temiz; `dotnet build -o .qa/f6f/bin3` 0 uyarı 0 hata; em/en-dash, transition-all, outline-none, h-screen, onclick, CDN, `...` grep 0. Sunucum (5851) kapatıldı; PID 32568 ve `dotnet run -watch` (20552) dokunulmadı.

Backend'den beklenen / sapma:
- `LoginViewModel`: PIN modunda `Pin` doluluğu ve `Password` zorunsuzluğu sunucu doğrulaması (sicil boşsa "Sicil no girin.", PIN boşsa "PIN girin." `Pin` anahtarına, model hatası olarak). Görünüm yalnız seçili moda ait alanı çizer.
- Başarısız POST'ta `Mode`, `RegistryNo` korunur; `Pin` ve `Password` boşaltılmalı (zaten çizilmez). Kilitli yanıtta `LockedUntilLocal` yerel saat olarak dolu olmalı; görünüm metni kendisi kurar, `ErrorMessage` kilitliyken görmezden gelinir.
- `UserFormViewModel`: `LoginLockedUntilLocal` yalnız kilit sürüyorsa dolu (süresi dolmuşsa null), `CanUnlockLogin` kapsam ve yetkiye göre; `UnlockLogin` (UserResetViewModel) hata dönüşünde ModelState anahtarı `UnlockLogin.Pin`, dönüş yolu `/users/unlock-login/{id}` kalmalı (panelin açık gelmesi buna bağlı).
- Çıkış ve "Kullanıcı değiştir" yönlendirmesi `/account/login?mode=pin` olmalı (backend); görünüm `mode` yoksa PIN varsayılanını denetleyiciden alır.
- `_PinInput` ipucu sabit ("Kendi PIN'inizi girin."); giriş ekranında yerinde, "Hesabınız ya da PIN'iniz" gibi bir söz gerekmez.

Değişen dosyalar: Views/Account/Login.cshtml, Views/Users/Form.cshtml, Styles/app.css, Styles/BuildAssets.mjs, wwwroot/css/app.css, wwwroot/lib/icons/Icons.svg, wwwroot/js/FocusFirst.js.
DB talebi: yok

Metin düzeltmesi (koordinatör, ön tur): ek saate uymadığından kilit metni "Giriş HH:mm saatine kadar kapalı" oldu; `Views/Account/Login.cshtml` ve `Views/Users/Form.cshtml:124` aynı kalıp. Derleme 0 uyarı 0 hata; kilit durumu `.qa/f6f` kopyasında görüldü: giriş "Giriş 01:24 saatine kadar kapalı; yöneticiniz erken açabilir.", E-04 "Giriş 01:23 saatine kadar kapalı, süre dolunca kendiliğinden açılır." Sunucu kapatıldı. Plandaki "HH:mm'e kadar" kalıbı analist tarafında güncellenmeli.

D-2 (PL-106): `_Participants.cshtml` metinleri istenen tabloya göre değişti (panel özeti "Yardımcı ekle" ve `user-plus` ikonu, "Yardımcı ara" iki etiket, ipucu "yardımcıyı seçin", adım sürerken "Eklenen yardımcı...", "Yardımcı seç", "Yardımcıyı ekle"). Listede "Görevliler", kart düğmeleri, "Kişileri ara", "Bir kişi seçin." ve sunucu iletileri aynı. `user-plus` Icons.svg'de vardı, BuildAssets.mjs'e dokunulmadı. Ölçüm (`.qa/f6fqa/qa.db` yedek kopyası `.qa/f6d2/qa.db`, ayrı derleme, port 5851): hazır ve devam eden adımda metinler doğru; 320, 360, 1440 px, açık ve koyu, 6 durumda taşma ve sayfa kaydırması 0; klavyeyle (özet, arama, ok, Enter, PIN, Enter) yardımcı eklendi, konsol hatası 0. Sunucu kapatıldı, 20552 ve gerçek DB'ye dokunulmadı.
