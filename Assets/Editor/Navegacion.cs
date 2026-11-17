using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Navegacion
{
    [MenuItem("Tools/Umbral/14 Rehacer navegacion")]
    public static void Build()
    {
        var surfaces = Object.FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);
        if (surfaces.Length == 0)
        {
            Debug.LogWarning("Navegacion: no hay ningun NavMeshSurface en la escena.");
            return;
        }

        foreach (var surface in surfaces)
        {
            surface.BuildNavMesh();
            EditorUtility.SetDirty(surface);
            Debug.Log("Navegacion: se rehizo " + surface.name + " (agentes " + surface.agentTypeID + ", capas " + surface.collectObjects + ").");
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
    }
}
