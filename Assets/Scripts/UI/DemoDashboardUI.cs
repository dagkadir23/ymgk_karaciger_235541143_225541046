using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PatientLive.UI
{
    /// <summary>
    /// Self-contained demo overlay that makes the MVP scene feel populated during presentations.
    /// It intentionally uses mock values and does not depend on backend or medical data.
    /// </summary>
    public class DemoDashboardUI : MonoBehaviour
    {
        private const float ReferenceWidth = 1080f;
        private const float ReferenceHeight = 1920f;

        [SerializeField] private bool showOnStart = true;

        private readonly Color navy = new Color(0.035f, 0.055f, 0.085f, 0.96f);
        private readonly Color panel = new Color(0.07f, 0.095f, 0.13f, 0.92f);
        private readonly Color panelSoft = new Color(0.105f, 0.14f, 0.18f, 0.92f);
        private readonly Color accent = new Color(0.18f, 0.72f, 0.88f, 1f);
        private readonly Color success = new Color(0.28f, 0.82f, 0.54f, 1f);
        private readonly Color warning = new Color(1f, 0.68f, 0.22f, 1f);
        private readonly Color danger = new Color(1f, 0.28f, 0.34f, 1f);

        private Font font;
        private Text statusText;
        private Text progressText;
        private Image scanProgressFill;
        private Image confidenceFill;
        private CanvasGroup rootGroup;

        private float elapsed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindObjectOfType<DemoDashboardUI>() != null)
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

            if (EventSystem.current == null)
            {
                var eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            BuildUi();
            rootGroup.alpha = showOnStart ? 1f : 0f;
            rootGroup.interactable = showOnStart;
            rootGroup.blocksRaycasts = showOnStart;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            float scanProgress = Mathf.PingPong(elapsed * 0.12f, 1f);
            scanProgressFill.fillAmount = Mathf.Lerp(0.18f, 0.92f, scanProgress);
            confidenceFill.fillAmount = Mathf.Lerp(0.72f, 0.97f, Mathf.PingPong(elapsed * 0.18f, 1f));
            progressText.text = $"{Mathf.RoundToInt(scanProgressFill.fillAmount * 100f)}%";
            statusText.text = scanProgress > 0.82f ? "Analiz tamamlanıyor" : "Model taranıyor";
        }

        private void BuildUi()
        {
            var canvasObject = new GameObject("DemoCanvas");
            canvasObject.transform.SetParent(transform, false);

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            rootGroup = canvasObject.AddComponent<CanvasGroup>();

            CreateFullScreenBackground(canvasObject.transform);
            CreateHeader(canvasObject.transform);
            CreateLeftPanel(canvasObject.transform);
            CreateRightPanel(canvasObject.transform);
            CreateBottomPanel(canvasObject.transform);
        }

        private void CreateFullScreenBackground(Transform parent)
        {
            var background = CreatePanel(parent, "DemoTint", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, navy);
            background.raycastTarget = false;
        }

        private void CreateHeader(Transform parent)
        {
            var header = CreatePanel(parent, "Header", new Vector2(0f, 1f), Vector2.one, new Vector2(0f, 1f), new Vector2(0f, -132f), panel);
            AddText(header.transform, "PatientLive", 42, FontStyle.Bold, Color.white, new Vector2(40f, -22f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(420f, 52f), TextAnchor.UpperLeft);
            AddText(header.transform, "Karaciğer MR Görselleştirme - Demo Oturumu", 24, FontStyle.Normal, new Color(0.74f, 0.84f, 0.9f), new Vector2(40f, -78f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(620f, 38f), TextAnchor.UpperLeft);

            CreateChip(header.transform, "ONLINE", success, new Vector2(-310f, -40f));
            statusText = AddText(header.transform, "Model taranıyor", 26, FontStyle.Bold, Color.white, new Vector2(-190f, -36f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(160f, 40f), TextAnchor.MiddleLeft);
        }

        private void CreateLeftPanel(Transform parent)
        {
            var left = CreatePanel(parent, "ToolPanel", new Vector2(0f, 0.34f), new Vector2(0f, 0.94f), new Vector2(0f, 1f), new Vector2(28f, -170f), panel);
            SetSize(left.rectTransform, 310f, 1050f);

            AddText(left.transform, "Kontroller", 30, FontStyle.Bold, Color.white, new Vector2(28f, -26f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(240f, 46f), TextAnchor.MiddleLeft);
            AddButton(left.transform, "3B Model", accent, new Vector2(26f, -112f));
            AddButton(left.transform, "Lezyonlar", danger, new Vector2(26f, -214f));
            AddButton(left.transform, "Damar Yapısı", warning, new Vector2(26f, -316f));
            AddButton(left.transform, "Rapor Önizleme", success, new Vector2(26f, -418f));

            AddText(left.transform, "Katmanlar", 26, FontStyle.Bold, Color.white, new Vector2(28f, -560f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(240f, 40f), TextAnchor.MiddleLeft);
            AddToggleRow(left.transform, "Sağlıklı Doku", success, new Vector2(28f, -628f));
            AddToggleRow(left.transform, "Tümör Adayı", danger, new Vector2(28f, -704f));
            AddToggleRow(left.transform, "Kist Bölgesi", accent, new Vector2(28f, -780f));
        }

        private void CreateRightPanel(Transform parent)
        {
            var right = CreatePanel(parent, "MetricsPanel", Vector2.one, Vector2.one, Vector2.one, new Vector2(-28f, -170f), panel);
            SetSize(right.rectTransform, 330f, 620f);

            AddText(right.transform, "Canlı Analiz", 30, FontStyle.Bold, Color.white, new Vector2(26f, -26f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(260f, 44f), TextAnchor.MiddleLeft);
            AddMetric(right.transform, "Segmentasyon", "Aktif", success, new Vector2(26f, -105f));
            AddMetric(right.transform, "Bulgu Sayısı", "3", warning, new Vector2(26f, -204f));
            AddMetric(right.transform, "Risk Skoru", "Orta", danger, new Vector2(26f, -303f));

            AddText(right.transform, "Tarama", 24, FontStyle.Bold, Color.white, new Vector2(26f, -420f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(140f, 34f), TextAnchor.MiddleLeft);
            scanProgressFill = CreateProgress(right.transform, new Vector2(26f, -472f), accent);
            progressText = AddText(right.transform, "0%", 22, FontStyle.Bold, Color.white, new Vector2(242f, -456f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(60f, 34f), TextAnchor.MiddleRight);

            AddText(right.transform, "Güven", 24, FontStyle.Bold, Color.white, new Vector2(26f, -535f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(140f, 34f), TextAnchor.MiddleLeft);
            confidenceFill = CreateProgress(right.transform, new Vector2(26f, -586f), success);
        }

        private void CreateBottomPanel(Transform parent)
        {
            var bottom = CreatePanel(parent, "InfoPanel", Vector2.zero, new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(28f, 28f), panel);
            bottom.rectTransform.offsetMin = new Vector2(28f, 28f);
            bottom.rectTransform.offsetMax = new Vector2(-28f, 322f);

            AddText(bottom.transform, "Seçili Bölge: Segment IV - Tümör Adayı", 34, FontStyle.Bold, Color.white, new Vector2(32f, -28f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(760f, 48f), TextAnchor.MiddleLeft);
            AddText(bottom.transform, "MR kesitleri üzerinden işaretlenen alanlar eğitim amaçlı olarak renklendirildi. Sistem, doktor incelemesine yardımcı olacak ön rapor görünümünü hazırlıyor.", 25, FontStyle.Normal, new Color(0.78f, 0.86f, 0.9f), new Vector2(32f, -90f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-64f, 102f), TextAnchor.UpperLeft);

            AddPill(bottom.transform, "HCC olasılığı", "68%", danger, new Vector2(32f, 42f));
            AddPill(bottom.transform, "Kist", "22%", accent, new Vector2(300f, 42f));
            AddPill(bottom.transform, "Normal doku", "91%", success, new Vector2(510f, 42f));
        }

        private Image CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;

            var image = obj.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private Text AddText(Transform parent, string text, int size, FontStyle style, Color color, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, TextAnchor alignment)
        {
            var obj = new GameObject(text);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(anchorMin.x, anchorMin.y);
            rect.anchoredPosition = anchoredPosition;
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

        private void AddButton(Transform parent, string label, Color color, Vector2 position)
        {
            var image = CreatePanel(parent, label + " Button", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, panelSoft);
            SetSize(image.rectTransform, 258f, 78f);
            image.color = new Color(color.r, color.g, color.b, 0.24f);
            image.gameObject.AddComponent<Button>();
            AddText(image.transform, label, 24, FontStyle.Bold, Color.white, new Vector2(24f, -15f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(210f, 46f), TextAnchor.MiddleLeft);
        }

        private void AddToggleRow(Transform parent, string label, Color color, Vector2 position)
        {
            var dot = CreatePanel(parent, label + " Dot", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, color);
            SetSize(dot.rectTransform, 28f, 28f);
            AddText(parent, label, 22, FontStyle.Normal, Color.white, position + new Vector2(42f, 8f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(190f, 34f), TextAnchor.MiddleLeft);
        }

        private void AddMetric(Transform parent, string label, string value, Color color, Vector2 position)
        {
            var metric = CreatePanel(parent, label + " Metric", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, panelSoft);
            SetSize(metric.rectTransform, 278f, 76f);
            AddText(metric.transform, label, 20, FontStyle.Normal, new Color(0.72f, 0.81f, 0.87f), new Vector2(18f, -10f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(150f, 30f), TextAnchor.MiddleLeft);
            AddText(metric.transform, value, 26, FontStyle.Bold, color, new Vector2(180f, -18f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(80f, 40f), TextAnchor.MiddleRight);
        }

        private Image CreateProgress(Transform parent, Vector2 position, Color fillColor)
        {
            var track = CreatePanel(parent, "ProgressTrack", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, new Color(1f, 1f, 1f, 0.12f));
            SetSize(track.rectTransform, 210f, 20f);

            var fill = CreatePanel(track.transform, "ProgressFill", Vector2.zero, Vector2.one, new Vector2(0f, 0.5f), Vector2.zero, fillColor);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 0.1f;
            fill.rectTransform.offsetMin = Vector2.zero;
            fill.rectTransform.offsetMax = Vector2.zero;
            return fill;
        }

        private void AddPill(Transform parent, string label, string value, Color color, Vector2 position)
        {
            var pill = CreatePanel(parent, label + " Pill", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), position, new Color(color.r, color.g, color.b, 0.22f));
            SetSize(pill.rectTransform, 230f, 72f);
            AddText(pill.transform, label, 18, FontStyle.Normal, new Color(0.8f, 0.88f, 0.92f), new Vector2(18f, 46f), Vector2.zero, Vector2.zero, new Vector2(150f, 24f), TextAnchor.MiddleLeft);
            AddText(pill.transform, value, 28, FontStyle.Bold, color, new Vector2(150f, 36f), Vector2.zero, Vector2.zero, new Vector2(58f, 32f), TextAnchor.MiddleRight);
        }

        private void CreateChip(Transform parent, string label, Color color, Vector2 position)
        {
            var chip = CreatePanel(parent, label + " Chip", Vector2.one, Vector2.one, Vector2.one, position, new Color(color.r, color.g, color.b, 0.24f));
            SetSize(chip.rectTransform, 108f, 42f);
            AddText(chip.transform, label, 18, FontStyle.Bold, color, new Vector2(0f, 0f), Vector2.zero, Vector2.one, Vector2.zero, TextAnchor.MiddleCenter);
        }

        private static void SetSize(RectTransform rect, float width, float height)
        {
            rect.sizeDelta = new Vector2(width, height);
        }
    }
}
