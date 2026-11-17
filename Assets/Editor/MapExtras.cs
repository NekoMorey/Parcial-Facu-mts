using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MapExtras
{
    private const string RootName = "04_Exploracion";

    [MenuItem("Tools/Umbral/1 Mapa y enemigos")]
    public static void Build()
    {
        BuilderUtils.Remove(RootName);

        Material stone = BuilderUtils.Mat("Umbral_Stone");
        Material floor = BuilderUtils.Mat("Umbral_Floor");
        Material path = BuilderUtils.Mat("Umbral_Pathstone");
        Material brick = BuilderUtils.Mat("Umbral_Red_Brick");
        Material wood = BuilderUtils.Mat("Umbral_Chest_Wood");
        Material brass = BuilderUtils.Mat("Umbral_Chest_Brass");
        Material glow = BuilderUtils.Mat("Umbral_Guide_Light");
        Material sentinelBody = BuilderUtils.Mat("Umbral_Sentinel", new Color(0.2f, 0.1f, 0.32f), 0.5f, 0.6f);
        Material sentinelEye = BuilderUtils.Mat("Umbral_Sentinel_Eye", new Color(0.75f, 0.3f, 1f), 0f, 0.4f, 2.2f);
        Material doorMetal = BuilderUtils.Mat("Umbral_Door", new Color(0.36f, 0.25f, 0.12f), 0.8f, 0.45f);
        Material sealGlow = BuilderUtils.Mat("Umbral_Seal", new Color(0.15f, 0.8f, 0.85f), 0f, 0.4f, 1.8f);

        GameObject sentinelPrefab = MakeSentinel(sentinelBody, sentinelEye);
        GameObject chestPrefab = MakeUpgradeChest(wood, brass);
        GameObject keyPrefab = MakeKey(glow);

        GameObject root = BuilderUtils.Group(RootName, null);
        BuildCrypt(root.transform, stone, floor, path, brick, sentinelPrefab, chestPrefab, keyPrefab);
        BuildParkour(root.transform, path, doorMetal, sealGlow, sentinelPrefab, chestPrefab);
        BuildArenaGate(root.transform, path, doorMetal, glow);

        GameObject boss = GameObject.FindWithTag("Boss");
        if (boss != null)
        {
            BossAI ai = boss.GetComponent<BossAI>();
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
        GameObject sentinel = BuilderUtils.Shape(PrimitiveType.Sphere, "Sentinel", null, Vector3.zero, Vector3.one * 1.1f, body, true);
        BuilderUtils.Shape(PrimitiveType.Cube, "Eye", sentinel.transform, new Vector3(0f, 0.08f, 0.44f), new Vector3(0.5f, 0.14f, 0.2f), eye, false);
        BuilderUtils.Shape(PrimitiveType.Cube, "Horn_L", sentinel.transform, new Vector3(-0.38f, 0.45f, 0f), new Vector3(0.12f, 0.45f, 0.12f), body, false);
        BuilderUtils.Shape(PrimitiveType.Cube, "Horn_R", sentinel.transform, new Vector3(0.38f, 0.45f, 0f), new Vector3(0.12f, 0.45f, 0.12f), body, false);

        Rigidbody rb = sentinel.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        sentinel.AddComponent<Sentinel>();
        return BuilderUtils.SavePrefab(sentinel, "Sentinel");
    }

    private static GameObject MakeUpgradeChest(Material wood, Material brass)
    {
        GameObject chest = new GameObject("UpgradeChest");
        BoxCollider box = chest.AddComponent<BoxCollider>();
        box.center = new Vector3(0f, 0.4f, 0f);
        box.size = new Vector3(1.2f, 0.8f, 0.8f);

        BuilderUtils.Shape(PrimitiveType.Cube, "Base", chest.transform, new Vector3(0f, 0.3f, 0f), new Vector3(1.2f, 0.6f, 0.8f), wood, false);
        BuilderUtils.Shape(PrimitiveType.Cube, "Band", chest.transform, new Vector3(0f, 0.3f, 0f), new Vector3(0.2f, 0.62f, 0.82f), brass, false);

        GameObject lid = BuilderUtils.Group("Lid", chest.transform);
        lid.transform.localPosition = new Vector3(0f, 0.6f, -0.4f);
        BuilderUtils.Shape(PrimitiveType.Cube, "LidTop", lid.transform, new Vector3(0f, 0.12f, 0.4f), new Vector3(1.24f, 0.24f, 0.84f), brass, false);

        UpgradeChest upgrade = chest.AddComponent<UpgradeChest>();
        upgrade.lid = lid.transform;

        return BuilderUtils.SavePrefab(chest, "UpgradeChest");
    }

    private static GameObject MakeKey(Material glow)
    {
        GameObject key = new GameObject("UmbralKey");
        SphereCollider trigger = key.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.9f;

        Rigidbody rb = key.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        BuilderUtils.Shape(PrimitiveType.Cylinder, "Ring", key.transform, new Vector3(0f, 0.3f, 0f), new Vector3(0.45f, 0.05f, 0.45f), glow, false)
            .transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        BuilderUtils.Shape(PrimitiveType.Cube, "Shaft", key.transform, new Vector3(0f, -0.15f, 0f), new Vector3(0.1f, 0.6f, 0.08f), glow, false);
        BuilderUtils.Shape(PrimitiveType.Cube, "Tooth", key.transform, new Vector3(0.12f, -0.38f, 0f), new Vector3(0.18f, 0.1f, 0.08f), glow, false);

        Light light = BuilderUtils.PointLight("Glow", key.transform, Vector3.zero, new Color(1f, 0.7f, 0.25f), 2.5f, 5f, LightmapBakeType.Realtime);
        light.transform.localPosition = Vector3.zero;

        key.AddComponent<KeyPickup>();
        return BuilderUtils.SavePrefab(key, "UmbralKey");
    }

    private static void BuildCrypt(Transform root, Material stone, Material floor, Material path, Material brick, GameObject sentinelPrefab, GameObject chestPrefab, GameObject keyPrefab)
    {
        Transform crypt = BuilderUtils.Group("Cripta", root).transform;

        GameObject map = GameObject.Find("Mapa");
        Transform oldWall = map != null ? map.transform.Find("01_Lobby/Lobby_Wall_East") : null;
        if (oldWall != null) oldWall.gameObject.SetActive(false);

        BuilderUtils.Block("Lobby_Wall_East_A", crypt, new Vector3(11f, 3f, -26.75f), new Vector3(1f, 6f, 8.5f), stone);
        BuilderUtils.Block("Lobby_Wall_East_B", crypt, new Vector3(11f, 3f, -13.25f), new Vector3(1f, 6f, 8.5f), stone);
        BuilderUtils.Block("Lobby_Wall_East_Top", crypt, new Vector3(11f, 5f, -20f), new Vector3(1f, 2f, 5f), stone);

        GameObject cracked = BuilderUtils.Block("Crypt_CrackedWall", crypt, new Vector3(11f, 2f, -20f), new Vector3(0.8f, 4f, 5f), brick);
        cracked.tag = "Destructible";

        BuilderUtils.Block("Crypt_Floor", crypt, new Vector3(19f, -0.5f, -20f), new Vector3(15f, 1f, 15f), floor);
        BuilderUtils.Block("Crypt_Wall_East", crypt, new Vector3(27f, 3f, -20f), new Vector3(1f, 6f, 16f), stone);
        BuilderUtils.Block("Crypt_Wall_North", crypt, new Vector3(19.25f, 3f, -12f), new Vector3(16.5f, 6f, 1f), stone);
        BuilderUtils.Block("Crypt_Wall_South", crypt, new Vector3(19.25f, 3f, -28f), new Vector3(16.5f, 6f, 1f), stone);
        BuilderUtils.Block("Crypt_Pillar_1", crypt, new Vector3(15.5f, 3f, -23.5f), new Vector3(1.2f, 6f, 1.2f), stone);
        BuilderUtils.Block("Crypt_Pillar_2", crypt, new Vector3(21.5f, 3f, -16f), new Vector3(1.2f, 6f, 1.2f), stone);
        BuilderUtils.Block("Crypt_Pedestal", crypt, new Vector3(24f, 0.5f, -20f), new Vector3(1.4f, 1f, 1.4f), path);

        BuilderUtils.Spawn(keyPrefab, "UmbralKey", crypt, new Vector3(24f, 2f, -20f));

        GameObject chest = BuilderUtils.Spawn(chestPrefab, "Crypt_StaminaChest", crypt, new Vector3(19f, 0f, -26.5f));
        UpgradeChest upgrade = chest.GetComponent<UpgradeChest>();
        upgrade.upgrade = UpgradeChest.Upgrade.Stamina;
        upgrade.amount = 40f;

        BuilderUtils.Spawn(sentinelPrefab, "Crypt_Sentinel_1", crypt, new Vector3(17f, 1.6f, -17f));
        BuilderUtils.Spawn(sentinelPrefab, "Crypt_Sentinel_2", crypt, new Vector3(22f, 1.6f, -23f));

        BuilderUtils.PointLight("Crypt_Light", crypt, new Vector3(19f, 4.5f, -20f), new Color(0.55f, 0.4f, 1f), 9f, 14f, LightmapBakeType.Baked);
    }

    private static void BuildParkour(Transform root, Material path, Material metal, Material sealGlow, GameObject sentinelPrefab, GameObject chestPrefab)
    {
        Transform parkour = BuilderUtils.Group("Parkour", root).transform;

        BuilderUtils.Block("Secret_Ledge", parkour, new Vector3(10.25f, 4.5f, 30f), new Vector3(4.5f, 1f, 5f), path);
        BuilderUtils.Block("Cage_Side_A", parkour, new Vector3(10.25f, 7f, 27.6f), new Vector3(4.5f, 4f, 0.3f), metal);
        BuilderUtils.Block("Cage_Side_B", parkour, new Vector3(10.25f, 7f, 32.4f), new Vector3(4.5f, 4f, 0.3f), metal);
        BuilderUtils.Block("Cage_Top", parkour, new Vector3(10.25f, 9.15f, 30f), new Vector3(4.5f, 0.3f, 5.1f), metal);

        GameObject gate = BuilderUtils.Group("Cage_Gate", parkour);
        gate.transform.position = new Vector3(8.1f, 7f, 30f);
        BuilderUtils.Block("Gate_Bars", gate.transform, Vector3.zero, new Vector3(0.3f, 4f, 5f), metal);

        GameObject seal = BuilderUtils.Shape(PrimitiveType.Cylinder, "Cage_Seal", gate.transform, new Vector3(-0.25f, 0f, 0f), new Vector3(1.7f, 0.1f, 1.7f), sealGlow, false);
        seal.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        seal.AddComponent<BoxCollider>();

        ShootableSwitch shootable = seal.AddComponent<ShootableSwitch>();
        shootable.movingPiece = gate.transform;
        shootable.moveOffset = new Vector3(0f, -4.6f, 0f);
        shootable.message = "El sello se rompió: la jaula está abierta";

        GameObject chest = BuilderUtils.Spawn(chestPrefab, "Ledge_StrengthChest", parkour, new Vector3(10.8f, 5f, 30f));
        chest.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        UpgradeChest upgrade = chest.GetComponent<UpgradeChest>();
        upgrade.upgrade = UpgradeChest.Upgrade.Strength;
        upgrade.amount = 8f;

        BuilderUtils.Spawn(sentinelPrefab, "Parkour_Sentinel", parkour, new Vector3(-3.5f, 6.4f, 27f));
        BuilderUtils.Spawn(sentinelPrefab, "Corridor_Sentinel", parkour, new Vector3(0f, 8.4f, 52f));

        BuilderUtils.PointLight("Ledge_Light", parkour, new Vector3(9.5f, 8f, 30f), new Color(0.2f, 0.85f, 0.9f), 6f, 9f, LightmapBakeType.Baked);
    }

    private static void BuildArenaGate(Transform root, Material path, Material metal, Material glow)
    {
        Transform arena = BuilderUtils.Group("Arena", root).transform;

        BuilderUtils.Block("Arena_Threshold", arena, new Vector3(0f, 6.2f, 58f), new Vector3(8f, 1f, 4f), path);

        GameObject door = BuilderUtils.Block("Arena_Door", arena, new Vector3(0f, 9.2f, 60f), new Vector3(8f, 5f, 0.8f), metal);
        BuilderUtils.Shape(PrimitiveType.Cube, "Door_Seal", door.transform, new Vector3(0f, 0f, -0.55f), new Vector3(0.12f, 0.5f, 0.2f), glow, false);
        door.AddComponent<LockedDoor>();

        if (!PrefabUtility.IsPartOfPrefabInstance(door))
        {
            BuilderUtils.EnsureFolder(BuilderUtils.PrefabFolder);
            PrefabUtility.SaveAsPrefabAssetAndConnect(door, BuilderUtils.PrefabFolder + "/ArenaDoor.prefab", InteractionMode.AutomatedAction);
        }
    }
}
