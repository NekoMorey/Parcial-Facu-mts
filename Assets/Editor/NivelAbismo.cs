using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class NivelAbismo
{
    private const string RootName = "05_Abismo";
    private const string PortalName = "Portal_Abismo";
    private static readonly Vector3 Center = new Vector3(0f, 0f, 340f);
    private static readonly Vector3 Arrival = new Vector3(0f, 1.2f, 293f);

    [MenuItem("Tools/Umbral/8 Nivel 2 Abismo")]
    public static void Build()
    {
        UtilesConstructor.Remove(RootName);

        Material rock = UtilesConstructor.Mat("Abismo_Roca", new Color(0.1f, 0.2f, 0.24f), 0.2f, 0.45f);
        Material path = UtilesConstructor.Mat("Abismo_Camino", new Color(0.2f, 0.32f, 0.34f), 0.25f, 0.5f);
        Material water = UtilesConstructor.Mat("Abismo_Agua", new Color(0.02f, 0.16f, 0.22f), 0.3f, 0.95f);
        Material pool = UtilesConstructor.Mat("Abismo_Estanque", new Color(0.01f, 0.04f, 0.07f), 0.4f, 0.95f);
        Material coral = UtilesConstructor.Mat("Abismo_Coral", new Color(0.75f, 0.3f, 0.4f), 0f, 0.35f);
        Material glowCyan = UtilesConstructor.Mat("Abismo_Resplandor_Cian", new Color(0.15f, 0.85f, 0.9f), 0f, 0.4f, 2f);
        Material glowPink = UtilesConstructor.Mat("Abismo_Resplandor_Rosa", new Color(1f, 0.3f, 0.6f), 0f, 0.4f, 2f);

        Transform root = UtilesConstructor.Group(RootName, null).transform;

        BuildIsland(root, rock, path, water, pool, coral, glowCyan, glowPink);

        GameObject bulletPrefab = MakeBullet(glowPink);
        GameObject octopus = MakeOctopus(bulletPrefab);
        octopus.transform.SetParent(root, false);
        octopus.transform.position = Center;
        octopus.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        UtilesConstructor.EnsureFolder(UtilesConstructor.PrefabFolder);
        PrefabUtility.SaveAsPrefabAssetAndConnect(octopus, UtilesConstructor.PrefabFolder + "/Pulpo.prefab", InteractionMode.AutomatedAction);

        Transform pools = root.Find("Estanques");
        JefePulpo brain = octopus.GetComponent<JefePulpo>();
        brain.pools = new Transform[pools.childCount];
        for (int i = 0; i < pools.childCount; i++)
            brain.pools[i] = pools.GetChild(i);
        PrefabUtility.RecordPrefabInstancePropertyModifications(brain);

        GameObject sentinel = AssetDatabase.LoadAssetAtPath<GameObject>(UtilesConstructor.PrefabFolder + "/Centinela.prefab");
        if (sentinel != null)
        {
            UtilesConstructor.Spawn(sentinel, "Abismo_Centinela_1", root, new Vector3(-1.6f, 1.7f, 306f));
            UtilesConstructor.Spawn(sentinel, "Abismo_Centinela_2", root, new Vector3(1.6f, 1.7f, 311f));
        }

        BuildPortal(glowCyan, rock);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            FijarObjetivo targetLock = player.GetComponent<FijarObjetivo>();
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
        GameObject island = UtilesConstructor.Shape(PrimitiveType.Cylinder, "Isla", root, Center + Vector3.down * 0.5f, new Vector3(46f, 0.5f, 46f), rock, false);
        island.AddComponent<MeshCollider>();

        UtilesConstructor.Block("Puente", root, new Vector3(0f, -0.5f, 308f), new Vector3(6f, 1f, 20f), path);
        UtilesConstructor.Block("Llegada_Plataforma", root, new Vector3(0f, -0.5f, 293f), new Vector3(12f, 1f, 12f), path);
        UtilesConstructor.Block("Llegada_Arco_Izq", root, new Vector3(-3.4f, 2.5f, 299f), new Vector3(0.8f, 5f, 0.8f), rock);
        UtilesConstructor.Block("Llegada_Arco_Der", root, new Vector3(3.4f, 2.5f, 299f), new Vector3(0.8f, 5f, 0.8f), rock);
        UtilesConstructor.Block("Llegada_Arco_Alto", root, new Vector3(0f, 5.3f, 299f), new Vector3(8f, 0.7f, 1f), rock);

        UtilesConstructor.Shape(PrimitiveType.Cube, "Mar", root, Center + new Vector3(0f, -2.4f, -15f), new Vector3(260f, 0.4f, 260f), water, false);

        GameObject fall = UtilesConstructor.Group("Mar_ReinicioCaida", root);
        fall.transform.position = Center + new Vector3(0f, -1.7f, -15f);
        BoxCollider fallBox = fall.AddComponent<BoxCollider>();
        fallBox.isTrigger = true;
        fallBox.size = new Vector3(260f, 1.2f, 260f);
        Rigidbody fallBody = fall.AddComponent<Rigidbody>();
        fallBody.isKinematic = true;
        fallBody.useGravity = false;
        fall.AddComponent<ZonaJuego>().action = ZonaJuego.AccionZona.Caida;

        Transform pools = UtilesConstructor.Group("Estanques", root).transform;
        Vector3[] poolSpots =
        {
            Center,
            Center + new Vector3(-12f, 0f, 6f),
            Center + new Vector3(12f, 0f, 6f),
            Center + new Vector3(0f, 0f, 14f)
        };
        for (int i = 0; i < poolSpots.Length; i++)
            UtilesConstructor.Shape(PrimitiveType.Cylinder, "Estanque_" + i, pools, poolSpots[i] + Vector3.up * 0.01f, new Vector3(7.5f, 0.01f, 7.5f), pool, false);

        Transform cover = UtilesConstructor.Group("Cubierta", root).transform;
        Vector3[] pillars =
        {
            new Vector3(-8f, 0f, -12f), new Vector3(8f, 0f, -12f),
            new Vector3(-17f, 0f, -3f), new Vector3(17f, 0f, -3f),
            new Vector3(-6f, 0f, -18f), new Vector3(6f, 0f, -18f)
        };
        for (int i = 0; i < pillars.Length; i++)
        {
            Vector3 spot = Center + pillars[i];
            UtilesConstructor.Block("Pilar_" + i, cover, spot + Vector3.up * 1.75f, new Vector3(2.2f, 3.5f, 2.2f), coral);
            UtilesConstructor.Block("Pilar_Capitel_" + i, cover, spot + Vector3.up * 3.65f, new Vector3(2.6f, 0.3f, 2.6f), rock);
        }

        Transform rim = UtilesConstructor.Group("Borde", root).transform;
        for (int i = 0; i < 18; i++)
        {
            float angle = i * 20f;
            if (angle > 160f && angle < 200f) continue;

            GameObject stone = UtilesConstructor.Block("Borde_" + i, rim, Around(angle, 22.3f, 0.6f), new Vector3(7f, 1.2f, 1.4f), rock);
            stone.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        Transform corals = UtilesConstructor.Group("Corales", root).transform;
        for (int i = 0; i < 12; i++)
        {
            float angle = 15f + i * 30f;
            if (angle > 160f && angle < 200f) continue;

            Material glow = i % 2 == 0 ? glowCyan : glowPink;
            float height = 2f + (i % 3) * 0.9f;
            UtilesConstructor.Shape(PrimitiveType.Sphere, "Coral_" + i, corals, Around(angle, 20.2f, height * 0.5f), new Vector3(0.7f, height, 0.7f), glow, false);
        }

        Transform lights = UtilesConstructor.Group("Luces", root).transform;
        for (int i = 0; i < 4; i++)
        {
            Color tint = i % 2 == 0 ? new Color(0.2f, 0.85f, 0.95f) : new Color(1f, 0.35f, 0.65f);
            UtilesConstructor.PointLight("Abismo_Luz_" + i, lights, Around(45f + i * 90f, 14f, 5f), tint, 14f, 20f, LightmapBakeType.Baked);
        }
        UtilesConstructor.PointLight("Abismo_Luz_Centro", lights, Center + Vector3.up * 10f, new Color(0.5f, 0.6f, 1f), 16f, 26f, LightmapBakeType.Baked);
        UtilesConstructor.PointLight("Abismo_Luz_Puente", lights, new Vector3(0f, 4f, 302f), new Color(0.2f, 0.85f, 0.95f), 8f, 14f, LightmapBakeType.Baked);

        Ambientacion.Dust("Bruma_Abismo", root, Center + Vector3.up * 3f, new Vector3(44f, 6f, 44f), 120, new Color(0.5f, 0.9f, 1f, 0.35f), UtilesConstructor.ParticleMaterial());
    }

    private static GameObject MakeBullet(Material glow)
    {
        GameObject bullet = UtilesConstructor.Shape(PrimitiveType.Sphere, "BalaEnemiga", null, Vector3.zero, Vector3.one * 0.55f, glow, true);
        bullet.GetComponent<SphereCollider>().isTrigger = true;

        Rigidbody body = bullet.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        bullet.AddComponent<BalaEnemiga>();
        return UtilesConstructor.SavePrefab(bullet, "BalaEnemiga");
    }

    private static GameObject MakeOctopus(GameObject bulletPrefab)
    {
        Material skin = UtilesConstructor.Mat("Abismo_Pulpo", new Color(0.5f, 0.18f, 0.5f), 0.1f, 0.75f);
        Material belly = UtilesConstructor.Mat("Abismo_Pulpo_Panza", new Color(0.85f, 0.55f, 0.7f), 0f, 0.6f);
        Material eyeWhite = UtilesConstructor.Mat("Abismo_Ojo", new Color(0.95f, 0.95f, 0.85f), 0f, 0.8f);
        Material eyeDark = UtilesConstructor.Mat("Abismo_Pupila", new Color(0.03f, 0.03f, 0.03f), 0f, 0.9f);
        Material leather = UtilesConstructor.Mat("Abismo_Sombrero", new Color(0.3f, 0.18f, 0.08f), 0f, 0.3f);
        Material gunMetal = UtilesConstructor.Mat("Abismo_Pistola", new Color(0.12f, 0.12f, 0.14f), 0.95f, 0.6f);

        GameObject octopus = new GameObject("Kraken");
        SphereCollider hitbox = octopus.AddComponent<SphereCollider>();
        hitbox.center = new Vector3(0f, 3f, 0f);
        hitbox.radius = 2.7f;

        Transform model = UtilesConstructor.Group("Modelo", octopus.transform).transform;

        UtilesConstructor.Shape(PrimitiveType.Sphere, "Manto", model, new Vector3(0f, 4f, -0.4f), new Vector3(4.4f, 5f, 4.8f), skin, false);
        UtilesConstructor.Shape(PrimitiveType.Sphere, "Cabeza", model, new Vector3(0f, 2.2f, 0f), new Vector3(5f, 3.2f, 5f), skin, false);
        UtilesConstructor.Shape(PrimitiveType.Sphere, "Panza", model, new Vector3(0f, 1.5f, 1.2f), new Vector3(3f, 1.6f, 2.6f), belly, false);

        for (int side = -1; side <= 1; side += 2)
        {
            string tag = side < 0 ? "L" : "R";
            UtilesConstructor.Shape(PrimitiveType.Sphere, "Ojo_" + tag, model, new Vector3(side * 1.05f, 3.1f, 1.95f), Vector3.one * 1.05f, eyeWhite, false);
            UtilesConstructor.Shape(PrimitiveType.Sphere, "Pupila_" + tag, model, new Vector3(side * 1.05f, 3.05f, 2.4f), Vector3.one * 0.45f, eyeDark, false);
            UtilesConstructor.Shape(PrimitiveType.Cube, "Ceja_" + tag, model, new Vector3(side * 1.05f, 3.75f, 2.2f), new Vector3(1.2f, 0.22f, 0.3f), eyeDark, false)
                .transform.localRotation = Quaternion.Euler(0f, 0f, side * 22f);
        }

        UtilesConstructor.Shape(PrimitiveType.Cylinder, "Sombrero_Ala", model, new Vector3(0f, 6.2f, -0.4f), new Vector3(4.6f, 0.08f, 4.6f), leather, false);
        UtilesConstructor.Shape(PrimitiveType.Cylinder, "Sombrero_Copa", model, new Vector3(0f, 6.9f, -0.4f), new Vector3(2.4f, 0.65f, 2.4f), leather, false);
        UtilesConstructor.Shape(PrimitiveType.Cylinder, "Sombrero_Banda", model, new Vector3(0f, 6.45f, -0.4f), new Vector3(2.5f, 0.12f, 2.5f), gunMetal, false);

        List<Transform> tentacles = new List<Transform>();
        List<Transform> muzzles = new List<Transform>();

        for (int i = 0; i < 8; i++)
        {
            float angle = 20f + i * 45f;
            if (i >= 4) angle += 5f;
            bool armed = i == 0 || i == 1 || i == 6 || i == 7;

            Transform joint = UtilesConstructor.Group("Tentaculo_" + i, model).transform;
            joint.localPosition = new Vector3(0f, armed ? 1.6f : 0.9f, 0f);
            joint.localRotation = Quaternion.Euler(armed ? -18f : 6f, angle, 0f);

            for (int s = 0; s < 4; s++)
            {
                float thickness = Mathf.Lerp(1.3f, 0.6f, s / 3f);
                UtilesConstructor.Shape(PrimitiveType.Sphere, "Segmento_" + s, joint, new Vector3(0f, 0f, 2.2f + s * 1.05f), new Vector3(thickness, thickness, 1.5f), skin, false);
            }

            if (armed)
            {
                UtilesConstructor.Shape(PrimitiveType.Cube, "Pistola_Canon", joint, new Vector3(0f, 0.25f, 6.4f), new Vector3(0.32f, 0.32f, 1.5f), gunMetal, false);
                UtilesConstructor.Shape(PrimitiveType.Cylinder, "Pistola_Tambor", joint, new Vector3(0f, 0.25f, 5.75f), new Vector3(0.6f, 0.25f, 0.6f), gunMetal, false)
                    .transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                UtilesConstructor.Shape(PrimitiveType.Cube, "Pistola_Empunadura", joint, new Vector3(0f, -0.15f, 5.55f), new Vector3(0.26f, 0.7f, 0.3f), leather, false);

                Transform muzzle = UtilesConstructor.Group("Boquilla", joint).transform;
                muzzle.localPosition = new Vector3(0f, 0.25f, 7.3f);
                muzzles.Add(muzzle);
            }

            tentacles.Add(joint);
        }

        VidaJefe stats = octopus.AddComponent<VidaJefe>();
        stats.bossName = "Kraken Pistolero";
        stats.maxHP = 650f;
        stats.isFinalBoss = true;

        JefePulpo brain = octopus.AddComponent<JefePulpo>();
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

        UtilesConstructor.Block("Pilar_Izq", portal.transform, new Vector3(-1.7f, 2.1f, 0f), new Vector3(0.6f, 4.2f, 0.6f), rock);
        UtilesConstructor.Block("Pilar_Der", portal.transform, new Vector3(1.7f, 2.1f, 0f), new Vector3(0.6f, 4.2f, 0.6f), rock);
        UtilesConstructor.Block("Dintel", portal.transform, new Vector3(0f, 4.4f, 0f), new Vector3(4.2f, 0.6f, 0.7f), rock);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Velo", portal.transform, new Vector3(0f, 2.1f, 0f), new Vector3(2.8f, 4f, 0.08f), glow, false);

        Light light = UtilesConstructor.PointLight("Resplandor", portal.transform, Vector3.zero, new Color(0.2f, 0.9f, 0.95f), 6f, 9f, LightmapBakeType.Realtime);
        light.transform.localPosition = new Vector3(0f, 2.2f, 0f);

        BoxCollider trigger = portal.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.center = new Vector3(0f, 2.1f, 0f);
        trigger.size = new Vector3(2.8f, 4f, 1.6f);

        Rigidbody body = portal.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        portal.AddComponent<Portal>().destination = Arrival;

        CofreBase chest = Object.FindFirstObjectByType<CofreBase>();
        if (chest != null)
        {
            chest.portal = portal;
            EditorUtility.SetDirty(chest);
            PrefabUtility.RecordPrefabInstancePropertyModifications(chest);
        }

        portal.SetActive(false);
    }
}
