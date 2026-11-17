using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ListarNombres
{
    [MenuItem("Tools/Umbral/Listar nombres")]
    public static void Listar()
    {
        var conteo = new SortedDictionary<string, int>();
        foreach (var go in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            string n = go.name;
            if (!conteo.ContainsKey(n)) conteo[n] = 0;
            conteo[n]++;
        }
        var sb = new StringBuilder();
        sb.AppendLine("== ESCENA ==");
        foreach (var par in conteo) sb.AppendLine(par.Key + "\t" + par.Value);
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" }))
        {
            string ruta = AssetDatabase.GUIDToAssetPath(guid);
            var raiz = PrefabUtility.LoadPrefabContents(ruta);
            sb.AppendLine("== " + ruta + " ==");
            var vistos = new HashSet<string>();
            foreach (var t in raiz.GetComponentsInChildren<Transform>(true))
                if (vistos.Add(t.name)) sb.AppendLine(t.name);
            PrefabUtility.UnloadPrefabContents(raiz);
        }
        File.WriteAllText("Assets/../nombres.txt", sb.ToString());
        Debug.Log("Nombres listados: " + conteo.Count);
    }
}
