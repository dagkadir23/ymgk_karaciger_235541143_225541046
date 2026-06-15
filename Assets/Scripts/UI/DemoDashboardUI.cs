using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PatientLive.UI
{
    /// <summary>
    /// Polished presentation UI with working fake controls for the project demo.
    /// </summary>
    public class DemoDashboardUI : MonoBehaviour
    {
        private const float ReferenceWidth = 1080f;
        private const float ReferenceHeight = 1920f;

        private readonly Color page = new Color(0.02f, 0.028f, 0.04f, 0.68f);
        private readonly Color panel = new Color(0.055f, 0.072f, 0.096f, 0.94f);
        private readonly Color panelAlt = new Color(0.078f, 0.105f, 0.135f, 0.92f);
        private readonly Color line = new Color(1f, 1f, 1f, 0.12f);
        private readonly Color cyan = new Color(0.15f, 0.78f, 0.94f, 1f);
        private readonly Color green = new Color(0.24f, 0.82f, 0.52f, 1f);
        private readonly Color amber = new Color(1f, 0.68f, 0.22f, 1f);
        private readonly Color red = new Color(1f, 0.24f, 0.31f, 1f);
        private readonly Color textSoft = new Color(0.72f, 0.82f, 0.88f, 1f);

        private Font font;
        private Text statusText;
        private Text insightTitle;
        private Text insightBody;
        private Text scanPercentText;
        private Text confidenceText;
        private Image scanFill;
        private Image confidenceFill;
        private float elapsed;

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
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            float scan = Mathf.PingPong(elapsed * 0.1f, 1f);
            float confidence = Mathf.Lerp(0.78f, 0.96f, Mathf.PingPong(elapsed * 0.16f, 1f));

            scanFill.fillAmount = Mathf.Lerp(0.28f, 0.94f, scan);
            confidenceFill.fillAmount = confidence;
            scanPercentText.text = Mathf.RoundToInt(scanFill.fillAmount * 100f) + "%";
            confidenceText.text = Mathf.RoundToInt(confidence * 100f) + "%";
            statusText.text = scan > 0.86f ? "Rapor hazırlanıyor" : "Canlı analiz";
        }

        private void BuildUi()
        {
            var canvasObject = new GameObject("DemoCanvas");
            canvasObject.transform.SetParent(transform, false);

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 80;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            var tint = Panel(canvasObject.transform, "ScreenTint", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, page);
            tint.rectTransform.offsetMin = Vector2.zero;
            tint.rectTransform.offsetMax = Vector2.zero;
            tint.raycastTarget = false;

            Header(canvasObject.transform);
            LeftControls(canvasObject.transform);
            RightVitals(canvasObject.transform);
            BottomInsight(canvasObject.transform);
            CenterHints(canvasObject.transform);
        }

        private void Header(Transform parent)
        {
            var header = Panel(parent, "Header", new Vector2(0f, 1f), Vector2.one, new Vector2(0f, 1f), Vector2.zero, panel);
            header.rectTransform.offsetMin = new Vector2(0f, -132f);
            header.rectTransform.offsetMax = Vector2.zero;

            AddText(header.transform, "PatientLive", 42, FontStyle.Bold, Color.white, new Vector2(36f, -22f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(330f, 54f), TextAnchor.UpperLeft);
            AddText(header.transform, "Karaciğer sağlığı görselleştirme paneli", 24, FontStyle.Normal, textSoft, new Vector2(36f, -78f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(520f, 36f), TextAnchor.UpperLeft);
            AddChip(header.transform, "DEMO", cyan, new Vector2(-420f, -42f), 104f);
            AddChip(header.transform, "MR-SEG AI", green, new Vector2(-294f, -42f), 144f);
            statusText = AddText(header.transform, "Canlı analiz", 25, FontStyle.Bold, Color.white, new Vector2(-132f, -37f), Vector2.one, Vector2.one, new Vector2(112f, 34f), TextAnchor.MiddleRight);
        }

        private void LeftControls(Transform parent)
        {
            var left = Panel(parent, "ControlsPanel", new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(28f, 12f), panel);
            SetSize(left.rectTransform, 305f, 1120f);

            AddText(left.transform, "Kontrol Merkezi", 29, FontStyle.Bold, Color.white, new Vector2(24f, 520f), Vector2.zero, Vector2.zero, new Vector2(250f, 42f), TextAnchor.MiddleLeft);
            AddText(left.transform, "Sunum için çalışan sahte modlar", 18, FontStyle.Normal, textSoft, new Vector2(24f, 486f), Vector2.zero, Vector2.zero, new Vector2(250f, 28f), TextAnchor.MiddleLeft);

            AddActionButton(left.transform, "Risk Haritası", "Kırmızı taralı alanı aç", red, new Vector2(24f, 398f), () => SetInsight("Risk Haritası", DemoLiverShowcase.Instance.ShowRiskMode()));
            AddActionButton(left.transform, "Sağlıklı Doku", "Normal alanları yeşil göster", green, new Vector2(24f, 290f), () => SetInsight("Sağlıklı Doku", DemoLiverShowcase.Instance.ShowHealthyMode()));
            AddActionButton(left.transform, "Taralı Bölgeler", "Hastalık katmanını aç/kapat", amber, new Vector2(24f, 182f), () => SetInsight("Taralı Bölgeler", DemoLiverShowcase.Instance.ToggleLesionLayer()));
            AddActionButton(left.transform, "Damar Katmanı", "Mavi damar görünümü", cyan, new Vector2(24f, 74f), () => SetInsight("Damar Katmanı", DemoLiverShowcase.Instance.ToggleVesselLayer()));
            AddActionButton(left.transform, "Ön Rapor", "Şüpheli alanları büyüt", green, new Vector2(24f, -34f), () => SetInsight("Ön Rapor", DemoLiverShowcase.Instance.ToggleReportMode()));

            AddText(left.transform, "Model Görünümü", 24, FontStyle.Bold, Color.white, new Vector2(24f, -178f), Vector2.zero, Vector2.zero, new Vector2(250f, 36f), TextAnchor.MiddleLeft);
            AddSquareButton(left.transform, "+", cyan, new Vector2(24f, -250f), () => { DemoLiverShowcase.Instance.ZoomIn(); SetInsight("Yakınlaştırma", "3B karaciğer modeli yaklaştırıldı; taralı bölgeler daha net incelenebilir."); });
            AddSquareButton(left.transform, "-", cyan, new Vector2(112f, -250f), () => { DemoLiverShowcase.Instance.ZoomOut(); SetInsight("Uzaklaştırma", "Model uzaklaştırıldı; tüm karaciğer anatomisi tek ekranda görüntüleniyor."); });
            AddSquareButton(left.transform, "0", amber, new Vector2(200f, -250f), () => { DemoLiverShowcase.Instance.ResetView(); SetInsight("Görünüm Sıfırlandı", "Model varsayılan açı ve ölçeğe döndürüldü."); });
        }

        private void RightVitals(Transform parent)
        {
            var right = Panel(parent, "VitalsPanel", new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-28f, 90f), panel);
            SetSize(right.rectTransform, 325f, 880f);

            AddText(right.transform, "Analiz Özeti", 29, FontStyle.Bold, Color.white, new Vector2(24f, 392f), Vector2.zero, Vector2.zero, new Vector2(260f, 42f), TextAnchor.MiddleLeft);
            AddMetric(right.transform, "Karaciğer hacmi", "1.42 L", cyan, new Vector2(24f, 306f));
            AddMetric(right.transform, "Şüpheli alan", "2 bölge", red, new Vector2(24f, 202f));
            AddMetric(right.transform, "Kist olasılığı", "Düşük", amber, new Vector2(24f, 98f));
            AddMetric(right.transform, "Genel durum", "İzlem", green, new Vector2(24f, -6f));

            AddText(right.transform, "Tarama ilerlemesi", 22, FontStyle.Bold, Color.white, new Vector2(24f, -142f), Vector2.zero, Vector2.zero, new Vector2(210f, 32f), TextAnchor.MiddleLeft);
            scanFill = AddProgress(right.transform, new Vector2(24f, -192f), cyan);
            scanPercentText = AddText(right.transform, "0%", 21, FontStyle.Bold, Color.white, new Vector2(244f, -180f), Vector2.zero, Vector2.zero, new Vector2(50f, 30f), TextAnchor.MiddleRight);

            AddText(right.transform, "Model güveni", 22, FontStyle.Bold, Color.white, new Vector2(24f, -260f), Vector2.zero, Vector2.zero, new Vector2(210f, 32f), TextAnchor.MiddleLeft);
            confidenceFill = AddProgress(right.transform, new Vector2(24f, -310f), green);
            confidenceText = AddText(right.transform, "0%", 21, FontStyle.Bold, Color.white, new Vector2(244f, -298f), Vector2.zero, Vector2.zero, new Vector2(50f, 30f), TextAnchor.MiddleRight);
        }

        private void BottomInsight(Transform parent)
        {
            var bottom = Panel(parent, "InsightPanel", Vector2.zero, new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(28f, 28f), panel);
            bottom.rectTransform.offsetMin = new Vector2(28f, 28f);
            bottom.rectTransform.offsetMax = new Vector2(-28f, 322f);

            insightTitle = AddText(bottom.transform, "Karaciğer Sağlığı Bilgilendirmesi", 32, FontStyle.Bold, Color.white, new Vector2(30f, -22f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(760f, 44f), TextAnchor.MiddleLeft);
            insightBody = AddText(bottom.transform,
                "Bu demo panelinde taralı alanların rengi seçilen hastalık senaryosuna göre değişir. Kırmızı bölge tümör şüphesi gibi dikkat gerektiren bulguları, camgöbeği bölge kistik oluşumu, yeşil alanlar ise sağlıklı karaciğer dokusunu temsil eder. Görselleştirme teşhis yerine eğitim ve hekim incelemesine yardımcı ön bilgilendirme amacıyla kullanılır.",
                24,
                FontStyle.Normal,
                textSoft,
                new Vector2(30f, -80f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-60f, 120f),
                TextAnchor.UpperLeft);

            AddSmallBadge(bottom.transform, "Tümör şüphesi", "Kırmızı", red, new Vector2(30f, 48f));
            AddSmallBadge(bottom.transform, "Kistik alan", "Camgöbeği", cyan, new Vector2(260f, 48f));
            AddSmallBadge(bottom.transform, "Sağlıklı doku", "Yeşil", green, new Vector2(492f, 48f));
            AddSmallBadge(bottom.transform, "Ön rapor", "Sahte veri", amber, new Vector2(722f, 48f));
        }

        private void CenterHints(Transform parent)
        {
            var title = AddText(parent, "3B KARACİĞER MODELİ", 22, FontStyle.Bold, new Color(1f, 1f, 1f, 0.78f), new Vector2(0f, 300f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(360f, 34f), TextAnchor.MiddleCenter);
            title.raycastTarget = false;
            var hint = AddText(parent, "Butonlarla katmanları değiştir, + / - ile yakınlaştır", 18, FontStyle.Normal, new Color(0.8f, 0.9f, 0.96f, 0.72f), new Vector2(0f, 262f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(520f, 30f), TextAnchor.MiddleCenter);
            hint.raycastTarget = false;
        }

        private void SetInsight(string title, string body)
        {
            insightTitle.text = title;
            insightBody.text = body + "\n\nRenkler demonstrasyon amaçlıdır; gerçek klinik değerlendirme için hekim incelemesi gerekir.";
        }

        private Image Panel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = position;

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

            var label = obj.AddComponent<Text>();
            label.font = font;
            label.text = text;
            label.fontSize = size;
            label.fontStyle = style;
            label.color = color;
            label.alignment = alignment;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            return label;
        }

        private void AddActionButton(Transform parent, string title, string subtitle, Color color, Vector2 position, UnityAction action)
        {
            var image = Panel(parent, title, Vector2.zero, Vector2.zero, Vector2.zero, position, panelAlt);
            SetSize(image.rectTransform, 257f, 86f);
            var stripe = Panel(image.transform, "Accent", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0f), Vector2.zero, color);
            stripe.rectTransform.sizeDelta = new Vector2(6f, 0f);

            var button = image.gameObject.AddComponent<Button>();
            button.onClick.AddListener(action);
            AddText(image.transform, title, 22, FontStyle.Bold, Color.white, new Vector2(18f, 49f), Vector2.zero, Vector2.zero, new Vector2(210f, 26f), TextAnchor.MiddleLeft);
            AddText(image.transform, subtitle, 16, FontStyle.Normal, textSoft, new Vector2(18f, 20f), Vector2.zero, Vector2.zero, new Vector2(218f, 22f), TextAnchor.MiddleLeft);
        }

        private void AddSquareButton(Transform parent, string label, Color color, Vector2 position, UnityAction action)
        {
            var image = Panel(parent, label, Vector2.zero, Vector2.zero, Vector2.zero, position, new Color(color.r, color.g, color.b, 0.25f));
            SetSize(image.rectTransform, 68f, 68f);
            var button = image.gameObject.AddComponent<Button>();
            button.onClick.AddListener(action);
            AddText(image.transform, label, 30, FontStyle.Bold, color, new Vector2(0f, 0f), Vector2.zero, Vector2.one, Vector2.zero, TextAnchor.MiddleCenter);
        }

        private void AddMetric(Transform parent, string label, string value, Color color, Vector2 position)
        {
            var metric = Panel(parent, label, Vector2.zero, Vector2.zero, Vector2.zero, position, panelAlt);
            SetSize(metric.rectTransform, 277f, 76f);
            AddText(metric.transform, label, 18, FontStyle.Normal, textSoft, new Vector2(18f, 46f), Vector2.zero, Vector2.zero, new Vector2(150f, 24f), TextAnchor.MiddleLeft);
            AddText(metric.transform, value, 25, FontStyle.Bold, color, new Vector2(154f, 22f), Vector2.zero, Vector2.zero, new Vector2(104f, 34f), TextAnchor.MiddleRight);
        }

        private Image AddProgress(Transform parent, Vector2 position, Color color)
        {
            var track = Panel(parent, "Track", Vector2.zero, Vector2.zero, Vector2.zero, position, line);
            SetSize(track.rectTransform, 210f, 18f);

            var fill = Panel(track.transform, "Fill", Vector2.zero, Vector2.one, new Vector2(0f, 0.5f), Vector2.zero, color);
            fill.rectTransform.offsetMin = Vector2.zero;
            fill.rectTransform.offsetMax = Vector2.zero;
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 0.5f;
            return fill;
        }

        private void AddChip(Transform parent, string label, Color color, Vector2 position, float width)
        {
            var chip = Panel(parent, label, Vector2.one, Vector2.one, Vector2.one, position, new Color(color.r, color.g, color.b, 0.2f));
            SetSize(chip.rectTransform, width, 42f);
            AddText(chip.transform, label, 18, FontStyle.Bold, color, Vector2.zero, Vector2.zero, Vector2.one, Vector2.zero, TextAnchor.MiddleCenter);
        }

        private void AddSmallBadge(Transform parent, string label, string value, Color color, Vector2 position)
        {
            var badge = Panel(parent, label, Vector2.zero, Vector2.zero, Vector2.zero, position, new Color(color.r, color.g, color.b, 0.16f));
            SetSize(badge.rectTransform, 204f, 62f);
            AddText(badge.transform, label, 15, FontStyle.Normal, textSoft, new Vector2(14f, 36f), Vector2.zero, Vector2.zero, new Vector2(150f, 20f), TextAnchor.MiddleLeft);
            AddText(badge.transform, value, 20, FontStyle.Bold, color, new Vector2(14f, 12f), Vector2.zero, Vector2.zero, new Vector2(170f, 24f), TextAnchor.MiddleLeft);
        }

        private static void SetSize(RectTransform rect, float width, float height)
        {
            rect.sizeDelta = new Vector2(width, height);
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
