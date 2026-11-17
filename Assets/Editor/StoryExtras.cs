using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class StoryExtras
{
    static readonly string[] Lines =
    {
        "Los guardianes juraron proteger el umbral hasta que no quedara nadie para cruzarlo.",
        "La llave no abre una puerta, abre una promesa. Cuídala.",
        "Bajo la isla, algo con muchos brazos y mala puntería antigua aprendió a disparar.",
        "Quien cruza el portal no vuelve igual. Yo todavía escucho el mar.",
        "El caballero no siempre fue de piedra. Alguna vez tuvo nombre."
    };

    [MenuItem("Tools/Umbral/10 Tablillas y secuencias")]
    public static void Build()
    {
        BuilderUtils.EnsureFolder("Assets/Prefabs");
        BuilderUtils.Remove("Tablillas");
        BuilderUtils.Remove("Secuencias");

        var player = Object.FindFirstObjectByType<PlayerController>();
        var knight = Object.FindFirstObjectByType<BossAI>();
        var kraken = Object.FindFirstObjectByType<OctopusBoss>();

        var prefab = BuildTabletPrefab();
        var tablets = new GameObject("Tablillas").transform;

        var spots = new System.Collections.Generic.List<Vector3>();
        if (player != null)
        {
            Vector3 origin = player.transform.position;
            spots.Add(origin + new Vector3(24f, 0f, 22f));
            spots.Add(origin + new Vector3(-30f, 0f, 38f));
            spots.Add(origin + new Vector3(42f, 0f, -18f));
        }
        spots.Add(new Vector3(-16f, 0f, 318f));
        spots.Add(new Vector3(18f, 0f, 332f));

        for (int i = 0; i < spots.Count; i++)
        {
            Vector3 ground = Ground(spots[i]);
            var tablet = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            tablet.name = "Tablilla_" + (i + 1);
            tablet.transform.SetParent(tablets, false);
            tablet.transform.position = ground + Vector3.up * 1.2f;
            tablet.GetComponent<LoreTablet>().message = Lines[i % Lines.Length];
        }

        var sequences = new GameObject("Secuencias").transform;
        if (knight != null)
            MakeShot(sequences, "Intro_Caballero", knight.transform, knight.transform.position + new Vector3(0f, 2f, 0f), new Vector3(34f, 12f, 30f),
                new Vector3(0f, 7f, -16f), "El Caballero Corrupto despierta");
        if (kraken != null)
            MakeShot(sequences, "Intro_Kraken", kraken.transform, kraken.transform.position + new Vector3(0f, 2f, -30f), new Vector3(44f, 20f, 8f),
                new Vector3(0f, 10f, -24f), "Algo se mueve bajo el agua");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Tablillas y secuencias de introducción listas.");
    }

    static Vector3 Ground(Vector3 around)
    {
        var from = new Vector3(around.x, around.y + 120f, around.z);
        if (Physics.Raycast(from, Vector3.down, out RaycastHit hit, 400f)) return hit.point;
        return around;
    }

    static void MakeShot(Transform parent, string name, Transform target, Vector3 center, Vector3 size, Vector3 offset, string caption)
    {
        var holder = new GameObject(name);
        holder.transform.SetParent(parent, false);
        holder.transform.position = center;
        var box = holder.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = size;
        var shot = holder.AddComponent<CinematicShot>();
        shot.lookAt = target;
        shot.cameraOffset = offset;
        shot.caption = caption;
        shot.duration = 3.5f;
    }

    static GameObject BuildTabletPrefab()
    {
        var root = new GameObject("Tablilla");
        var glow = BuilderUtils.Mat("TablillaBrillo", new Color(0.35f, 0.85f, 1f), 0f, 0.7f, 2.2f);
        var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Piedra";
        body.transform.SetParent(root.transform, false);
        body.transform.localScale = new Vector3(0.7f, 1f, 0.15f);
        body.GetComponent<MeshRenderer>().sharedMaterial = glow;
        Object.DestroyImmediate(body.GetComponent<BoxCollider>());

        var trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.6f;
        root.AddComponent<LoreTablet>();

        var saved = PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Tablilla.prefab");
        Object.DestroyImmediate(root);
        return saved;
    }
}
