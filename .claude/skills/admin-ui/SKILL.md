---
name: admin-ui
description: Admin dashboard UI kuralları (Razor + Tailwind CLI + vanilla JS) - özgün tasarım DNA'sı, responsive ve erişilebilirlik standartları, UI teslim kuralları. cms-codebase html-to-cms-v2 tasarım dosyalarından dashboard'a uyarlanmıştır.
---

# admin-ui

Kaynak: `C:\projects\cms-codebase\.claude\skills\html-to-cms-v2` (design-dna, design-vocab, design-patterns §4, visual-qa) ve
`web-design-guidelines`. Kurumsal site kuralları (hero, SEO, hreflang, çok dil) çıkarıldı; dashboard karşılıkları eklendi.
**Teslim kuralları (`UiDelivery.md`) cms-codebase ile aynıdır**: hafif tur, tam tur, mekanik eval, guideline grep tablosu, şiddet ve tur sınırı.

| Dosya | İçerik | Okur |
|---|---|---|
| `DesignDna.md` | Design Read, kadranlar, DNA kartı, anti-slop yasakları, özgünlük kapısı | frontend-engineer, qa-tester (Faz T) |
| `UiRules.md` | Tema token'ları, Tailwind, bileşen ve durum kuralları, responsive taban, hareket, erişilebilirlik, JS | frontend-engineer |
| `UiDelivery.md` | Öz denetim turu, tam tur, mekanik eval, grep tablosu, responsive matris | frontend-engineer (§1, §4), qa-tester (tamamı) |
| `../web-design-guidelines/command.md` | Vercel arayüz kurallarının tam metni | gerekince |

Ajanlar `Skill` aracını çağırmaz; bu dosyaları yoldan okur. `playwright-cli` yalnız qa-tester'ın faz tesliminde ve kullanıcı açıkça isteyince kullanılır.
