using PatientLive.Utilities;
using UnityEngine;

namespace PatientLive.Interaction
{
    /// <summary>
    /// Owns rotation and zoom behavior for the liver model transform.
    /// Input is intentionally handled by ModelInteractionController so MR input can be added later.
    /// </summary>
    public class LiverModelController : MonoBehaviour
    {
        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 0.5f;
        [SerializeField] private bool autoRotate = true;
        [SerializeField] private float autoRotateSpeed = 8f;

        [Header("Zoom")]
        [SerializeField] private float minZoom = 0.6f;
        [SerializeField] private float maxZoom = 2.6f;
        [SerializeField] private float zoomSpeed = 0.12f;
        [SerializeField] private float zoomSmoothSpeed = 8f;

        private Vector3 initialScale;
        private float currentZoom = 1f;
        private float targetZoom = 1f;
        private bool isUserInteracting;

        private void Awake()
        {
            initialScale = transform.localScale;
        }

        private void Update()
        {
            ApplyAutoRotation();
            ApplySmoothZoom();
        }

        public void RotateModel(Vector2 pointerDelta)
        {
            try
            {
                float yaw = -pointerDelta.x * rotationSpeed;
                float pitch = pointerDelta.y * rotationSpeed;

                transform.Rotate(Vector3.up, yaw, Space.World);
                transform.Rotate(Vector3.right, pitch, Space.World);
            }
            catch (System.Exception exception)
            {
                SimpleLogger.LogException(exception, "LiverModel");
            }
        }

        public void Zoom(float zoomDelta)
        {
            targetZoom = Mathf.Clamp(targetZoom + zoomDelta * zoomSpeed, minZoom, maxZoom);
        }

        public void SetUserInteracting(bool value)
        {
            isUserInteracting = value;
        }

        public void ResetModel()
        {
            transform.rotation = Quaternion.identity;
            currentZoom = 1f;
            targetZoom = 1f;
            transform.localScale = initialScale;

            SimpleLogger.LogInfo("Liver model reset.", "LiverModel");
        }

        private void ApplyAutoRotation()
        {
            if (!autoRotate || isUserInteracting)
            {
                return;
            }

            transform.Rotate(Vector3.up, autoRotateSpeed * Time.deltaTime, Space.World);
        }

        private void ApplySmoothZoom()
        {
            if (Mathf.Approximately(currentZoom, targetZoom))
            {
                return;
            }

            currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomSmoothSpeed);
            transform.localScale = initialScale * currentZoom;
        }
    }
}
