using UnityEngine;
using UnityEngine.EventSystems;

namespace PatientLive.UI
{
    /// <summary>
    /// Presentation-only 3D liver model used when the real scene is not wired yet.
    /// </summary>
    public class DemoLiverShowcase : MonoBehaviour
    {
        public static DemoLiverShowcase Instance { get; private set; }

        private readonly Color liverColor = new Color(0.62f, 0.16f, 0.18f, 1f);
        private readonly Color healthyColor = new Color(0.22f, 0.8f, 0.45f, 1f);
        private readonly Color lesionColor = new Color(1f, 0.22f, 0.3f, 1f);
        private readonly Color vesselColor = new Color(0.16f, 0.72f, 1f, 1f);
        private readonly Color cystColor = new Color(0.22f, 0.86f, 0.95f, 1f);

        private Transform modelRoot;
        private Transform assetModel;
        private Transform lesionRegion;
        private Transform cystRegion;
        private Transform healthyRegion;
        private Transform vesselLayer;
        private Camera showcaseCamera;
        private float targetZoom = 1f;
        private float currentZoom = 1f;
        private bool lesionMode = true;
        private bool vesselMode = true;
        private bool reportMode;
        private bool isDragging;
        private Vector2 lastPointerPosition;
        private float manualInputTimer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindFirstObjectByType<DemoLiverShowcase>() != null)
            {
                return;
            }

            var host = new GameObject("DemoLiverShowcase");
            DontDestroyOnLoad(host);
            host.AddComponent<DemoLiverShowcase>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            BuildScene();
        }

        private void Update()
        {
            if (modelRoot == null)
            {
                return;
            }

            HandleModelInput();

            manualInputTimer = Mathf.Max(0f, manualInputTimer - Time.deltaTime);
            if (manualInputTimer <= 0f)
            {
                modelRoot.Rotate(Vector3.up, 12f * Time.deltaTime, Space.World);
                modelRoot.Rotate(Vector3.forward, Mathf.Sin(Time.time * 0.8f) * 2f * Time.deltaTime, Space.Self);
            }

            currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * 5f);
            modelRoot.localScale = Vector3.one * currentZoom;
        }

        public void ZoomIn()
        {
            targetZoom = Mathf.Clamp(targetZoom + 0.18f, 0.75f, 1.75f);
        }

        public void ZoomOut()
        {
            targetZoom = Mathf.Clamp(targetZoom - 0.18f, 0.75f, 1.75f);
        }

        public void ResetView()
        {
            targetZoom = 1f;
            currentZoom = 1f;
            modelRoot.rotation = Quaternion.Euler(-8f, -20f, 8f);
        }

        public void RotateByDrag(Vector2 delta)
        {
            manualInputTimer = 1.4f;
            modelRoot.Rotate(Vector3.up, -delta.x * 0.22f, Space.World);
            modelRoot.Rotate(Vector3.right, delta.y * 0.16f, Space.World);
        }

        public string ToggleLesionLayer()
        {
            lesionMode = !lesionMode;
            lesionRegion.gameObject.SetActive(lesionMode);
            cystRegion.gameObject.SetActive(lesionMode);
            return lesionMode ? "Taralı hastalık bölgeleri görünür." : "Taralı hastalık bölgeleri gizlendi.";
        }

        public string ToggleVesselLayer()
        {
            vesselMode = !vesselMode;
            vesselLayer.gameObject.SetActive(vesselMode);
            return vesselMode ? "Damar katmanı açıldı." : "Damar katmanı kapatıldı.";
        }

        public string ToggleReportMode()
        {
            reportMode = !reportMode;
            SetRegionScale(lesionRegion, reportMode ? 1.22f : 1f);
            SetRegionScale(cystRegion, reportMode ? 1.16f : 1f);
            return reportMode ? "Ön rapor modu: şüpheli alanlar vurgulandı." : "Ön rapor modu kapatıldı.";
        }

        public string ShowHealthyMode()
        {
            lesionRegion.GetComponent<Renderer>().material.color = healthyColor;
            cystRegion.GetComponent<Renderer>().material.color = cystColor;
            healthyRegion.GetComponent<Renderer>().material.color = healthyColor;
            return "Sağlıklı doku modunda yeşil alanlar normal karaciğer dokusunu temsil eder.";
        }

        public string ShowRiskMode()
        {
            lesionRegion.GetComponent<Renderer>().material.color = lesionColor;
            cystRegion.GetComponent<Renderer>().material.color = cystColor;
            healthyRegion.GetComponent<Renderer>().material.color = healthyColor;
            lesionRegion.gameObject.SetActive(true);
            cystRegion.gameObject.SetActive(true);
            lesionMode = true;
            return "Risk modunda kırmızı taralı bölge tümör şüphesini, camgöbeği bölge kistik alanı gösterir.";
        }

        private void BuildScene()
        {
            CreateCamera();
            CreateLights();

            modelRoot = new GameObject("Demo_3D_Liver_Model").transform;
            modelRoot.position = new Vector3(0f, 0.05f, 0f);
            modelRoot.rotation = Quaternion.Euler(-8f, -20f, 8f);

            if (!TryCreateAssetModel())
            {
                CreateLiverMass("RightLobe", new Vector3(0.35f, 0f, 0f), new Vector3(2.4f, 1.35f, 1.15f), liverColor);
                CreateLiverMass("LeftLobe", new Vector3(-1.15f, -0.02f, 0.02f), new Vector3(1.45f, 0.95f, 0.9f), new Color(0.55f, 0.12f, 0.15f, 1f));
                CreateLiverMass("LowerCurve", new Vector3(-0.1f, -0.48f, 0.02f), new Vector3(2.1f, 0.55f, 0.85f), new Color(0.5f, 0.1f, 0.13f, 1f));
            }

            healthyRegion = CreateRegion("HealthyRegion", new Vector3(0.92f, 0.28f, -0.63f), 0.26f, healthyColor);
            lesionRegion = CreateRegion("TumorScanRegion", new Vector3(-0.42f, 0.12f, -0.66f), 0.36f, lesionColor);
            cystRegion = CreateRegion("CystScanRegion", new Vector3(0.32f, -0.38f, -0.68f), 0.28f, cystColor);

            vesselLayer = new GameObject("VesselLayer").transform;
            vesselLayer.SetParent(modelRoot, false);
            CreateVessel(new Vector3(-1.1f, 0.12f, -0.72f), new Vector3(1.9f, 0.25f, -0.74f), 0.035f);
            CreateVessel(new Vector3(-0.15f, 0.1f, -0.78f), new Vector3(0.4f, -0.45f, -0.78f), 0.028f);
            CreateVessel(new Vector3(0.2f, 0.13f, -0.78f), new Vector3(0.95f, 0.45f, -0.78f), 0.028f);
        }

        private bool TryCreateAssetModel()
        {
            GameObject liverPrefab = Resources.Load<GameObject>("DemoLiverModel");
            if (liverPrefab == null)
            {
                liverPrefab = Resources.Load<GameObject>("liver_model");
            }

            if (liverPrefab == null)
            {
                return false;
            }

            GameObject liver = Instantiate(liverPrefab, modelRoot);
            liver.name = "ProjectAsset_LiverModel";
            assetModel = liver.transform;

            Bounds bounds = CalculateBounds(liver);
            Vector3 center = bounds.center;
            float maxSize = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
            float scale = maxSize > 0.001f ? 2.75f / maxSize : 1f;

            assetModel.localPosition = -center * scale;
            assetModel.localRotation = Quaternion.Euler(0f, 180f, 0f);
            assetModel.localScale = Vector3.one * scale;
            ApplyResourceMaterial(liver);
            return true;
        }

        private void HandleModelInput()
        {
            if (Input.touchCount > 0)
            {
                HandleTouchInput();
                return;
            }

            if (Input.GetMouseButtonDown(0) && !IsPointerOverUi())
            {
                isDragging = true;
                lastPointerPosition = Input.mousePosition;
                manualInputTimer = 1.4f;
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector2 current = Input.mousePosition;
                RotateByDrag(current - lastPointerPosition);
                lastPointerPosition = current;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began && IsPointerOverUi(touch.fingerId))
                {
                    isDragging = false;
                    return;
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    RotateByDrag(touch.deltaPosition);
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                }
            }

            if (Input.touchCount == 2)
            {
                Touch first = Input.GetTouch(0);
                Touch second = Input.GetTouch(1);
                Vector2 firstPrevious = first.position - first.deltaPosition;
                Vector2 secondPrevious = second.position - second.deltaPosition;
                float previousDistance = Vector2.Distance(firstPrevious, secondPrevious);
                float currentDistance = Vector2.Distance(first.position, second.position);
                targetZoom = Mathf.Clamp(targetZoom + (currentDistance - previousDistance) * 0.0018f, 0.75f, 1.75f);
                manualInputTimer = 1.4f;
            }
        }

        private static bool IsPointerOverUi(int fingerId = -1)
        {
            if (EventSystem.current == null)
            {
                return false;
            }

            return fingerId >= 0
                ? EventSystem.current.IsPointerOverGameObject(fingerId)
                : EventSystem.current.IsPointerOverGameObject();
        }

        private void CreateCamera()
        {
            if (Camera.main != null)
            {
                Camera.main.gameObject.SetActive(false);
            }

            var cameraObject = new GameObject("DemoShowcaseCamera");
            showcaseCamera = cameraObject.AddComponent<Camera>();
            showcaseCamera.clearFlags = CameraClearFlags.SolidColor;
            showcaseCamera.backgroundColor = new Color(0.05f, 0.07f, 0.1f, 1f);
            showcaseCamera.fieldOfView = 42f;
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0.15f, -6.5f);
            cameraObject.transform.LookAt(Vector3.zero);
        }

        private void CreateLights()
        {
            var key = new GameObject("KeyLight");
            var keyLight = key.AddComponent<Light>();
            keyLight.type = LightType.Directional;
            keyLight.intensity = 1.6f;
            keyLight.color = new Color(1f, 0.98f, 0.95f);
            key.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            var fill = new GameObject("FillLight");
            var fillLight = fill.AddComponent<Light>();
            fillLight.type = LightType.Directional;
            fillLight.intensity = 0.6f;
            fillLight.color = new Color(0.6f, 0.75f, 1f);
            fill.transform.rotation = Quaternion.Euler(15f, 60f, 0f);

            var rim = new GameObject("RimLight");
            var rimLight = rim.AddComponent<Light>();
            rimLight.type = LightType.Directional;
            rimLight.intensity = 1.8f;
            rimLight.color = new Color(0.1f, 0.85f, 0.95f);
            rim.transform.rotation = Quaternion.Euler(30f, 150f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.08f, 0.12f, 0.18f);
        }

        private void CreateLiverMass(string name, Vector3 localPosition, Vector3 localScale, Color color)
        {
            var part = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            part.name = name;
            part.transform.SetParent(modelRoot, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            ApplyMaterial(part, color, 0.18f, 0.55f);
        }

        private static Bounds CalculateBounds(GameObject root)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                return new Bounds(Vector3.zero, Vector3.one);
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        private Transform CreateRegion(string name, Vector3 localPosition, float radius, Color color)
        {
            var region = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            region.name = name;
            region.transform.SetParent(modelRoot, false);
            region.transform.localPosition = localPosition;
            region.transform.localScale = Vector3.one * radius;
            ApplyMaterial(region, color, 0.45f, 1.2f);
            return region.transform;
        }

        private void CreateVessel(Vector3 from, Vector3 to, float width)
        {
            var vessel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            vessel.name = "Vessel";
            vessel.transform.SetParent(vesselLayer, false);

            Vector3 midpoint = (from + to) * 0.5f;
            Vector3 direction = to - from;
            vessel.transform.localPosition = midpoint;
            vessel.transform.localRotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
            vessel.transform.localScale = new Vector3(width, direction.magnitude * 0.5f, width);
            ApplyMaterial(vessel, vesselColor, 0.35f, 0.85f);
        }

        private static void ApplyMaterial(GameObject obj, Color color, float metallic, float smoothness)
        {
            var material = new Material(Shader.Find("Standard"));
            material.color = color;
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Glossiness", smoothness);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 0.05f);
            obj.GetComponent<Renderer>().material = material;
        }

        private static void ApplyResourceMaterial(GameObject root)
        {
            Texture2D albedo = Resources.Load<Texture2D>("liver_albedo");
            Texture2D normal = Resources.Load<Texture2D>("liver_normal");
            Texture2D roughness = Resources.Load<Texture2D>("liver_roughness");
            Material material = new Material(Shader.Find("Standard"));
            material.color = new Color(1.0f, 0.95f, 0.95f); // Slight warm organic tint
            material.mainTexture = albedo;
            material.SetFloat("_Metallic", 0.02f); // Low metallic for organic tissue
            material.SetFloat("_Glossiness", 0.75f); // Higher glossiness for "wet" liver look

            if (normal != null)
            {
                material.SetTexture("_BumpMap", normal);
                material.EnableKeyword("_NORMALMAP");
            }

            if (roughness != null)
            {
                // In Unity Standard, Glossiness is derived from texture alpha. 
                // Using a roughness map directly as metallic gloss map may yield incorrect results 
                // if it's greyscale. So we rely on the _Glossiness scalar mostly.
                material.SetTexture("_MetallicGlossMap", roughness);
            }

            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                renderer.material = material;
            }
        }

        private static void SetRegionScale(Transform region, float scale)
        {
            region.localScale = Vector3.one * (region.name.Contains("Tumor") ? 0.36f : 0.28f) * scale;
        }
    }
}
