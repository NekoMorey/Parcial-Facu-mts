using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PlayerRig
{
    private const string Folder = "Assets/Animations/Player";
    private const string Body = "Cuerpo";
    private const string ArmL = "Cuerpo/Arm_L";
    private const string ArmR = "Cuerpo/Arm_R";
    private const string LegL = "Cuerpo/Leg_L";
    private const string LegR = "Cuerpo/Leg_R";

    [MenuItem("Tools/Umbral/2 Personaje")]
    public static void Build()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No hay un objeto con tag Player en la escena.");
            return;
        }

        Transform previous = player.transform.Find("Modelo");
        if (previous != null) Object.DestroyImmediate(previous.gameObject);

        MeshRenderer capsule = player.GetComponent<MeshRenderer>();
        if (capsule != null) capsule.enabled = false;

        Material cloth = BuilderUtils.Mat("Umbral_Hero");
        Material skin = BuilderUtils.Mat("Umbral_Hero_Skin", new Color(0.78f, 0.62f, 0.5f), 0f, 0.3f);
        Material armor = BuilderUtils.Mat("Umbral_Hero_Armor", new Color(0.55f, 0.42f, 0.2f), 0.7f, 0.5f);

        GameObject model = BuilderUtils.Group("Modelo", player.transform);
        Transform body = BuilderUtils.Group("Cuerpo", model.transform).transform;

        BuilderUtils.Shape(PrimitiveType.Cube, "Torso", body, new Vector3(0f, 0.2f, 0f), new Vector3(0.62f, 0.75f, 0.36f), cloth, false);
        BuilderUtils.Shape(PrimitiveType.Cube, "Belt", body, new Vector3(0f, -0.14f, 0f), new Vector3(0.66f, 0.12f, 0.4f), armor, false);
        BuilderUtils.Shape(PrimitiveType.Sphere, "Head", body, new Vector3(0f, 0.82f, 0f), Vector3.one * 0.44f, skin, false);
        BuilderUtils.Shape(PrimitiveType.Cube, "Hood", body, new Vector3(0f, 0.9f, -0.06f), new Vector3(0.48f, 0.3f, 0.4f), cloth, false);

        Limb("Arm_L", body, new Vector3(-0.41f, 0.5f, 0f), new Vector3(0.17f, 0.62f, 0.17f), skin, armor);
        Limb("Arm_R", body, new Vector3(0.41f, 0.5f, 0f), new Vector3(0.17f, 0.62f, 0.17f), skin, armor);
        Limb("Leg_L", body, new Vector3(-0.17f, -0.2f, 0f), new Vector3(0.22f, 0.8f, 0.22f), cloth, armor);
        Limb("Leg_R", body, new Vector3(0.17f, -0.2f, 0f), new Vector3(0.22f, 0.8f, 0.22f), cloth, armor);

        Animator animator = model.AddComponent<Animator>();
        animator.runtimeAnimatorController = BuildController();
        animator.applyRootMotion = false;

        if (player.GetComponent<PlayerAnimation>() == null)
            player.AddComponent<PlayerAnimation>();

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(player.scene);
        Debug.Log("Personaje: modelo y animaciones listos.");
    }

    private static void Limb(string name, Transform body, Vector3 pivot, Vector3 size, Material main, Material tip)
    {
        Transform joint = BuilderUtils.Group(name, body).transform;
        joint.localPosition = pivot;

        BuilderUtils.Shape(PrimitiveType.Cube, name + "_Mesh", joint, new Vector3(0f, -size.y * 0.5f, 0f), size, main, false);
        BuilderUtils.Shape(PrimitiveType.Cube, name + "_Tip", joint, new Vector3(0f, -size.y + 0.07f, 0f), new Vector3(size.x * 1.15f, 0.16f, size.z * 1.15f), tip, false);
    }

    private static AnimatorController BuildController()
    {
        BuilderUtils.EnsureFolder(Folder);

        AnimationClip idle = MakeClip("Player_Idle", true, new Dictionary<string, Keyframe[]>
        {
            { ArmL, Keys(0f, 0f, 1f, 5f, 2f, 0f) },
            { ArmR, Keys(0f, 0f, 1f, -5f, 2f, 0f) },
            { Body, Keys(0f, 0f, 1f, 2f, 2f, 0f) }
        });

        AnimationClip run = MakeClip("Player_Run", true, new Dictionary<string, Keyframe[]>
        {
            { LegL, Keys(0f, 40f, 0.22f, -40f, 0.44f, 40f) },
            { LegR, Keys(0f, -40f, 0.22f, 40f, 0.44f, -40f) },
            { ArmL, Keys(0f, -35f, 0.22f, 35f, 0.44f, -35f) },
            { ArmR, Keys(0f, 35f, 0.22f, -35f, 0.44f, 35f) },
            { Body, Keys(0f, 10f, 0.44f, 10f) }
        });

        AnimationClip jump = MakeClip("Player_Jump", true, new Dictionary<string, Keyframe[]>
        {
            { LegL, Keys(0f, -25f, 0.5f, -25f) },
            { LegR, Keys(0f, 30f, 0.5f, 30f) },
            { ArmL, Keys(0f, -150f, 0.5f, -150f) },
            { ArmR, Keys(0f, -150f, 0.5f, -150f) }
        });

        AnimationClip roll = MakeClip("Player_Roll", false, new Dictionary<string, Keyframe[]>
        {
            { Body, Keys(0f, 0f, 0.35f, 360f) },
            { LegL, Keys(0f, -80f, 0.35f, -80f) },
            { LegR, Keys(0f, -80f, 0.35f, -80f) },
            { ArmL, Keys(0f, -60f, 0.35f, -60f) },
            { ArmR, Keys(0f, -60f, 0.35f, -60f) }
        });

        AnimationClip toss = MakeClip("Player_Throw", false, new Dictionary<string, Keyframe[]>
        {
            { ArmR, Keys(0f, 0f, 0.1f, -170f, 0.2f, 45f, 0.32f, 0f) },
            { Body, Keys(0f, 0f, 0.1f, -8f, 0.2f, 12f, 0.32f, 0f) }
        });

        string path = Folder + "/Player.controller";
        AssetDatabase.DeleteAsset(path);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Dodge", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Throw", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine machine = controller.layers[0].stateMachine;
        AnimatorState idleState = machine.AddState("Idle");
        AnimatorState runState = machine.AddState("Run");
        AnimatorState jumpState = machine.AddState("Jump");
        AnimatorState rollState = machine.AddState("Roll");
        AnimatorState throwState = machine.AddState("Throw");

        idleState.motion = idle;
        runState.motion = run;
        jumpState.motion = jump;
        rollState.motion = roll;
        throwState.motion = toss;
        machine.defaultState = idleState;

        Link(idleState, runState).AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
        Link(runState, idleState).AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
        Link(idleState, jumpState).AddCondition(AnimatorConditionMode.IfNot, 0f, "Grounded");
        Link(runState, jumpState).AddCondition(AnimatorConditionMode.IfNot, 0f, "Grounded");
        Link(jumpState, idleState).AddCondition(AnimatorConditionMode.If, 0f, "Grounded");

        Link(idleState, rollState).AddCondition(AnimatorConditionMode.If, 0f, "Dodge");
        Link(runState, rollState).AddCondition(AnimatorConditionMode.If, 0f, "Dodge");
        Link(jumpState, rollState).AddCondition(AnimatorConditionMode.If, 0f, "Dodge");
        Link(idleState, throwState).AddCondition(AnimatorConditionMode.If, 0f, "Throw");
        Link(runState, throwState).AddCondition(AnimatorConditionMode.If, 0f, "Throw");
        Link(jumpState, throwState).AddCondition(AnimatorConditionMode.If, 0f, "Throw");

        Return(rollState, idleState);
        Return(throwState, idleState);

        EditorUtility.SetDirty(controller);
        return controller;
    }

    public static AnimatorStateTransition Link(AnimatorState from, AnimatorState to)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.hasExitTime = false;
        transition.duration = 0.06f;
        return transition;
    }

    public static void Return(AnimatorState from, AnimatorState to)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.hasExitTime = true;
        transition.exitTime = 0.9f;
        transition.duration = 0.08f;
    }

    public static Keyframe[] Keys(params float[] values)
    {
        Keyframe[] keys = new Keyframe[values.Length / 2];
        for (int i = 0; i < keys.Length; i++)
            keys[i] = new Keyframe(values[i * 2], values[i * 2 + 1]);
        return keys;
    }

    private static AnimationClip MakeClip(string name, bool loop, Dictionary<string, Keyframe[]> swings)
    {
        return MakeClip(Folder, name, loop, swings);
    }

    public static AnimationClip MakeClip(string folder, string name, bool loop, Dictionary<string, Keyframe[]> swings)
    {
        BuilderUtils.EnsureFolder(folder);
        string path = folder + "/" + name + ".anim";
        AssetDatabase.DeleteAsset(path);

        AnimationClip clip = new AnimationClip { name = name, frameRate = 30f };
        AssetDatabase.CreateAsset(clip, path);

        foreach (KeyValuePair<string, Keyframe[]> swing in swings)
        {
            Keyframe[] keys = swing.Value;
            float end = keys[keys.Length - 1].time;
            AnimationCurve still = AnimationCurve.Constant(0f, end, 0f);

            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(swing.Key, typeof(Transform), "localEulerAnglesRaw.x"), new AnimationCurve(keys));
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(swing.Key, typeof(Transform), "localEulerAnglesRaw.y"), still);
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(swing.Key, typeof(Transform), "localEulerAnglesRaw.z"), still);
        }

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);
        return clip;
    }
}
