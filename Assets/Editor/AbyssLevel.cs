using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AbyssLevel
{
    private const string RootName = "05_Abismo";
    private const string PortalName = "Portal_Abismo";
    private static readonly Vector3 Center = new Vector3(0f, 0f, 340f);
    private static readonly Vector3 Arrival = new Vector3(0f, 1.2f, 293f);

    [MenuItem("Tools/Umbral/8 Nivel 2 Abismo")]
    public static void Build()
    {
        BuilderUtils.Remove(RootName);

        Material rock = BuilderUtils.Mat("Abyss_Rock", new Color(0.1f, 0.2f, 0.24f), 0.2f, 0.45f);
        Material path = BuilderUtils.Mat("Abyss_Path", new Color(0.2f, 0.32f, 0.34f), 0.25f, 0.5f);
        Material water = BuilderUtils.Mat("Abyss_Water", new Color(0.02f, 0.16f, 0.22f), 0.3f, 0.95f);
        Material pool = BuilderUtils.Mat("Abyss_Pool", new Color(0.01f, 0.04f, 0.07f), 0.4f, 0.95f);
        Material coral = BuilderUtils.Mat("Abyss_Coral", new Color(0.75f, 0.3f, 0.4f), 0f, 0.35f);
        Material glowCyan = BuilderUtils.Mat("Abyss_Glow_Cyan", new Color(0.15f, 0.85f, 0.9f), 0f, 0.4f, 2f);
        Material glowPink = BuilderUtils.Mat("Abyss_Glow_Pink", new Color(1f, 0.3f, 0.6f), 0f, 0.4f, 2f);

        Transform root = BuilderUtils.Group(RootName, null).transform;

        BuildIsland(root, rock, path, water, pool, coral, glowCyan, glowPink);

        GameObject bulletPrefab = MakeBullet(glowPink);
        GameObject octopus = MakeOctopus(bulletPrefab);
        octopus.transform.SetParent(root, false);
        octopus.transform.position = Center;
        octopus.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        BuilderUtils.EnsureFolder(BuilderUtils.PrefabFolder);
        PrefabUtility.SaveAsPrefabAssetAndConnect(octopus, BuilderUtils.PrefabFolder + "/Octopus.prefab", InteractionMode.AutomatedAction);

        Transform pools = root.Find("Pools");
        OctopusBoss brain = octopus.GetComponent<OctopusBoss>();
        brain.pools = new Transform[pools.childCount];
        for (int i = 0; i < pools.childCount; i++)
            brain.pools[i] = pools.GetChild(i);
        PrefabUtility.RecordPrefabInstancePropertyModifications(brain);

        GameObject sentinel = AssetDatabase.LoadAssetAtPath<GameObject>(BuilderUtils.PrefabFolder + "/Sentinel.prefab");
        if (sentinel != null)
        {
            BuilderUtils.Spawn(sentinel, "Abyss_Sentinel_1", root, new Vector3(-1.6f, 1.7f, 306f));
            BuilderUtils.Spawn(sentinel, "Abyss_Sentinel_2", root, new Vector3(1.6f, 1.7f, 311f));
        }

        BuildPortal(glowCyan, rock);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            TargetLock targetLock = player.GetComponent<TargetLock>();
            if (targetLock != null)
            {
                targetLock.targetNoticeDistance = 32f;
                EditorUtility.SetDirty(targetLock);
            }
        }

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(root.gameObject.scene);
        Debug.Log("Nivel 2: isla del Abismo, Kraken y portal listos.");
    }

    private static Vector3 Around(float angle, float radius, float height)
    {
        float radians = angle * Mathf.Deg2Rad;
        return Center + new Vector3(Mathf.Sin(radians) * radius, height, Mathf.Cos(radians) * radius);
    }

    private static void BuildIsland(Transform root, Material rock, Material path, Material water, Material pool, Material coral, Material glowCyan, Material glowPink)
    {
        GameObject island = BuilderUtils.Shape(PrimitiveType.Cylinder, "Island", root, Center + Vector3.down * 0.5f, new Vector3(46f, 0.5f, 46f), rock, false);
        island.AddComponent<MeshCollider>();

        BuilderUtils.Block("Bridge", root, new Vector3(0f, -0.5f, 308f), new Vector3(6f, 1f, 20f), path);
        BuilderUtils.Block("Arrival_Pad", root, new Vector3(0f, -0.5f, 293f), new Vector3(12f, 1f, 12f), path);
        BuilderUtils.Block("Arrival_Arch_L", root, new Vector3(-3.4f, 2.5f, 299f), new Vector3(0.8f, 5f, 0.8f), rock);
        BuilderUtils.Block("Arrival_Arch_R", root, new Vector3(3.4f, 2.5f, 299f), new Vector3(0.8f, 5f, 0.8f), rock);
        BuilderUtils.Block("Arrival_Arch_Top", root, new Vector3(0f, 5.3f, 299f), new Vector3(8f, 0.7f, 1f), rock);

        BuilderUtils.Shape(PrimitiveType.Cube, "Sea", root, Center + new Vector3(0f, -2.4f, -15f), new Vector3(260f, 0.4f, 260f), water, false);

        GameObject fall = BuilderUtils.Group("Sea_FallReset", root);
        fall.transform.position = Center + new Vector3(0f, -1.7f, -15f);
        BoxCollider fallBox = fall.AddComponent<BoxCollider>();
        fallBox.isTrigger = true;
        fallBox.size = new Vector3(260f, 1.2f, 260f);
        Rigidbody fallBody = fall.AddComponent<Rigidbody>();
        fallBody.isKinematic = true;
        fallBody.useGravity = false;
        fall.AddComponent<GameplayTrigger>().action = GameplayTrigger.TriggerAction.FallReset;

        Transform pools = BuilderUtils.Group("Pools", root).transform;
        Vector3[] poolSpots =
        {
            Center,
            Center + new Vector3(-12f, 0f, 6f),
            Center + new Vector3(12f, 0f, 6f),
            Center + new Vector3(0f, 0f, 14f)
        };
        for (int i = 0; i < poolSpots.Length; i++)
            BuilderUtils.Shape(PrimitiveType.Cylinder, "Pool_" + i, pools, poolSpots[i] + Vector3.up * 0.01f, new Vector3(7.5f, 0.01f, 7.5f), pool, false);

        Transform cover = BuilderUtils.Group("Cover", root).transform;
        Vector3[] pillars =
        {
            new Vector3(-8f, 0f, -12f), new Vector3(8f, 0f, -12f),
            new Vector3(-17f, 0f, -3f), new Vector3(17f, 0f, -3f),
            new Vector3(-6f, 0f, -18f), new Vector3(6f, 0f, -18f)
        };
        for (int i = 0; i < pillars.Length; i++)
        {
            Vector3 spot = Center + pillars[i];
            BuilderUtils.Block("Pillar_" + i, cover, spot + Vector3.up * 1.75f, new Vector3(2.2f, 3.5f, 2.2f), coral);
            BuilderUtils.Block("Pillar_Cap_" + i, cover, spot + Vector3.up * 3.65f, new Vector3(2.6f, 0.3f, 2.6f), rock);
        }

        Transform rim = BuilderUtils.Group("Rim", root).transform;
        for (int i = 0; i < 18; i++)
        {
            float angle = i * 20f;
            if (angle > 160f && angle < 200f) continue;

            GameObject stone = BuilderUtils.Block("Rim_" + i, rim, Around(angle, 22.3f, 0.6f), new Vector3(7f, 1.2f, 1.4f), rock);
            stone.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        Transform corals = BuilderUtils.Group("Corals", root).transform;
        for (int i = 0; i < 12; i++)
        {
            float angle = 15f + i * 30f;
            if (angle > 160f && angle < 200f) continue;

            Material glow = i % 2 == 0 ? glowCyan : glowPink;
            float height = 2f + (i % 3) * 0.9f;
            BuilderUtils.Shape(PrimitiveType.Sphere, "Coral_" + i, corals, Around(angle, 20.2f, height * 0.5f), new Vector3(0.7f, height, 0.7f), glow, false);
        }

        Transform lights = BuilderUtils.Group("Lights", root).transform;
        for (int i = 0; i < 4; i++)
        {
            Color tint = i % 2 == 0 ? new Color(0.2f, 0.85f, 0.95f) : new Color(1f, 0.35f, 0.65f);
            BuilderUtils.PointLight("Abyss_Light_" + i, lights, Around(45f + i * 90f, 14f, 5f), tint, 14f, 20f, LightmapBakeType.Baked);
        }
        BuilderUtils.PointLight("Abyss_Light_Center", lights, Center + Vector3.up * 10f, new Color(0.5f, 0.6f, 1f), 16f, 26f, LightmapBakeType.Baked);
        BuilderUtils.PointLight("Abyss_Light_Bridge", lights, new Vector3(0f, 4f, 302f), new Color(0.2f, 0.85f, 0.95f), 8f, 14f, LightmapBakeType.Baked);

        Ambience.Dust("Bruma_Abismo", root, Center + Vector3.up * 3f, new Vector3(44f, 6f, 44f), 120, new Color(0.5f, 0.9f, 1f, 0.35f), BuilderUtils.ParticleMaterial());
    }

    private static GameObject MakeBullet(Material glow)
    {
        GameObject bullet = BuilderUtils.Shape(PrimitiveType.Sphere, "EnemyBullet", null, Vector3.zero, Vector3.one * 0.55f, glow, true);
        bullet.GetComponent<SphereCollider>().isTrigger = true;

        Rigidbody body = bullet.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        bullet.AddComponent<EnemyBullet>();
        return BuilderUtils.SavePrefab(bullet, "EnemyBullet");
    }

    private static GameObject MakeOctopus(GameObject bulletPrefab)
    {
        Material skin = BuilderUtils.Mat("Abyss_Octopus", new Color(0.5f, 0.18f, 0.5f), 0.1f, 0.75f);
        Material belly = BuilderUtils.Mat("Abyss_Octopus_Belly", new Color(0.85f, 0.55f, 0.7f), 0f, 0.6f);
        Material eyeWhite = BuilderUtils.Mat("Abyss_Eye", new Color(0.95f, 0.95f, 0.85f), 0f, 0.8f);
        Material eyeDark = BuilderUtils.Mat("Abyss_Pupil", new Color(0.03f, 0.03f, 0.03f), 0f, 0.9f);
        Material leather = BuilderUtils.Mat("Abyss_Hat", new Color(0.3f, 0.18f, 0.08f), 0f, 0.3f);
        Material gunMetal = BuilderUtils.Mat("Abyss_Gun", new Color(0.12f, 0.12f, 0.14f), 0.95f, 0.6f);

        GameObject octopus = new GameObject("Kraken");
        SphereCollider hitbox = octopus.AddComponent<SphereCollider>();
        hitbox.center = new Vector3(0f, 3f, 0f);
        hitbox.radius = 2.7f;

        Transform model = BuilderUtils.Group("Modelo", octopus.transform).transform;

        BuilderUtils.Shape(PrimitiveType.Sphere, "Mantle", model, new Vector3(0f, 4f, -0.4f), new Vector3(4.4f, 5f, 4.8f), skin, false);
        BuilderUtils.Shape(PrimitiveType.Sphere, "Head", model, new Vector3(0f, 2.2f, 0f), new Vector3(5f, 3.2f, 5f), skin, false);
        BuilderUtils.Shape(PrimitiveType.Sphere, "Belly", model, new Vector3(0f, 1.5f, 1.2f), new Vector3(3f, 1.6f, 2.6f), belly, false);

        for (int side = -1; side <= 1; side += 2)
        {
            string tag = side < 0 ? "L" : "R";
            BuilderUtils.Shape(PrimitiveType.Sphere, "Eye_" + tag, model, new Vector3(side * 1.05f, 3.1f, 1.95f), Vector3.one * 1.05f, eyeWhite, false);
            BuilderUtils.Shape(PrimitiveType.Sphere, "Pupil_" + tag, model, new Vector3(side * 1.05f, 3.05f, 2.4f), Vector3.one * 0.45f, eyeDark, false);
            BuilderUtils.Shape(PrimitiveType.Cube, "Brow_" + tag, model, new Vector3(side * 1.05f, 3.75f, 2.2f), new Vector3(1.2f, 0.22f, 0.3f), eyeDark, false)
                .transform.localRotation = Quaternion.Euler(0f, 0f, side * 22f);
        }

        BuilderUtils.Shape(PrimitiveType.Cylinder, "Hat_Brim", model, new Vector3(0f, 6.2f, -0.4f), new Vector3(4.6f, 0.08f, 4.6f), leather, false);
        BuilderUtils.Shape(PrimitiveType.Cylinder, "Hat_Crown", model, new Vector3(0f, 6.9f, -0.4f), new Vector3(2.4f, 0.65f, 2.4f), leather, false);
        BuilderUtils.Shape(PrimitiveType.Cylinder, "Hat_Band", model, new Vector3(0f, 6.45f, -0.4f), new Vector3(2.5f, 0.12f, 2.5f), gunMetal, false);

        List<Transform> tentacles = new List<Transform>();
        List<Transform> muzzles = new List<Transform>();

        for (int i = 0; i < 8; i++)
        {
            float angle = 20f + i * 45f;
            if (i >= 4) angle += 5f;
            bool armed = i == 0 || i == 1 || i == 6 || i == 7;

            Transform joint = BuilderUtils.Group("Tentacle_" + i, model).transform;
            joint.localPosition = new Vector3(0f, armed ? 1.6f : 0.9f, 0f);
            joint.localRotation = Quaternion.Euler(armed ? -18f : 6f, angle, 0f);

            for (int s = 0; s < 4; s++)
            {
                float thickness = Mathf.Lerp(1.3f, 0.6f, s / 3f);
                BuilderUtils.Shape(PrimitiveType.Sphere, "Segment_" + s, joint, new Vector3(0f, 0f, 2.2f + s * 1.05f), new Vector3(thickness, thickness, 1.5f), skin, false);
            }

            if (armed)
            {
                BuilderUtils.Shape(PrimitiveType.Cube, "Gun_Barrel", joint, new Vector3(0f, 0.25f, 6.4f), new Vector3(0.32f, 0.32f, 1.5f), gunMetal, false);
                BuilderUtils.Shape(PrimitiveType.Cylinder, "Gun_Drum", joint, new Vector3(0f, 0.25f, 5.75f), new Vector3(0.6f, 0.25f, 0.6f), gunMetal, false)
                    .transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                BuilderUtils.Shape(PrimitiveType.Cube, "Gun_Grip", joint, new Vector3(0f, -0.15f, 5.55f), new Vector3(0.26f, 0.7f, 0.3f), leather, false);

                Transform muzzle = BuilderUtils.Group("Muzzle", joint).transform;
                muzzle.localPosition = new Vector3(0f, 0.25f, 7.3f);
                muzzles.Add(muzzle);
            }

            tentacles.Add(joint);
        }

        BossStats stats = octopus.AddComponent<BossStats>();
        stats.bossName = "Kraken Pistolero";
        stats.maxHP = 650f;
        stats.isFinalBoss = true;

        OctopusBoss brain = octopus.AddComponent<OctopusBoss>();
        brain.bulletPrefab = bulletPrefab;
        brain.tentacles = tentacles.ToArray();
        brain.muzzles = muzzles.ToArray();

        return octopus;
    }

    private static void BuildPortal(Material glow, Material rock)
    {
        GameObject old = null;
        foreach (Portal existing in Object.FindObjectsByType<Portal>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (existing.name == PortalName) old = existing.gameObject;
        if (old != null) Object.DestroyImmediate(old);

        GameObject portal = new GameObject(PortalName);
        portal.transform.position = new Vector3(-8f, 6.7f, 90f);

        BuilderUtils.Block("Pillar_L", portal.transform, new Vector3(-1.7f, 2.1f, 0f), new Vector3(0.6f, 4.2f, 0.6f), rock);
        BuilderUtils.Block("Pillar_R", portal.transform, new Vector3(1.7f, 2.1f, 0f), new Vector3(0.6f, 4.2f, 0.6f), rock);
        BuilderUtils.Block("Lintel", portal.transform, new Vector3(0f, 4.4f, 0f), new Vector3(4.2f, 0.6f, 0.7f), rock);
        BuilderUtils.Shape(PrimitiveType.Cube, "Veil", portal.transform, new Vector3(0f, 2.1f, 0f), new Vector3(2.8f, 4f, 0.08f), glow, false);

        Light light = BuilderUtils.PointLight("Glow", portal.transform, Vector3.zero, new Color(0.2f, 0.9f, 0.95f), 6f, 9f, LightmapBakeType.Realtime);
        light.transform.localPosition = new Vector3(0f, 2.2f, 0f);

        BoxCollider trigger = portal.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.center = new Vector3(0f, 2.1f, 0f);
        trigger.size = new Vector3(2.8f, 4f, 1.6f);

        Rigidbody body = portal.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        portal.AddComponent<Portal>().destination = Arrival;

        ChestBase chest = Object.FindFirstObjectByType<ChestBase>();
        if (chest != null)
        {
            chest.portal = portal;
            EditorUtility.SetDirty(chest);
            PrefabUtility.RecordPrefabInstancePropertyModifications(chest);
        }

        portal.SetActive(false);
    }
}
