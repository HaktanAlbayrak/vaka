# UI Teslim Kuralları

> cms-codebase `html-to-cms-v2/visual-qa.md`, `cms-qa-responsive` ve `docs/QA-SWARM.md` ile aynı kurallar; yollar ve sayfa örneklemi bu projeye uyarlandı.
> Okur: frontend-engineer (§1, §4 öz denetim), qa-tester (tamamı, faz tesliminde).
> **Tarayıcı politikası:** varsayılan **hafif tur** (§1, tarayıcısız). `playwright-cli` yalnız qa-tester'ın faz tesliminde ve kullanıcı açıkça isteyince.

## 0. Ortam (qa-tester)

Kaynak DB'ye ve çalışma klasörüne dokunulmaz; Release yayını ve DB kopyası `.qa/` altında:

```bash
dotnet publish Src/Vaka.Web -c Release -o .qa/build
cp <geliştirme db> .qa/qa.db        # yoksa: boş dosyada dbadmin'in migration + seed adımı QA kopyasına uygulanır
( cd .qa/build && ASPNETCORE_ENVIRONMENT=Production ConnectionStrings__Default="Data Source=../qa.db" dotnet Vaka.Web.dll --urls http://localhost:5101 > ../run.log 2>&1 & )
```

Port çakışırsa başka boş port. İş bitince **yalnız kendi PID'in** kapatılır (`netstat -ano | grep ":5101 "` -> `taskkill //PID <pid> //F`);
makine geneli süreç öldürme yok. Playwright oturumu `close` ile kapatılır. Commit'lenmemiş çalışma ağacı test ediliyorsa raporda belirtilir.

## 1. Hafif tur (tarayıcısız, her öz denetimde)

`dotnet build` (uyarı 0) -> `npm run build:css` -> uygulamayı boş portta başlat -> giriş yap (cookie jar) -> fazın her sayfası için:

```bash
curl -s -c .qa/jar -b .qa/jar -o /dev/null -w "%{http_code} %{size_download}\n" http://localhost:5101/<yol>   # 200
curl -s -b .qa/jar http://localhost:5101/<yol> | grep -cE '<h1'                                              # 1
curl -s -b .qa/jar http://localhost:5101/<yol> | grep -oE '[—–]' | wc -l                                     # 0
curl -s -b .qa/jar http://localhost:5101/<yol> | grep -oE '<img[^>]*>' | grep -vcE 'width=|height='           # 0
curl -s -b .qa/jar http://localhost:5101/<yol> | grep -oE '(src|href)="https?://[^"]+"' | grep -v localhost   # dış kaynak yok
curl -s -o /dev/null -w "%{http_code}\n" http://localhost:5101/<korumalı yol>                                 # oturumsuz 302
```

Sonra §4 grep tablosu kaynak dosyalarda; süreç kapatılır. Görsel kontrol qa-tester'a kalır.

## 2. Tam tur (playwright-cli, faz tesliminde)

```bash
playwright-cli -s=qa open http://localhost:5101/<yol>
playwright-cli -s=qa resize 1440 900 && playwright-cli -s=qa screenshot --filename=.qa/shots/<sayfa>-1440.png
playwright-cli -s=qa resize 768 1024 && playwright-cli -s=qa screenshot --filename=.qa/shots/<sayfa>-768.png
playwright-cli -s=qa resize 390 844  && playwright-cli -s=qa screenshot --filename=.qa/shots/<sayfa>-390.png
playwright-cli -s=qa set-color-scheme dark && playwright-cli -s=qa reload
playwright-cli -s=qa set-reduced-motion reduce && playwright-cli -s=qa reload
playwright-cli -s=qa console warning          # 0 hata/uyarı
playwright-cli -s=qa requests                 # 4xx/5xx yok, dış kaynak yok
```

- **Matris:** genişlikler 360, 768, 1024, 1440, 1920 (mobilde dikey + yatay); modlar açık/koyu, reduced-motion, %200 yakınlaştırma (320 CSS px reflow).
  Sayfalar: fazın **tüm** yeni/değişen ekranları + özet ekranı + giriş + 403/404. Durumlar: dolu, boş liste, uzun metin, doğrulama hatası.
- **Sayısal önce, görüntü sonra:** her hücrede §3 eval çalışır. Ekran görüntüsü yalnız **başarısız hücrede** alınır ve `Read` ile açılır;
  geçen hücre için görüntü alınmaz. İstisna: Faz T'de her prototip sayfası 3 genişlikte (1440/768/390) görüntülenir (özgünlük kapısı).
- Görüntüler `.qa/` altında (gitignore). Rapora kanıt olarak en çok **10 dosya** `Docs/Delivery/Evidence/Faz-<N>/` altına kopyalanır.
- **Klavye turu:** `press Tab` ile ilk odak atlama bağlantısı; odak halkası her etkileşimli öğede görünür; yapışkan üst bar odağı örtmez;
  çekmece/modal Esc ile kapanır ve odak tetikleyiciye döner; mobil menü `aria-expanded` değişir; boş form gönderimi satır içi hata verir ve ilk hatalı alana odaklanır.
- **Yalnız insan gözü isteyenler:** DNA uygunluğu (kart maddeleri tek tek), grafiklerin okunurluğu, koyu tema ve reduced-motion görünümü, iskelet/boş/hata durumlarının görsel dili.

## 3. Mekanik eval (her hücrede)

```bash
playwright-cli -s=qa --raw eval "JSON.stringify({
  h1: document.querySelectorAll('h1').length,
  skip: !!document.querySelector('a[href=\"#main\"]'),
  imgNoAlt: [...document.images].filter(i=>!i.hasAttribute('alt')).length,
  imgNoSize: [...document.images].filter(i=>!i.getAttribute('width')||!i.getAttribute('height')).length,
  inputNoLabel: [...document.querySelectorAll('input,textarea,select')].filter(e=>e.type!='hidden'&&!e.labels?.length&&!e.getAttribute('aria-label')).length,
  iconBtnNoName: [...document.querySelectorAll('button,a')].filter(e=>!e.textContent.trim()&&!e.getAttribute('aria-label')&&!e.querySelector('img[alt]')).length,
  emdash: /[—–]/.test(document.body.innerText),
  font: getComputedStyle(document.body).fontFamily,
  hScroll: document.documentElement.scrollWidth > innerWidth,
  overflowing: [...document.querySelectorAll('body *')].filter(e=>{const r=e.getBoundingClientRect();return r.width&&r.right>innerWidth+1&&!e.closest('[data-scroll-x]')}).length,
  smallTargets: [...document.querySelectorAll('a,button,input,select,[role=button]')].filter(e=>{const r=e.getBoundingClientRect();return r.width&&(r.height<40||r.width<40)}).length,
  extScripts: [...document.scripts].map(s=>s.src).filter(s=>s&&!s.includes(location.host))
})"
```

Beklenen: `h1` 1, `skip` true, `imgNoAlt` 0, `imgNoSize` 0, `inputNoLabel` 0, `iconBtnNoName` 0, `emdash` false, `hScroll` false,
`overflowing` 0 (yatay kaydırmalı tablo sarmalayıcısı `data-scroll-x` taşır), `smallTargets` 0 (24px altı YÜKSEK, 24-40px ORTA; hedef 44px),
`extScripts` [], `font` DNA ön bloğundaki sans ile aynı (`document.fonts.check` true).

Ek sayısal kontroller: üst bar tek satır ve en çok 72px; mobilde gövde metni en az 16px; satır uzunluğu en çok 75ch;
1920'de içerik `max-w` ile sınırlı ya da anlamlı dolu; koyu temada metin kontrastı AA.

## 4. Guideline grep tablosu (kaynak: `Views/**`, `Styles/app.css`, `wwwroot/js/**`)

| Kalıp | Kural |
|---|---|
| `transition-all`, `transition: all` | Yasak; özellik say |
| `outline-none` | Yalnız `focus-visible:` halka ile |
| `<img` boyutsuz | `width/height` zorunlu; ekran altı `loading="lazy"` |
| `<input` autocomplete/inputmode yok | Ekle; e-posta/kod alanında `spellcheck="false"` |
| ikon-only `<button>` | `aria-label` + ikon `aria-hidden` |
| `user-scalable=no`, `maximum-scale` | Yasak |
| `<div onclick>`, `<span onclick>`, eylem için `<a>` | Eylem `button`, gezinme `a` |
| animasyon + `prefers-reduced-motion` yok | Her animasyon kapanmalı |
| `@latest`, `unpkg`, `jsdelivr`, `cdnjs`, `googleapis` | Self-host, sürüm sabit, `defer` |
| `[—–]` | Em/en-dash yok |
| `h-screen` | `min-h-[100dvh]` |
| `...` (görünür metinde) | `…` |
| `jquery`, `gsap`, `aos` | Yasak (vanilla JS) |
| ham hex / `slate-`, `gray-`, `zinc-` sınıfı görünümlerde | Token sınıfı kullan (`_Theme.cshtml` dışı) |

Çıktı `dosya:satır - sorun`; ihlaller **aynı fazda** kapanır. Tam kural metni `../web-design-guidelines/command.md`.

## 5. Kapı kuralları

- frontend-engineer devri: §1 hafif tur temiz + §4 sıfır ihlal. Tarayıcı kullanmaz.
- qa-tester faz teslimi: §2 matrisinde kırık hücre yok, §3 beklenen değerler, §4 sıfır ihlal, konsol 0 hata, DNA sapması yok.
  KRİTİK/YÜKSEK açıkken öneri KALDI. En çok **2 tur**; tur 2 yalnız başarısız hücreler.
- Bulgu biçimi: genişlik, sayfa, mod, seçici ya da `dosya:satır`, kanıt yolu, şiddet, sahibi. Kanıtsız bulgu yok; qa-tester düzeltme yapmaz.
- Ekran görüntüleri repoya girmez (`.qa/`); yalnız en çok 10 kanıt dosyası.
