using System;
using UnityEngine;

[Serializable]
public class AudioSourceConfig
{
    public enum SoundType
    {
        BulletSound = 0,
        UISound = 1,
    }

    [SerializeField] private SoundType type;

    public SoundType Type
    {
        get => type;
        set => type = value;
    }

    [SerializeField] private AudioSource audioSource;

    public AudioSource AudioSource
    {
        get => audioSource;
        // set => source = value;
    }

    [SerializeField] private string sfxName;

    public string SfxName
    {
        get => sfxName;
    }

    [SerializeField] private float volume;

    public float Volume
    {
        get => volume;
        set
        {
            volume = value;
            audioSource.volume = volume;
        }
    }
}