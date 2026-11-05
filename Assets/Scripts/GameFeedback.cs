using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Procedural audio and particle feedback so the vertical slice has no missing scene references.</summary>
public sealed class GameFeedback : MonoBehaviour
{
    public enum Cue { Shoot, Dodge, Hit, Impact, Charge, BossDown, Reward }

    private static GameFeedback instance;
    private readonly Dictionary<Cue, AudioClip> clips = new Dictionary<Cue, AudioClip>();
    private static Material particleMaterial;
    private AudioSource musicSource;
    private AudioSource ambienceSource;
    private AudioSource sfxSource;
    private const int SampleRate = 22050;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        EnsureInstance();
    }

    private static GameFeedback EnsureInstance()
    {
        if (instance != null) return instance;
        instance = FindFirstObjectByType<GameFeedback>();
        if (instance != null) return instance;
        var host = new GameObject("Game Feedback");
        instance = host.AddComponent<GameFeedback>();
        DontDestroyOnLoad(host);
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = 0.22f;
        musicSource.clip = CreateMusic();

        ambienceSource = gameObject.AddComponent<AudioSource>();
        ambienceSource.loop = true;
        ambienceSource.spatialBlend = 0f;
        ambienceSource.volume = 0.16f;
        ambienceSource.clip = CreateAmbience();

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.spatialBlend = 0f;
        sfxSource.volume = 0.72f;

        musicSource.Play();
        ambienceSource.Play();
    }

    public static void Play(Cue cue)
    {
        GameFeedback feedback = EnsureInstance();
        if (feedback.sfxSource != null)
            feedback.sfxSource.PlayOneShot(feedback.GetClip(cue));
    }

    public static void Burst(Vector3 position, Color color, int count = 20, float size = 0.2f)
    {
        EnsureInstance().CreateBurstAt(position, color, count, size);
    }

    private AudioClip GetClip(Cue cue)
    {
        if (!clips.TryGetValue(cue, out AudioClip clip))
        {
            clip = CreateCue(cue);
            clips.Add(cue, clip);
        }
        return clip;
    }

    private static AudioClip MakeClip(string name, float seconds, Func<float, float> sample)
    {
        int count = Mathf.CeilToInt(seconds * SampleRate);
        float[] data = new float[count];
        for (int i = 0; i < count; i++)
        {
            float t = i / (float)SampleRate;
            data[i] = Mathf.Clamp(sample(t), -1f, 1f);
        }
        AudioClip clip = AudioClip.Create(name, count, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateMusic()
    {
        const float length = 16f;
        float[] roots = { 55f, 65.41f, 73.42f, 49f };
        return MakeClip("Umbral Music", length, t =>
        {
            float bar = Mathf.Floor(t / 4f);
            float root = roots[(int)bar % roots.Length];
            float swell = 0.55f + 0.45f * Mathf.Sin(t * 0.32f);
            float low = Mathf.Sin(2f * Mathf.PI * root * t);
            float fifth = Mathf.Sin(2f * Mathf.PI * root * 1.5f * t + 0.3f);
            float shimmer = Mathf.Sin(2f * Mathf.PI * root * 4f * t) * (0.25f + 0.75f * Mathf.Pow(Mathf.Max(0f, Mathf.Sin(t * Mathf.PI / 2f)), 8f));
            return swell * (low * 0.11f + fifth * 0.045f + shimmer * 0.018f);
        });
    }

    private AudioClip CreateAmbience()
    {
        const float length = 12f;
        return MakeClip("Arena Wind", length, t =>
        {
            float wind = Mathf.Sin(2f * Mathf.PI * 0.17f * t) * 0.5f + Mathf.Sin(2f * Mathf.PI * 0.071f * t + 1.4f) * 0.5f;
            float rumble = Mathf.Sin(2f * Mathf.PI * 38f * t) * 0.5f + Mathf.Sin(2f * Mathf.PI * 52f * t + 0.7f) * 0.5f;
            return wind * rumble * 0.055f;
        });
    }

    private AudioClip CreateCue(Cue cue)
    {
        float duration = cue == Cue.Charge || cue == Cue.BossDown || cue == Cue.Reward ? 0.75f : 0.28f;
        return MakeClip("SFX " + cue, duration, t =>
        {
            float n = t / duration;
            float envelope = Mathf.Pow(1f - n, cue == Cue.Reward ? 0.8f : 2f);
            float noise = Mathf.Sin(2f * Mathf.PI * (311f + 1200f * n) * t) * 0.35f + Mathf.Sin(2f * Mathf.PI * 83f * t) * 0.25f;
            switch (cue)
            {
                case Cue.Shoot: return envelope * (noise + Mathf.Sin(2f * Mathf.PI * (220f - 130f * n) * t) * 0.4f) * 0.48f;
                case Cue.Dodge: return envelope * (Mathf.Sin(2f * Mathf.PI * (850f - 600f * n) * t) * 0.35f + noise * 0.25f);
                case Cue.Hit: return envelope * (Mathf.Sin(2f * Mathf.PI * (120f - 70f * n) * t) * 0.65f + noise * 0.2f);
                case Cue.Impact: return envelope * (Mathf.Sin(2f * Mathf.PI * (95f - 40f * n) * t) * 0.55f + noise * 0.3f);
                case Cue.Charge: return (0.45f + 0.55f * n) * (Mathf.Sin(2f * Mathf.PI * 48f * t) * 0.28f + noise * 0.16f);
                case Cue.BossDown: return envelope * (Mathf.Sin(2f * Mathf.PI * (150f - 80f * n) * t) * 0.55f + Mathf.Sin(2f * Mathf.PI * (225f - 140f * n) * t) * 0.2f);
                case Cue.Reward: return envelope * (Mathf.Sin(2f * Mathf.PI * 523.25f * t) * 0.27f + Mathf.Sin(2f * Mathf.PI * 659.25f * t) * 0.2f + Mathf.Sin(2f * Mathf.PI * 783.99f * t) * 0.16f);
                default: return 0f;
            }
        });
    }

    private void CreateBurstAt(Vector3 position, Color color, int count, float size)
    {
        GameObject burst = new GameObject("Feedback Burst");
        burst.transform.position = position;
        ParticleSystem particles = burst.AddComponent<ParticleSystem>();
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = particles.main;
        main.playOnAwake = false;
        main.duration = 0.45f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 4.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(size * 0.45f, size);
        main.startColor = color;
        main.gravityModifier = 0.45f;
        main.maxParticles = Mathf.Max(1, count);
        var emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)Mathf.Clamp(count, 1, 100)) });
        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.18f;
        var renderer = particles.GetComponent<ParticleSystemRenderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Sprites/Default");
        if (particleMaterial == null && shader != null) particleMaterial = new Material(shader);
        if (particleMaterial != null) renderer.sharedMaterial = particleMaterial;
        particles.Play();
        Destroy(burst, 2f);
    }
}
