using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class LightingManager : MonoBehaviour
{
    private const float Z_LOBBY_MAX   = 0f;
    private const float Z_PARKOUR_MAX = 55f;

    [Header("Colores de zona")]
    public Color lobbyAmbient   = new Color(0.18f, 0.16f, 0.22f);
    public Color parkourAmbient = new Color(0.10f, 0.13f, 0.18f);
    public Color arenaAmbient   = new Color(0.22f, 0.08f, 0.06f);

    [Header("Intensidad de luz direccional")]
    public float lobbyIntensity   = 0.9f;
    public float parkourIntensity = 0.6f;
    public float arenaIntensity   = 1.1f;

    [Header("Transición")]
    public float transitionSpeed = 1.2f;

    private Light directionalLight;
    private Color targetAmbient;
    private float targetIntensity;
    private Color targetLightColor;
    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;

        FindOrCreateDirectionalLight();
        CreateAccentLights();

        targetAmbient   = lobbyAmbient;
        targetIntensity = lobbyIntensity;
        RenderSettings.ambientMode  = AmbientMode.Flat;
        RenderSettings.ambientLight = lobbyAmbient;
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            return;
        }

        UpdateZone(player.position.z);
        ApplyTransition();
    }

    void UpdateZone(float playerZ)
    {
        if (playerZ < Z_LOBBY_MAX)
        {
            targetAmbient   = lobbyAmbient;
            targetIntensity = lobbyIntensity;
            targetLightColor = new Color(1f, 0.92f, 0.78f);
        }
        else if (playerZ < Z_PARKOUR_MAX)
        {
            targetAmbient   = parkourAmbient;
            targetIntensity = parkourIntensity;
            targetLightColor = new Color(0.72f, 0.82f, 1f);
        }
        else
        {
            targetAmbient   = arenaAmbient;
            targetIntensity = arenaIntensity;
            targetLightColor = new Color(1f, 0.42f, 0.28f);
        }
    }

    void ApplyTransition()
    {
        float t = transitionSpeed * Time.deltaTime;
        RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, targetAmbient, t);

        if (directionalLight != null)
        {
            directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, targetIntensity, t);
            directionalLight.color     = Color.Lerp(directionalLight.color, targetLightColor, t);
        }
    }

    void FindOrCreateDirectionalLight()
    {
        Light[] lights = FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Light l in lights)
        {
            if (l.type == LightType.Directional)
            {
                directionalLight = l;
                break;
            }
        }

        if (directionalLight == null)
        {
            GameObject go = new GameObject("Directional Light");
            directionalLight = go.AddComponent<Light>();
            directionalLight.type      = LightType.Directional;
            directionalLight.intensity = lobbyIntensity;
            directionalLight.color     = new Color(1f, 0.92f, 0.78f);
            directionalLight.shadows   = LightShadows.Soft;
            go.transform.rotation      = Quaternion.Euler(45f, -30f, 0f);
        }
    }

    void CreateAccentLights()
    {
        Light chestLight = CreatePointLight("Chest Accent Light", new Vector3(0f, 3f, 85f), new Color(1f, 0.72f, 0.2f), 3.5f, 10f);
        StartCoroutine(PulsateLight(chestLight, 3.0f, 4.2f, 0.9f));
        CreatePointLight("Arena Light L", new Vector3(-8f, 4f, 75f), new Color(0.9f, 0.2f, 0.1f), 2.5f, 12f);
        CreatePointLight("Arena Light R", new Vector3( 8f, 4f, 75f), new Color(0.9f, 0.2f, 0.1f), 2.5f, 12f);
    }

    Light CreatePointLight(string name, Vector3 position, Color color, float intensity, float range)
    {
        GameObject go = new GameObject(name);
        go.transform.position = position;
        Light l     = go.AddComponent<Light>();
        l.type      = LightType.Point;
        l.color     = color;
        l.intensity = intensity;
        l.range     = range;
        l.shadows   = LightShadows.None;
        return l;
    }

    IEnumerator PulsateLight(Light light, float minIntensity, float maxIntensity, float speed)
    {
        while (true)
        {
            if (light != null)
                light.intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
            yield return null;
        }
    }
}
