using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GameBuild
{
    private const string ScenePath = "Assets/Scenes/scene.unity";

    [MenuItem("Tools/Umbral/0 Armar todo")]
    public static void BuildEverything()
    {
        MapExtras.Build();
        PlayerRig.Build();
        BossKnight.Build();
        AbyssLevel.Build();
        TerrainMaker.Build();
        Ambience.Build();
        LightBake.Bake();
    }

    [MenuItem("Tools/Umbral/6 Build Windows")]
    public static void BuildWindows()
    {
        EditorSceneManager.SaveOpenScenes();

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = new[] { ScenePath };
        options.locationPathName = "Builds/Windows/EcosDelUmbral.exe";
        options.target = BuildTarget.StandaloneWindows64;
        options.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(options);
        Debug.Log("Build Windows: " + report.summary.result + " (" + report.summary.totalSize / (1024 * 1024) + " MB)");
    }
}
