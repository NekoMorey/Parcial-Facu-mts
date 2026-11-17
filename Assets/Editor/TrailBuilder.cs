using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class TrailBuilder
{
    const string RootName = "Sendero";
    const float PathSpacing = 2.2f;
    const float CarveCore = 4.5f;
    const float CarveEdge = 9f;

    [MenuItem("Tools/Umbral/12 Salida y senderos")]
    public static void Build()
    {
        BuilderUtils.Remove(RootName);
        var root = new GameObject(RootName).transform;

        var terrain = Object.FindFirstObjectByType<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("Senderos: no encontré el terreno.");
            return;
        }

        Material stone = BuilderUtils.Mat("SenderoPiedra", new Color(0.42f, 0.4f, 0.36f), 0f, 0.1f);
        Material trim = BuilderUtils.Mat("PuestoBorde", new Color(0.2f, 0.21f, 0.24f), 0f, 0.1f);
        Material ember = BuilderUtils.Mat("PuestoBrasa", new Color(1f, 0.55f, 0.2f), 0f, 0.3f, 2.5f);

        Vector3 gate = OpenSouthGate(root, trim, ember);

        var stops = new List<Vector3>();
        var exploration = GameObject.Find("Exploracion");
        if (exploration != null)
        {
            foreach (Transform post in exploration.transform) stops.Add(post.position);
        }

        var route = new List<Vector3> { gate };
        var pending = new List<Vector3>(stops);
        Vector3 current = gate;
        while (pending.Count > 0)
        {
            int best = 0;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < pending.Count; i++)
            {
                float d = Vector2.Distance(new Vector2(current.x, current.z), new Vector2(pending[i].x, pending[i].z));
                if (d < bestDistance) { bestDistance = d; best = i; }
            }
            current = pending[best];
            route.Add(current);
            pending.RemoveAt(best);
        }

        var samples = SamplePolyline(route);
        SmoothHeights(terrain, samples);
        CarveTerrain(terrain, samples);
        Physics.SyncTransforms();
        PlaceSlabs(root, stone, samples);
        PlaceLamps(root, trim, ember, samples);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Senderos listos: " + samples.Count + " tramos desde la salida hasta " + stops.Count + " puestos.");
    }

    static Vector3 OpenSouthGate(Transform root, Material trim, Material glow)
    {
        var colliders = Physics.OverlapBox(new Vector3(0f, 5f, -31f), new Vector3(8f, 6f, 3.5f));
        var removed = new List<string>();
        foreach (var hit in colliders)
        {
            if (hit is TerrainCollider) continue;
            if (hit.bounds.size.y < 3f) continue;
            if (hit.GetComponentInParent<PlayerController>() != null) continue;
            hit.gameObject.SetActive(false);
            removed.Add(hit.name);
        }
        Debug.Log("Salida sur: se abrieron " + removed.Count + " muros (" + string.Join(", ", removed) + ").");

        float floorY = 0f;
        if (Physics.Raycast(new Vector3(0f, 50f, -33f), Vector3.down, out RaycastHit ground, 200f)) floorY = ground.point.y;

        var gate = BuilderUtils.Group("Puerta_Sur", root).transform;
        gate.position = new Vector3(0f, floorY, -32f);
        BuilderUtils.Block("Jamba_Izq", gate, new Vector3(-8f, 4.5f, 0f), new Vector3(1.6f, 9f, 1.6f), trim);
        BuilderUtils.Block("Jamba_Der", gate, new Vector3(8f, 4.5f, 0f), new Vector3(1.6f, 9f, 1.6f), trim);
        BuilderUtils.Block("Dintel", gate, new Vector3(0f, 9.4f, 0f), new Vector3(18f, 1.2f, 1.8f), trim);
        BuilderUtils.Shape(PrimitiveType.Sphere, "Llama_Izq", gate, new Vector3(-8f, 9.9f, 0f), Vector3.one * 0.9f, glow, false);
        BuilderUtils.Shape(PrimitiveType.Sphere, "Llama_Der", gate, new Vector3(8f, 9.9f, 0f), Vector3.one * 0.9f, glow, false);
        BuilderUtils.PointLight("Luz_Puerta", gate, new Vector3(0f, 8f, -2f), new Color(1f, 0.65f, 0.35f), 4f, 20f, LightmapBakeType.Realtime);

        return new Vector3(0f, floorY, -36f);
    }

    static List<Vector3> SamplePolyline(List<Vector3> route)
    {
        var samples = new List<Vector3>();
        for (int i = 0; i < route.Count - 1; i++)
        {
            Vector3 a = route[i];
            Vector3 b = route[i + 1];
            float length = Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
            int steps = Mathf.Max(1, Mathf.CeilToInt(length / PathSpacing));
            for (int s = 0; s < steps; s++)
            {
                float t = s / (float)steps;
                samples.Add(Vector3.Lerp(a, b, t));
            }
        }
        samples.Add(route[route.Count - 1]);
        return samples;
    }

    static void SmoothHeights(Terrain terrain, List<Vector3> samples)
    {
        var raw = new float[samples.Count];
        for (int i = 0; i < samples.Count; i++)
        {
            Vector3 p = samples[i];
            raw[i] = terrain.SampleHeight(p) + terrain.transform.position.y;
            if (i == 0) raw[i] = p.y;
        }

        for (int pass = 0; pass < 12; pass++)
        {
            var next = (float[])raw.Clone();
            for (int i = 1; i < raw.Length - 1; i++) next[i] = (raw[i - 1] + raw[i] + raw[i + 1]) / 3f;
            next[0] = raw[0];
            raw = next;
        }

        for (int i = 0; i < samples.Count; i++)
        {
            var p = samples[i];
            p.y = raw[i];
            samples[i] = p;
        }
    }

    static void CarveTerrain(Terrain terrain, List<Vector3> samples)
    {
        var data = terrain.terrainData;
        int resolution = data.heightmapResolution;
        Vector3 origin = terrain.transform.position;
        float cellX = data.size.x / (resolution - 1);
        float cellZ = data.size.z / (resolution - 1);
        var heights = data.GetHeights(0, 0, resolution, resolution);
        var weight = new float[resolution, resolution];
        var target = new float[resolution, resolution];

        foreach (var s in samples)
        {
            int cx = Mathf.RoundToInt((s.x - origin.x) / cellX);
            int cz = Mathf.RoundToInt((s.z - origin.z) / cellZ);
            int rx = Mathf.CeilToInt(CarveEdge / cellX);
            int rz = Mathf.CeilToInt(CarveEdge / cellZ);
            for (int z = Mathf.Max(0, cz - rz); z <= Mathf.Min(resolution - 1, cz + rz); z++)
            {
                for (int x = Mathf.Max(0, cx - rx); x <= Mathf.Min(resolution - 1, cx + rx); x++)
                {
                    float wx = origin.x + x * cellX;
                    float wz = origin.z + z * cellZ;
                    float d = Vector2.Distance(new Vector2(wx, wz), new Vector2(s.x, s.z));
                    if (d > CarveEdge) continue;
                    float w = d <= CarveCore ? 1f : 1f - Mathf.SmoothStep(0f, 1f, (d - CarveCore) / (CarveEdge - CarveCore));
                    if (w > weight[z, x])
                    {
                        weight[z, x] = w;
                        target[z, x] = (s.y - origin.y) / data.size.y;
                    }
                }
            }
        }

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                if (weight[z, x] <= 0f) continue;
                heights[z, x] = Mathf.Lerp(heights[z, x], Mathf.Clamp01(target[z, x]), weight[z, x]);
            }
        }

        data.SetHeights(0, 0, heights);
        var collider = terrain.GetComponent<TerrainCollider>();
        if (collider != null)
        {
            collider.enabled = false;
            collider.enabled = true;
        }
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
    }

    static void PlaceSlabs(Transform root, Material stone, List<Vector3> samples)
    {
        var slabs = BuilderUtils.Group("Losas", root).transform;
        for (int i = 0; i < samples.Count; i++)
        {
            Vector3 p = samples[i];
            Vector3 forward = i < samples.Count - 1 ? samples[i + 1] - p : p - samples[i - 1];
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.01f) forward = Vector3.forward;

            if (!Physics.Raycast(new Vector3(p.x, 300f, p.z), Vector3.down, out RaycastHit hit, 600f)) continue;
            if (hit.collider.transform.IsChildOf(root)) continue;

            var slab = BuilderUtils.Shape(PrimitiveType.Cube, "Losa", slabs, hit.point + Vector3.up * 0.03f, new Vector3(2.6f, 0.1f, 2f), stone, false);
            slab.transform.rotation = Quaternion.LookRotation(forward.normalized) * Quaternion.Euler(0f, (i % 3 - 1) * 4f, 0f);
            slab.transform.position = hit.point + Vector3.up * 0.03f;
            slab.isStatic = true;
        }
    }

    static void PlaceLamps(Transform root, Material trim, Material glow, List<Vector3> samples)
    {
        var lamps = BuilderUtils.Group("Faroles", root).transform;
        for (int i = 6; i < samples.Count; i += 9)
        {
            Vector3 p = samples[i];
            Vector3 forward = samples[Mathf.Min(i + 1, samples.Count - 1)] - samples[Mathf.Max(i - 1, 0)];
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.01f) continue;
            Vector3 side = Vector3.Cross(Vector3.up, forward.normalized) * 3.6f * (i % 2 == 0 ? 1f : -1f);
            Vector3 spot = p + side;
            if (!Physics.Raycast(new Vector3(spot.x, 300f, spot.z), Vector3.down, out RaycastHit hit, 600f)) continue;
            if (hit.collider.transform.IsChildOf(root)) continue;

            var lamp = BuilderUtils.Group("Farol_" + i, lamps).transform;
            lamp.position = hit.point;
            BuilderUtils.Shape(PrimitiveType.Cylinder, "Poste", lamp, new Vector3(0f, 1.1f, 0f), new Vector3(0.25f, 1.1f, 0.25f), trim, false);
            BuilderUtils.Shape(PrimitiveType.Sphere, "Llama", lamp, new Vector3(0f, 2.4f, 0f), Vector3.one * 0.45f, glow, false);
            BuilderUtils.PointLight("Luz", lamp, new Vector3(0f, 2.6f, 0f), new Color(1f, 0.62f, 0.32f), 2.5f, 11f, LightmapBakeType.Realtime);
        }
    }
}
