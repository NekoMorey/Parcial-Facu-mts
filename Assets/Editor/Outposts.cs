using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Outposts
{
    const string RootName = "Exploracion";

    [MenuItem("Tools/Umbral/11 Puestos de avanzada")]
    public static void Build()
    {
        BuilderUtils.Remove(RootName);
        var root = new GameObject(RootName).transform;

        Material stone = BuilderUtils.Mat("PuestoPiedra", new Color(0.32f, 0.33f, 0.36f), 0f, 0.15f);
        Material trim = BuilderUtils.Mat("PuestoBorde", new Color(0.2f, 0.21f, 0.24f), 0f, 0.1f);
        Material ember = BuilderUtils.Mat("PuestoBrasa", new Color(1f, 0.55f, 0.2f), 0f, 0.3f, 2.5f);
        Material gunnerBody = BuilderUtils.Mat("TiradorCuerpo", new Color(0.35f, 0.12f, 0.12f), 0.2f, 0.4f);
        Material gunnerEye = BuilderUtils.Mat("TiradorOjo", new Color(1f, 0.3f, 0.2f), 0f, 0.5f, 3f);
        Material brass = BuilderUtils.Mat("PuestoLaton", new Color(0.75f, 0.58f, 0.25f), 0.7f, 0.5f);

        GameObject bullet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EnemyBullet.prefab");
        GameObject gunner = MakeGunner(gunnerBody, gunnerEye, bullet);
        GameObject chest = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UpgradeChest.prefab");
        GameObject tablet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tablilla.prefab");

        Vector3 center = Vector3.zero;
        float reach = 40f;
        var map = GameObject.Find("Mapa");
        if (map != null)
        {
            var renderers = map.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                foreach (var r in renderers) bounds.Encapsulate(r.bounds);
                center = bounds.center;
                reach = Mathf.Max(bounds.extents.x, bounds.extents.z);
            }
        }

        float[] angles = { 20f, 80f, 140f, 200f, 260f, 320f };
        float[] extra = { 30f, 55f, 40f, 65f, 35f, 50f };
        var upgrades = new[] { UpgradeChest.Upgrade.Health, UpgradeChest.Upgrade.Stamina, UpgradeChest.Upgrade.Strength };
        string[] names = { "Puesto_Norte", "Puesto_Colina", "Puesto_Barranca", "Puesto_Mirador", "Puesto_Ruina", "Puesto_Olvidado" };

        int placed = 0;
        for (int i = 0; i < angles.Length; i++)
        {
            float radians = angles[i] * Mathf.Deg2Rad;
            Vector3 flat = center + new Vector3(Mathf.Sin(radians), 0f, Mathf.Cos(radians)) * (reach + extra[i]);
            if (flat.z > 245f) continue;
            if (!Physics.Raycast(new Vector3(flat.x, 300f, flat.z), Vector3.down, out RaycastHit hit, 600f)) continue;

            Transform post = BuilderUtils.Group(names[i], root).transform;
            post.position = hit.point;
            BuildRuin(post, stone, trim, ember);

            if (chest != null)
            {
                GameObject box = BuilderUtils.Spawn(chest, "Cofre_" + names[i], post, hit.point + Vector3.up * 0.05f);
                var upgrade = box.GetComponent<UpgradeChest>();
                if (upgrade != null)
                {
                    upgrade.upgrade = upgrades[i % upgrades.Length];
                    upgrade.amount = upgrade.upgrade == UpgradeChest.Upgrade.Strength ? 5f : 20f;
                }
            }

            for (int g = 0; g < 2; g++)
            {
                float side = g == 0 ? -1f : 1f;
                Vector3 spot = hit.point + new Vector3(side * 7f, 0f, 7f);
                if (Physics.Raycast(spot + Vector3.up * 100f, Vector3.down, out RaycastHit gHit, 200f))
                    BuilderUtils.Spawn(gunner, "Tirador_" + (g + 1), post, gHit.point + Vector3.up * 0.1f);
            }

            if (tablet != null && i % 2 == 0)
            {
                GameObject piece = BuilderUtils.Spawn(tablet, "Tablilla_" + names[i], post, hit.point + new Vector3(-3.5f, 1.3f, -3.5f));
                piece.GetComponent<LoreTablet>().message = "Los puestos olvidados guardan lo que el umbral no quiso llevarse.";
            }

            placed++;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Puestos de avanzada listos: " + placed + " colocados.");
    }

    static void BuildRuin(Transform post, Material stone, Material trim, Material ember)
    {
        for (int i = 0; i < 4; i++)
        {
            float a = i * 90f * Mathf.Deg2Rad + 0.4f;
            Vector3 offset = new Vector3(Mathf.Sin(a), 0f, Mathf.Cos(a)) * 5f;
            float height = 4f + (i % 2) * 1.5f;
            GameObject pillar = BuilderUtils.Shape(PrimitiveType.Cylinder, "Pilar_" + i, post, offset + Vector3.up * height * 0.5f, new Vector3(1.1f, height * 0.5f, 1.1f), stone, true);
            pillar.transform.localPosition = offset + Vector3.up * (height * 0.5f - 0.3f);
            BuilderUtils.Shape(PrimitiveType.Cube, "Capitel_" + i, pillar.transform, new Vector3(0f, 1f, 0f), new Vector3(1.5f, 0.15f, 1.5f), trim, false);
        }

        BuilderUtils.Block("Muro_A", post, new Vector3(0f, 0.8f, 5.5f), new Vector3(6f, 1.6f, 0.6f), stone);
        BuilderUtils.Block("Muro_B", post, new Vector3(-5.5f, 0.6f, 0f), new Vector3(0.6f, 1.2f, 4f), stone);
        BuilderUtils.Shape(PrimitiveType.Cylinder, "Brasero", post, new Vector3(3f, 0.45f, -3f), new Vector3(0.9f, 0.45f, 0.9f), trim, true);
        BuilderUtils.Shape(PrimitiveType.Sphere, "Brasa", post, new Vector3(3f, 1f, -3f), new Vector3(0.6f, 0.4f, 0.6f), ember, false);
        BuilderUtils.PointLight("Fuego", post, new Vector3(3f, 1.8f, -3f), new Color(1f, 0.6f, 0.3f), 3f, 14f, LightmapBakeType.Realtime);
    }

    static GameObject MakeGunner(Material body, Material eye, GameObject bullet)
    {
        GameObject root = new GameObject("Tirador");
        var capsule = root.AddComponent<CapsuleCollider>();
        capsule.center = new Vector3(0f, 1f, 0f);
        capsule.height = 2f;
        capsule.radius = 0.5f;

        BuilderUtils.Shape(PrimitiveType.Capsule, "Cuerpo", root.transform, new Vector3(0f, 1f, 0f), new Vector3(0.9f, 1f, 0.9f), body, false);
        BuilderUtils.Shape(PrimitiveType.Cube, "Visor", root.transform, new Vector3(0f, 1.45f, 0.4f), new Vector3(0.5f, 0.14f, 0.2f), eye, false);
        BuilderUtils.Shape(PrimitiveType.Cube, "Pistola", root.transform, new Vector3(0.45f, 1.1f, 0.5f), new Vector3(0.14f, 0.18f, 0.6f), body, false);

        var rb = root.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var shooter = root.AddComponent<Gunner>();
        shooter.bulletPrefab = bullet;
        return BuilderUtils.SavePrefab(root, "Tirador");
    }
}
