using UnityEngine;

namespace PatientLive.Data
{
    [CreateAssetMenu(fileName = "DiseaseRegionData", menuName = "PatientLive/Disease Region Data")]
    public class DiseaseRegionData : ScriptableObject
    {
        [Header("Basic Information")]
        [Tooltip("Displayed name of the selected liver region.")]
        public string regionName = "Bilinmeyen Bölge";

        [Tooltip("Educational classification for the selected region.")]
        public DiseaseType diseaseType = DiseaseType.Healthy;

        [Header("Patient Education Text")]
        [TextArea(3, 8)]
        public string description = "Bu bölge hasta bilgilendirme amacıyla gösterilmektedir.";

        [Header("Visual Feedback")]
        public Color highlightColor = Color.yellow;

        [Range(0f, 3f)]
        public float highlightIntensity = 1.2f;
    }

    public enum DiseaseType
    {
        Healthy,
        Tumor,
        Cyst,
        Cirrhosis,
        Fibrosis,
        Steatosis
    }
}
