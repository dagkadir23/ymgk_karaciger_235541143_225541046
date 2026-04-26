# PatientLive - Kullanıcı Senaryosu (User Scenario)

**Senaryo Adı:** Rutin Karaciğer Hastalığı Bilgilendirmesi
**Aktörler:** Uzman Doktor (Kullanıcı), Hasta (İzleyici/Deneyimleyen)
**Kullanılan Ortam:** Klinik odası, PatientLive MR/3D Uygulaması

### Adım Adım Kullanım Senaryosu:

1. **Sistemi Başlatma (Güvenlik Önlemi):**
   Doktor veya asistanı PatientLive uygulamasını başlatır. Ekranda veya MR gözlüğünde ilk olarak "Bu uygulama teşhis amaçlı değildir. Sadece bilgilendirme ve eğitim amaçlıdır." uyarısı belirir. Doktor "Anladım" butonuna basarak sorumluluk reddini onaylar ve ana ekrana geçer.

2. **Modelin İncelenmesi:**
   Hasta, MR gözlüğünü takar (veya bir tablet/monitör ekranına bakar). Karşısında sağlıklı, üç boyutlu bir karaciğer modeli belirir. Doktor, el hareketleri veya dokunmatik ekran aracılığıyla modeli hastaya farklı açılardan göstererek genel yapıyı anlatır.

3. **Hastalık Tespiti ve Vurgulama:**
   Doktor, hastanın güncel durumunu açıklamak için 3D model üzerinde daha önceden sisteme tanımlanmış olan "Tümör (HCC)" bölgesine tıklar / işaret eder. 
   
4. **Bilgi Aktarımı ve Görselleştirme:**
   Seçilen tümörlü bölge, 3D model üzerinde farklı bir renkte (örneğin kırmızı) parlayarak (highlight) belirginleşir. Eş zamanlı olarak görüş alanında sade bir UI paneli açılır. Bu panelde "Hepatoselüler Karsinom (HCC)" başlığı ve hastalığın ne olduğu, nasıl bir kitle olduğu hastanın anlayabileceği dilde yazar.

5. **Karşılaştırma ve Anlayış:**
   Doktor daha sonra "Sağlıklı Doku" bölgesine tıklayarak tümörlü alan ile sağlıklı alan arasındaki farkı görsel olarak hastaya gösterir. Hasta, 2 boyutlu karmaşık MR/Tomografi kağıtları yerine, kendi hastalığının karaciğerdeki konumunu ve boyutunu 3D ortamda somut bir şekilde görerek tedavi sürecini daha iyi idrak eder.
