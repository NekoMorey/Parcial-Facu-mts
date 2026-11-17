using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Ambience
{
    private const string RootName = "Ambiente";

    [MenuItem("Tools/Umbral/4 Particulas y prefabs")]
    public static void Build()
    {
        BuilderUtils.Remove(RootName);
        Transform root = BuilderUtils.Group(RootName, null).transform;
        Material material = BuilderUtils.ParticleMaterial();

        foreach (Light light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            if (light.type != LightType.Point) continue;
            if (light.GetComponent<MeshRenderer>() == null) continue;
            Flame("Llama_" + light.name, root, light.transform.position + Vector3.up * 0.12f, material);
        }

        Dust("Polvo_Arena", root, new Vector3(0f, 10.5f, 78f), new Vector3(34f, 7f, 34f), 90, new Color(1f, 0.6f, 0.4f, 0.35f), material);
        Dust("Polvo_Lobby", root, new Vector3(0f, 3f, -20f), new Vector3(20f, 5f, 20f), 40, new Color(0.8f, 0.85f, 1f, 0.3f), material);
        Dust("Polvo_Cripta", root, new Vector3(19f, 3f, -20f), new Vector3(14f, 5f, 14f), 35, new Color(0.7f, 0.5f, 1f, 0.35f), material);

        GameObject boss = GameObject.FindWithTag("Boss");
        if (boss != null)
        {
            BuilderUtils.Unpack(boss);
            Transform oldAura = boss.transform.Find("Aura");
            if (oldAura != null) Object.DestroyImmediate(oldAura.gameObject);
            Aura(boss.transform, material);

            BuilderUtils.EnsureFolder(BuilderUtils.PrefabFolder);
            PrefabUtility.SaveAsPrefabAssetAndConnect(boss, BuilderUtils.PrefabFolder + "/Boss.prefab", InteractionMode.AutomatedAction);
        }

        GameObject chest = GameObject.Find("Cofre");
        if (chest != null && !PrefabUtility.IsPartOfPrefabInstance(chest))
        {
            BuilderUtils.EnsureFolder(BuilderUtils.PrefabFolder);
            PrefabUtility.SaveAsPrefabAssetAndConnect(chest, BuilderUtils.PrefabFolder + "/FinalChest.prefab", InteractionMode.AutomatedAction);
        }

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(root.gameObject.scene);
        Debug.Log("Ambiente: particulas colocadas y prefabs guardados.");
    }

    private static ParticleSystem NewSystem(string name, Transform parent, Vector3 position, Material material)
    {
        GameObject go = BuilderUtils.Group(name, parent);
        go.transform.position = position;

        ParticleSystem particles = go.AddComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
        if (material != null) renderer.sharedMaterial = material;
        return particles;
    }

    private static Gradient Fade(Color start, Color end)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(start, 0f), new GradientColorKey(end, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(start.a, 0.2f), new GradientAlphaKey(0f, 1f) });
        return gradient;
    }

    private static void Flame(string name, Transform parent, Vector3 position, Material material)
    {
        ParticleSystem particles = NewSystem(name, parent, position, material);
        particles.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        ParticleSystem.MainModule main = particles.main;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 1f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.1f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.18f, 0.38f);
        main.startColor = new Color(1f, 0.6f, 0.2f, 0.9f);
        main.maxParticles = 50;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 22f;

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 9f;
        shape.radius = 0.07f;

        ParticleSystem.ColorOverLifetimeModule color = particles.colorOverLifetime;
        color.enabled = true;
        color.color = Fade(new Color(1f, 0.75f, 0.3f, 0.9f), new Color(0.9f, 0.15f, 0.05f, 0f));

        ParticleSystem.SizeOverLifetimeModule size = particles.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.15f));
    }

    public static void Dust(string name, Transform parent, Vector3 position, Vector3 box, int amount, Color tint, Material material)
    {
        ParticleSystem particles = NewSystem(name, parent, position, material);

        ParticleSystem.MainModule main = particles.main;
        main.loop = true;
        main.prewarm = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(6f, 11f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.05f, 0.3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.14f);
        main.startColor = tint;
        main.maxParticles = amount * 2;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = amount / 8f;

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = box;
        shape.randomDirectionAmount = 1f;

        ParticleSystem.ColorOverLifetimeModule color = particles.colorOverLifetime;
        color.enabled = true;
        color.color = Fade(tint, tint);
    }

    private static void Aura(Transform boss, Material material)
    {
        ParticleSystem particles = NewSystem("Aura", boss, boss.position, material);
        particles.transform.localPosition = new Vector3(0f, -0.8f, 0f);
        particles.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

        ParticleSystem.MainModule main = particles.main;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.6f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.6f, 1.6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.3f);
        main.startColor = new Color(1f, 0.3f, 0.1f, 0.8f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 80;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 30f;

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.6f;

        ParticleSystem.ColorOverLifetimeModule color = particles.colorOverLifetime;
        color.enabled = true;
        color.color = Fade(new Color(1f, 0.45f, 0.15f, 0.8f), new Color(0.5f, 0.05f, 0.05f, 0f));
    }
}
