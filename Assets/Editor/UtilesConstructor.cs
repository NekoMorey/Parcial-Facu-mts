using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class UtilesConstructor
{
    public const string MaterialFolder = "Assets/Materiales";
    public const string PrefabFolder = "Assets/Prefabs";

    public static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        string name = Path.GetFileName(path);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }

    public static Material Mat(string name)
    {
        return AssetDatabase.LoadAssetAtPath<Material>(MaterialFolder + "/" + name + ".mat");
    }

    public static Material Mat(string name, Color color, float metallic, float smoothness, float emission = 0f)
    {
        EnsureFolder(MaterialFolder);
        string path = MaterialFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }

        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
        if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);

        if (emission > 0f && material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
            material.SetColor("_EmissionColor", color * emission);
        }

        EditorUtility.SetDirty(material);
        return material;
    }

    public static Material ParticleMaterial()
    {
        RenderPipelineAsset pipeline = GraphicsSettings.currentRenderPipeline;
        return pipeline != null ? pipeline.defaultParticleMaterial : null;
    }

    public static GameObject Group(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        return go;
    }

    public static GameObject Block(string name, Transform parent, Vector3 position, Vector3 size, Material material)
    {
        return Shape(PrimitiveType.Cube, name, parent, position, size, material, true);
    }

    public static GameObject Shape(PrimitiveType type, string name, Transform parent, Vector3 position, Vector3 size, Material material, bool keepCollider)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        if (parent != null) go.transform.SetParent(parent, false);
        go.transform.localPosition = position;
        go.transform.localScale = size;

        if (material != null) go.GetComponent<MeshRenderer>().sharedMaterial = material;

        if (!keepCollider)
        {
            Collider collider = go.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);
        }

        return go;
    }

    public static void Remove(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go != null) Object.DestroyImmediate(go);
    }

    public static void Unpack(GameObject go)
    {
        if (!PrefabUtility.IsPartOfPrefabInstance(go)) return;

        GameObject instanceRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
        PrefabUtility.UnpackPrefabInstance(instanceRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
    }

    public static GameObject SavePrefab(GameObject source, string name)
    {
        EnsureFolder(PrefabFolder);
        string path = PrefabFolder + "/" + name + ".prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(source, path);
        Object.DestroyImmediate(source);
        return prefab;
    }

    public static GameObject Spawn(GameObject prefab, string name, Transform parent, Vector3 position)
    {
        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        return go;
    }

    public static Light PointLight(string name, Transform parent, Vector3 position, Color color, float intensity, float range, LightmapBakeType mode)
    {
        GameObject go = Group(name, parent);
        go.transform.position = position;
        Light light = go.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.shadows = mode == LightmapBakeType.Baked ? LightShadows.Soft : LightShadows.None;
        light.lightmapBakeType = mode;
        return light;
    }
}
