using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

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

        var apagados = new List<GameObject>();
        foreach (var agente in Object.FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None))
        {
            if (agente.gameObject.activeSelf) apagados.Add(agente.gameObject);
        }
        foreach (var puerta in Object.FindObjectsByType<PuertaCerrada>(FindObjectsSortMode.None))
        {
            if (!apagados.Contains(puerta.gameObject)) apagados.Add(puerta.gameObject);
        }
        var jugador = GameObject.FindWithTag("Player");
        if (jugador != null && !apagados.Contains(jugador)) apagados.Add(jugador);

        foreach (var objeto in apagados) objeto.SetActive(false);

        foreach (var surface in surfaces)
        {
            surface.BuildNavMesh();
            EditorUtility.SetDirty(surface);
            Debug.Log("Navegacion: se rehizo " + surface.name + " (agentes " + surface.agentTypeID + ", capas " + surface.collectObjects + ").");
        }

        foreach (var objeto in apagados) objeto.SetActive(true);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
    }
}
