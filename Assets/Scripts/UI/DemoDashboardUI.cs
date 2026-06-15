using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PatientLive.UI
{
    /// <summary>
    /// Responsive demo app shell: model dashboard plus fake detail pages for presentation.
    /// </summary>
    public class DemoDashboardUI : MonoBehaviour
    {
        private const float ReferenceWidth = 1080f;
        private const float ReferenceHeight = 2400f;

        private readonly Color overlay = new Color(0.015f, 0.02f, 0.03f, 0.45f);
        private readonly Color panel = new Color(0.08f, 0.12f, 0.18f, 0.85f);
        private readonly Color card = new Color(0.12f, 0.16f, 0.22f, 0.85f);
        private readonly Color cardLight = new Color(0.16f, 0.22f, 0.3f, 0.85f);
        private readonly Color cyan = new Color(0.18f, 0.85f, 0.98f, 1f);
        private readonly Color green = new Color(0.28f, 0.9f, 0.6f, 1f);
        private readonly Color amber = new Color(1f, 0.75f, 0.25f, 1f);
        private readonly Color red = new Color(1f, 0.28f, 0.38f, 1f);
        private readonly Color softText = new Color(0.8f, 0.88f, 0.95f, 1f);

        private Font font;
        private CanvasScaler scaler;
        private RectTransform homePage;
        private RectTransform nutritionPage;
        private RectTransform reportPage;
        private RectTransform labsPage;
        private RectTransform appointmentPage;
        private RectTransform header;
        private RectTransform controlsPanel;
        private RectTransform vitalsPanel;
        private RectTransform insightPanel;
        private RectTransform hintPanel;
        private Text statusText;
        private Text insightTitle;
        private Text insightBody;
        private Text scanText;
        private Text confidenceText;
        private Image scanFill;
        private Image confidenceFill;
        private float elapsed;
        private int lastWidth;
        private int lastHeight;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindFirstObjectByType<DemoDashboardUI>() != null)
            {
                return;
            }

            var host = new GameObject("DemoDashboardUI");
            DontDestroyOnLoad(host);
            host.AddComponent<DemoDashboardUI>();
        }

        private void Awake()
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            EnsureEventSystem();
            EnsureShowcase();
            BuildUi();
            ShowPage(homePage);
            ApplyResponsiveLayout(true);
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float scan = Mathf.PingPong(elapsed * 0.11f, 1f);
            float confidence = Mathf.Lerp(0.78f, 0.96f, Mathf.PingPong(elapsed * 0.15f, 1f));

            scanFill.fillAmount = Mathf.Lerp(0.32f, 0.94f, scan);
            confidenceFill.fillAmount = confidence;
            scanText.text = Mathf.RoundToInt(scanFill.fillAmount * 100f) + "%";
            confidenceText.text = Mathf.RoundToInt(confidence * 100f) + "%";
            statusText.text = scan > 0.86f ? "Rapor hazırlanıyor" : "Canlı analiz";

            ApplyResponsiveLayout(false);
        }

        private void BuildUi()
        {
            var canvasObject = new GameObject("DemoCanvas");
            canvasObject.transform.SetParent(transform, false);

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 90;

            scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            var tint = Image(canvasObject.transform, "ScreenTint", overlay);
            Stretch(tint.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            tint.raycastTarget = false;

            homePage = Page(canvasObject.transform, "HomePage");
            nutritionPage = Page(canvasObject.transform, "NutritionPage");
            reportPage = Page(canvasObject.transform, "ReportPage");
            labsPage = Page(canvasObject.transform, "LabsPage");
            appointmentPage = Page(canvasObject.transform, "AppointmentPage");

            BuildHome(homePage);
            BuildNutritionPage(nutritionPage);
            BuildReportPage(reportPage);
            BuildLabsPage(labsPage);
            BuildAppointmentPage(appointmentPage);
        }

        private void BuildHome(Transform parent)
        {
            header = Image(parent, "Header", panel).rectTransform;
            AddText(header, "PatientLive", 42, FontStyle.Bold, Color.white, new Vector2(34f, -20f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(310f, 54f), TextAnchor.UpperLeft);
            AddText(header, "Karaciğer sağlığı demo uygulaması", 23, FontStyle.Normal, softText, new Vector2(34f, -76f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(470f, 34f), TextAnchor.UpperLeft);
            AddChip(header, "RESPONSIVE", cyan, new Vector2(-420f, -42f), 160f);
            AddChip(header, "DOKUN-ÇEVİR", green, new Vector2(-238f, -42f), 160f);
            statusText = AddText(header, "Canlı analiz", 25, FontStyle.Bold, Color.white, new Vector2(-58f, -36f), Vector2.one, Vector2.one, new Vector2(42f, 34f), TextAnchor.MiddleRight);

            controlsPanel = Image(parent, "ControlsPanel", panel).rectTransform;
            AddText(controlsPanel, "Kontroller", 28, FontStyle.Bold, Color.white, new Vector2(22f, -20f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(250f, 40f), TextAnchor.MiddleLeft);
            AddButton(controlsPanel, "Risk Haritası", "Taralı alanları kırmızı göster", red, new Vector2(22f, -92f), () => SetInsight("Risk Haritası", DemoLiverShowcase.Instance.ShowRiskMode()));
            AddButton(controlsPanel, "Beslenme Önerileri", "Ayrı sayfaya geç", green, new Vector2(22f, -194f), () => ShowPage(nutritionPage));
            AddButton(controlsPanel, "Kan Değerleri", "ALT / AST simülasyonu", cyan, new Vector2(22f, -296f), () => ShowPage(labsPage));
            AddButton(controlsPanel, "Ön Rapor", "Rapor sayfasını aç", amber, new Vector2(22f, -398f), () => ShowPage(reportPage));
            AddButton(controlsPanel, "Randevu Planla", "Kontrol hatırlatıcısı", green, new Vector2(22f, -500f), () => ShowPage(appointmentPage));
            AddButton(controlsPanel, "Damar Katmanı", "Mavi damar görünümü", cyan, new Vector2(22f, -602f), () => SetInsight("Damar Katmanı", DemoLiverShowcase.Instance.ToggleVesselLayer()));
            AddText(controlsPanel, "Model", 22, FontStyle.Bold, Color.white, new Vector2(22f, -728f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(120f, 30f), TextAnchor.MiddleLeft);
            AddSquareButton(controlsPanel, "+", cyan, new Vector2(22f, -788f), () => { DemoLiverShowcase.Instance.ZoomIn(); SetInsight("Yakınlaştırma", "Model yaklaştırıldı. Ayrıca iki parmakla pinch hareketi de çalışır."); });
            AddSquareButton(controlsPanel, "-", cyan, new Vector2(106f, -788f), () => { DemoLiverShowcase.Instance.ZoomOut(); SetInsight("Uzaklaştırma", "Model uzaklaştırıldı. Sunumda tüm karaciğer görünümü rahatça gösterilebilir."); });
            AddSquareButton(controlsPanel, "0", amber, new Vector2(190f, -788f), () => { DemoLiverShowcase.Instance.ResetView(); SetInsight("Görünüm Sıfırlandı", "Model varsayılan açı ve ölçeğe döndü."); });

            vitalsPanel = Image(parent, "VitalsPanel", panel).rectTransform;
            AddText(vitalsPanel, "Canlı Özet", 28, FontStyle.Bold, Color.white, new Vector2(22f, -20f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(250f, 40f), TextAnchor.MiddleLeft);
            AddMetric(vitalsPanel, "Karaciğer hacmi", "1.42 L", cyan, new Vector2(22f, -92f));
            AddMetric(vitalsPanel, "Şüpheli alan", "2 bölge", red, new Vector2(22f, -190f));
            AddMetric(vitalsPanel, "Genel durum", "İzlem", green, new Vector2(22f, -288f));
            AddText(vitalsPanel, "Tarama", 21, FontStyle.Bold, Color.white, new Vector2(22f, -412f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(160f, 28f), TextAnchor.MiddleLeft);
            scanFill = Progress(vitalsPanel, new Vector2(22f, -458f), cyan);
            scanText = AddText(vitalsPanel, "0%", 20, FontStyle.Bold, Color.white, new Vector2(236f, -446f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(54f, 28f), TextAnchor.MiddleRight);
            AddText(vitalsPanel, "Güven", 21, FontStyle.Bold, Color.white, new Vector2(22f, -524f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(160f, 28f), TextAnchor.MiddleLeft);
            confidenceFill = Progress(vitalsPanel, new Vector2(22f, -570f), green);
            confidenceText = AddText(vitalsPanel, "0%", 20, FontStyle.Bold, Color.white, new Vector2(236f, -558f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(54f, 28f), TextAnchor.MiddleRight);

            insightPanel = Image(parent, "InsightPanel", panel).rectTransform;
            insightTitle = AddText(insightPanel, "Karaciğer Sağlığı Bilgilendirmesi", 31, FontStyle.Bold, Color.white, new Vector2(28f, -20f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(760f, 42f), TextAnchor.MiddleLeft);
            insightBody = AddText(insightPanel, "Taralı alanların rengi hastalık senaryosuna göre değişir. Kırmızı tümör şüphesi gibi dikkat gerektiren bulguları, camgöbeği kistik alanı, yeşil ise sağlıklı dokuyu temsil eder. Modeli sağa sola sürükleyerek döndürebilir, + / - ile yakınlaştırabilirsiniz.", 23, FontStyle.Normal, softText, new Vector2(28f, -76f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-56f, 100f), TextAnchor.UpperLeft);
            AddBadge(insightPanel, "Kırmızı", "Riskli alan", red, new Vector2(28f, 34f));
            AddBadge(insightPanel, "Yeşil", "Sağlıklı doku", green, new Vector2(236f, 34f));
            AddBadge(insightPanel, "Camgöbeği", "Kistik alan", cyan, new Vector2(444f, 34f));

            hintPanel = Image(parent, "ModelHint", new Color(0f, 0f, 0f, 0f)).rectTransform;
            AddText(hintPanel, "Modeli sağa sola sürükle", 22, FontStyle.Bold, new Color(1f, 1f, 1f, 0.78f), Vector2.zero, Vector2.zero, Vector2.one, Vector2.zero, TextAnchor.MiddleCenter).raycastTarget = false;
        }

        private void BuildNutritionPage(Transform parent)
        {
            AddPageHeader(parent, "Sağlıklı Beslenme Önerileri", "Karaciğer dostu günlük alışkanlıklar", () => ShowPage(homePage));
            AddInfoCard(parent, "Akdeniz tipi beslenme", "Sebze, tam tahıl, baklagil, zeytinyağı ve balık ağırlıklı tabaklar karaciğer sağlığı için iyi bir temel oluşturur.", green, new Vector2(54f, -240f));
            AddInfoCard(parent, "Şekerli içecekleri azalt", "Fazla fruktoz ve paketli içecekler yağlı karaciğer riskini artırabilir. Su ve şekersiz içecekler daha güvenli tercihtir.", cyan, new Vector2(54f, -430f));
            AddInfoCard(parent, "Alkol ve işlenmiş gıda", "Alkol tüketimini sınırlamak, kızartma ve yoğun işlenmiş ürünleri azaltmak karaciğer yükünü düşürür.", amber, new Vector2(54f, -620f));
            AddInfoCard(parent, "Düzenli takip", "Bu öneriler genel bilgilendirmedir. Şüpheli bulgu, ağrı veya kan tahlili bozukluğunda hekime başvurulmalıdır.", red, new Vector2(54f, -810f));
        }

        private void BuildReportPage(Transform parent)
        {
            AddPageHeader(parent, "Ön Rapor Görünümü", "Sahte demo verileri ile hasta bilgilendirme ekranı", () => ShowPage(homePage));
            AddInfoCard(parent, "Segment IV", "Kırmızı taralı alan: tümör şüphesi simülasyonu. Sunumda butona basınca model üzerinde alan büyütülür.", red, new Vector2(54f, -240f));
            AddInfoCard(parent, "Segment VI", "Camgöbeği alan: basit kist simülasyonu. Görsel olarak farklı renkle ayrıştırılır.", cyan, new Vector2(54f, -430f));
            AddInfoCard(parent, "Genel Not", "Bu ekran klinik karar vermez; kullanıcıya model, renk ve katman mantığını anlatan demo sayfasıdır.", green, new Vector2(54f, -620f));
            AddLargeButton(parent, "Şüpheli Alanları Modelde Vurgula", red, new Vector2(54f, -850f), () =>
            {
                DemoLiverShowcase.Instance.ShowRiskMode();
                DemoLiverShowcase.Instance.ToggleReportMode();
                ShowPage(homePage);
                SetInsight("Ön Rapor", "Rapor sayfasından dönüldü; şüpheli alanlar model üzerinde büyütülerek vurgulandı.");
            });
        }

        private void BuildLabsPage(Transform parent)
        {
            AddPageHeader(parent, "Kan Değerleri", "Sahte ALT / AST takip ekranı", () => ShowPage(homePage));
            AddInfoCard(parent, "ALT", "42 U/L - hafif yüksek simülasyon. Uygulama bunu sarı uyarı olarak gösterir.", amber, new Vector2(54f, -240f));
            AddInfoCard(parent, "AST", "36 U/L - referans aralığa yakın demo değeri.", green, new Vector2(54f, -430f));
            AddInfoCard(parent, "GGT", "58 U/L - takip önerisi üretmek için kullanılan sahte veri.", cyan, new Vector2(54f, -620f));
            AddLargeButton(parent, "Değerleri Ön Rapora Aktar", cyan, new Vector2(54f, -850f), () =>
            {
                ShowPage(reportPage);
            });
        }

        private void BuildAppointmentPage(Transform parent)
        {
            AddPageHeader(parent, "Randevu Planla", "İşlevsel görünümlü takip akışı", () => ShowPage(homePage));
            AddInfoCard(parent, "Kontrol zamanı", "Sonraki karaciğer kontrolü için 30 gün sonrası önerildi.", green, new Vector2(54f, -240f));
            AddInfoCard(parent, "Hatırlatma", "Bildirim: 3 gün önce ve randevu sabahı şeklinde planlandı.", cyan, new Vector2(54f, -430f));
            AddInfoCard(parent, "Paylaşım", "Ön rapor PDF olarak hekime gönderilmeye hazır görünüyor.", amber, new Vector2(54f, -620f));
            AddLargeButton(parent, "Randevuyu Onayla", green, new Vector2(54f, -850f), () =>
            {
                ShowPage(homePage);
                SetInsight("Randevu Planlandı", "Demo randevu akışı tamamlandı. Ana ekranda canlı analiz görünümü devam ediyor.");
            });
        }

        private void ApplyResponsiveLayout(bool force)
        {
            if (!force && lastWidth == Screen.width && lastHeight == Screen.height)
            {
                return;
            }

            lastWidth = Screen.width;
            lastHeight = Screen.height;
            float aspect = (float)Screen.width / Screen.height;
            bool compact = aspect < 0.8f;
            scaler.matchWidthOrHeight = compact ? 0f : 0.5f;

            Stretch(header, new Vector2(0f, 1f), Vector2.one, new Vector2(0f, -128f), Vector2.zero);
            if (compact)
            {
                AnchorBox(controlsPanel, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(24f, 24f), new Vector2(-24f, 890f));
                AnchorBox(insightPanel, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(24f, 914f), new Vector2(-24f, 1146f));
                AnchorBox(vitalsPanel, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-344f, -770f), new Vector2(-24f, -150f));
                AnchorBox(hintPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-210f, 80f), new Vector2(210f, 122f));
            }
            else
            {
                AnchorBox(controlsPanel, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(28f, -445f), new Vector2(338f, 445f));
                AnchorBox(vitalsPanel, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-348f, -310f), new Vector2(-28f, 310f));
                AnchorBox(insightPanel, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-380f, 28f), new Vector2(380f, 260f));
                AnchorBox(hintPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-260f, -22f), new Vector2(260f, 22f));
            }
        }

        private void ShowPage(RectTransform page)
        {
            homePage.gameObject.SetActive(page == homePage);
            nutritionPage.gameObject.SetActive(page == nutritionPage);
            reportPage.gameObject.SetActive(page == reportPage);
            labsPage.gameObject.SetActive(page == labsPage);
            appointmentPage.gameObject.SetActive(page == appointmentPage);
        }

        private void SetInsight(string title, string body)
        {
            insightTitle.text = title;
            insightBody.text = body + "\n\nRenkler demonstrasyon amaçlıdır; gerçek klinik değerlendirme için hekim incelemesi gerekir.";
        }

        private void AddPageHeader(Transform parent, string title, string subtitle, UnityAction backAction)
        {
            var top = Image(parent, "PageHeader", panel).rectTransform;
            Stretch(top, new Vector2(0f, 1f), Vector2.one, new Vector2(0f, -150f), Vector2.zero);
            AddSquareButton(top, "<", cyan, new Vector2(36f, -104f), backAction);
            AddText(top, title, 36, FontStyle.Bold, Color.white, new Vector2(126f, -42f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-160f, 46f), TextAnchor.MiddleLeft);
            AddText(top, subtitle, 22, FontStyle.Normal, softText, new Vector2(126f, -88f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-160f, 34f), TextAnchor.MiddleLeft);
        }

        private RectTransform Page(Transform parent, string name)
        {
            var page = new GameObject(name).AddComponent<RectTransform>();
            page.SetParent(parent, false);
            Stretch(page, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return page;
        }

        private Image Image(Transform parent, string name, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            var image = obj.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private Text AddText(Transform parent, string text, int size, FontStyle style, Color color, Vector2 position, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, TextAnchor alignment)
        {
            var obj = new GameObject("Text");
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(anchorMin.x, anchorMin.y);
            rect.anchoredPosition = position;
            rect.sizeDelta = sizeDelta;

            var textComponent = obj.AddComponent<Text>();
            textComponent.font = font;
            textComponent.text = text;
            textComponent.fontSize = size;
            textComponent.fontStyle = style;
            textComponent.color = color;
            textComponent.alignment = alignment;
            textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
            textComponent.verticalOverflow = VerticalWrapMode.Truncate;
            return textComponent;
        }

        private void AddButton(Transform parent, string title, string subtitle, Color color, Vector2 position, UnityAction action)
        {
            var buttonRect = Image(parent, title, card).rectTransform;
            SetRect(buttonRect, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, new Vector2(260f, 82f));
            Image(buttonRect, "Accent", color).rectTransform.sizeDelta = new Vector2(6f, 82f);
            var button = buttonRect.gameObject.AddComponent<Button>();
            button.onClick.AddListener(action);
            AddText(buttonRect, title, 21, FontStyle.Bold, Color.white, new Vector2(18f, -12f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(214f, 28f), TextAnchor.MiddleLeft);
            AddText(buttonRect, subtitle, 15, FontStyle.Normal, softText, new Vector2(18f, -44f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(218f, 24f), TextAnchor.MiddleLeft);
        }

        private void AddLargeButton(Transform parent, string title, Color color, Vector2 position, UnityAction action)
        {
            var rect = Image(parent, title, new Color(color.r, color.g, color.b, 0.22f)).rectTransform;
            SetRect(rect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), position, new Vector2(-108f, 82f));
            rect.gameObject.AddComponent<Button>().onClick.AddListener(action);
            AddText(rect, title, 24, FontStyle.Bold, Color.white, Vector2.zero, Vector2.zero, Vector2.one, Vector2.zero, TextAnchor.MiddleCenter);
        }

        private void AddSquareButton(Transform parent, string label, Color color, Vector2 position, UnityAction action)
        {
            var rect = Image(parent, label, new Color(color.r, color.g, color.b, 0.22f)).rectTransform;
            SetRect(rect, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, new Vector2(66f, 66f));
            rect.gameObject.AddComponent<Button>().onClick.AddListener(action);
            AddText(rect, label, 28, FontStyle.Bold, color, Vector2.zero, Vector2.zero, Vector2.one, Vector2.zero, TextAnchor.MiddleCenter);
        }

        private void AddMetric(Transform parent, string label, string value, Color color, Vector2 position)
        {
            var rect = Image(parent, label, card).rectTransform;
            SetRect(rect, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, new Vector2(276f, 76f));
            AddText(rect, label, 17, FontStyle.Normal, softText, new Vector2(16f, -10f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(150f, 24f), TextAnchor.MiddleLeft);
            AddText(rect, value, 24, FontStyle.Bold, color, new Vector2(150f, -24f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(108f, 32f), TextAnchor.MiddleRight);
        }

        private Image Progress(Transform parent, Vector2 position, Color color)
        {
            var track = Image(parent, "ProgressTrack", new Color(1f, 1f, 1f, 0.12f)).rectTransform;
            SetRect(track, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, new Vector2(206f, 18f));
            var fill = Image(track, "ProgressFill", color);
            Stretch(fill.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            fill.type = UnityEngine.UI.Image.Type.Filled;
            fill.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            return fill;
        }

        private void AddChip(Transform parent, string label, Color color, Vector2 position, float width)
        {
            var rect = Image(parent, label, new Color(color.r, color.g, color.b, 0.18f)).rectTransform;
            SetRect(rect, Vector2.one, Vector2.one, Vector2.one, position, new Vector2(width, 40f));
            AddText(rect, label, 17, FontStyle.Bold, color, Vector2.zero, Vector2.zero, Vector2.one, Vector2.zero, TextAnchor.MiddleCenter);
        }

        private void AddBadge(Transform parent, string title, string subtitle, Color color, Vector2 position)
        {
            var rect = Image(parent, title, new Color(color.r, color.g, color.b, 0.16f)).rectTransform;
            SetRect(rect, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), position, new Vector2(184f, 60f));
            AddText(rect, title, 19, FontStyle.Bold, color, new Vector2(12f, 32f), Vector2.zero, Vector2.zero, new Vector2(150f, 22f), TextAnchor.MiddleLeft);
            AddText(rect, subtitle, 14, FontStyle.Normal, softText, new Vector2(12f, 12f), Vector2.zero, Vector2.zero, new Vector2(150f, 20f), TextAnchor.MiddleLeft);
        }

        private void AddInfoCard(Transform parent, string title, string body, Color color, Vector2 position)
        {
            var rect = Image(parent, title, cardLight).rectTransform;
            SetRect(rect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), position, new Vector2(-108f, 156f));
            Image(rect, "Accent", color).rectTransform.sizeDelta = new Vector2(8f, 156f);
            AddText(rect, title, 26, FontStyle.Bold, Color.white, new Vector2(28f, -22f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-54f, 34f), TextAnchor.MiddleLeft);
            AddText(rect, body, 21, FontStyle.Normal, softText, new Vector2(28f, -70f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-54f, 72f), TextAnchor.UpperLeft);
        }

        private static void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void AnchorBox(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static void EnsureShowcase()
        {
            if (DemoLiverShowcase.Instance != null || FindFirstObjectByType<DemoLiverShowcase>() != null)
            {
                return;
            }

            var host = new GameObject("DemoLiverShowcase");
            DontDestroyOnLoad(host);
            host.AddComponent<DemoLiverShowcase>();
        }
    }
}
