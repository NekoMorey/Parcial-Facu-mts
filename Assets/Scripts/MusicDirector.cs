using UnityEngine;

public class MusicDirector : MonoBehaviour
{
    public float abyssStartsAtZ = 270f;
    public float padVolume = 0.32f;
    public float tensionVolume = 0.35f;
    public float fadeSpeed = 0.5f;

    const int SampleRate = 44100;

    AudioSource ruinsPad;
    AudioSource abyssPad;
    AudioSource tension;
    Transform player;
    float checkTimer;
    bool fighting;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Spawn()
    {
        if (FindFirstObjectByType<MusicDirector>() != null) return;
        new GameObject("Musica").AddComponent<MusicDirector>();
    }

    void Start()
    {
        ruinsPad = MakeSource("Ruinas", BuildPad(12f, new[] { 55f, 82.4f, 110f, 130.8f, 164.8f }, 0.18f));
        abyssPad = MakeSource("Abismo", BuildPad(12f, new[] { 36.7f, 55f, 73.4f, 87.3f, 110f }, 0.22f));
        tension = MakeSource("Tension", BuildTension(2f));
    }

    AudioSource MakeSource(string label, AudioClip clip)
    {
        var holder = new GameObject("Capa_" + label);
        holder.transform.SetParent(transform, false);
        var source = holder.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.spatialBlend = 0f;
        source.volume = 0f;
        source.Play();
        return source;
    }

    void Update()
    {
        if (player == null)
        {
            var found = FindFirstObjectByType<PlayerController>();
            if (found != null) player = found.transform;
            return;
        }

        bool inAbyss = player.position.z >= abyssStartsAtZ;
        checkTimer -= Time.unscaledDeltaTime;
        if (checkTimer <= 0f)
        {
            checkTimer = 0.5f;
            fighting = false;
            foreach (var boss in FindObjectsByType<BossStats>(FindObjectsSortMode.None))
            {
                if (boss.engaged) { fighting = true; break; }
            }
        }

        float step = fadeSpeed * Time.unscaledDeltaTime;
        ruinsPad.volume = Mathf.MoveTowards(ruinsPad.volume, inAbyss ? 0f : padVolume, step);
        abyssPad.volume = Mathf.MoveTowards(abyssPad.volume, inAbyss ? padVolume : 0f, step);
        tension.volume = Mathf.MoveTowards(tension.volume, fighting ? tensionVolume : 0f, step * 1.5f);
        tension.pitch = Mathf.MoveTowards(tension.pitch, inAbyss ? 1.25f : 1f, step);
    }

    static float Snap(float frequency, float seconds)
    {
        return Mathf.Max(1f, Mathf.Round(frequency * seconds)) / seconds;
    }

    AudioClip BuildPad(float seconds, float[] notes, float gain)
    {
        int length = Mathf.RoundToInt(seconds * SampleRate);
        var data = new float[length];
        for (int n = 0; n < notes.Length; n++)
        {
            float frequency = Snap(notes[n], seconds);
            float detune = Snap(notes[n] * 1.004f, seconds);
            float phaseShift = n * 0.7f;
            for (int i = 0; i < length; i++)
            {
                float t = i / (float)SampleRate;
                float swell = 0.6f + 0.4f * Mathf.Sin(2f * Mathf.PI * t / seconds + phaseShift);
                float wave = Mathf.Sin(2f * Mathf.PI * frequency * t) + 0.6f * Mathf.Sin(2f * Mathf.PI * detune * t);
                data[i] += wave * swell * gain / notes.Length;
            }
        }
        var clip = AudioClip.Create("Pad", length, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    AudioClip BuildTension(float seconds)
    {
        int length = Mathf.RoundToInt(seconds * SampleRate);
        var data = new float[length];
        float drone = Snap(55f, seconds);
        for (int i = 0; i < length; i++)
        {
            float t = i / (float)SampleRate;
            data[i] = Mathf.Sin(2f * Mathf.PI * drone * t) * 0.18f;
        }

        float beat = seconds / 4f;
        for (int b = 0; b < 4; b++)
        {
            int start = Mathf.RoundToInt(b * beat * SampleRate);
            int hitLength = Mathf.RoundToInt(0.35f * SampleRate);
            for (int i = 0; i < hitLength && start + i < length; i++)
            {
                float t = i / (float)SampleRate;
                float envelope = Mathf.Exp(-t * 9f);
                float sweep = 90f - 55f * Mathf.Min(1f, t * 4f);
                data[start + i] += Mathf.Sin(2f * Mathf.PI * sweep * t) * envelope * 0.7f;
            }
        }

        var clip = AudioClip.Create("Tension", length, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
