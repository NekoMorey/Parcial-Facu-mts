using UnityEngine;
using UnityEngine.Audio;

public sealed class VoiceOver : MonoBehaviour
{
    private static VoiceOver instance;
    [Header("Lineas grabadas")]
    [SerializeField] private AudioClip ruta;
    [SerializeField] private AudioClip peligro;
    [SerializeField] private AudioClip victoria;
    [SerializeField] private AudioMixer audioMixer;

    private AudioSource voiceSource;

    public static AudioMixer Mixer => instance != null ? instance.audioMixer : null;

    private void Awake()
    {
        instance = this;
        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.playOnAwake = false;
        voiceSource.spatialBlend = 0f;
        voiceSource.volume = 0.9f;
        RouteVoice();
    }

    public void PlayRuta()
    {
        PlayLine(ruta);
    }

    public static void PlayRouteLine()
    {
        FindVoiceOver()?.PlayRuta();
    }

    public void PlayPeligro()
    {
        PlayLine(peligro);
    }

    public static void PlayDangerLine()
    {
        FindVoiceOver()?.PlayPeligro();
    }

    public void PlayVictoria()
    {
        PlayLine(victoria);
    }

    public static void PlayVictoryLine()
    {
        FindVoiceOver()?.PlayVictoria();
    }

    private void PlayLine(AudioClip line)
    {
        if (line == null || voiceSource.isPlaying) return;
        voiceSource.PlayOneShot(line);
    }

    private void RouteVoice()
    {
        if (audioMixer == null) return;
        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("Voz");
        if (groups.Length > 0)
            voiceSource.outputAudioMixerGroup = groups[0];
    }

    private static VoiceOver FindVoiceOver()
    {
        if (instance == null)
            instance = FindFirstObjectByType<VoiceOver>();
        return instance;
    }
}
