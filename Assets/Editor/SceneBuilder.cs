#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using PatientLive.Core;
using PatientLive.Data;
using PatientLive.Interaction;
using PatientLive.UI;
using System.IO;

namespace PatientLive.Editor
{
    public class SceneBuilder
    {
        [MenuItem("PatientLive/Sahneyi Otomatik Kur (Mobil-Dikey)")]
        public static void BuildScene()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Play modundayken sahne kurulumu yapılamaz!");
                return;
            }

            // Yeni boş bir sahne oluştur
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Klasör yapısını oluştur
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
                AssetDatabase.CreateFolder("Assets", "Data");
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");

            // Build Target'i Android'e çekme uyarısı (İsteğe bağlı yapılabilir, manuel bırakıldı)
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android && EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
            {
                Debug.Log("Mobil deneyim için File -> Build Settings'ten platformu Android veya iOS yapmanız önerilir.");
            }

            // 1. Veri Dosyalarını Oluştur
            var healthyData = CreateDiseaseData("HealthyRegionData", "Sağlıklı Doku", DiseaseType.Healthy, "Karaciğerin normal, sağlıklı işlev gören dokusudur.", Color.green);
            var tumorData = CreateDiseaseData("TumorRegionData", "Hücreli Karsinom (HCC)", DiseaseType.Tumor, "Primer karaciğer kanserinin en yaygın türüdür.", Color.red);
            var cystData = CreateDiseaseData("CystRegionData", "Basit Kist", DiseaseType.Cyst, "İçi sıvı dolu, genellikle iyi huylu olan kistik oluşumdur.", Color.blue);

            // 2. Kameralar ve Işık (Mobil için Kamera biraz daha geride)
            GameObject mainCamera = new GameObject("Main Camera");
            Camera cam = mainCamera.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.12f, 0.12f);
            mainCamera.tag = "MainCamera";
            mainCamera.transform.position = new Vector3(0, 0, -14); // Daha geride
            cam.fieldOfView = 60f;

            GameObject dirLight = new GameObject("Directional Light");
            Light light = dirLight.AddComponent<Light>();
            light.type = LightType.Directional;
            dirLight.transform.rotation = Quaternion.Euler(50, -30, 0);

            // 3. Karaciğer Modelini Oluştur
            GameObject liverRoot = null;
            GameObject loadedModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Liver/VH_M_Liver.glb");
            
            if (loadedModel != null)
            {
                liverRoot = (GameObject)PrefabUtility.InstantiatePrefab(loadedModel);
                liverRoot.name = "LiverModel_Real";
                liverRoot.transform.localScale = new Vector3(10f, 10f, 10f); // Boyutu modelin büyüklüğüne göre ayarlamak gerekebilir
                liverRoot.transform.position = Vector3.zero;
            }
            else
            {
                liverRoot = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                liverRoot.name = "LiverModel_Prototype";
                liverRoot.transform.localScale = new Vector3(3.5f, 2.5f, 2.5f); // Biraz daha büyük
                liverRoot.transform.position = Vector3.zero;
                
                Material liverMat = new Material(Shader.Find("Standard"));
                liverMat.color = new Color(0.6f, 0.2f, 0.2f);
                AssetDatabase.CreateAsset(liverMat, "Assets/Materials/LiverMat.mat");
                liverRoot.GetComponent<MeshRenderer>().sharedMaterial = liverMat;
            }

            LiverModelController liverController = liverRoot.AddComponent<LiverModelController>();

            // Hastalık Bölgeleri
            CreateRegionNode(liverRoot.transform, new Vector3(0.3f, 0.4f, -0.4f), 0.35f, healthyData);
            CreateRegionNode(liverRoot.transform, new Vector3(-0.4f, 0.2f, -0.4f), 0.45f, tumorData);
            CreateRegionNode(liverRoot.transform, new Vector3(0.2f, -0.3f, 0.4f), 0.4f, cystData);

            // 4. UI Canvas Oluştur (DİKEY EKRAN 1080x1920)
            GameObject canvasObj = new GameObject("MainCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920); // Portrait Çözünürlük
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            // --- Disease Info Panel (Bottom Sheet Tasarımı) ---
            GameObject infoPanelObj = new GameObject("DiseaseInfoPanel");
            infoPanelObj.transform.SetParent(canvasObj.transform, false);
            RectTransform infoRect = infoPanelObj.AddComponent<RectTransform>();
            infoRect.anchorMin = new Vector2(0, 0); // Ekranın en altı
            infoRect.anchorMax = new Vector2(1, 0); // Yatayda tam esneklik
            infoRect.pivot = new Vector2(0.5f, 0);
            infoRect.offsetMin = new Vector2(0, 0); // Sol kenar sıfır
            infoRect.offsetMax = new Vector2(0, 700); // Sağ kenar sıfır, yükseklik 700 piksel

            Image panelBg = infoPanelObj.AddComponent<Image>();
            panelBg.color = new Color(0.08f, 0.08f, 0.08f, 0.95f); // Koyu arka plan
            
            CanvasGroup infoCanvasGroup = infoPanelObj.AddComponent<CanvasGroup>();
            DiseaseInfoPanel diseaseInfoPanel = infoPanelObj.AddComponent<DiseaseInfoPanel>();

            // Info Panel Metinleri (Mobilde Okunabilir Büyük Fontlar)
            TMP_Text titleText = CreateText(infoPanelObj.transform, "TitleText", "Hastalık Adı", 60, new Vector2(0, -100), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
            titleText.rectTransform.offsetMin = new Vector2(50, -180);
            titleText.rectTransform.offsetMax = new Vector2(-50, -60);

            TMP_Text typeText = CreateText(infoPanelObj.transform, "TypeText", "Tipi", 40, new Vector2(0, -220), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
            typeText.rectTransform.offsetMin = new Vector2(120, -280);
            typeText.rectTransform.offsetMax = new Vector2(-50, -180);
            typeText.alignment = TextAlignmentOptions.Left;

            TMP_Text descText = CreateText(infoPanelObj.transform, "DescriptionText", "Açıklama...", 36, new Vector2(0, -450), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
            descText.rectTransform.offsetMin = new Vector2(50, -650);
            descText.rectTransform.offsetMax = new Vector2(-50, -300);
            descText.enableWordWrapping = true;
            descText.alignment = TextAlignmentOptions.TopLeft;

            // Renk Göstergesi (Kare/Daire)
            GameObject colorIndicatorObj = new GameObject("TypeColorIndicator");
            colorIndicatorObj.transform.SetParent(infoPanelObj.transform, false);
            RectTransform colorRect = colorIndicatorObj.AddComponent<RectTransform>();
            colorRect.anchorMin = new Vector2(0, 1);
            colorRect.anchorMax = new Vector2(0, 1);
            colorRect.pivot = new Vector2(0, 1);
            colorRect.anchoredPosition = new Vector2(50, -200);
            colorRect.sizeDelta = new Vector2(50, 50); // Büyük indicator
            Image colorImage = colorIndicatorObj.AddComponent<Image>();

            SetPrivateField(diseaseInfoPanel, "titleText", titleText);
            SetPrivateField(diseaseInfoPanel, "typeText", typeText);
            SetPrivateField(diseaseInfoPanel, "descriptionText", descText);
            SetPrivateField(diseaseInfoPanel, "typeColorIndicator", colorImage);

            // --- Safety Warning Panel ---
            GameObject warningObj = new GameObject("SafetyWarningPanel");
            warningObj.transform.SetParent(canvasObj.transform, false);
            RectTransform warnRect = warningObj.AddComponent<RectTransform>();
            warnRect.anchorMin = Vector2.zero;
            warnRect.anchorMax = Vector2.one;
            warnRect.offsetMin = Vector2.zero;
            warnRect.offsetMax = Vector2.zero;

            Image warnBg = warningObj.AddComponent<Image>();
            warnBg.color = new Color(0.05f, 0.05f, 0.05f, 0.98f);

            CanvasGroup warnCanvasGroup = warningObj.AddComponent<CanvasGroup>();
            SafetyWarningController safetyWarningController = warningObj.AddComponent<SafetyWarningController>();

            TMP_Text warnTitle = CreateText(warningObj.transform, "WarnTitle", "ÖNEMLİ UYARI", 80, new Vector2(0, 400), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            warnTitle.color = new Color(1f, 0.3f, 0.3f); // Kırmızımsı
            warnTitle.rectTransform.sizeDelta = new Vector2(900, 150);

            TMP_Text warnDesc = CreateText(warningObj.transform, "WarnDesc", "Bu uygulama teşhis amaçlı değildir.\n\nEğitim ve bilgilendirme amaçlıdır.\n\nLütfen medikal sorunlarınız için bir uzmana başvurun.", 45, new Vector2(0, 0), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            warnDesc.rectTransform.sizeDelta = new Vector2(900, 500);

            // Onay Butonu (Mobil Büyük Buton)
            GameObject btnObj = new GameObject("AcceptButton");
            btnObj.transform.SetParent(warningObj.transform, false);
            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0);
            btnRect.anchorMax = new Vector2(0.5f, 0);
            btnRect.pivot = new Vector2(0.5f, 0);
            btnRect.anchoredPosition = new Vector2(0, 200);
            btnRect.sizeDelta = new Vector2(800, 150); // Ekrana yayılan büyük buton
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.6f, 0.3f); // Yeşilimsi
            Button acceptBtn = btnObj.AddComponent<Button>();
            
            TMP_Text btnText = CreateText(btnObj.transform, "BtnText", "ANLADIM, KABUL EDİYORUM", 40, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            btnText.color = Color.white;
            btnText.rectTransform.sizeDelta = new Vector2(800, 150);

            SetPrivateField(safetyWarningController, "titleText", warnTitle);
            SetPrivateField(safetyWarningController, "warningText", warnDesc);
            SetPrivateField(safetyWarningController, "acceptButton", acceptBtn);

            // 5. Yöneticileri Ayarla
            GameObject managerObj = new GameObject("_AppManagers");
            AppInitializer appInitializer = managerObj.AddComponent<AppInitializer>();
            SetPrivateField(appInitializer, "safetyWarningController", safetyWarningController);
            SetPrivateField(appInitializer, "liverModel", liverController);

            ModelInteractionController interactionController = managerObj.AddComponent<ModelInteractionController>();
            SetPrivateField(interactionController, "liverModel", liverController);
            SetPrivateField(interactionController, "infoPanel", diseaseInfoPanel);
            SetPrivateField(interactionController, "raycastCamera", cam);

            // Projeyi Kaydet
            AssetDatabase.SaveAssets();
            
            // Sahneyi Assets klasörüne kaydet
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/Scenes/MobileScene.unity");

            Debug.Log("<color=green>Mobil Uyumlu PatientLive Sahnesi Başarıyla Oluşturuldu!</color> Unity Game penceresini '1080x1920 Portrait' moduna alıp test edebilirsiniz.");
        }

        private static DiseaseRegionData CreateDiseaseData(string fileName, string name, DiseaseType type, string desc, Color color)
        {
            string path = $"Assets/Data/{fileName}.asset";
            DiseaseRegionData data = AssetDatabase.LoadAssetAtPath<DiseaseRegionData>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<DiseaseRegionData>();
                data.regionName = name;
                data.diseaseType = type;
                data.description = desc;
                data.highlightColor = color;
                AssetDatabase.CreateAsset(data, path);
            }
            return data;
        }

        private static void CreateRegionNode(Transform parent, Vector3 localPos, float scale, DiseaseRegionData data)
        {
            GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            node.name = "Region_" + data.regionName;
            node.transform.SetParent(parent, false);
            node.transform.localPosition = localPos;
            node.transform.localScale = Vector3.one * scale;

            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.white;
            AssetDatabase.CreateAsset(mat, $"Assets/Materials/Mat_{data.regionName}.mat");
            node.GetComponent<MeshRenderer>().sharedMaterial = mat;

            DiseaseRegion region = node.AddComponent<DiseaseRegion>();
            SetPrivateField(region, "regionData", data);
        }

        private static TMP_Text CreateText(Transform parent, string name, string text, int fontSize, Vector2 pos, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = pos;

            TMP_Text tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            return tmp;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var prop = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (prop != null)
            {
                prop.SetValue(target, value);
            }
        }
    }
}
#endif
