---
name: web-design-guidelines
description: Review UI code against Vercel Web Interface Guidelines (local copy in command.md). Use for "review my UI", "check accessibility", "audit design".
metadata:
  author: vercel
  version: "1.0.0"
  argument-hint: <file-or-pattern>
---

# Web Interface Guidelines

Kurallar yerel kopyada: `command.md` (bu klasör). Ağdan çekme; kopya aylık güncellenir:

```bash
curl -sL https://raw.githubusercontent.com/vercel-labs/web-interface-guidelines/main/command.md -o .claude/skills/web-design-guidelines/command.md
```

Kullanım: `command.md`'yi oku, belirtilen dosyaları (`Views/**`, `Styles/app.css`, `wwwroot/js/**`) kurallara göre tara,
çıktıyı `dosya:satır - sorun` biçiminde ver. Çekirdek Grep tablosu `admin-ui/UiDelivery.md §4`'tedir.
