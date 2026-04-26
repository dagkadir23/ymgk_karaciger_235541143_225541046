using PatientLive.UI;
using PatientLive.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PatientLive.Interaction
{
    /// <summary>
    /// Reads mouse/touch input, rotates/zooms the model, and selects disease regions by raycast.
    /// This is the future extension point for Meta Quest, HoloLens, or another MR input provider.
    /// </summary>
    public class ModelInteractionController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LiverModelController liverModel;
        [SerializeField] private DiseaseInfoPanel infoPanel;
        [SerializeField] private Camera raycastCamera;

        [Header("Selection")]
        [SerializeField] private float raycastDistance = 100f;
        [SerializeField] private float clickThreshold = 8f;

        private Vector2 pointerDownPosition;
        private Vector2 lastPointerPosition;
        private DiseaseRegion selectedRegion;

        private void Awake()
        {
            if (raycastCamera == null)
            {
                raycastCamera = Camera.main;
            }
        }

        private void Start()
        {
            ValidateReferences();
        }

        private void Update()
        {
            if (IsPointerOverUI())
            {
                liverModel?.SetUserInteracting(false);
                return;
            }

            HandleMouseInput();
            HandleTouchInput();
            HandleScrollZoom();
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                pointerDownPosition = Input.mousePosition;
                lastPointerPosition = Input.mousePosition;
                liverModel?.SetUserInteracting(true);
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 currentPosition = Input.mousePosition;
                Vector2 delta = currentPosition - lastPointerPosition;

                if (delta.sqrMagnitude > 0.01f)
                {
                    liverModel?.RotateModel(delta);
                }

                lastPointerPosition = currentPosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                liverModel?.SetUserInteracting(false);

                float dragDistance = Vector2.Distance(pointerDownPosition, Input.mousePosition);
                if (dragDistance <= clickThreshold)
                {
                    TrySelectRegion(Input.mousePosition);
                }
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 0)
            {
                return;
            }

            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        pointerDownPosition = touch.position;
                        liverModel?.SetUserInteracting(true);
                        break;
                    case TouchPhase.Moved:
                        liverModel?.RotateModel(touch.deltaPosition);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        liverModel?.SetUserInteracting(false);
                        if (Vector2.Distance(pointerDownPosition, touch.position) <= clickThreshold)
                        {
                            TrySelectRegion(touch.position);
                        }
                        break;
                }
            }

            if (Input.touchCount == 2)
            {
                Touch first = Input.GetTouch(0);
                Touch second = Input.GetTouch(1);

                Vector2 previousFirst = first.position - first.deltaPosition;
                Vector2 previousSecond = second.position - second.deltaPosition;
                float previousDistance = Vector2.Distance(previousFirst, previousSecond);
                float currentDistance = Vector2.Distance(first.position, second.position);

                liverModel?.Zoom((currentDistance - previousDistance) * 0.01f);
            }
        }

        private void HandleScrollZoom()
        {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                liverModel?.Zoom(scrollDelta * 10f);
            }
        }

        private void TrySelectRegion(Vector2 screenPosition)
        {
            if (raycastCamera == null)
            {
                SimpleLogger.LogError("Selection failed because raycast camera is missing.", "Interaction");
                return;
            }

            try
            {
                Ray ray = raycastCamera.ScreenPointToRay(screenPosition);
                if (!Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
                {
                    DeselectCurrentRegion();
                    return;
                }

                DiseaseRegion region = hit.collider.GetComponentInParent<DiseaseRegion>();
                if (region == null)
                {
                    DeselectCurrentRegion();
                    return;
                }

                SelectRegion(region);
            }
            catch (System.Exception exception)
            {
                SimpleLogger.LogException(exception, "Interaction.Selection");
            }
        }

        private void SelectRegion(DiseaseRegion region)
        {
            if (selectedRegion == region)
            {
                DeselectCurrentRegion();
                return;
            }

            selectedRegion?.RemoveHighlight();
            selectedRegion = region;
            selectedRegion.Highlight();

            if (selectedRegion.RegionData != null)
            {
                infoPanel?.ShowRegionInfo(selectedRegion.RegionData);
                SimpleLogger.LogInfo($"Region selected: {selectedRegion.RegionData.regionName}", "Interaction");
            }
            else
            {
                infoPanel?.HidePanel();
                SimpleLogger.LogWarning("Selected region has no data.", "Interaction");
            }
        }

        private void DeselectCurrentRegion()
        {
            selectedRegion?.RemoveHighlight();
            selectedRegion = null;
            infoPanel?.HidePanel();
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private void ValidateReferences()
        {
            if (liverModel == null)
            {
                SimpleLogger.LogError("LiverModelController reference is missing.", "Interaction");
            }

            if (infoPanel == null)
            {
                SimpleLogger.LogWarning("DiseaseInfoPanel reference is missing. Selection text will not be shown.", "Interaction");
            }

            if (raycastCamera == null)
            {
                SimpleLogger.LogError("Raycast camera is missing.", "Interaction");
            }
        }
    }
}
