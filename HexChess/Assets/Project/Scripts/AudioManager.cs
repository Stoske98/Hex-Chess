using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    #region AudioManager Singleton
    private static AudioManager _instance;

    public static AudioManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
                s.source.outputAudioMixerGroup = s.group;
            }
        }

    }
    #endregion

    public Sound[] sounds;
    public AudioMixer music_mixer;
    public AudioMixer sfx_mixer;

    private void Start()
    {
        Play("Background");
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.Stop();

    }

    public void Pitch(string name, float strength)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.pitch = strength;

    }

    public void SetMusicVolume(float volume)
    {
        music_mixer.SetFloat("volume",volume);
    }
    public void SetSFXVolume(float volume)
    {
        sfx_mixer.SetFloat("sfx_volume", volume);
    }

    public void OnClick()
    {
        Play("Click");
    }
}

[System.Serializable]
public class Sound
{

    public string name;

    public AudioClip clip;
    public AudioMixerGroup group;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
