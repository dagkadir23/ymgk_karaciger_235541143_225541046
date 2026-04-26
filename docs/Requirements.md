# PatientLive - Gereksinim Analizi (Requirements)

## 1. Fonksiyonel Gereksinimler

**FR01 - 3D Model Görüntüleme:** Sistem, karaciğer organının 3D modelini ana sahnede göstermelidir.
*(Durum: Modül mantığı hazır / Unity entegrasyonu bekliyor)*

**FR02 - Model Etkileşimi:** Kullanıcı (doktor), modeli döndürebilmeli (rotate) ve detayları görmek için yakınlaştırıp uzaklaştırabilmelidir (zoom/pinch).
*(Durum: LiverModelController kodlandı / Input entegrasyonu bekliyor)*

**FR03 - Hastalık Bölgesi Seçimi:** 3D model üzerinde sağlıklı doku, tümör veya kist gibi farklı bölgeler seçilebilir olmalıdır.
*(Durum: Raycast ve DiseaseRegion modülleri hazır / 3D collider entegrasyonu bekliyor)*

**FR04 - Bilgi Gösterimi:** Bir bölge seçildiğinde, o hastalıkla ilgili kısa ve anlaşılır bir açıklama metni arayüzde (UI) belirmelidir.
*(Durum: DiseaseInfoPanel modülü ve ScriptableObject veri altyapısı hazır / UI Canvas entegrasyonu bekliyor)*

**FR05 - Yasal Uyarı Ekranı:** Uygulama açılışında, uygulamanın teşhis amaçlı olmadığına dair güvenlik ve sorumluluk reddi uyarısı çıkmalı ve onaylanmadan geçilememelidir.
*(Durum: SafetyWarningController kodlandı / Tamamlandı)*

## 2. Fonksiyonel Olmayan Gereksinimler (Non-Functional - RAMS Uyumlu)

**NFR01 - Güvenilirlik (Reliability) ve Loglama:** Sistem, hataları ve istisnai durumları yakalamalı ve `SimpleLogger` üzerinden bellek içi bir kuyrukta tutarak geriye dönük hata analizi yapılmasına olanak sağlamalıdır.

**NFR02 - Performans ve Optimizasyon (Availability):** Uygulama, kısıtlı MR cihazı donanımlarında sorunsuz çalışabilmek için kare hızını (FPS) dinamik olarak sınırlandırmalı (örn: 60 veya 72 FPS) ve düşük RAM durumlarında grafik kalitesini düşürmelidir.

**NFR03 - Modülerlik (Maintainability):** Sistem, Input kontrolü ile 3D model manipülasyonunu ayırmalı (SRP), ileride eklenebilecek yeni hastalık tiplerini kod değişikliği gerektirmeden (ScriptableObject ile) desteklemelidir.

**NFR04 - Güvenlik (Safety):** Kullanıcının yanlış teşhis veya tedavi çıkarımı yapmasını engellemek amacıyla, her oturum başında net bir bilgilendirme ekranı sunulmalı ve uygulamanın sadece eğitim aracı olduğu vurgulanmalıdır.
