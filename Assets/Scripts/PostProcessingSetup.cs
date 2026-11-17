using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class PostProcessingSetup : MonoBehaviour
{
    [Header("Bloom")]
    public float bloomIntensity = 0.6f;
    public float bloomThreshold = 0.9f;
    public Color bloomTint = new Color(1f, 0.85f, 0.6f);

    [Header("Vignette")]
    public float vignetteIntensity = 0.38f;
    public Color vignetteColor = Color.black;

    [Header("Color Adjustments")]
    public float contrast = 18f;
    public float saturation = 12f;
    public float postExposure = 0.1f;

    [Header("Combat Mode")]
    public float combatVignetteIntensity = 0.52f;
    public Color combatVignetteColor = new Color(0.35f, 0f, 0f);
    public float combatContrast = 28f;
    public float transitionSpeed = 2f;

    private Volume globalVolume;
    private Vignette vignette;
    private Bloom bloom;
    private ColorAdjustments colorAdjust;
    private BossAI bossAI;
    private float nextBossSearch;

    void Start()
    {
        globalVolume = FindFirstObjectByType<Volume>();
        if (globalVolume == null)
        {
            GameObject go = new GameObject("Global Post-Process Volume");
            globalVolume = go.AddComponent<Volume>();
            globalVolume.isGlobal = true;
            globalVolume.priority = 1f;
            globalVolume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
        }

        SetupEffects();

        UniversalAdditionalCameraData cameraData = GetComponent<UniversalAdditionalCameraData>();
        if (cameraData != null)
            cameraData.renderPostProcessing = true;
    }

    void SetupEffects()
    {
        VolumeProfile profile = globalVolume.profile;

        if (!profile.TryGet(out bloom))
            bloom = profile.Add<Bloom>(false);
        bloom.active = true;
        bloom.intensity.overrideState = true;
        bloom.intensity.value = bloomIntensity;
        bloom.threshold.overrideState = true;
        bloom.threshold.value = bloomThreshold;
        bloom.tint.overrideState = true;
        bloom.tint.value = bloomTint;

        if (!profile.TryGet(out vignette))
            vignette = profile.Add<Vignette>(false);
        vignette.active = true;
        vignette.intensity.overrideState = true;
        vignette.intensity.value = vignetteIntensity;
        vignette.color.overrideState = true;
        vignette.color.value = vignetteColor;

        if (!profile.TryGet(out colorAdjust))
            colorAdjust = profile.Add<ColorAdjustments>(false);
        colorAdjust.active = true;
        colorAdjust.contrast.overrideState = true;
        colorAdjust.contrast.value = contrast;
        colorAdjust.saturation.overrideState = true;
        colorAdjust.saturation.value = saturation;
        colorAdjust.postExposure.overrideState = true;
        colorAdjust.postExposure.value = postExposure;
    }

    void Update()
    {
        nextBossSearch -= Time.deltaTime;
        if (bossAI == null && nextBossSearch <= 0f)
        {
            bossAI = FindFirstObjectByType<BossAI>();
            nextBossSearch = 1f;
        }

        bool inCombat = bossAI != null && bossAI.isPlayerDetected && bossAI.gameObject.activeInHierarchy;
        float t = transitionSpeed * Time.deltaTime;

        if (vignette != null)
        {
            float targetVig = inCombat ? combatVignetteIntensity : vignetteIntensity;
            Color targetCol = inCombat ? combatVignetteColor : vignetteColor;
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetVig, t);
            vignette.color.value = Color.Lerp(vignette.color.value, targetCol, t);
        }

        if (colorAdjust != null)
        {
            float targetContrast = inCombat ? combatContrast : contrast;
            colorAdjust.contrast.value = Mathf.Lerp(colorAdjust.contrast.value, targetContrast, t);
        }
    }
}
