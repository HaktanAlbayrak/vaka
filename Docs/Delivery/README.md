# Teslim Raporları

Her faz için tek dosya: `Faz-<N>-Teslim.md`. Biçim sabittir: [_Template.md](_Template.md) başlıkları ve tablo kolonları değiştirilmez,
bölüm silinmez (boşsa "yok" yazılır). Rapor, faz commit'iyle **aynı commit'te** repoya girer.

- Yazan: qa-tester (bölüm 1-9). Bölüm 10 "Analist kararı" yalnız analyst tarafından doldurulur.
- Tur 2: aynı dosyaya `## Tur 2 (<tarih>)` bölümü eklenir; yalnız yeniden test edilen maddeler ve güncel karar yazılır. Bulgu tablosundaki "Durum" kolonu güncellenir.
- Kanıt dosyaları: `Evidence/Faz-<N>/`, faz başına en çok 10 dosya. Diğer tüm ekran görüntüleri `.qa/` altında kalır (gitignore).

## Şiddet

| Şiddet | Anlam | Kapı etkisi |
|---|---|---|
| KRİTİK | Sayfa düşüyor, veri sızıyor, yetki atlanıyor, veri kaybı | Teslim alınmaz |
| YÜKSEK | Kırık yerleşim, kabul kriteri karşılanmıyor, policy/DROP kuralı ihlali, erişilemeyen işlev | Teslim alınmaz |
| ORTA | Küçük taşma, eksik durum (boş/yükleniyor/hata), eksik etiket | Aynı fazda kapanır |
| DÜŞÜK | Cila | Yazılı gerekçeyle ertelenebilir |

Kanıtsız bulgu yazılmaz. qa-tester düzeltme yapmaz; bulgu sahibine gider.
