using PatientLive.Data;
using PatientLive.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PatientLive.UI
{
    /// <summary>
    /// Displays educational text for the selected liver region.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class DiseaseInfoPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text typeText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Image typeColorIndicator;

        [Header("Animation")]
        [SerializeField] private float fadeSpeed = 6f;

        private CanvasGroup canvasGroup;
        private float targetAlpha;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            SetVisible(false, true);
        }

        private void Update()
        {
            if (Mathf.Approximately(canvasGroup.alpha, targetAlpha))
            {
                return;
            }

            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

            if (canvasGroup.alpha <= 0.01f)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public void ShowRegionInfo(DiseaseRegionData data)
        {
            if (data == null)
            {
                SimpleLogger.LogWarning("Cannot show region info because data is null.", "UI");
                return;
            }

            if (titleText != null)
            {
                titleText.text = data.regionName;
            }

            if (typeText != null)
            {
                typeText.text = GetDiseaseTypeLabel(data.diseaseType);
            }

            if (descriptionText != null)
            {
                descriptionText.text = data.description;
            }

            if (typeColorIndicator != null)
            {
                typeColorIndicator.color = data.highlightColor;
            }

            SetVisible(true);
        }

        public void HidePanel()
        {
            SetVisible(false);
        }

        private void SetVisible(bool visible, bool instant = false)
        {
            targetAlpha = visible ? 1f : 0f;

            if (instant)
            {
                canvasGroup.alpha = targetAlpha;
            }

            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        private static string GetDiseaseTypeLabel(DiseaseType type)
        {
            switch (type)
            {
                case DiseaseType.Healthy:
                    return "Sağlıklı Doku";
                case DiseaseType.Tumor:
                    return "Tümör";
                case DiseaseType.Cyst:
                    return "Kist";
                case DiseaseType.Cirrhosis:
                    return "Siroz";
                case DiseaseType.Fibrosis:
                    return "Fibrozis";
                case DiseaseType.Steatosis:
                    return "Yağlı Karaciğer";
                default:
                    return "Bilinmeyen";
            }
        }
    }
}
