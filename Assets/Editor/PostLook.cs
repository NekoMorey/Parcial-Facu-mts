using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class PostLook
{
    [MenuItem("Tools/Umbral/9 Post-procesado y pausa")]
    public static void Build()
    {
        BuilderUtils.EnsureFolder("Assets/Settings");

        var ruinsProfile = MakeProfile("Assets/Settings/PerfilRuinas.asset", new Color(1f, 0.93f, 0.85f), 0.9f, 0.3f, 0.7f, 12f);
        var abyssProfile = MakeProfile("Assets/Settings/PerfilAbismo.asset", new Color(0.75f, 0.9f, 1f), 1.3f, 0.35f, 0.6f, -8f);

        var oldRoot = GameObject.Find("PostProcesado");
        if (oldRoot != null) Object.DestroyImmediate(oldRoot);
        var root = new GameObject("PostProcesado");

        var ruinsVolume = new GameObject("Volumen_Ruinas").AddComponent<Volume>();
        ruinsVolume.transform.SetParent(root.transform, false);
        ruinsVolume.isGlobal = true;
        ruinsVolume.priority = 0;
        ruinsVolume.sharedProfile = ruinsProfile;

        var abyssVolume = new GameObject("Volumen_Abismo").AddComponent<Volume>();
        abyssVolume.transform.SetParent(root.transform, false);
        abyssVolume.transform.position = new Vector3(0f, 0f, 340f);
        abyssVolume.isGlobal = false;
        abyssVolume.priority = 5;
        abyssVolume.blendDistance = 25f;
        abyssVolume.sharedProfile = abyssProfile;
        var box = abyssVolume.gameObject.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(170f, 80f, 150f);

        foreach (var cam in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            var data = cam.GetUniversalAdditionalCameraData();
            data.renderPostProcessing = true;
        }

        var pause = Object.FindFirstObjectByType<PauseMenu>();
        if (pause == null)
        {
            var holder = new GameObject("MenuPausa");
            holder.AddComponent<PauseMenu>();
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.22f, 0.25f, 0.34f);

        var stats = Object.FindFirstObjectByType<PlayerStats>();
        if (stats != null && stats.gameOverPanel != null) stats.gameOverPanel.SetActive(false);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Post-procesado y pausa listos.");
    }

    static VolumeProfile MakeProfile(string path, Color tint, float bloomPower, float vignettePower, float exposure, float temperature)
    {
        var existing = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
        if (existing != null) AssetDatabase.DeleteAsset(path);

        var profile = ScriptableObject.CreateInstance<VolumeProfile>();
        AssetDatabase.CreateAsset(profile, path);

        var bloom = profile.Add<Bloom>(true);
        bloom.intensity.Override(bloomPower);
        bloom.threshold.Override(0.9f);
        bloom.scatter.Override(0.7f);

        var vignette = profile.Add<Vignette>(true);
        vignette.intensity.Override(vignettePower);
        vignette.smoothness.Override(0.4f);

        var color = profile.Add<ColorAdjustments>(true);
        color.colorFilter.Override(tint);
        color.postExposure.Override(exposure);
        color.saturation.Override(10f);
        color.contrast.Override(12f);

        var white = profile.Add<WhiteBalance>(true);
        white.temperature.Override(temperature);

        var tone = profile.Add<Tonemapping>(true);
        tone.mode.Override(TonemappingMode.ACES);

        foreach (var component in profile.components) AssetDatabase.AddObjectToAsset(component, profile);
        EditorUtility.SetDirty(profile);
        AssetDatabase.SaveAssets();
        return profile;
    }
}
