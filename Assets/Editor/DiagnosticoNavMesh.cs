using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static class DiagnosticoNavMesh
{
    [MenuItem("Tools/Umbral/Diagnostico NavMesh")]
    static void Revisar()
    {
        var datos = NavMesh.CalculateTriangulation();
        Debug.Log("NavMesh triangulos: " + datos.indices.Length / 3 + " vertices: " + datos.vertices.Length);
        foreach (var agente in Object.FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None))
        {
            Vector3 pos = agente.transform.position;
            bool hay = NavMesh.SamplePosition(pos, out NavMeshHit golpe, 50f, NavMesh.AllAreas);
            Debug.Log(agente.name + " pos " + pos + " baseOffset " + agente.baseOffset + " enNavMesh " + agente.isOnNavMesh + " cercano " + (hay ? golpe.position + " dist " + golpe.distance : "NINGUNO") + " agentType " + agente.agentTypeID);
        }
        foreach (var portal in Object.FindObjectsByType<Portal>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            bool okDestino = NavMesh.SamplePosition(portal.destination, out NavMeshHit pd, 50f, NavMesh.AllAreas);
            Debug.Log("Portal " + portal.name + " destino " + portal.destination + " cercano " + (okDestino ? pd.position + " dist " + pd.distance : "NINGUNO"));
            foreach (var agente in Object.FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None))
            {
                NavMesh.SamplePosition(agente.transform.position, out NavMeshHit pa, 50f, NavMesh.AllAreas);
                var caminoPortal = new NavMeshPath();
                NavMesh.CalculatePath(pa.position, pd.position, NavMesh.AllAreas, caminoPortal);
                Debug.Log(agente.name + " camino al destino del portal: " + caminoPortal.status);
            }
        }
        var jugador = GameObject.FindWithTag("Player");
        if (jugador != null)
        {
            bool hay = NavMesh.SamplePosition(jugador.transform.position, out NavMeshHit golpe, 50f, NavMesh.AllAreas);
            var camino = new NavMeshPath();
            NavMesh.SamplePosition(jugador.transform.position, out NavMeshHit pj, 50f, NavMesh.AllAreas);
            foreach (var agente in Object.FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None))
            {
                NavMesh.SamplePosition(agente.transform.position, out NavMeshHit pa, 50f, NavMesh.AllAreas);
                NavMesh.CalculatePath(pa.position, pj.position, NavMesh.AllAreas, camino);
                Debug.Log(agente.name + " camino al jugador: " + camino.status + " esquinas " + camino.corners.Length + " ultima " + (camino.corners.Length > 0 ? camino.corners[camino.corners.Length - 1].ToString() : "-"));
            }
            Debug.Log("Jugador pos " + jugador.transform.position + " cercano " + (hay ? golpe.position + " dist " + golpe.distance : "NINGUNO"));
        }
    }
}
