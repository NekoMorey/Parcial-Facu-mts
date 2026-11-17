using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ExtrasMapa
{
    private const string RootName = "04_Exploracion";

    [MenuItem("Tools/Umbral/1 Mapa y enemigos")]
    public static void Build()
    {
        UtilesConstructor.Remove(RootName);

        Material stone = UtilesConstructor.Mat("Umbral_Piedra");
        Material floor = UtilesConstructor.Mat("Umbral_Suelo");
        Material path = UtilesConstructor.Mat("Umbral_PiedraCamino");
        Material brick = UtilesConstructor.Mat("Umbral_Ladrillo_Rojo");
        Material wood = UtilesConstructor.Mat("Umbral_Cofre_Madera");
        Material brass = UtilesConstructor.Mat("Umbral_Cofre_Laton");
        Material glow = UtilesConstructor.Mat("Umbral_Guia_Luz");
        Material sentinelBody = UtilesConstructor.Mat("Umbral_Centinela", new Color(0.2f, 0.1f, 0.32f), 0.5f, 0.6f);
        Material sentinelEye = UtilesConstructor.Mat("Umbral_Centinela_Ojo", new Color(0.75f, 0.3f, 1f), 0f, 0.4f, 2.2f);
        Material doorMetal = UtilesConstructor.Mat("Umbral_Puerta", new Color(0.36f, 0.25f, 0.12f), 0.8f, 0.45f);
        Material sealGlow = UtilesConstructor.Mat("Umbral_Sello", new Color(0.15f, 0.8f, 0.85f), 0f, 0.4f, 1.8f);

        GameObject sentinelPrefab = MakeSentinel(sentinelBody, sentinelEye);
        GameObject chestPrefab = MakeUpgradeChest(wood, brass);
        GameObject keyPrefab = MakeKey(glow);

        GameObject root = UtilesConstructor.Group(RootName, null);
        BuildCrypt(root.transform, stone, floor, path, brick, sentinelPrefab, chestPrefab, keyPrefab);
        BuildParkour(root.transform, path, doorMetal, sealGlow, sentinelPrefab, chestPrefab);
        BuildArenaGate(root.transform, path, doorMetal, glow);

        GameObject boss = GameObject.FindWithTag("Boss");
        if (boss != null)
        {
            IAJefe ai = boss.GetComponent<IAJefe>();
            if (ai != null)
            {
                ai.detectionRange = 17f;
                EditorUtility.SetDirty(ai);
            }
        }

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log("Mapa: cripta, sentinelas, llave y puerta de la arena listos.");
    }

    private static GameObject MakeSentinel(Material body, Material eye)
    {
        GameObject sentinel = UtilesConstructor.Shape(PrimitiveType.Sphere, "Centinela", null, Vector3.zero, Vector3.one * 1.1f, body, true);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Ojo", sentinel.transform, new Vector3(0f, 0.08f, 0.44f), new Vector3(0.5f, 0.14f, 0.2f), eye, false);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Cuerno_Izq", sentinel.transform, new Vector3(-0.38f, 0.45f, 0f), new Vector3(0.12f, 0.45f, 0.12f), body, false);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Cuerno_Der", sentinel.transform, new Vector3(0.38f, 0.45f, 0f), new Vector3(0.12f, 0.45f, 0.12f), body, false);

        Rigidbody rb = sentinel.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        sentinel.AddComponent<Centinela>();
        return UtilesConstructor.SavePrefab(sentinel, "Centinela");
    }

    private static GameObject MakeUpgradeChest(Material wood, Material brass)
    {
        GameObject chest = new GameObject("CofreMejora");
        BoxCollider box = chest.AddComponent<BoxCollider>();
        box.center = new Vector3(0f, 0.4f, 0f);
        box.size = new Vector3(1.2f, 0.8f, 0.8f);

        UtilesConstructor.Shape(PrimitiveType.Cube, "Base", chest.transform, new Vector3(0f, 0.3f, 0f), new Vector3(1.2f, 0.6f, 0.8f), wood, false);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Banda", chest.transform, new Vector3(0f, 0.3f, 0f), new Vector3(0.2f, 0.62f, 0.82f), brass, false);

        GameObject lid = UtilesConstructor.Group("Tapa", chest.transform);
        lid.transform.localPosition = new Vector3(0f, 0.6f, -0.4f);
        UtilesConstructor.Shape(PrimitiveType.Cube, "TapaSuperior", lid.transform, new Vector3(0f, 0.12f, 0.4f), new Vector3(1.24f, 0.24f, 0.84f), brass, false);

        CofreMejora upgrade = chest.AddComponent<CofreMejora>();
        upgrade.lid = lid.transform;

        return UtilesConstructor.SavePrefab(chest, "CofreMejora");
    }

    private static GameObject MakeKey(Material glow)
    {
        GameObject key = new GameObject("LlaveUmbral");
        SphereCollider trigger = key.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.9f;

        Rigidbody rb = key.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        UtilesConstructor.Shape(PrimitiveType.Cylinder, "Anillo", key.transform, new Vector3(0f, 0.3f, 0f), new Vector3(0.45f, 0.05f, 0.45f), glow, false)
            .transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Vastago", key.transform, new Vector3(0f, -0.15f, 0f), new Vector3(0.1f, 0.6f, 0.08f), glow, false);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Diente", key.transform, new Vector3(0.12f, -0.38f, 0f), new Vector3(0.18f, 0.1f, 0.08f), glow, false);

        Light light = UtilesConstructor.PointLight("Resplandor", key.transform, Vector3.zero, new Color(1f, 0.7f, 0.25f), 2.5f, 5f, LightmapBakeType.Realtime);
        light.transform.localPosition = Vector3.zero;

        key.AddComponent<RecogerLlave>();
        return UtilesConstructor.SavePrefab(key, "LlaveUmbral");
    }

    private static void BuildCrypt(Transform root, Material stone, Material floor, Material path, Material brick, GameObject sentinelPrefab, GameObject chestPrefab, GameObject keyPrefab)
    {
        Transform crypt = UtilesConstructor.Group("Cripta", root).transform;

        GameObject map = GameObject.Find("Mapa");
        Transform oldWall = map != null ? map.transform.Find("01_Vestibulo/Vestibulo_Muro_Este") : null;
        if (oldWall != null) oldWall.gameObject.SetActive(false);

        UtilesConstructor.Block("Vestibulo_Muro_Este_A", crypt, new Vector3(11f, 3f, -26.75f), new Vector3(1f, 6f, 8.5f), stone);
        UtilesConstructor.Block("Vestibulo_Muro_Este_B", crypt, new Vector3(11f, 3f, -13.25f), new Vector3(1f, 6f, 8.5f), stone);
        UtilesConstructor.Block("Vestibulo_Muro_Este_Alto", crypt, new Vector3(11f, 5f, -20f), new Vector3(1f, 2f, 5f), stone);

        GameObject cracked = UtilesConstructor.Block("Cripta_MuroAgrietado", crypt, new Vector3(11f, 2f, -20f), new Vector3(0.8f, 4f, 5f), brick);
        cracked.tag = "Destructible";

        UtilesConstructor.Block("Cripta_Suelo", crypt, new Vector3(19f, -0.5f, -20f), new Vector3(15f, 1f, 15f), floor);
        UtilesConstructor.Block("Cripta_Muro_Este", crypt, new Vector3(27f, 3f, -20f), new Vector3(1f, 6f, 16f), stone);
        UtilesConstructor.Block("Cripta_Muro_Norte", crypt, new Vector3(19.25f, 3f, -12f), new Vector3(16.5f, 6f, 1f), stone);
        UtilesConstructor.Block("Cripta_Muro_Sur", crypt, new Vector3(19.25f, 3f, -28f), new Vector3(16.5f, 6f, 1f), stone);
        UtilesConstructor.Block("Cripta_Pilar_1", crypt, new Vector3(15.5f, 3f, -23.5f), new Vector3(1.2f, 6f, 1.2f), stone);
        UtilesConstructor.Block("Cripta_Pilar_2", crypt, new Vector3(21.5f, 3f, -16f), new Vector3(1.2f, 6f, 1.2f), stone);
        UtilesConstructor.Block("Cripta_Pedestal", crypt, new Vector3(24f, 0.5f, -20f), new Vector3(1.4f, 1f, 1.4f), path);

        UtilesConstructor.Spawn(keyPrefab, "LlaveUmbral", crypt, new Vector3(24f, 2f, -20f));

        GameObject chest = UtilesConstructor.Spawn(chestPrefab, "Cripta_CofreEstamina", crypt, new Vector3(19f, 0f, -26.5f));
        CofreMejora upgrade = chest.GetComponent<CofreMejora>();
        upgrade.upgrade = CofreMejora.TipoMejora.Estamina;
        upgrade.amount = 40f;

        UtilesConstructor.Spawn(sentinelPrefab, "Cripta_Centinela_1", crypt, new Vector3(17f, 1.6f, -17f));
        UtilesConstructor.Spawn(sentinelPrefab, "Cripta_Centinela_2", crypt, new Vector3(22f, 1.6f, -23f));

        UtilesConstructor.PointLight("Cripta_Luz", crypt, new Vector3(19f, 4.5f, -20f), new Color(0.55f, 0.4f, 1f), 9f, 14f, LightmapBakeType.Baked);
    }

    private static void BuildParkour(Transform root, Material path, Material metal, Material sealGlow, GameObject sentinelPrefab, GameObject chestPrefab)
    {
        Transform parkour = UtilesConstructor.Group("Parkour", root).transform;

        UtilesConstructor.Block("Saliente_Secreto", parkour, new Vector3(10.25f, 4.5f, 30f), new Vector3(4.5f, 1f, 5f), path);
        UtilesConstructor.Block("Jaula_Lado_A", parkour, new Vector3(10.25f, 7f, 27.6f), new Vector3(4.5f, 4f, 0.3f), metal);
        UtilesConstructor.Block("Jaula_Lado_B", parkour, new Vector3(10.25f, 7f, 32.4f), new Vector3(4.5f, 4f, 0.3f), metal);
        UtilesConstructor.Block("Jaula_Alto", parkour, new Vector3(10.25f, 9.15f, 30f), new Vector3(4.5f, 0.3f, 5.1f), metal);

        GameObject gate = UtilesConstructor.Group("Jaula_Reja", parkour);
        gate.transform.position = new Vector3(8.1f, 7f, 30f);
        UtilesConstructor.Block("Reja_Barrotes", gate.transform, Vector3.zero, new Vector3(0.3f, 4f, 5f), metal);

        GameObject seal = UtilesConstructor.Shape(PrimitiveType.Cylinder, "Jaula_Sello", gate.transform, new Vector3(-0.25f, 0f, 0f), new Vector3(1.7f, 0.1f, 1.7f), sealGlow, false);
        seal.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        seal.AddComponent<BoxCollider>();

        InterruptorDisparable shootable = seal.AddComponent<InterruptorDisparable>();
        shootable.movingPiece = gate.transform;
        shootable.moveOffset = new Vector3(0f, -4.6f, 0f);
        shootable.message = "El sello se rompió: la jaula está abierta";

        GameObject chest = UtilesConstructor.Spawn(chestPrefab, "Saliente_CofreFuerza", parkour, new Vector3(10.8f, 5f, 30f));
        chest.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        CofreMejora upgrade = chest.GetComponent<CofreMejora>();
        upgrade.upgrade = CofreMejora.TipoMejora.Fuerza;
        upgrade.amount = 8f;

        UtilesConstructor.Spawn(sentinelPrefab, "Parkour_Centinela", parkour, new Vector3(-3.5f, 6.4f, 27f));
        UtilesConstructor.Spawn(sentinelPrefab, "Pasillo_Centinela", parkour, new Vector3(0f, 8.4f, 52f));

        UtilesConstructor.PointLight("Saliente_Luz", parkour, new Vector3(9.5f, 8f, 30f), new Color(0.2f, 0.85f, 0.9f), 6f, 9f, LightmapBakeType.Baked);
    }

    private static void BuildArenaGate(Transform root, Material path, Material metal, Material glow)
    {
        Transform arena = UtilesConstructor.Group("Arena", root).transform;

        UtilesConstructor.Block("Arena_Umbral", arena, new Vector3(0f, 6.2f, 58f), new Vector3(8f, 1f, 4f), path);

        GameObject door = UtilesConstructor.Block("Arena_Puerta", arena, new Vector3(0f, 9.2f, 60f), new Vector3(8f, 5f, 0.8f), metal);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Puerta_Sello", door.transform, new Vector3(0f, 0f, -0.55f), new Vector3(0.12f, 0.5f, 0.2f), glow, false);
        door.AddComponent<PuertaCerrada>();

        if (!PrefabUtility.IsPartOfPrefabInstance(door))
        {
            UtilesConstructor.EnsureFolder(UtilesConstructor.PrefabFolder);
            PrefabUtility.SaveAsPrefabAssetAndConnect(door, UtilesConstructor.PrefabFolder + "/PuertaArena.prefab", InteractionMode.AutomatedAction);
        }
    }
}
