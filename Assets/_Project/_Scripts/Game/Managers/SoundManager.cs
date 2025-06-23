using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : StaticInstance<SoundManager>
{
    [SerializeField] private AudioClipRefsSO _audioClipRefsSO;
    [SerializeField] private List<AudioSourceConfig> listAudioSourceConfig;

    private void Start()
    {
        foreach (AudioSourceConfig audioSourceConfig in listAudioSourceConfig)
        {
            audioSourceConfig.Volume = GetVolume(audioSourceConfig);
        }
    }

    public void PlaySound(List<AudioClip> clipList, AudioSourceConfig.SoundType soundType)
    {
        int randomIndex = Random.Range(0, clipList.Count);
        PlaySound(clipList[randomIndex], soundType);
    }

    public void PlaySound(AudioClip clip, AudioSourceConfig.SoundType soundType)
    {
        FindAudioSourcesConfig(soundType).AudioSource.PlayOneShot(clip);
    }

    public float GetVolume(AudioSourceConfig.SoundType soundType)
    {
        AudioSourceConfig audioSourceConfig = FindAudioSourcesConfig(soundType);
        return GetVolume(audioSourceConfig);
    }

    public float GetVolume(AudioSourceConfig audioSourceConfig)
    {
        return PlayerPrefs.GetFloat(audioSourceConfig.SfxName, 0.5f);
    }

    public void SetVolume(AudioSourceConfig.SoundType soundType, float volume)
    {
        FindAudioSourcesConfig(soundType).Volume = volume;
        PlayerPrefs.SetFloat(soundType.ToString(), volume);
    }

    public AudioSourceConfig FindAudioSourcesConfig(AudioSourceConfig.SoundType soundType)
    {
        foreach (AudioSourceConfig audioSourceConfig in listAudioSourceConfig)
        {
            if (audioSourceConfig.Type == soundType)
            {
                return audioSourceConfig;
            }
        }

        return null;
    }
}