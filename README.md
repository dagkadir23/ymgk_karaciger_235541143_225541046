# PatientLive: Karma Gerçeklik Tabanlı İnteraktif Hasta Bilgilendirme Sistemi

## 📌 Proje Tanımı
PatientLive, tıp alanında hastaların hastalık süreçlerini ve organlarındaki durumları (özellikle karaciğer hastalıkları) daha iyi anlamalarını sağlamak amacıyla geliştirilmiş, Karma Gerçeklik (Mixed Reality - MR) tabanlı bir 3D interaktif bilgilendirme prototipidir. Bu proje, karmaşık tıbbi verileri hastanın anlayabileceği görsel ve etkileşimli bir formata dönüştürmeyi hedefler. 

**Not:** Bu uygulama kesinlikle teşhis amaçlı değildir. Sadece hasta eğitimi ve bilgilendirmesi amacıyla kullanılmak üzere tasarlanmıştır.

## 🏗️ Sistem Mimarisi ve Modüler Yapı
Proje, Unity oyun motoru üzerinde çalışacak şekilde tasarlanmış olup, SRP (Single Responsibility Principle) prensiplerine sadık kalarak modüler bir mimariyle inşa edilmiştir. İleride Meta Quest veya Microsoft HoloLens gibi MR cihazlarına kolayca entegre edilebilmesi için girdi (input) işlemleri ile iş mantığı (business logic) birbirinden izole edilmiştir.

### 📂 Kod Yapısı ve Modüller (Scripts/)
*   **Core (`AppInitializer.cs`):** Uygulamanın giriş noktasıdır. Başlangıç ayarlarını (FPS sınırı, platform tespiti, VSync) yapar ve güvenlik uyarısı onaylanana kadar ana sistemi bekletir.
*   **Interaction (`ModelInteractionController.cs`, `LiverModelController.cs`, `DiseaseRegion.cs`):** 
    *   Kullanıcı girdilerini (mouse, touch) alır ve işler.
    *   3D karaciğer modelinin döndürülmesini ve yakınlaştırılmasını (zoom) sağlar.
    *   Raycast kullanarak model üzerindeki spesifik hastalık bölgelerinin seçilmesini yönetir.
*   **Data (`DiseaseRegionData.cs`):** ScriptableObject yapısını kullanarak hastalık verilerini (isim, açıklama, tür, renk) Unity Editor üzerinden kod yazmadan yönetilebilir hale getirir.
*   **UI (`DiseaseInfoPanel.cs`, `SafetyWarningController.cs`):** Kullanıcı arayüzünü yönetir. Açılıştaki zorunlu yasal uyarı ekranını ve seçilen hastalık bölgesiyle ilgili detayların gösterildiği bilgi panellerini kontrol eder.
*   **Utilities (`SimpleLogger.cs`):** Uygulama genelinde RAMS (Reliability, Availability, Maintainability, Safety) standartlarına uygun, hata takibini kolaylaştıran özelleştirilmiş bir loglama servisidir.

## 📊 Mevcut Durum (Hafta 7 - Ara Sunum)
**Şu anki durum:** Core logic (Çekirdek mantık) tamamlandı, Unity entegrasyonu devam ediyor.
Sistem için gerekli olan temel etkileşim, veri yönetimi ve UI algoritmaları modüler C# scriptleri olarak yazılmıştır. Proje, THS (Teknoloji Hazırlık Seviyesi) 4 hedefine uygun olarak laboratuvar/geliştirme ortamında bileşen bazında doğrulanmaya hazırdır.

## ⚙️ Kurulum ve Çalıştırma
> **Yakında:** Unity import aşaması ve sahne kurulumu tamamlandığında detaylı kurulum adımları buraya eklenecektir.

1. Bu depoyu klonlayın: `git clone https://github.com/dagkadir23/ymgk_karaciger_235541143_225541046.git`
2. Unity Hub üzerinden yeni bir 3D/URP projesi oluşturun.
3. `Assets/Scripts/` klasörünü Unity projenizin içine sürükleyin.
4. *Sahne kurulumu için `docs/UserScenario.pdf` ve gelecekteki dokümanları takip edin.*

## 🎬 Demo ve Videolar
> **Durum:** Prototip geliştirme aşamasında. 
Şu anda kod walkthrough (kod okuma/anlatım) demosu mevcuttur. Unity sahnesi tamamlandığında, MR gözlüğü veya PC üzerinden alınmış interaktif bir demo videosu (`Demo_video.mp4`) eklenecektir.
