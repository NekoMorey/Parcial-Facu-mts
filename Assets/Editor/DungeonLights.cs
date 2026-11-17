using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DungeonLights
{
    const string RootName = "Antorchas";
    const float Spacing = 9f;
    const int MaxTorches = 70;
    const float FillSpacing = 5f;
    const int MaxFill = 160;

    [MenuItem("Tools/Umbral/15 Antorchas de la mazmorra")]
    public static void Build()
    {
        BuilderUtils.Remove(RootName);

        var floor = GameObject.Find("Suelo");
        if (floor == null)
        {
            Debug.LogWarning("Antorchas: no encontre el objeto Suelo.");
            return;
        }

        var root = new GameObject(RootName).transform;
        Material wood = BuilderUtils.Mat("AntorchaMadera", new Color(0.25f, 0.17f, 0.1f), 0f, 0.2f);
        Material brass = BuilderUtils.Mat("AntorchaLaton", new Color(0.7f, 0.52f, 0.22f), 0.7f, 0.5f);
        Material flame = BuilderUtils.Mat("AntorchaLlama", new Color(1f, 0.6f, 0.2f), 0f, 0.3f, 4f);

        Bounds area = floor.GetComponent<Renderer>().bounds;
        int placed = 0;

        for (float z = area.min.z + 4f; z < area.max.z - 2f; z += Spacing)
        {
            for (float x = area.min.x + 4f; x < area.max.x - 2f; x += Spacing)
            {
                if (placed >= MaxTorches) break;
                if (!Physics.Raycast(new Vector3(x, area.max.y + 6f, z), Vector3.down, out RaycastHit hit, 20f)) continue;
                if (hit.collider.gameObject != floor) continue;
                if (Physics.CheckSphere(hit.point + Vector3.up * 1f, 0.7f)) continue;

                PlaceTorch(root, hit.point, wood, brass, flame);
                placed++;
            }
        }

        int fill = 0;
        for (float z = area.min.z + 2.5f; z < area.max.z - 1f; z += FillSpacing)
        {
            for (float x = area.min.x + 2.5f; x < area.max.x - 1f; x += FillSpacing)
            {
                if (fill >= MaxFill) break;
                if (!Physics.Raycast(new Vector3(x, area.max.y + 6f, z), Vector3.down, out RaycastHit hit, 20f)) continue;
                if (hit.collider.gameObject != floor) continue;

                Vector3 spot = hit.point + Vector3.up * 2.2f;
                if (Physics.CheckSphere(spot, 0.3f)) continue;
                if (Physics.Raycast(hit.point + Vector3.up * 1.2f, Vector3.up, out RaycastHit roof, 1.4f)) continue;

                BuilderUtils.PointLight("Relleno", root, spot, new Color(1f, 0.72f, 0.45f), 2.6f, 9f, LightmapBakeType.Baked);
                fill++;
            }
        }

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            BuilderUtils.PointLight("Luz_Inicio", root, player.transform.position + Vector3.up * 2.2f, new Color(1f, 0.7f, 0.4f), 4f, 14f, LightmapBakeType.Baked);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Antorchas listas: " + placed + " y " + fill + " luces de relleno en la mazmorra. Falta rehornear la luz (menu 5).");
    }

    static void PlaceTorch(Transform root, Vector3 ground, Material wood, Material brass, Material flame)
    {
        var torch = BuilderUtils.Group("Antorcha", root).transform;
        torch.position = ground;

        BuilderUtils.Shape(PrimitiveType.Cylinder, "Poste", torch, new Vector3(0f, 0.9f, 0f), new Vector3(0.14f, 0.9f, 0.14f), wood, false);
        BuilderUtils.Shape(PrimitiveType.Cylinder, "Cuenco", torch, new Vector3(0f, 1.85f, 0f), new Vector3(0.4f, 0.12f, 0.4f), brass, false);
        BuilderUtils.Shape(PrimitiveType.Sphere, "Llama", torch, new Vector3(0f, 2.15f, 0f), new Vector3(0.3f, 0.42f, 0.3f), flame, false);

        float height = 2.3f;
        if (Physics.Raycast(ground + Vector3.up * 1.5f, Vector3.up, out RaycastHit roof, 3f)) height = Mathf.Max(1.2f, roof.point.y - ground.y - 0.5f);

        BuilderUtils.PointLight("Luz", torch, ground + Vector3.up * height, new Color(1f, 0.66f, 0.34f), 5f, 15f, LightmapBakeType.Baked);
    }
}
