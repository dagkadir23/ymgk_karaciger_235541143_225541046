#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PatientLive.Editor
{
    public static class DemoLiverAssetBuilder
    {
        private const string ModelPath = "Assets/Models/Liver/liver_model.fbx";
        private const string AlbedoPath = "Assets/unity_liver_asset_package/liver_albedo.png";
        private const string NormalPath = "Assets/unity_liver_asset_package/liver_normal.png";
        private const string RoughnessPath = "Assets/unity_liver_asset_package/liver_roughness.png";
        private const string MaterialPath = "Assets/Resources/DemoLiverMaterial.mat";
        private const string PrefabPath = "Assets/Resources/DemoLiverModel.prefab";

        public static void EnsureDemoLiverResource()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material == null)
            {
                material = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, MaterialPath);
            }

            material.color = Color.white;
            material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AlbedoPath);

            Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>(NormalPath);
            if (normal != null)
            {
                material.SetTexture("_BumpMap", normal);
                material.EnableKeyword("_NORMALMAP");
            }

            Texture2D roughness = AssetDatabase.LoadAssetAtPath<Texture2D>(RoughnessPath);
            if (roughness != null)
            {
                material.SetTexture("_MetallicGlossMap", roughness);
            }

            material.SetFloat("_Metallic", 0.08f);
            material.SetFloat("_Glossiness", 0.42f);
            EditorUtility.SetDirty(material);

            GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            if (source == null)
            {
                Debug.LogWarning($"Demo liver source model not found: {Path.GetFullPath(ModelPath)}");
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
            instance.name = "DemoLiverModel";

            foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true))
            {
                renderer.sharedMaterial = material;
            }

            PrefabUtility.SaveAsPrefabAsset(instance, PrefabPath);
            Object.DestroyImmediate(instance);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
