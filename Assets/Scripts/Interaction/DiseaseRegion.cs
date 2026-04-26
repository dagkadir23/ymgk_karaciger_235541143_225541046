using PatientLive.Data;
using PatientLive.Utilities;
using UnityEngine;

namespace PatientLive.Interaction
{
    /// <summary>
    /// Selectable educational region placed on or near the liver model.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DiseaseRegion : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private DiseaseRegionData regionData;

        [Header("Highlight")]
        [SerializeField] private Material highlightMaterial;

        private readonly MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        private Renderer cachedRenderer;
        private Material originalMaterial;
        private bool isSelected;

        public DiseaseRegionData RegionData => regionData;
        public bool IsSelected => isSelected;

        private void Awake()
        {
            cachedRenderer = GetComponent<Renderer>();

            if (cachedRenderer != null)
            {
                originalMaterial = cachedRenderer.sharedMaterial;
            }

            if (regionData == null)
            {
                SimpleLogger.LogWarning($"Region data is missing on {name}.", "DiseaseRegion");
            }
        }

        public void Highlight()
        {
            isSelected = true;

            if (cachedRenderer == null)
            {
                return;
            }

            try
            {
                if (highlightMaterial != null)
                {
                    cachedRenderer.sharedMaterial = highlightMaterial;
                    return;
                }

                Color color = regionData != null ? regionData.highlightColor : Color.yellow;
                float intensity = regionData != null ? regionData.highlightIntensity : 1f;

                cachedRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_Color", color);
                propertyBlock.SetColor("_BaseColor", color);
                propertyBlock.SetColor("_EmissionColor", color * intensity);
                cachedRenderer.SetPropertyBlock(propertyBlock);
            }
            catch (System.Exception exception)
            {
                SimpleLogger.LogException(exception, "DiseaseRegion.Highlight");
            }
        }

        public void RemoveHighlight()
        {
            isSelected = false;

            if (cachedRenderer == null)
            {
                return;
            }

            try
            {
                if (highlightMaterial != null && originalMaterial != null)
                {
                    cachedRenderer.sharedMaterial = originalMaterial;
                }

                cachedRenderer.SetPropertyBlock(null);
            }
            catch (System.Exception exception)
            {
                SimpleLogger.LogException(exception, "DiseaseRegion.RemoveHighlight");
            }
        }
    }
}
