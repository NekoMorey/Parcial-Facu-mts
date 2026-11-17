using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class CreadorTerreno
{
    private const string Folder = "Assets/Terreno";
    private const int Resolution = 257;
    private const float Size = 420f;
    private const float Height = 70f;
    private static readonly Vector3 Origin = new Vector3(-204f, -16f, -178f);

    [MenuItem("Tools/Umbral/3 Terreno")]
    public static void Build()
    {
        UtilesConstructor.Remove("Terreno");
        UtilesConstructor.EnsureFolder(Folder);

        string dataPath = Folder + "/UmbralTerreno.asset";
        AssetDatabase.DeleteAsset(dataPath);

        TerrainData data = new TerrainData();
        data.heightmapResolution = Resolution;
        data.size = new Vector3(Size, Height, Size);
        data.SetHeights(0, 0, MakeHeights());
        AssetDatabase.CreateAsset(data, dataPath);

        TerrainLayer ground = MakeLayer("Suelo", MakeGroundTexture(), 14f);
        TerrainLayer rock = MakeLayer("Roca", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Texturas/UmbralPiedra.png"), 22f);
        data.terrainLayers = new[] { ground, rock };
        data.alphamapResolution = 256;
        Paint(data);

        GameObject terrainObject = Terrain.CreateTerrainGameObject(data);
        terrainObject.name = "Terreno";
        terrainObject.transform.position = Origin;

        Terrain terrain = terrainObject.GetComponent<Terrain>();
        RenderPipelineAsset pipeline = GraphicsSettings.currentRenderPipeline;
        if (pipeline != null && pipeline.defaultTerrainMaterial != null)
            terrain.materialTemplate = pipeline.defaultTerrainMaterial;

        ScatterBoulders(terrain);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(terrainObject.scene);
        Debug.Log("Terreno: valle exterior generado.");
    }

    private static float Smooth(float from, float to, float value)
    {
        float t = Mathf.InverseLerp(from, to, value);
        return t * t * (3f - 2f * t);
    }

    private static float DistanceToRuins(float x, float z)
    {
        float dx = Mathf.Max(-18f - x, 0f, x - 32f);
        float dz = Mathf.Max(-36f - z, 0f, z - 101f);
        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    private static float[,] MakeHeights()
    {
        float[,] heights = new float[Resolution, Resolution];

        for (int j = 0; j < Resolution; j++)
        {
            for (int i = 0; i < Resolution; i++)
            {
                float x = Origin.x + i / (Resolution - 1f) * Size;
                float z = Origin.z + j / (Resolution - 1f) * Size;
                float distance = DistanceToRuins(x, z);

                float hills = Smooth(10f, 120f, distance) * 46f;
                float ridges = (Mathf.PerlinNoise(x * 0.018f + 40f, z * 0.018f + 12f) - 0.5f) * 20f * Smooth(0f, 50f, distance);
                float detail = Mathf.PerlinNoise(x * 0.09f, z * 0.09f) * 2.2f * Smooth(0f, 20f, distance);

                heights[j, i] = Mathf.Clamp01((5f + hills + ridges + detail) / Height);
            }
        }

        return heights;
    }

    private static Texture2D MakeGroundTexture()
    {
        string path = "Assets/Texturas/UmbralSuelo.png";
        Texture2D texture = new Texture2D(256, 256, TextureFormat.RGB24, false);

        for (int y = 0; y < 256; y++)
        {
            for (int x = 0; x < 256; x++)
            {
                float broad = Mathf.PerlinNoise(x * 0.03f, y * 0.03f);
                float fine = Mathf.PerlinNoise(x * 0.25f + 9f, y * 0.25f + 3f);
                float shade = 0.55f + broad * 0.3f + fine * 0.15f;
                texture.SetPixel(x, y, new Color(0.16f * shade, 0.2f * shade, 0.17f * shade));
            }
        }

        texture.Apply();
        File.WriteAllBytes(path, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static TerrainLayer MakeLayer(string name, Texture2D texture, float tile)
    {
        string path = Folder + "/" + name + ".terrainlayer";
        AssetDatabase.DeleteAsset(path);

        TerrainLayer layer = new TerrainLayer();
        layer.diffuseTexture = texture;
        layer.tileSize = new Vector2(tile, tile);
        AssetDatabase.CreateAsset(layer, path);
        return layer;
    }

    private static void Paint(TerrainData data)
    {
        int size = data.alphamapResolution;
        float[,,] weights = new float[size, size, 2];

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                float steepness = data.GetSteepness(i / (size - 1f), j / (size - 1f));
                float rock = Mathf.InverseLerp(16f, 34f, steepness);
                weights[j, i, 0] = 1f - rock;
                weights[j, i, 1] = rock;
            }
        }

        data.SetAlphamaps(0, 0, weights);
    }

    private static void ScatterBoulders(Terrain terrain)
    {
        Material stone = UtilesConstructor.Mat("Umbral_Piedra");
        Transform parent = UtilesConstructor.Group("Rocas", terrain.transform).transform;
        System.Random random = new System.Random(7);
        int placed = 0;

        while (placed < 30)
        {
            float x = Mathf.Lerp(-80f, 95f, (float)random.NextDouble());
            float z = Mathf.Lerp(-95f, 160f, (float)random.NextDouble());
            float distance = DistanceToRuins(x, z);
            if (distance < 6f || distance > 70f) continue;

            Vector3 position = new Vector3(x, 0f, z);
            position.y = terrain.SampleHeight(position) + Origin.y;

            Vector3 scale = new Vector3(
                Mathf.Lerp(2f, 6f, (float)random.NextDouble()),
                Mathf.Lerp(1.5f, 4.5f, (float)random.NextDouble()),
                Mathf.Lerp(2f, 6f, (float)random.NextDouble()));

            GameObject boulder = UtilesConstructor.Shape(PrimitiveType.Sphere, "Roca_" + placed, null, position, scale, stone, true);
            boulder.transform.rotation = Quaternion.Euler(0f, (float)random.NextDouble() * 360f, (float)random.NextDouble() * 20f);
            boulder.transform.SetParent(parent, true);
            placed++;
        }
    }
}
