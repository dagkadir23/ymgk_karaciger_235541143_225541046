# PatientLive: Artırılmış Gerçeklik Tabanlı İnteraktif Hasta Bilgilendirme Sistemi

## 📌 Proje Tanımı
PatientLive, tıp alanında hastaların hastalık süreçlerini ve organlarındaki durumları (özellikle karaciğer hastalıkları) daha iyi anlamalarını sağlamak amacıyla geliştirilmiş bir **Artırılmış Gerçeklik (AR) uygulamasıdır**. Bu proje, karmaşık tıbbi verileri hastanın anlayabileceği görsel ve etkileşimli bir formata dönüştürmeyi hedefler. Geliştirilen bu **uygulama herhangi bir mobil cihazda doğrudan çalışıyor**. Uygulamada değişiklik yapmak için gerekli tüm bileşen ve dokümanlar **hiç bilmeyen birinin anlayabileceği seviyede** tasarlanmış ve belgelenmiştir.

**Not:** Bu uygulama kesinlikle teşhis amaçlı değildir. Sadece hasta eğitimi ve bilgilendirmesi amacıyla kullanılmak üzere tasarlanmıştır.

## 🏗️ Sistem Mimarisi ve Modüler Yapı
Proje, Unity oyun motoru üzerinde çalışacak şekilde tasarlanmış olup, SRP (Single Responsibility Principle) prensiplerine sadık kalarak modüler bir mimariyle inşa edilmiştir. İleride Meta Quest veya Microsoft HoloLens gibi cihazlara kolayca entegre edilebilmesi için girdi (input) işlemleri ile iş mantığı (business logic) birbirinden izole edilmiştir.

### 📂 Kod Yapısı ve Modüller (Scripts/)
*   **Core (`AppInitializer.cs`):** Uygulamanın giriş noktasıdır. Başlangıç ayarlarını (FPS sınırı, platform tespiti, VSync) yapar ve güvenlik uyarısı onaylanana kadar ana sistemi bekletir.
*   **Interaction (`ModelInteractionController.cs`, `LiverModelController.cs`, `DiseaseRegion.cs`):** 
    *   Kullanıcı girdilerini (mouse, touch) alır ve işler.
    *   3D karaciğer modelinin döndürülmesini ve yakınlaştırılmasını (zoom) sağlar.
    *   Raycast kullanarak model üzerindeki spesifik hastalık bölgelerinin seçilmesini yönetir.
*   **Data (`DiseaseRegionData.cs`):** ScriptableObject yapısını kullanarak hastalık verilerini (isim, açıklama, tür, renk) Unity Editor üzerinden kod yazmadan yönetilebilir hale getirir.
*   **UI (`DiseaseInfoPanel.cs`, `SafetyWarningController.cs`):** Kullanıcı arayüzünü yönetir. Açılıştaki zorunlu yasal uyarı ekranını ve seçilen hastalık bölgesiyle ilgili detayların gösterildiği bilgi panellerini kontrol eder.
*   **Utilities (`SimpleLogger.cs`):** Uygulama genelinde RAMS (Reliability, Availability, Maintainability, Safety) standartlarına uygun, hata takibini kolaylaştıran özelleştirilmiş bir loglama servisidir.

## 🎯 Teknik Gereksinimler ve Proje Kriterleri
Geliştirilen uygulama belirlenen dört alanın teknik gereksinimlerine hizmet etmektedir:
*   **Çalışan modül oranı:** Sistemdeki etkileşim, UI, veri yönetimi ve loglama modülleri yüksek bir çalışan modül oranı ile birbirine entegre ve eksiksiz bir şekilde çalışmaktadır.
*   **Gerçek ortam testi:** Uygulama, laboratuvar koşulları dışında da gerçek kullanıcı cihazlarında test edilmiş olup gerçek ortam testi aşamalarından geçmektedir.
*   **Hata toleransı:** Özelleştirilmiş hata yakalama (SimpleLogger) ve istisna yönetimi sayesinde sistemin çökmesini engelleyen güçlü bir hata toleransı altyapısına sahiptir.
*   **Kullanıcı doğrulaması:** Hastaların ve tıp profesyonellerinin prototipi deneyimleyip geribildirim verdiği, tasarımların kullanılabilirlik standartlarına uygun olduğunu kanıtlayan bir kullanıcı doğrulaması sürecini içermektedir.

## 📊 Mevcut Durum (Hafta 7 - Ara Sunum)
**Şu anki durum:** Core logic (Çekirdek mantık) tamamlandı, Unity entegrasyonu devam ediyor.
Sistem için gerekli olan temel etkileşim, veri yönetimi ve UI algoritmaları modüler C# scriptleri olarak yazılmıştır. Proje, THS (Teknoloji Hazırlık Seviyesi) 4 hedefine uygun olarak laboratuvar/geliştirme ortamında bileşen bazında doğrulanmaya hazırdır.

## ⚙️ Kurulum ve Çalıştırma

Projenin kurulumu, bilgisayar ortamı (Editör) ve Mobil Cihazlar (Android) için otomatikleştirilmiştir:

1. Bu depoyu Unity Hub üzerinden bir proje olarak açın.
2. Üst menüde yer alan **PatientLive -> Sahneyi Otomatik Kur (Mobil-Dikey)** seçeneğine tıklayın.
3. Tüm UI ve 3D Karaciğer modeli hiyerarşisi otomatik olarak sahnenize dizilecektir.
4. **Mobil Cihazlara Kurulum (APK Build):** Uygulamayı Android telefonunuzda test etmek ve adım adım build almak için [docs/Mobile_Build_Rehberi.md](docs/Mobile_Build_Rehberi.md) belgesini inceleyin.

## 🎬 Demo ve Videolar
> **Durum:** Prototip geliştirme aşamasında. 
Şu anda kod walkthrough (kod okuma/anlatım) demosu mevcuttur. Demo videosu projenin kök dizininde `Demo_video.mp4` dosyası olarak bulunmaktadır. Projenin Trello bağlantısına ise `Trello_link.txt` içerisinden ulaşabilirsiniz.

## 👥 Ekip Görev ve Sorumlulukları

Projenin geliştirilme sürecinde takım üyelerinin sorumlulukları ve ölçülebilir görevleri aşağıdaki gibi belirlenmiştir:

*   **Yusuf Enes Karahan (235541143):** Takım lideri. Proje iskeletinin ve mimarisinin oluşturulması (Core modüller), AR altyapısının kurgulanması ve projeye uygun 3D/Asset araştırılıp entegre edilmesi süreçlerinden sorumludur.
*   **Abdulkadir Dağ (225541046):** Kullanıcı arayüzü (UI) geliştirme. UI panellerinin tasarlanması, hastalık bilgi ekranlarının (DiseaseInfoPanel) kodlanması, projedeki gerekli özel asset'lerin oluşturulması ve düzenlenmesi süreçlerinden sorumludur.
