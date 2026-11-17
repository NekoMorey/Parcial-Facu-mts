using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public static class LightBake
{
    private const string SettingsPath = "Assets/Settings/UmbralLighting.lighting";

    [MenuItem("Tools/Umbral/5 Iluminacion y bake")]
    public static void Bake()
    {
        SetupLights();
        MarkStatics();
        PlaceProbes();
        PlaceReflections();
        ApplySettings();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Lightmapping.bakeCompleted -= SaveAfterBake;
        Lightmapping.bakeCompleted += SaveAfterBake;
        Lightmapping.BakeAsync();
        Debug.Log("Iluminacion: luces configuradas, bake en curso.");
    }

    private static void SaveAfterBake()
    {
        Lightmapping.bakeCompleted -= SaveAfterBake;
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Iluminacion: bake terminado y escena guardada.");
    }

    private static void SetupLights()
    {
        foreach (Light light in Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (light.GetComponentInParent<KeyPickup>(true) != null) continue;
            if (light.GetComponentInParent<Portal>(true) != null) continue;

            if (light.type == LightType.Directional)
            {
                light.lightmapBakeType = LightmapBakeType.Mixed;
                light.shadows = LightShadows.Soft;
            }
            else
            {
                light.lightmapBakeType = LightmapBakeType.Baked;
                light.shadows = LightShadows.Soft;
            }

            EditorUtility.SetDirty(light);
        }
    }

    private static bool CanBeStatic(Transform target)
    {
        if (target.name.Contains("Gate") || target.name == "Sea") return false;

        for (Transform current = target; current != null; current = current.parent)
        {
            if (current.CompareTag("Player") || current.CompareTag("Boss") || current.CompareTag("Destructible")) return false;
            if (current.GetComponent<Rigidbody>() != null) return false;
            if (current.GetComponent<Animator>() != null) return false;
            if (current.GetComponent<NavMeshAgent>() != null) return false;
            if (current.GetComponent<Terrain>() != null) return false;
            if (current.GetComponent<ChestBase>() != null) return false;
            if (current.GetComponent<BossStats>() != null) return false;
            if (current.GetComponent<UpgradeChest>() != null) return false;
            if (current.GetComponent<LockedDoor>() != null) return false;
            if (current.GetComponent<ShootableSwitch>() != null) return false;
            if (current.name == "Cage_Gate") return false;
        }

        return true;
    }

    private static void MarkStatics()
    {
        StaticEditorFlags flags = StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic |
                                  StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic |
                                  StaticEditorFlags.ReflectionProbeStatic;

        foreach (MeshRenderer renderer in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            if (!CanBeStatic(renderer.transform)) continue;
            GameObjectUtility.SetStaticEditorFlags(renderer.gameObject, flags);
        }
    }

    private static void Grid(List<Vector3> points, Vector3 min, Vector3 max, float step, params float[] heights)
    {
        for (float x = min.x; x <= max.x + 0.01f; x += step)
            for (float z = min.z; z <= max.z + 0.01f; z += step)
                foreach (float y in heights)
                    points.Add(new Vector3(x, y, z));
    }

    private static void PlaceProbes()
    {
        BuilderUtils.Remove("Light Probes");
        GameObject go = new GameObject("Light Probes");
        LightProbeGroup group = go.AddComponent<LightProbeGroup>();

        List<Vector3> points = new List<Vector3>();
        Grid(points, new Vector3(-9f, 0f, -29f), new Vector3(9f, 0f, -11f), 6f, 1f, 4f);
        Grid(points, new Vector3(-2f, 0f, -8f), new Vector3(2f, 0f, 4f), 4f, 1f, 3.5f);
        Grid(points, new Vector3(13f, 0f, -26f), new Vector3(25f, 0f, -14f), 6f, 1f, 4f);
        Grid(points, new Vector3(-10f, 0f, 6f), new Vector3(10f, 0f, 46f), 10f, -4f, 2f, 8f, 12f);
        Grid(points, new Vector3(-2f, 0f, 47f), new Vector3(2f, 0f, 59f), 4f, 8f, 11f);
        Grid(points, new Vector3(-15f, 0f, 63f), new Vector3(15f, 0f, 93f), 7.5f, 8f, 12f);

        Grid(points, new Vector3(-20f, 0f, 292f), new Vector3(20f, 0f, 362f), 10f, 1.5f, 6f);

        group.probePositions = points.ToArray();
    }

    private static void Reflection(Transform parent, string name, Vector3 center, Vector3 size)
    {
        GameObject go = BuilderUtils.Group(name, parent);
        go.transform.position = center;

        ReflectionProbe probe = go.AddComponent<ReflectionProbe>();
        probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Baked;
        probe.size = size;
        probe.boxProjection = true;
        probe.resolution = 128;
    }

    private static void PlaceReflections()
    {
        BuilderUtils.Remove("Reflection Probes");
        Transform root = new GameObject("Reflection Probes").transform;

        Reflection(root, "Probe_Lobby", new Vector3(0f, 3f, -20f), new Vector3(22f, 6f, 22f));
        Reflection(root, "Probe_Cripta", new Vector3(19f, 3f, -20f), new Vector3(15f, 6f, 15f));
        Reflection(root, "Probe_Parkour", new Vector3(0f, 4f, 24f), new Vector3(26f, 20f, 46f));
        Reflection(root, "Probe_Abismo", new Vector3(0f, 5f, 335f), new Vector3(60f, 14f, 100f));
        Reflection(root, "Probe_Arena", new Vector3(0f, 11f, 78f), new Vector3(36f, 10f, 36f));
    }

    private static void ApplySettings()
    {
        BuilderUtils.EnsureFolder("Assets/Settings");

        LightingSettings settings = AssetDatabase.LoadAssetAtPath<LightingSettings>(SettingsPath);
        if (settings == null)
        {
            settings = new LightingSettings();
            AssetDatabase.CreateAsset(settings, SettingsPath);
        }

        settings.bakedGI = true;
        settings.realtimeGI = false;
        settings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
        settings.mixedBakeMode = MixedLightingMode.IndirectOnly;
        settings.lightmapResolution = 12f;
        settings.lightmapMaxSize = 2048;
        settings.lightmapPadding = 2;
        settings.directSampleCount = 32;
        settings.indirectSampleCount = 256;
        settings.environmentSampleCount = 128;
        settings.ao = true;
        settings.aoMaxDistance = 1.5f;

        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        Lightmapping.lightingSettings = settings;
    }
}
