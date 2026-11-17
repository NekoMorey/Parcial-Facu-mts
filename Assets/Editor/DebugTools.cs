using UnityEditor;
using UnityEngine;

public static class DebugTools
{
    [MenuItem("Tools/Umbral/Probar derrota")]
    static void ProbarDerrota()
    {
        if (!Application.isPlaying) return;
        var stats = Object.FindFirstObjectByType<PlayerStats>();
        if (stats != null) stats.TakeDamage(stats.maxHP * 2f);
    }

    [MenuItem("Tools/Umbral/Probar golpe")]
    static void ProbarGolpe()
    {
        if (!Application.isPlaying) return;
        var stats = Object.FindFirstObjectByType<PlayerStats>();
        if (stats != null) stats.TakeDamage(10f);
    }

    [MenuItem("Tools/Umbral/Reporte del mapa")]
    static void ReporteMapa()
    {
        var text = new System.Text.StringBuilder();
        foreach (var root in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (root.parent != null) continue;
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                text.AppendLine(root.name + " pos " + Fmt(root.position));
                continue;
            }
            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers) bounds.Encapsulate(r.bounds);
            text.AppendLine(root.name + " centro " + Fmt(bounds.center) + " tam " + Fmt(bounds.size));
        }

        var terrain = Object.FindFirstObjectByType<Terrain>();
        if (terrain != null) text.AppendLine("TERRENO pos " + Fmt(terrain.transform.position) + " tam " + Fmt(terrain.terrainData.size));
        var player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null) text.AppendLine("JUGADOR " + Fmt(player.transform.position));
        var portal = Object.FindFirstObjectByType<Portal>(FindObjectsInactive.Include);
        if (portal != null) text.AppendLine("PORTAL " + Fmt(portal.transform.position) + " destino " + Fmt(portal.destination));
        foreach (var chest in Object.FindObjectsByType<ChestBase>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            text.AppendLine("COFRE FINAL " + Fmt(chest.transform.position));
        Debug.Log(text.ToString());
    }

    static string Fmt(Vector3 v)
    {
        return "(" + Mathf.Round(v.x) + ", " + Mathf.Round(v.y) + ", " + Mathf.Round(v.z) + ")";
    }
}
