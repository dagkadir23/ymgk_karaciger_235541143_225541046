#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace PatientLive.Editor
{
    public static class AndroidApkBuilder
    {
        private const string ScenePath = "Assets/Scenes/PatientLive_MVP.unity";
        private const string OutputPath = "Builds/PatientLive_Demo.apk";

        public static void BuildApk()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));

            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            PlayerSettings.applicationIdentifier = "com.patientlive.demo";
            PlayerSettings.productName = "PatientLive Demo";

            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = OutputPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result != BuildResult.Succeeded)
            {
                throw new System.Exception($"Android APK build failed: {summary.result}");
            }

            Debug.Log($"Android APK build succeeded: {Path.GetFullPath(OutputPath)} ({summary.totalSize} bytes)");
        }
    }
}
#endif
