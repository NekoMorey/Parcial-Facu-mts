using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public static class BossKnight
{
    private const string Folder = "Assets/Animations/Boss";
    private const string Body = "Modelo/Cuerpo";
    private const string ArmL = "Modelo/Cuerpo/Arm_L";
    private const string ArmR = "Modelo/Cuerpo/Arm_R";
    private const string LegL = "Modelo/Cuerpo/Leg_L";
    private const string LegR = "Modelo/Cuerpo/Leg_R";

    [MenuItem("Tools/Umbral/7 Jefe caballero")]
    public static void Build()
    {
        GameObject boss = GameObject.FindWithTag("Boss");
        if (boss == null)
        {
            Debug.LogWarning("No hay un objeto con tag Boss en la escena.");
            return;
        }

        BuilderUtils.Unpack(boss);

        Transform previous = boss.transform.Find("Modelo");
        if (previous != null) Object.DestroyImmediate(previous.gameObject);

        MeshRenderer capsule = boss.GetComponent<MeshRenderer>();
        if (capsule != null) capsule.enabled = false;

        Material armor = BuilderUtils.Mat("Umbral_Knight_Armor", new Color(0.19f, 0.2f, 0.24f), 0.9f, 0.62f);
        Material trim = BuilderUtils.Mat("Umbral_Knight_Trim", new Color(0.5f, 0.38f, 0.16f), 0.85f, 0.55f);
        Material cloth = BuilderUtils.Mat("Umbral_Knight_Cape", new Color(0.4f, 0.06f, 0.06f), 0f, 0.2f);
        Material steel = BuilderUtils.Mat("Umbral_Knight_Blade", new Color(0.7f, 0.72f, 0.78f), 1f, 0.8f);
        Material eyes = BuilderUtils.Mat("Umbral_Knight_Eyes", new Color(1f, 0.3f, 0.1f), 0f, 0.4f, 3f);

        Transform model = BuilderUtils.Group("Modelo", boss.transform).transform;
        Transform body = BuilderUtils.Group("Cuerpo", model).transform;

        Piece("Chestplate", body, new Vector3(0f, 0.22f, 0f), new Vector3(0.62f, 0.56f, 0.4f), armor);
        Piece("Chest_Emblem", body, new Vector3(0f, 0.28f, 0.21f), new Vector3(0.18f, 0.3f, 0.03f), trim);
        Piece("Waist", body, new Vector3(0f, -0.14f, 0f), new Vector3(0.52f, 0.2f, 0.34f), trim);
        Piece("Tasset_L", body, new Vector3(-0.2f, -0.32f, 0.02f), new Vector3(0.24f, 0.26f, 0.3f), armor);
        Piece("Tasset_R", body, new Vector3(0.2f, -0.32f, 0.02f), new Vector3(0.24f, 0.26f, 0.3f), armor);

        Piece("Helmet", body, new Vector3(0f, 0.72f, 0f), new Vector3(0.36f, 0.38f, 0.38f), armor);
        Piece("Visor", body, new Vector3(0f, 0.75f, 0.195f), new Vector3(0.26f, 0.05f, 0.02f), eyes);
        Piece("Helmet_Crest", body, new Vector3(0f, 0.97f, -0.03f), new Vector3(0.06f, 0.2f, 0.34f), cloth);
        Piece("Horn_L", body, new Vector3(-0.22f, 0.86f, 0f), new Vector3(0.07f, 0.22f, 0.07f), trim).transform.localRotation = Quaternion.Euler(0f, 0f, 30f);
        Piece("Horn_R", body, new Vector3(0.22f, 0.86f, 0f), new Vector3(0.07f, 0.22f, 0.07f), trim).transform.localRotation = Quaternion.Euler(0f, 0f, -30f);

        BuilderUtils.Shape(PrimitiveType.Sphere, "Pauldron_L", body, new Vector3(-0.43f, 0.5f, 0f), new Vector3(0.36f, 0.28f, 0.4f), trim, false);
        BuilderUtils.Shape(PrimitiveType.Sphere, "Pauldron_R", body, new Vector3(0.43f, 0.5f, 0f), new Vector3(0.36f, 0.28f, 0.4f), trim, false);

        Piece("Cape", body, new Vector3(0f, 0.02f, -0.25f), new Vector3(0.58f, 0.95f, 0.04f), cloth).transform.localRotation = Quaternion.Euler(-7f, 0f, 0f);

        Transform armL = Joint("Arm_L", body, new Vector3(-0.44f, 0.44f, 0f));
        Piece("Arm_L_Mesh", armL, new Vector3(0f, -0.28f, 0f), new Vector3(0.17f, 0.56f, 0.17f), armor);
        Piece("Gauntlet_L", armL, new Vector3(0f, -0.5f, 0f), new Vector3(0.2f, 0.16f, 0.2f), trim);
        Piece("Shield", armL, new Vector3(-0.13f, -0.36f, 0.06f), new Vector3(0.06f, 0.66f, 0.5f), armor);
        Piece("Shield_Emblem", armL, new Vector3(-0.17f, -0.36f, 0.06f), new Vector3(0.02f, 0.3f, 0.2f), cloth);

        Transform armR = Joint("Arm_R", body, new Vector3(0.44f, 0.44f, 0f));
        Piece("Arm_R_Mesh", armR, new Vector3(0f, -0.28f, 0f), new Vector3(0.17f, 0.56f, 0.17f), armor);
        Piece("Gauntlet_R", armR, new Vector3(0f, -0.5f, 0f), new Vector3(0.2f, 0.16f, 0.2f), trim);
        Piece("Sword_Grip", armR, new Vector3(0f, -0.54f, 0.06f), new Vector3(0.05f, 0.05f, 0.26f), cloth);
        Piece("Sword_Guard", armR, new Vector3(0f, -0.54f, 0.2f), new Vector3(0.34f, 0.06f, 0.05f), trim);
        Piece("Sword_Blade", armR, new Vector3(0f, -0.54f, 0.85f), new Vector3(0.1f, 0.03f, 1.25f), steel);

        Transform legL = Joint("Leg_L", body, new Vector3(-0.16f, -0.25f, 0f));
        Piece("Leg_L_Mesh", legL, new Vector3(0f, -0.36f, 0f), new Vector3(0.21f, 0.72f, 0.23f), armor);
        Piece("Boot_L", legL, new Vector3(0f, -0.69f, 0.04f), new Vector3(0.24f, 0.12f, 0.34f), trim);

        Transform legR = Joint("Leg_R", body, new Vector3(0.16f, -0.25f, 0f));
        Piece("Leg_R_Mesh", legR, new Vector3(0f, -0.36f, 0f), new Vector3(0.21f, 0.72f, 0.23f), armor);
        Piece("Boot_R", legR, new Vector3(0f, -0.69f, 0.04f), new Vector3(0.24f, 0.12f, 0.34f), trim);

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

        BuilderUtils.EnsureFolder(BuilderUtils.PrefabFolder);
        PrefabUtility.SaveAsPrefabAssetAndConnect(boss, BuilderUtils.PrefabFolder + "/Boss.prefab", InteractionMode.AutomatedAction);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(boss.scene);
        Debug.Log("Jefe: caballero armado y animaciones listas.");
    }

    private static GameObject Piece(string name, Transform parent, Vector3 position, Vector3 size, Material material)
    {
        return BuilderUtils.Shape(PrimitiveType.Cube, name, parent, position, size, material, false);
    }

    private static Transform Joint(string name, Transform parent, Vector3 position)
    {
        Transform joint = BuilderUtils.Group(name, parent).transform;
        joint.localPosition = position;
        return joint;
    }

    private static AnimatorController BuildController()
    {
        AnimationClip idle = PlayerRig.MakeClip(Folder, "Knight_Idle", true, new Dictionary<string, Keyframe[]>
        {
            { ArmR, PlayerRig.Keys(0f, 0f, 1.2f, -7f, 2.4f, 0f) },
            { ArmL, PlayerRig.Keys(0f, 0f, 1.2f, 5f, 2.4f, 0f) },
            { Body, PlayerRig.Keys(0f, 0f, 1.2f, 2f, 2.4f, 0f) }
        });

        AnimationClip walk = PlayerRig.MakeClip(Folder, "Knight_Walk", true, new Dictionary<string, Keyframe[]>
        {
            { LegL, PlayerRig.Keys(0f, 28f, 0.45f, -28f, 0.9f, 28f) },
            { LegR, PlayerRig.Keys(0f, -28f, 0.45f, 28f, 0.9f, -28f) },
            { ArmL, PlayerRig.Keys(0f, -14f, 0.45f, 14f, 0.9f, -14f) },
            { ArmR, PlayerRig.Keys(0f, 14f, 0.45f, -14f, 0.9f, 14f) },
            { Body, PlayerRig.Keys(0f, 5f, 0.9f, 5f) }
        });

        AnimationClip attack = PlayerRig.MakeClip(Folder, "Knight_Attack", false, new Dictionary<string, Keyframe[]>
        {
            { ArmR, PlayerRig.Keys(0f, 0f, 0.15f, -165f, 0.3f, 50f, 0.55f, 0f) },
            { ArmL, PlayerRig.Keys(0f, 0f, 0.15f, 25f, 0.3f, -20f, 0.55f, 0f) },
            { Body, PlayerRig.Keys(0f, 0f, 0.15f, -10f, 0.3f, 18f, 0.55f, 0f) }
        });

        AnimationClip charge = PlayerRig.MakeClip(Folder, "Knight_Charge", false, new Dictionary<string, Keyframe[]>
        {
            { Body, PlayerRig.Keys(0f, 0f, 0.3f, 24f, 1.1f, 24f, 1.3f, 0f) },
            { ArmL, PlayerRig.Keys(0f, 0f, 0.3f, -75f, 1.1f, -75f, 1.3f, 0f) },
            { ArmR, PlayerRig.Keys(0f, 0f, 0.3f, 45f, 1.1f, 45f, 1.3f, 0f) },
            { LegL, PlayerRig.Keys(0f, 0f, 0.3f, 0f, 0.5f, 38f, 0.7f, -38f, 0.9f, 38f, 1.1f, -38f, 1.3f, 0f) },
            { LegR, PlayerRig.Keys(0f, 0f, 0.3f, 0f, 0.5f, -38f, 0.7f, 38f, 0.9f, -38f, 1.1f, 38f, 1.3f, 0f) }
        });

        AnimationClip slam = PlayerRig.MakeClip(Folder, "Knight_Slam", false, new Dictionary<string, Keyframe[]>
        {
            { ArmR, PlayerRig.Keys(0f, 0f, 0.7f, -170f, 0.9f, -170f, 1.05f, 35f, 1.4f, 0f) },
            { ArmL, PlayerRig.Keys(0f, 0f, 0.7f, -160f, 0.9f, -160f, 1.05f, 30f, 1.4f, 0f) },
            { Body, PlayerRig.Keys(0f, 0f, 0.7f, -14f, 0.9f, -14f, 1.05f, 26f, 1.4f, 0f) }
        });

        string path = Folder + "/Knight.controller";
        AssetDatabase.DeleteAsset(path);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Charge", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Slam", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine machine = controller.layers[0].stateMachine;
        AnimatorState idleState = machine.AddState("Idle");
        AnimatorState walkState = machine.AddState("Walk");
        AnimatorState attackState = machine.AddState("Attack");
        AnimatorState chargeState = machine.AddState("Charge");
        AnimatorState slamState = machine.AddState("Slam");

        idleState.motion = idle;
        walkState.motion = walk;
        attackState.motion = attack;
        chargeState.motion = charge;
        slamState.motion = slam;
        machine.defaultState = idleState;

        PlayerRig.Link(idleState, walkState).AddCondition(AnimatorConditionMode.Greater, 0.3f, "Speed");
        PlayerRig.Link(walkState, idleState).AddCondition(AnimatorConditionMode.Less, 0.3f, "Speed");

        foreach (AnimatorState state in new[] { idleState, walkState })
        {
            PlayerRig.Link(state, attackState).AddCondition(AnimatorConditionMode.If, 0f, "Attack");
            PlayerRig.Link(state, chargeState).AddCondition(AnimatorConditionMode.If, 0f, "Charge");
            PlayerRig.Link(state, slamState).AddCondition(AnimatorConditionMode.If, 0f, "Slam");
        }

        PlayerRig.Return(attackState, idleState);
        PlayerRig.Return(chargeState, idleState);
        PlayerRig.Return(slamState, idleState);

        EditorUtility.SetDirty(controller);
        return controller;
    }
}
