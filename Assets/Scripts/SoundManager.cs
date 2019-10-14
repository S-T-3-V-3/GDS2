using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Linq;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public Sound[] sounds;
    public Sound[] music;
    GameManager gameManager;

    void Awake() {
        if (SoundManager.Instance != null)
            GameObject.Destroy(this.gameObject);
        else
            Instance = this;

        gameManager = GameManager.Instance;

        DontDestroyOnLoad(this.gameObject);

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound m in music) {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.clip;
            m.source.volume = m.volume;
            m.source.pitch = m.pitch;
            m.source.loop = m.loop;
        }
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        if (s == null) {
            Debug.LogWarning($"Warning: File '{name}' was not found.");
            return;
        }

        s.source.Play();
    }

    public void Stop(string name) {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        if (s == null) {
            Debug.LogWarning($"Warning: File '{name}' was not found.");
            return;
        }

        s.source.Stop();
    }

    public void PlayMusic(string name) {
        foreach (Sound m in music) {
            if (m.source.isPlaying)
                m.source.Stop();
        }

        Sound s = Array.Find(music, Sound => Sound.name == name);
        if (s == null) {
            Debug.LogWarning($"Warning: Music file '{name}' was not found.");
            return;
        }

        s.source.Play();
    }

    public void StopMusic(string name) {
        Sound s = Array.Find(music, Sound => Sound.name == name);
        if (s == null) {
            Debug.LogWarning($"Warning: Music file '{name}' was not found.");
            return;
        }

        s.source.Stop();
    }
}

[System.Serializable]
public class Sound {
    public string name;
    public AudioClip clip;
    [HideInInspector]
    public AudioSource source;

    [Range(0f,1f)]
    public float volume;
    [Range(0.1f,3f)]
    public float pitch;

    public bool loop = false;
}