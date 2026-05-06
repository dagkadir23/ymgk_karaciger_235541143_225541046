# PatientLive - MVP Sahne Kurulum Rehberi

Aşağıdaki adımlar, kodların hazır olduğu ama Unity sahne entegrasyonunun henüz yapılmadığı duruma göre yazılmıştır. Amaç: mevcut scriptleri Editor içinde doğru objelere bağlayıp Hafta 7 için demo alınabilir bir MVP sahnesi kurmaktır.

*(Not: Projedeki `SceneBuilder.cs` scripti bu adımların büyük bir kısmını otomatize etmektedir. Manuel kontrol ve kurulum için bu rehberi takip edebilirsiniz.)*

## 1. Sahne Kurulumu

Unity’de önce sahneyi oluştur:

1. `File > New Scene` seç.
2. Sahneyi kaydet:
   - `File > Save As`
   - Path: `Assets/Scenes/PatientLive_MVP.unity`
3. Hierarchy’de şu temel yapıyı oluştur:

```text
PatientLive_MVP
├── AppManager
├── MainContentRoot
│   ├── Main Camera
│   ├── Directional Light
│   ├── LiverModelRoot
│   └── InteractionManager
└── Canvas
    ├── SafetyWarningPanel
    └── DiseaseInfoPanel
```

`MainContentRoot`, güvenlik uyarısı kabul edilene kadar kapalı tutulacak ana içerik grubudur.

**Main Camera ayarları:**
- Position: 0, 1.2, -4
- Rotation: 10, 0, 0
- Projection: Perspective
- Field of View: 45
- Near Clip: 0.1
- Far Clip: 100
- Tag: MainCamera

**Directional Light ayarları:**
- Rotation: 45, -30, 0
- Intensity: 1
- Shadow Type: Soft Shadows

**`AppManager` oluştur:**
1. `GameObject > Create Empty`
2. Adı: `AppManager`
3. `AppInitializer.cs` scriptini ekle.
4. Inspector’da şimdilik alanları boş bırakma; birazdan bağlayacağız.

## 2. Karaciğer Modeli Entegrasyonu

Model dosyanı şu klasöre koy: `Assets/Models/Liver` (Örneğin: `Assets/Models/Liver/liver_model.fbx`)

Sonra sahneye ekle:
1. Modeli Project penceresinden Hierarchy’ye sürükle.
2. Modeli `MainContentRoot` altına taşı.
3. Boş parent oluştur (`GameObject > Create Empty`), adını `LiverModelRoot` yap.
4. Modeli `LiverModelRoot` altına child olarak taşı.

Yapı şöyle olmalı:
```text
MainContentRoot
└── LiverModelRoot
    └── LiverModel
```

`LiverModelController.cs`, modelin kendisine değil parent objeye bağlanmalı:
`LiverModelRoot -> Add Component -> LiverModelController`

Önerilen transform:
- `LiverModelRoot`: Position: 0, 0, 0 | Rotation: 0, 0, 0 | Scale: 1, 1, 1
- `LiverModel` child objesi: Position: 0, 0, 0 | Rotation: modele göre | Scale: modele göre 0.5 - 2 arası

*Model çok büyük/küçük görünüyorsa scale’i `LiverModel` child üzerinde ayarla. `LiverModelRoot` scale’i mümkünse `1,1,1` kalsın.*

## 3. Etkileşim Sistemi

Boş obje oluştur: `MainContentRoot > Create Empty` (Adı: `InteractionManager`)
Üzerine şu scripti ekle: `ModelInteractionController`

**Inspector alanlarını bağla:**
- Liver Model: LiverModelRoot üzerindeki LiverModelController
- Info Panel: Canvas altındaki DiseaseInfoPanel
- Raycast Camera: Main Camera
- Raycast Distance: 100
- Click Threshold: 8

**Testler:**
- **Mouse rotate testi:** Play’e bas, güvenlik popup’ını kapat, sol mouse tuşu ile modeli sürükle. Model dönmeli.
- **Zoom testi:** Mouse scroll yukarı: yakınlaşma, aşağı: uzaklaşma.

*Eğer mouse input çalışmıyorsa: `Edit > Project Settings > Player > Active Input Handling` "Input Manager (Old)" veya "Both" seçili olmalı.*

## 4. Hastalık Marker Sistemi

`LiverModelRoot` altında üç sphere oluştur (`GameObject > 3D Object > Sphere`):
- `Marker_Tumor`
- `Marker_Cyst`
- `Marker_Healthy`

**Önerilen değerler:**
- **Tumor:** Position: 0.35, 0.15, -0.15 | Scale: 0.15, 0.15, 0.15 | Material Color: Red
- **Cyst:** Position: -0.25, 0.05, 0.18 | Scale: 0.13, 0.13, 0.13 | Material Color: Yellow
- **Healthy:** Position: 0.05, 0.28, 0.12 | Scale: 0.11, 0.11, 0.11 | Material Color: Green

Her marker’da şunlar olmalı: Mesh Renderer, Sphere Collider, `DiseaseRegion.cs`

**DiseaseRegionData oluşturma:**
1. Project penceresinde `Assets/Data/DiseaseRegions` klasörü oluştur.
2. Sağ tık: `Create > PatientLive > Disease Region Data`
3. Üç asset oluştur: `TumorRegionData`, `CystRegionData`, `HealthyRegionData` ve içeriklerini doldur.
4. Her marker’daki `DiseaseRegion` scriptine ilgili veriyi bağla.

## 5. UI Bilgi Paneli

Canvas oluştur (`GameObject > UI > Canvas`)
- Render Mode: Screen Space - Overlay
- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1920 x 1080
- Match: 0.5

*(TextMeshPro kullanacaksan ilk kullanımda Unity TMP import isterse "Import TMP Essentials" tıkla)*

**`DiseaseInfoPanel` oluştur:**
1. Canvas altında panel oluştur, adını `DiseaseInfoPanel` yap.
2. Sağ tarafa konumlandır (Anchor: Middle Right, Width: 420, Height: 260, Pos X: -240).
3. İçine `TitleText`, `TypeText`, `DescriptionText`, `TypeColorIndicator` ekle.
4. Root objesine `DiseaseInfoPanel.cs` scriptini ekle ve Inspector'dan referansları bağla.
5. **Kritik Bağlantı:** `InteractionManager > ModelInteractionController > Info Panel` kısmına Canvas altındaki `DiseaseInfoPanel` objesini bağla.

## 6. Güvenlik Uyarısı Popup’ı

Canvas altında `SafetyWarningPanel` oluştur.
- Anchor: Stretch Full Screen, Background Color: siyah (alpha 180).
- İçine `WarningBox` oluştur ve alt elemanları ekle: `TitleText`, `WarningText`, `AcceptButton`.
- `SafetyWarningPanel` objesine `SafetyWarningController.cs` scriptini ekle ve referansları bağla.

**`AppInitializer` ile ilişki (AppManager objesine dön):**
- Safety Warning: SafetyWarningPanel
- Main Content Root: MainContentRoot
- Target Frame Rate: 60

## 7. Logging ve Hata Kontrolü

Play modunda Console’da beklenen loglar:
```text
[PatientLive] PatientLive prototype starting.
[PatientLive] Performance configured...
[PatientLive] Safety warning shown.
[PatientLive] Safety warning accepted by user.
[PatientLive] Region selected: Örnek Tümör Bölgesi
```

## 8. Prefab Yapısı

Oluşturduğunuz yapıları `Assets/Prefabs/` altına klasörleyerek (`Liver`, `Markers`, `UI`) prefab yapın. Bu, sahne dışında da nesnelerin kopyalanmasını kolaylaştırır.

## 9. Demo Video Akışı (60-90 saniye)

1. Unity Play Mode açılır. Güvenlik popup’ı görünür.
2. "Bu prototip teşhis amaçlı değil, hasta bilgilendirme amaçlıdır." sesli/metin açıklaması.
3. Anladım butonuna basılır. Karaciğer modeli görünür.
4. Mouse ile model yavaşça döndürülür, zoom yapılır.
5. Tümör, kist ve sağlıklı doku markerlarına tıklanıp bilgi panelinin güncellendiği gösterilir.
6. Kapanış.

## 10. Kontrol Listesi

- [ ] PatientLive_MVP.unity sahnesi açılıyor mu?
- [ ] Console’da kırmızı hata yok mu?
- [ ] Main Camera modeli görüyor mu?
- [ ] AppManager üzerinde AppInitializer bağlı mı?
- [ ] Güvenlik popup’ı açılışta görünüyor ve kapanıyor mu?
- [ ] InteractionManager'da ModelInteractionController var mı ve bağlı mı?
- [ ] Mouse rotate ve zoom çalışıyor mu?
- [ ] Marker objelerinde Collider ve DiseaseRegion scriptleri var mı?
- [ ] Tıklayınca DiseaseInfoPanel güncelleniyor mu?
- [ ] Boş alana tıklayınca seçim kapanıyor mu?
- [ ] File > Build Settings içinde sahne eklendi mi?
