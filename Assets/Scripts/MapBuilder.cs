using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class MapBuilder : MonoBehaviour
{
    public bool generarAlIniciar = true;

    void Start()
    {
        if (GameObject.Find("--- MAPA GENERADO ---") == null && generarAlIniciar)
        {
            BuildMap();
        }
    }

    [ContextMenu("Generar Mapa Ahora")]
    public void BuildMap()
    {
        GameObject existing = GameObject.Find("--- MAPA GENERADO ---");
        if (existing != null)
        {
            if (Application.isPlaying)
                Destroy(existing);
            else
                DestroyImmediate(existing);
        }

        GameObject root = new GameObject("--- MAPA GENERADO ---");

        Material defaultMat = null;
#if UNITY_EDITOR
        defaultMat = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
#endif

        CreateLobby(root.transform, defaultMat);
        CreateParkour(root.transform, defaultMat);
        CreateArena(root.transform, defaultMat);

        PositionEntities();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
#endif
        
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Generar Mapa (Lobby + Parkour + Arena)")]
    [MenuItem("Herramientas/Generar Mapa (Lobby + Parkour + Arena)")]
    public static void GenerateMapFromMenu()
    {
        MapBuilder builder = Object.FindFirstObjectByType<MapBuilder>();
        if (builder == null)
        {
            GameObject go = new GameObject("MapBuilder");
            builder = go.AddComponent<MapBuilder>();
        }
        builder.BuildMap();
    }
#endif

    private static GameObject CreatePart(string name, Vector3 pos, Vector3 scale, Transform parent, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = pos;
        obj.transform.localScale = scale;
        obj.transform.SetParent(parent);

        if (mat != null)
        {
            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            if (mr != null) mr.sharedMaterial = mat;
        }

#if UNITY_EDITOR
        GameObjectUtility.SetStaticEditorFlags(obj, 
            StaticEditorFlags.ContributeGI | 
            StaticEditorFlags.BatchingStatic | 
            StaticEditorFlags.OccludeeStatic);
#endif

        return obj;
    }

    private static void CreateLobby(Transform parent, Material mat)
    {
        Transform lobby = new GameObject("01_Lobby").transform;
        lobby.SetParent(parent);

        CreatePart("Lobby_Floor", new Vector3(0f, -0.5f, -20f), new Vector3(22f, 1f, 22f), lobby, mat);
        CreatePart("Lobby_Wall_South", new Vector3(0f, 3f, -31f), new Vector3(22f, 6f, 1f), lobby, mat);
        CreatePart("Lobby_Wall_West", new Vector3(-11f, 3f, -20f), new Vector3(1f, 6f, 22f), lobby, mat);
        CreatePart("Lobby_Wall_East", new Vector3(11f, 3f, -20f), new Vector3(1f, 6f, 22f), lobby, mat);

        CreatePart("Lobby_Wall_North_Left", new Vector3(-7f, 3f, -9f), new Vector3(8f, 6f, 1f), lobby, mat);
        CreatePart("Lobby_Wall_North_Right", new Vector3(7f, 3f, -9f), new Vector3(8f, 6f, 1f), lobby, mat);
        CreatePart("Lobby_Wall_North_Top", new Vector3(0f, 4.5f, -9f), new Vector3(6f, 3f, 1f), lobby, mat);

        CreatePart("Corridor_1_Floor", new Vector3(0f, -0.5f, -4f), new Vector3(6f, 1f, 10f), lobby, mat);
        CreatePart("Corridor_1_Wall_L", new Vector3(-3.5f, 2.5f, -4f), new Vector3(1f, 5f, 10f), lobby, mat);
        CreatePart("Corridor_1_Wall_R", new Vector3(3.5f, 2.5f, -4f), new Vector3(1f, 5f, 10f), lobby, mat);
    }

    private static void CreateParkour(Transform parent, Material mat)
    {
        Transform parkour = new GameObject("02_Parkour").transform;
        parkour.SetParent(parent);

        CreatePart("Parkour_FallFloor", new Vector3(0f, -6f, 24f), new Vector3(26f, 1f, 46f), parkour, mat);

        CreatePart("Parkour_Wall_West", new Vector3(-13f, 5f, 24f), new Vector3(1f, 16f, 46f), parkour, mat);
        CreatePart("Parkour_Wall_East", new Vector3(13f, 5f, 24f), new Vector3(1f, 16f, 46f), parkour, mat);

        CreatePart("Parkour_StartPad", new Vector3(0f, -0.5f, 3f), new Vector3(6f, 1f, 4f), parkour, mat);

        CreatePart("Parkour_Platform_1", new Vector3(0f, 0.8f, 9f), new Vector3(4f, 1f, 4f), parkour, mat);
        CreatePart("Parkour_Platform_2", new Vector3(3.5f, 2.2f, 16f), new Vector3(3.5f, 1f, 3.5f), parkour, mat);
        CreatePart("Parkour_Platform_3", new Vector3(-3.5f, 3.6f, 23f), new Vector3(3.5f, 1f, 3.5f), parkour, mat);
        CreatePart("Parkour_Platform_4", new Vector3(2.5f, 5.0f, 30f), new Vector3(3.5f, 1f, 3.5f), parkour, mat);
        CreatePart("Parkour_Platform_5", new Vector3(0f, 6.2f, 37f), new Vector3(4f, 1f, 4f), parkour, mat);

        CreatePart("Parkour_EndPad", new Vector3(0f, 6.2f, 43f), new Vector3(6f, 1f, 6f), parkour, mat);

        CreatePart("Corridor_2_Floor", new Vector3(0f, 6.2f, 51f), new Vector3(6f, 1f, 10f), parkour, mat);
        CreatePart("Corridor_2_Wall_L", new Vector3(-3.5f, 9.2f, 51f), new Vector3(1f, 5f, 10f), parkour, mat);
        CreatePart("Corridor_2_Wall_R", new Vector3(3.5f, 9.2f, 51f), new Vector3(1f, 5f, 10f), parkour, mat);

        CreatePart("Parkour_Safety_Ramp", new Vector3(-10f, -0.5f, 24f), new Vector3(2f, 1f, 40f), parkour, mat);
    }

    private static void CreateArena(Transform parent, Material mat)
    {
        Transform arena = new GameObject("03_BossArena").transform;
        arena.SetParent(parent);

        CreatePart("Arena_Floor", new Vector3(0f, 6.2f, 78f), new Vector3(36f, 1f, 36f), arena, mat);

        CreatePart("Arena_Wall_South_L", new Vector3(-11f, 11.2f, 60f), new Vector3(14f, 9f, 1f), arena, mat);
        CreatePart("Arena_Wall_South_R", new Vector3(11f, 11.2f, 60f), new Vector3(14f, 9f, 1f), arena, mat);
        CreatePart("Arena_Wall_South_Top", new Vector3(0f, 13.7f, 60f), new Vector3(8f, 4f, 1f), arena, mat);

        CreatePart("Arena_Wall_North", new Vector3(0f, 11.2f, 96f), new Vector3(36f, 9f, 1f), arena, mat);
        CreatePart("Arena_Wall_West", new Vector3(-18f, 11.2f, 78f), new Vector3(1f, 9f, 36f), arena, mat);
        CreatePart("Arena_Wall_East", new Vector3(18f, 11.2f, 78f), new Vector3(1f, 9f, 36f), arena, mat);
    }

    private static void PositionEntities()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(0f, 1f, -22f);
            player.transform.rotation = Quaternion.identity;
        }

        GameObject boss = GameObject.FindWithTag("Boss");
        if (boss != null)
        {
            boss.transform.position = new Vector3(0f, 8f, 78f);
            boss.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        RewardChest chest = Object.FindFirstObjectByType<RewardChest>(FindObjectsInactive.Include);
        if (chest != null)
        {
            chest.transform.position = new Vector3(0f, 7.5f, 92f);
            chest.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
}