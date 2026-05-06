# PatientLive - Mobil Build ve Dağıtım Rehberi (Android)

Bu belge, PatientLive Unity projesinin bir **Android (.apk)** uygulaması olarak nasıl derleneceğini ve bir cep telefonunda nasıl çalıştırılacağını adım adım açıklamaktadır.

## 1. Gerekli Modüllerin Kurulumu (Unity Hub)

Projeyi Android için derlemeden önce Unity'nin Android destek paketlerine sahip olduğunuzdan emin olmalısınız.

1. **Unity Hub** uygulamasını açın.
2. Sol menüden **Installs** (Kurulumlar) sekmesine gidin.
3. Projede kullanılan Unity sürümünün (`6000.x.x`) yanındaki **ayarlar/dişli** ikonuna tıklayın ve **Add modules** (Modül ekle) deyin.
4. Çıkan listeden şu modüllerin seçili ve kurulu olduğundan emin olun:
   - `Android Build Support`
   - `OpenJDK`
   - `Android SDK & NDK Tools`
5. Eğer kurulu değillerse seçip yüklenmelerini bekleyin.

## 2. Projenin Mobile Hazırlanması

1. Unity'de projeyi açın.
2. Üst menüden `PatientLive -> Sahneyi Otomatik Kur (Mobil-Dikey)` butonuna tıklayarak mobil uyumlu arayüzün (Bottom-Sheet, dev butonlar vb.) oluşturulmasını sağlayın.
3. Oluşturulan sahnenin `Assets/Scenes/MobileScene.unity` olarak kaydedildiğinden emin olun (Ctrl+S).

## 3. Platform Değişikliği (Switch Platform)

1. Unity'nin üst menüsünden `File -> Build Settings...` yolunu izleyin.
2. Sol taraftaki **Platform** listesinden **Android**'i seçin.
3. Eğer yan tarafında "Switch Platform" butonu varsa tıklayın. Unity, sahnelerdeki materyalleri Android için yeniden hesaplayacaktır (Bu işlem birkaç dakika sürebilir). 
   *(Platform zaten Android ise buton "Build" olarak görünür).*
4. Üst kısımdaki **Scenes In Build** kutusuna dikkat edin. Eğer boşsa, sol alttaki `Add Open Scenes` butonuna tıklayarak mevcut mobil sahnenizi derleme listesine ekleyin.

## 4. Uygulama Ayarları (Player Settings)

1. Aynı `Build Settings` penceresinde, sol altta bulunan **Player Settings...** butonuna tıklayın.
2. Açılan pencerede **Player** sekmesinde şu temel ayarları yapın:
   - **Company Name:** `DagKadir` (veya kurum/kendi isminiz).
   - **Product Name:** `PatientLive` (Uygulamanın telefonda görüneceği adı).
3. Aynı sayfada aşağı kaydırıp **Other Settings** sekmesini genişletin:
   - **Package Name:** `com.dagkadir.patientlive` şeklinde (boşluksuz, küçük harf ve noktalı) benzersiz bir paket adı girin.
   - **Minimum API Level:** Uygulamanın çalışacağı en düşük Android sürümünü seçin (Genellikle `Android 8.0 (API level 26)` veya `Android 10` seçilmesi idealdir).

## 5. Build (Derleme) ve Kurulum

1. Tüm ayarları yaptıktan sonra `Build Settings` penceresindeki **Build** butonuna basın.
2. Unity size APK dosyasını nereye kaydetmek istediğinizi soracaktır. Proje klasörünüzün dışında (örneğin Masaüstüne) bir yer seçip dosya adına `PatientLive_Demo` yazın ve kaydedin.
3. Derleme tamamlandığında bilgisayarınızda bir `PatientLive_Demo.apk` dosyası oluşacaktır.

### Telefona Kurulum
1. Android telefonunuzu bir USB kablosuyla bilgisayarınıza bağlayın.
2. `.apk` dosyasını telefonunuzun hafızasına (örneğin `Downloads` klasörüne) kopyalayın.
3. Telefonunuzda "Dosyalar" (File Manager) uygulamasını açıp APK'ya dokunun. 
4. *(Eğer ilk kez dışarıdan APK kuruyorsanız telefonunuz "Bilinmeyen kaynaklardan yüklemeye izin ver" onayı isteyecektir, buna izin verin).*
5. Kurulum bittiğinde uygulamayı telefonunuzun ana ekranından başlatıp mobil hasta simülasyonunu test edebilirsiniz!
