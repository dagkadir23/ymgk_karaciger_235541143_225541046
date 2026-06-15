using UnityEngine;

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

            modelRoot.Rotate(Vector3.up, 12f * Time.deltaTime, Space.World);
            modelRoot.Rotate(Vector3.forward, Mathf.Sin(Time.time * 0.8f) * 2f * Time.deltaTime, Space.Self);

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

            CreateLiverMass("RightLobe", new Vector3(0.35f, 0f, 0f), new Vector3(2.4f, 1.35f, 1.15f), liverColor);
            CreateLiverMass("LeftLobe", new Vector3(-1.15f, -0.02f, 0.02f), new Vector3(1.45f, 0.95f, 0.9f), new Color(0.55f, 0.12f, 0.15f, 1f));
            CreateLiverMass("LowerCurve", new Vector3(-0.1f, -0.48f, 0.02f), new Vector3(2.1f, 0.55f, 0.85f), new Color(0.5f, 0.1f, 0.13f, 1f));

            healthyRegion = CreateRegion("HealthyRegion", new Vector3(0.92f, 0.28f, -0.63f), 0.26f, healthyColor);
            lesionRegion = CreateRegion("TumorScanRegion", new Vector3(-0.42f, 0.12f, -0.66f), 0.36f, lesionColor);
            cystRegion = CreateRegion("CystScanRegion", new Vector3(0.32f, -0.38f, -0.68f), 0.28f, cystColor);

            vesselLayer = new GameObject("VesselLayer").transform;
            vesselLayer.SetParent(modelRoot, false);
            CreateVessel(new Vector3(-1.1f, 0.12f, -0.72f), new Vector3(1.9f, 0.25f, -0.74f), 0.035f);
            CreateVessel(new Vector3(-0.15f, 0.1f, -0.78f), new Vector3(0.4f, -0.45f, -0.78f), 0.028f);
            CreateVessel(new Vector3(0.2f, 0.13f, -0.78f), new Vector3(0.95f, 0.45f, -0.78f), 0.028f);
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
            showcaseCamera.backgroundColor = new Color(0.025f, 0.035f, 0.05f, 1f);
            showcaseCamera.fieldOfView = 38f;
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0.15f, -7f);
            cameraObject.transform.LookAt(Vector3.zero);
        }

        private void CreateLights()
        {
            var key = new GameObject("DemoKeyLight");
            var keyLight = key.AddComponent<Light>();
            keyLight.type = LightType.Directional;
            keyLight.intensity = 1.35f;
            key.transform.rotation = Quaternion.Euler(38f, -35f, 0f);

            var rim = new GameObject("DemoRimLight");
            var rimLight = rim.AddComponent<Light>();
            rimLight.type = LightType.Point;
            rimLight.color = new Color(0.25f, 0.68f, 1f, 1f);
            rimLight.intensity = 2.1f;
            rimLight.range = 8f;
            rim.transform.position = new Vector3(2.8f, 1.8f, -2.8f);
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

        private static void SetRegionScale(Transform region, float scale)
        {
            region.localScale = Vector3.one * (region.name.Contains("Tumor") ? 0.36f : 0.28f) * scale;
        }
    }
}
