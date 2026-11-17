using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public static class JefeCaballero
{
    private const string Folder = "Assets/Animaciones/Jefe";
    private const string Body = "Modelo/Cuerpo";
    private const string ArmL = "Modelo/Cuerpo/Brazo_Izq";
    private const string ArmR = "Modelo/Cuerpo/Brazo_Der";
    private const string LegL = "Modelo/Cuerpo/Pierna_Izq";
    private const string LegR = "Modelo/Cuerpo/Pierna_Der";

    [MenuItem("Tools/Umbral/7 Jefe caballero")]
    public static void Build()
    {
        GameObject boss = GameObject.FindWithTag("Boss");
        if (boss == null)
        {
            Debug.LogWarning("No hay un objeto con tag Boss en la escena.");
            return;
        }

        UtilesConstructor.Unpack(boss);

        Transform previous = boss.transform.Find("Modelo");
        if (previous != null) Object.DestroyImmediate(previous.gameObject);

        MeshRenderer capsule = boss.GetComponent<MeshRenderer>();
        if (capsule != null) capsule.enabled = false;

        Material armor = UtilesConstructor.Mat("Umbral_Caballero_Armadura", new Color(0.19f, 0.2f, 0.24f), 0.9f, 0.62f);
        Material trim = UtilesConstructor.Mat("Umbral_Caballero_Borde", new Color(0.5f, 0.38f, 0.16f), 0.85f, 0.55f);
        Material cloth = UtilesConstructor.Mat("Umbral_Caballero_Capa", new Color(0.4f, 0.06f, 0.06f), 0f, 0.2f);
        Material steel = UtilesConstructor.Mat("Umbral_Caballero_Hoja", new Color(0.7f, 0.72f, 0.78f), 1f, 0.8f);
        Material eyes = UtilesConstructor.Mat("Umbral_Caballero_Ojos", new Color(1f, 0.3f, 0.1f), 0f, 0.4f, 3f);

        Transform model = UtilesConstructor.Group("Modelo", boss.transform).transform;
        Transform body = UtilesConstructor.Group("Cuerpo", model).transform;

        Piece("Peto", body, new Vector3(0f, 0.22f, 0f), new Vector3(0.62f, 0.56f, 0.4f), armor);
        Piece("Pecho_Emblema", body, new Vector3(0f, 0.28f, 0.21f), new Vector3(0.18f, 0.3f, 0.03f), trim);
        Piece("Cintura", body, new Vector3(0f, -0.14f, 0f), new Vector3(0.52f, 0.2f, 0.34f), trim);
        Piece("Faldar_Izq", body, new Vector3(-0.2f, -0.32f, 0.02f), new Vector3(0.24f, 0.26f, 0.3f), armor);
        Piece("Faldar_Der", body, new Vector3(0.2f, -0.32f, 0.02f), new Vector3(0.24f, 0.26f, 0.3f), armor);

        Piece("Casco", body, new Vector3(0f, 0.72f, 0f), new Vector3(0.36f, 0.38f, 0.38f), armor);
        Piece("Visera", body, new Vector3(0f, 0.75f, 0.195f), new Vector3(0.26f, 0.05f, 0.02f), eyes);
        Piece("Casco_Cresta", body, new Vector3(0f, 0.97f, -0.03f), new Vector3(0.06f, 0.2f, 0.34f), cloth);
        Piece("Cuerno_Izq", body, new Vector3(-0.22f, 0.86f, 0f), new Vector3(0.07f, 0.22f, 0.07f), trim).transform.localRotation = Quaternion.Euler(0f, 0f, 30f);
        Piece("Cuerno_Der", body, new Vector3(0.22f, 0.86f, 0f), new Vector3(0.07f, 0.22f, 0.07f), trim).transform.localRotation = Quaternion.Euler(0f, 0f, -30f);

        UtilesConstructor.Shape(PrimitiveType.Sphere, "Hombrera_Izq", body, new Vector3(-0.43f, 0.5f, 0f), new Vector3(0.36f, 0.28f, 0.4f), trim, false);
        UtilesConstructor.Shape(PrimitiveType.Sphere, "Hombrera_Der", body, new Vector3(0.43f, 0.5f, 0f), new Vector3(0.36f, 0.28f, 0.4f), trim, false);

        Piece("Capa", body, new Vector3(0f, 0.02f, -0.25f), new Vector3(0.58f, 0.95f, 0.04f), cloth).transform.localRotation = Quaternion.Euler(-7f, 0f, 0f);

        Transform armL = Joint("Brazo_Izq", body, new Vector3(-0.44f, 0.44f, 0f));
        Piece("Brazo_Izq_Malla", armL, new Vector3(0f, -0.28f, 0f), new Vector3(0.17f, 0.56f, 0.17f), armor);
        Piece("Guantelete_Izq", armL, new Vector3(0f, -0.5f, 0f), new Vector3(0.2f, 0.16f, 0.2f), trim);
        Piece("Escudo", armL, new Vector3(-0.13f, -0.36f, 0.06f), new Vector3(0.06f, 0.66f, 0.5f), armor);
        Piece("Escudo_Emblema", armL, new Vector3(-0.17f, -0.36f, 0.06f), new Vector3(0.02f, 0.3f, 0.2f), cloth);

        Transform armR = Joint("Brazo_Der", body, new Vector3(0.44f, 0.44f, 0f));
        Piece("Brazo_Der_Malla", armR, new Vector3(0f, -0.28f, 0f), new Vector3(0.17f, 0.56f, 0.17f), armor);
        Piece("Guantelete_Der", armR, new Vector3(0f, -0.5f, 0f), new Vector3(0.2f, 0.16f, 0.2f), trim);
        Piece("Espada_Empunadura", armR, new Vector3(0f, -0.54f, 0.06f), new Vector3(0.05f, 0.05f, 0.26f), cloth);
        Piece("Espada_Guarda", armR, new Vector3(0f, -0.54f, 0.2f), new Vector3(0.34f, 0.06f, 0.05f), trim);
        Piece("Espada_Hoja", armR, new Vector3(0f, -0.54f, 0.85f), new Vector3(0.1f, 0.03f, 1.25f), steel);

        Transform legL = Joint("Pierna_Izq", body, new Vector3(-0.16f, -0.25f, 0f));
        Piece("Pierna_Izq_Malla", legL, new Vector3(0f, -0.36f, 0f), new Vector3(0.21f, 0.72f, 0.23f), armor);
        Piece("Bota_Izq", legL, new Vector3(0f, -0.69f, 0.04f), new Vector3(0.24f, 0.12f, 0.34f), trim);

        Transform legR = Joint("Pierna_Der", body, new Vector3(0.16f, -0.25f, 0f));
        Piece("Pierna_Der_Malla", legR, new Vector3(0f, -0.36f, 0f), new Vector3(0.21f, 0.72f, 0.23f), armor);
        Piece("Bota_Der", legR, new Vector3(0f, -0.69f, 0.04f), new Vector3(0.24f, 0.12f, 0.34f), trim);

        Animator animator = boss.GetComponent<Animator>();
        if (animator == null) animator = boss.AddComponent<Animator>();
        animator.runtimeAnimatorController = BuildController();
        animator.applyRootMotion = false;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        boss.transform.localScale = Vector3.one * 2.5f;

        NavMeshAgent agent = boss.GetComponent<NavMeshAgent>();
        if (agent != null) agent.baseOffset = 1f;

        Physics.SyncTransforms();
        if (Physics.Raycast(boss.transform.position, Vector3.down, out RaycastHit ground, 20f, ~0, QueryTriggerInteraction.Ignore))
            boss.transform.position = ground.point + Vector3.up * boss.transform.lossyScale.y;

        UtilesConstructor.EnsureFolder(UtilesConstructor.PrefabFolder);
        PrefabUtility.SaveAsPrefabAssetAndConnect(boss, UtilesConstructor.PrefabFolder + "/Jefe.prefab", InteractionMode.AutomatedAction);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(boss.scene);
        Debug.Log("Jefe: caballero armado y animaciones listas.");
    }

    private static GameObject Piece(string name, Transform parent, Vector3 position, Vector3 size, Material material)
    {
        return UtilesConstructor.Shape(PrimitiveType.Cube, name, parent, position, size, material, false);
    }

    private static Transform Joint(string name, Transform parent, Vector3 position)
    {
        Transform joint = UtilesConstructor.Group(name, parent).transform;
        joint.localPosition = position;
        return joint;
    }

    private static AnimatorController BuildController()
    {
        AnimationClip idle = EsqueletoJugador.MakeClip(Folder, "Caballero_Reposo", true, new Dictionary<string, Keyframe[]>
        {
            { ArmR, EsqueletoJugador.Keys(0f, 0f, 1.2f, -7f, 2.4f, 0f) },
            { ArmL, EsqueletoJugador.Keys(0f, 0f, 1.2f, 5f, 2.4f, 0f) },
            { Body, EsqueletoJugador.Keys(0f, 0f, 1.2f, 2f, 2.4f, 0f) }
        });

        AnimationClip walk = EsqueletoJugador.MakeClip(Folder, "Caballero_Caminar", true, new Dictionary<string, Keyframe[]>
        {
            { LegL, EsqueletoJugador.Keys(0f, 28f, 0.45f, -28f, 0.9f, 28f) },
            { LegR, EsqueletoJugador.Keys(0f, -28f, 0.45f, 28f, 0.9f, -28f) },
            { ArmL, EsqueletoJugador.Keys(0f, -14f, 0.45f, 14f, 0.9f, -14f) },
            { ArmR, EsqueletoJugador.Keys(0f, 14f, 0.45f, -14f, 0.9f, 14f) },
            { Body, EsqueletoJugador.Keys(0f, 5f, 0.9f, 5f) }
        });

        AnimationClip attack = EsqueletoJugador.MakeClip(Folder, "Caballero_Ataque", false, new Dictionary<string, Keyframe[]>
        {
            { ArmR, EsqueletoJugador.Keys(0f, 0f, 0.15f, -165f, 0.3f, 50f, 0.55f, 0f) },
            { ArmL, EsqueletoJugador.Keys(0f, 0f, 0.15f, 25f, 0.3f, -20f, 0.55f, 0f) },
            { Body, EsqueletoJugador.Keys(0f, 0f, 0.15f, -10f, 0.3f, 18f, 0.55f, 0f) }
        });

        AnimationClip charge = EsqueletoJugador.MakeClip(Folder, "Caballero_Embestida", false, new Dictionary<string, Keyframe[]>
        {
            { Body, EsqueletoJugador.Keys(0f, 0f, 0.3f, 24f, 1.1f, 24f, 1.3f, 0f) },
            { ArmL, EsqueletoJugador.Keys(0f, 0f, 0.3f, -75f, 1.1f, -75f, 1.3f, 0f) },
            { ArmR, EsqueletoJugador.Keys(0f, 0f, 0.3f, 45f, 1.1f, 45f, 1.3f, 0f) },
            { LegL, EsqueletoJugador.Keys(0f, 0f, 0.3f, 0f, 0.5f, 38f, 0.7f, -38f, 0.9f, 38f, 1.1f, -38f, 1.3f, 0f) },
            { LegR, EsqueletoJugador.Keys(0f, 0f, 0.3f, 0f, 0.5f, -38f, 0.7f, 38f, 0.9f, -38f, 1.1f, 38f, 1.3f, 0f) }
        });

        AnimationClip slam = EsqueletoJugador.MakeClip(Folder, "Caballero_Aplastar", false, new Dictionary<string, Keyframe[]>
        {
            { ArmR, EsqueletoJugador.Keys(0f, 0f, 0.7f, -170f, 0.9f, -170f, 1.05f, 35f, 1.4f, 0f) },
            { ArmL, EsqueletoJugador.Keys(0f, 0f, 0.7f, -160f, 0.9f, -160f, 1.05f, 30f, 1.4f, 0f) },
            { Body, EsqueletoJugador.Keys(0f, 0f, 0.7f, -14f, 0.9f, -14f, 1.05f, 26f, 1.4f, 0f) }
        });

        string path = Folder + "/Caballero.controller";
        AssetDatabase.DeleteAsset(path);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);

        controller.AddParameter("Velocidad", AnimatorControllerParameterType.Float);
        controller.AddParameter("Ataque", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Embestida", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Aplastar", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine machine = controller.layers[0].stateMachine;
        AnimatorState idleState = machine.AddState("Reposo");
        AnimatorState walkState = machine.AddState("Caminar");
        AnimatorState attackState = machine.AddState("Ataque");
        AnimatorState chargeState = machine.AddState("Embestida");
        AnimatorState slamState = machine.AddState("Aplastar");

        idleState.motion = idle;
        walkState.motion = walk;
        attackState.motion = attack;
        chargeState.motion = charge;
        slamState.motion = slam;
        machine.defaultState = idleState;

        EsqueletoJugador.Link(idleState, walkState).AddCondition(AnimatorConditionMode.Greater, 0.3f, "Velocidad");
        EsqueletoJugador.Link(walkState, idleState).AddCondition(AnimatorConditionMode.Less, 0.3f, "Velocidad");

        foreach (AnimatorState state in new[] { idleState, walkState })
        {
            EsqueletoJugador.Link(state, attackState).AddCondition(AnimatorConditionMode.If, 0f, "Ataque");
            EsqueletoJugador.Link(state, chargeState).AddCondition(AnimatorConditionMode.If, 0f, "Embestida");
            EsqueletoJugador.Link(state, slamState).AddCondition(AnimatorConditionMode.If, 0f, "Aplastar");
        }

        EsqueletoJugador.Return(attackState, idleState);
        EsqueletoJugador.Return(chargeState, idleState);
        EsqueletoJugador.Return(slamState, idleState);

        EditorUtility.SetDirty(controller);
        return controller;
    }
}
