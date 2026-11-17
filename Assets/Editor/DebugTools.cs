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
}
