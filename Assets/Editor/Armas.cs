using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Armas
{
    [MenuItem("Tools/Umbral/13 Armas")]
    public static void Build()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Armas: no hay un objeto con tag Player.");
            return;
        }

        Transform armL = player.transform.Find("Modelo/Cuerpo/Brazo_Izq");
        Transform armR = player.transform.Find("Modelo/Cuerpo/Brazo_Der");
        if (armL == null || armR == null)
        {
            Debug.LogWarning("Armas: primero ejecutá 2 Personaje.");
            return;
        }

        Material steel = UtilesConstructor.Mat("ArmaAcero", new Color(0.62f, 0.64f, 0.68f), 0.9f, 0.7f);
        Material dark = UtilesConstructor.Mat("ArmaOscura", new Color(0.12f, 0.12f, 0.14f), 0.5f, 0.4f);
        Material brass = UtilesConstructor.Mat("ArmaLaton", new Color(0.78f, 0.6f, 0.25f), 0.8f, 0.5f);
        Material glow = UtilesConstructor.Mat("ArmaBrillo", new Color(0.35f, 0.9f, 1f), 0f, 0.8f, 4f);

        Transform oldGun = armL.Find("Pistola");
        if (oldGun != null) Object.DestroyImmediate(oldGun.gameObject);
        Transform oldDagger = armR.Find("Daga");
        if (oldDagger != null) Object.DestroyImmediate(oldDagger.gameObject);

        float hand = -0.62f;

        Transform gun = UtilesConstructor.Group("Pistola", armL).transform;
        gun.localPosition = new Vector3(0f, hand, 0f);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Cuerpo", gun, new Vector3(0f, -0.16f, 0f), new Vector3(0.1f, 0.34f, 0.14f), dark, false);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Canon", gun, new Vector3(0f, -0.4f, 0f), new Vector3(0.06f, 0.2f, 0.06f), steel, false);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Empunadura", gun, new Vector3(0f, 0.02f, 0.1f), new Vector3(0.09f, 0.14f, 0.1f), brass, false);
        UtilesConstructor.Shape(PrimitiveType.Sphere, "Boca", gun, new Vector3(0f, -0.51f, 0f), Vector3.one * 0.07f, glow, false);

        Transform dagger = UtilesConstructor.Group("Daga", armR).transform;
        dagger.localPosition = new Vector3(0f, hand, 0f);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Mango", dagger, new Vector3(0f, -0.05f, 0f), new Vector3(0.07f, 0.16f, 0.07f), brass, false);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Guarda", dagger, new Vector3(0f, -0.15f, 0f), new Vector3(0.07f, 0.04f, 0.26f), dark, false);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Hoja", dagger, new Vector3(0f, -0.42f, 0f), new Vector3(0.05f, 0.5f, 0.14f), steel, false);
        UtilesConstructor.Shape(PrimitiveType.Cube, "Filo", dagger, new Vector3(0f, -0.42f, 0f), new Vector3(0.055f, 0.48f, 0.03f), glow, false);

        GameObject bullet = MakeBullet(glow);

        var controller = player.GetComponent<ControlJugador>();
        if (controller != null)
        {
            controller.rockPrefab = bullet;
            controller.rockForce = 38f;
            EditorUtility.SetDirty(controller);
        }

        if (player.GetComponent<ArmasJugador>() == null) player.AddComponent<ArmasJugador>();

        EditorSceneManager.MarkSceneDirty(player.scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Armas listas: pistola en la mano izquierda, daga en la derecha y bala nueva.");
    }

    static GameObject MakeBullet(Material glow)
    {
        GameObject root = new GameObject("Bala");
        UtilesConstructor.Shape(PrimitiveType.Sphere, "Nucleo", root.transform, Vector3.zero, Vector3.one * 0.26f, glow, false);

        var collider = root.AddComponent<SphereCollider>();
        collider.radius = 0.14f;

        var body = root.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var trail = root.AddComponent<TrailRenderer>();
        trail.time = 0.22f;
        trail.startWidth = 0.16f;
        trail.endWidth = 0f;
        trail.sharedMaterial = UtilesConstructor.ParticleMaterial();
        trail.startColor = new Color(0.4f, 0.95f, 1f, 1f);
        trail.endColor = new Color(0.1f, 0.4f, 1f, 0f);

        UtilesConstructor.PointLight("Resplandor", root.transform, Vector3.zero, new Color(0.4f, 0.9f, 1f), 2f, 5f, LightmapBakeType.Realtime);

        var rock = root.AddComponent<Proyectil>();
        rock.lifeTime = 3f;

        return UtilesConstructor.SavePrefab(root, "Bala");
    }
}
