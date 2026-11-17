using UnityEditor;
using UnityEngine;

public static class HerramientasDepuracion
{
    [MenuItem("Tools/Umbral/Probar derrota")]
    static void ProbarDerrota()
    {
        if (!Application.isPlaying) return;
        var stats = Object.FindFirstObjectByType<EstadisticasJugador>();
        if (stats != null) stats.TakeDamage(stats.maxHP * 2f);
    }

    [MenuItem("Tools/Umbral/Probar golpe")]
    static void ProbarGolpe()
    {
        if (!Application.isPlaying) return;
        var stats = Object.FindFirstObjectByType<EstadisticasJugador>();
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
        var player = Object.FindFirstObjectByType<ControlJugador>();
        if (player != null) text.AppendLine("JUGADOR " + Fmt(player.transform.position));
        var portal = Object.FindFirstObjectByType<Portal>(FindObjectsInactive.Include);
        if (portal != null) text.AppendLine("PORTAL " + Fmt(portal.transform.position) + " destino " + Fmt(portal.destination));
        foreach (var chest in Object.FindObjectsByType<CofreBase>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            text.AppendLine("COFRE FINAL " + Fmt(chest.transform.position));
        Debug.Log(text.ToString());
    }

    static string Fmt(Vector3 v)
    {
        return "(" + Mathf.Round(v.x) + ", " + Mathf.Round(v.y) + ", " + Mathf.Round(v.z) + ")";
    }

    [MenuItem("Tools/Umbral/Reporte de cofres")]
    static void ChestReport()
    {
        var terrain = Object.FindFirstObjectByType<Terrain>();
        var lines = new System.Text.StringBuilder("Cofres:\n");
        foreach (var chest in Object.FindObjectsByType<CofreMejora>(FindObjectsSortMode.None))
        {
            Vector3 p = chest.transform.position;
            float ground = terrain.SampleHeight(p) + terrain.transform.position.y;
            lines.AppendLine(chest.name + " pos " + p.ToString("F1") + " terreno " + ground.ToString("F1") + " diferencia " + (p.y - ground).ToString("F1") + " escala " + chest.transform.lossyScale.ToString("F2"));
        }
        foreach (var chest in Object.FindObjectsByType<CofreBase>(FindObjectsSortMode.None))
        {
            Vector3 p = chest.transform.position;
            float ground = terrain.SampleHeight(p) + terrain.transform.position.y;
            lines.AppendLine(chest.name + " (jefe) pos " + p.ToString("F1") + " terreno " + ground.ToString("F1") + " activo " + chest.gameObject.activeInHierarchy);
        }
        Debug.Log(lines.ToString());
    }

    [MenuItem("Tools/Umbral/Limpiar scripts perdidos")]
    static void CleanMissing()
    {
        var names = new System.Text.StringBuilder("Scripts perdidos quitados:\n");
        int total = 0;
        foreach (var go in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go.gameObject);
            if (count == 0) continue;
            string path = go.name;
            for (var up = go.parent; up != null; up = up.parent) path = up.name + "/" + path;
            names.AppendLine(path + " x" + count + (PrefabUtility.IsPartOfPrefabInstance(go) ? " (prefab)" : ""));
            if (!PrefabUtility.IsPartOfPrefabInstance(go)) GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go.gameObject);
            total += count;
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log(names + "Total: " + total);
    }
}
