using PatientLive.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PatientLive.UI
{
    /// <summary>
    /// Full-screen opening warning. Blocks scene interaction until the user acknowledges it.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class SafetyWarningController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text warningText;
        [SerializeField] private Button acceptButton;

        [Header("Content")]
        [SerializeField] private string title = "Önemli Uyarı";

        [TextArea(3, 8)]
        [SerializeField]
        private string warningMessage =
            "Bu uygulama teşhis amaçlı değildir.\n\n" +
            "Eğitim ve bilgilendirme amaçlıdır.\n\n" +
            "Herhangi bir sağlık sorununuz için mutlaka bir sağlık kuruluşuna başvurunuz.";

        [Header("Animation")]
        [SerializeField] private float fadeDuration = 0.35f;

        private CanvasGroup canvasGroup;
        private bool isDismissing;
        private float dismissTimer;

        public event System.Action OnWarningDismissed;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            ApplyContent();
        }

        private void OnEnable()
        {
            if (acceptButton != null)
            {
                acceptButton.onClick.AddListener(DismissWarning);
            }
        }

        private void OnDisable()
        {
            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveListener(DismissWarning);
            }
        }

        private void Update()
        {
            if (!isDismissing)
            {
                return;
            }

            dismissTimer += Time.deltaTime;
            float progress = fadeDuration <= 0f ? 1f : Mathf.Clamp01(dismissTimer / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

            if (progress >= 1f)
            {
                isDismissing = false;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                gameObject.SetActive(false);
                OnWarningDismissed?.Invoke();
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            isDismissing = false;
            dismissTimer = 0f;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            SimpleLogger.LogInfo("Safety warning shown.", "SafetyWarning");
        }

        public void DismissWarning()
        {
            if (isDismissing)
            {
                return;
            }

            isDismissing = true;
            dismissTimer = 0f;
            SimpleLogger.LogInfo("Safety warning accepted by user.", "SafetyWarning");
        }

        private void ApplyContent()
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (warningText != null)
            {
                warningText.text = warningMessage;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }
}
