using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class VerticalSliceSetup
{
    private const string ScenePath = "Assets/Scenes/scene.unity";
    private const string MaterialFolder = "Assets/Materials";
    private const string AnimationFolder = "Assets/Animations";
    private const string ControllerPath = AnimationFolder + "/Boss.controller";

    [MenuItem("Tools/Vertical Slice/Apply Presentation Setup")]
    public static void Apply()
    {
        EnsureFolder(MaterialFolder);
        EnsureFolder(AnimationFolder);
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ApplyMaterials();
        ApplyArenaLighting();
        ApplyBossAnimator();
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Vertical slice presentation setup applied.");
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        string name = Path.GetFileName(path);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }

    private static Material MakeMaterial(string name, Color color, float metallic, float smoothness, bool emission = false)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
        if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
        if (emission && material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 1.6f);
        }
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void ApplyMaterials()
    {
        Material stone = MakeMaterial("Umbral_Stone", new Color(0.17f, 0.2f, 0.23f), 0.18f, 0.28f);
        Material floor = MakeMaterial("Umbral_Floor", new Color(0.11f, 0.14f, 0.17f), 0.22f, 0.32f);
        Material platform = MakeMaterial("Umbral_Pathstone", new Color(0.29f, 0.33f, 0.35f), 0.24f, 0.38f);
        Material boss = MakeMaterial("Umbral_Boss", new Color(0.32f, 0.075f, 0.065f), 0.36f, 0.42f);
        Material player = MakeMaterial("Umbral_Hero", new Color(0.16f, 0.35f, 0.39f), 0.28f, 0.48f);
        Material destructible = MakeMaterial("Umbral_Red_Brick", new Color(0.38f, 0.13f, 0.09f), 0.12f, 0.24f);
        Material beacon = MakeMaterial("Umbral_Guide_Light", new Color(1f, 0.55f, 0.16f), 0.05f, 0.3f, true);

        foreach (MeshRenderer renderer in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            string name = renderer.gameObject.name.ToLowerInvariant();
            if (name == "boss") renderer.sharedMaterial = boss;
            else if (name == "player") renderer.sharedMaterial = player;
            else if (name.Contains("destructible")) renderer.sharedMaterial = destructible;
            else if (name.Contains("floor") || name.Contains("suelo")) renderer.sharedMaterial = floor;
            else if (name.Contains("platform") || name.Contains("pad") || name.Contains("ramp")) renderer.sharedMaterial = platform;
            else if (name.Contains("wall") || name.Contains("pared")) renderer.sharedMaterial = stone;
            else if (name.Contains("guide")) renderer.sharedMaterial = beacon;
        }
    }

    private static void ApplyArenaLighting()
    {
        CreateArenaLight("VS_ArenaLight_Ember", new Vector3(-11f, 14f, 76f), new Color(1f, 0.28f, 0.12f), 13f, 23f);
        CreateArenaLight("VS_ArenaLight_Azure", new Vector3(11f, 13f, 82f), new Color(0.15f, 0.52f, 0.8f), 10f, 20f);
        CreateArenaLight("VS_ParkourLight", new Vector3(0f, 10f, 42f), new Color(1f, 0.58f, 0.22f), 5f, 16f);
        Material beacon = AssetDatabase.LoadAssetAtPath<Material>(MaterialFolder + "/Umbral_Guide_Light.mat");
        CreateRouteBeacon("VS_RouteBeacon_Lobby", new Vector3(2.3f, 0.18f, -4f), beacon);
        CreateRouteBeacon("VS_RouteBeacon_Parkour", new Vector3(-2.4f, 0.95f, 9f), beacon);
        CreateRouteBeacon("VS_RouteBeacon_Exit", new Vector3(2.2f, 6.9f, 52f), beacon);
    }

    private static void CreateArenaLight(string name, Vector3 position, Color color, float intensity, float range)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) go = new GameObject(name);
        go.transform.position = position;
        Light light = go.GetComponent<Light>();
        if (light == null) light = go.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.shadows = LightShadows.None;
    }

    private static void CreateRouteBeacon(string name, Vector3 position, Material material)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.22f;
        Collider collider = go.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);
        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        if (renderer != null) renderer.sharedMaterial = material;
        Light light = go.GetComponent<Light>();
        if (light == null) light = go.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.52f, 0.18f);
        light.intensity = 1.8f;
        light.range = 4f;
        light.shadows = LightShadows.None;
    }

    private static AnimationClip MakeScaleClip(string path, string name, Keyframe[] x, Keyframe[] y, Keyframe[] z, bool loop)
    {
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (clip == null)
        {
            clip = new AnimationClip { name = name, frameRate = 30f };
            AssetDatabase.CreateAsset(clip, path);
        }
        AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve("", typeof(Transform), "m_LocalScale.x"), new AnimationCurve(x));
        AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve("", typeof(Transform), "m_LocalScale.y"), new AnimationCurve(y));
        AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve("", typeof(Transform), "m_LocalScale.z"), new AnimationCurve(z));
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);
        return clip;
    }

    private static void ApplyBossAnimator()
    {
        AnimationClip idle = MakeScaleClip(AnimationFolder + "/Boss_Idle.anim", "Boss Idle",
            new[] { new Keyframe(0f, 1f), new Keyframe(1f, 1.015f), new Keyframe(2f, 1f) },
            new[] { new Keyframe(0f, 1f), new Keyframe(1f, 0.985f), new Keyframe(2f, 1f) },
            new[] { new Keyframe(0f, 1f), new Keyframe(1f, 1.015f), new Keyframe(2f, 1f) }, true);
        AnimationClip attack = MakeScaleClip(AnimationFolder + "/Boss_Attack.anim", "Boss Attack",
            new[] { new Keyframe(0f, 1f), new Keyframe(0.12f, 1.12f), new Keyframe(0.38f, 1f) },
            new[] { new Keyframe(0f, 1f), new Keyframe(0.12f, 0.86f), new Keyframe(0.38f, 1f) },
            new[] { new Keyframe(0f, 1f), new Keyframe(0.12f, 1.12f), new Keyframe(0.38f, 1f) }, false);
        AnimationClip charge = MakeScaleClip(AnimationFolder + "/Boss_Charge.anim", "Boss Charge",
            new[] { new Keyframe(0f, 1f), new Keyframe(0.12f, 0.88f), new Keyframe(0.4f, 1.1f), new Keyframe(0.7f, 1f) },
            new[] { new Keyframe(0f, 1f), new Keyframe(0.12f, 1.12f), new Keyframe(0.4f, 0.9f), new Keyframe(0.7f, 1f) },
            new[] { new Keyframe(0f, 1f), new Keyframe(0.12f, 0.88f), new Keyframe(0.4f, 1.1f), new Keyframe(0.7f, 1f) }, false);

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        controller.parameters = new AnimatorControllerParameter[0];
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Charge", AnimatorControllerParameterType.Trigger);
        AnimatorStateMachine machine = controller.layers[0].stateMachine;
        machine.states = new ChildAnimatorState[0];
        AnimatorState idleState = machine.AddState("Idle");
        AnimatorState attackState = machine.AddState("Attack");
        AnimatorState chargeState = machine.AddState("Charge");
        idleState.motion = idle;
        attackState.motion = attack;
        chargeState.motion = charge;
        machine.defaultState = idleState;
        AddTriggerTransition(idleState, attackState, "Attack");
        AddTriggerTransition(idleState, chargeState, "Charge");
        AddReturnTransition(attackState, idleState);
        AddReturnTransition(chargeState, idleState);
        EditorUtility.SetDirty(controller);

        GameObject boss = GameObject.FindWithTag("Boss");
        if (boss != null)
        {
            Animator animator = boss.GetComponent<Animator>();
            if (animator == null) animator = boss.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }
    }

    private static void AddTriggerTransition(AnimatorState from, AnimatorState to, string trigger)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.hasExitTime = false;
        transition.duration = 0.04f;
        transition.AddCondition(AnimatorConditionMode.If, 0f, trigger);
    }

    private static void AddReturnTransition(AnimatorState from, AnimatorState to)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.hasExitTime = true;
        transition.exitTime = 0.95f;
        transition.duration = 0.08f;
    }
}
