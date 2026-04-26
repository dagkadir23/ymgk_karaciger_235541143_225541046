using PatientLive.UI;
using PatientLive.Utilities;
using UnityEngine;

namespace PatientLive.Core
{
    /// <summary>
    /// Scene-level bootstrapper. Applies prototype-wide settings and waits for the safety warning.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class AppInitializer : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private SafetyWarningController safetyWarning;
        [SerializeField] private GameObject mainContentRoot;

        [Header("Performance")]
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private bool enableVSync;
        [SerializeField] private bool useLowHardwareMode;

        [Header("Logging")]
        [SerializeField] private SimpleLogger.LogLevel logLevel = SimpleLogger.LogLevel.Debug;

        private void Awake()
        {
            SimpleLogger.SetLogLevel(logLevel);
            SimpleLogger.LogInfo("PatientLive prototype starting.", "Core");

            ConfigurePerformance();

            if (mainContentRoot != null)
            {
                mainContentRoot.SetActive(false);
            }
        }

        private void Start()
        {
            if (safetyWarning == null)
            {
                SimpleLogger.LogWarning("Safety warning is not assigned. Main content will be shown immediately.", "Core");
                ActivateMainContent();
                return;
            }

            safetyWarning.OnWarningDismissed += HandleWarningDismissed;
            safetyWarning.Show();
        }

        private void OnDestroy()
        {
            if (safetyWarning != null)
            {
                safetyWarning.OnWarningDismissed -= HandleWarningDismissed;
            }
        }

        private void HandleWarningDismissed()
        {
            SimpleLogger.LogInfo("Safety warning accepted.", "Core");
            ActivateMainContent();
        }

        private void ActivateMainContent()
        {
            if (mainContentRoot == null)
            {
                SimpleLogger.LogWarning("Main content root is not assigned.", "Core");
                return;
            }

            mainContentRoot.SetActive(true);
        }

        private void ConfigurePerformance()
        {
            QualitySettings.vSyncCount = enableVSync ? 1 : 0;
            Application.targetFrameRate = Mathf.Clamp(targetFrameRate, 30, 120);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (useLowHardwareMode || SystemInfo.systemMemorySize > 0 && SystemInfo.systemMemorySize < 4096)
            {
                ApplyLowHardwareSettings();
            }

            SimpleLogger.LogInfo(
                $"Performance configured. FPS={Application.targetFrameRate}, VSync={QualitySettings.vSyncCount}, MemoryMB={SystemInfo.systemMemorySize}",
                "Core");
        }

        private static void ApplyLowHardwareSettings()
        {
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.antiAliasing = 0;
            QualitySettings.masterTextureLimit = 1;

            SimpleLogger.LogWarning("Low hardware mode enabled: shadows, anti-aliasing and texture quality reduced.", "Core");
        }
    }
}
