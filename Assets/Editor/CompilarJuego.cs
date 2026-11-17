using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CompilarJuego
{
    private const string ScenePath = "Assets/Escenas/Escena.unity";

    [MenuItem("Tools/Umbral/0 Armar todo")]
    public static void BuildEverything()
    {
        ExtrasMapa.Build();
        EsqueletoJugador.Build();
        JefeCaballero.Build();
        NivelAbismo.Build();
        CreadorTerreno.Build();
        Ambientacion.Build();
        HornearLuz.Bake();
    }

    [MenuItem("Tools/Umbral/6 Compilar para Windows")]
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
