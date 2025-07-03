using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : StaticInstance<SoundManager>
{
    [SerializeField] private AudioClipRefsSO _audioClipRefsSO;
    [SerializeField] private List<AudioSourceConfig> listAudioSourceConfig;
    [SerializeField] private float downDefaultVolume = 0.5f;

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

    //Add core success 
    public void PlayBleepSound1()
    {
        PlaySound(_audioClipRefsSO.bleepSounds[0], AudioSourceConfig.SoundType.UISound);
    }

    public void PlayBleepSound3()
    {
        PlaySound(_audioClipRefsSO.bleepSounds[2], AudioSourceConfig.SoundType.UISound);
    }

    public void PlayBleepSound4()
    {
        PlaySound(_audioClipRefsSO.bleepSounds[3], AudioSourceConfig.SoundType.UISound);
    }

    //Hide popup 
    public void PlayBleepSound7()
    {
        PlaySound(_audioClipRefsSO.bleepSounds[6], AudioSourceConfig.SoundType.UISound);
    }

    public void PlayClickSound2()
    {
        PlaySound(_audioClipRefsSO.clickSounds[1], AudioSourceConfig.SoundType.UISound);
    }

    //Selected sound
    public void PlayClickSound4()
    {
        PlaySound(_audioClipRefsSO.clickSounds[3], AudioSourceConfig.SoundType.UISound);
    }

    public void PlaySequenceSound4()
    {
        PlaySound(_audioClipRefsSO.sequenceSounds[3], AudioSourceConfig.SoundType.UISound);
    }

    //complete 
    public void PlayCompleteSound2()
    {
        PlaySound(_audioClipRefsSO.completeSounds[1], AudioSourceConfig.SoundType.UISound);
    }

    public void PlayCompleteSound1()
    {
        PlaySound(_audioClipRefsSO.completeSounds[0], AudioSourceConfig.SoundType.UISound);
    }

    public void PlayConfirmSound1()
    {
        PlaySound(_audioClipRefsSO.confirmSounds[0], AudioSourceConfig.SoundType.UISound);
    }

    //click sound
    public void PlayConfirmSound3()
    {
        PlaySound(_audioClipRefsSO.confirmSounds[2], AudioSourceConfig.SoundType.UISound);
    }

    public void PlayClickSound()
    {
        PlayConfirmSound3();
    }

    public void PlayConfirmSound5()
    {
        PlaySound(_audioClipRefsSO.confirmSounds[4], AudioSourceConfig.SoundType.UISound);
    }

    public void PlayConfirmSound6()
    {
        PlaySound(_audioClipRefsSO.confirmSounds[5], AudioSourceConfig.SoundType.UISound);
    }

    public void PlayDeniedSound1()
    {
        PlaySound(_audioClipRefsSO.deniedSounds[0], AudioSourceConfig.SoundType.UISound);
    }

    public void PlayDataPointSound1()
    {
        PlaySound(_audioClipRefsSO.dataPointSounds[0], AudioSourceConfig.SoundType.UISound);
    }

    //Hover sound
    public void PlayDataPointSound3()
    {
        PlaySound(_audioClipRefsSO.dataPointSounds[2], AudioSourceConfig.SoundType.UISound);
    }

    public void PlayDataPointSound4()
    {
        PlaySound(_audioClipRefsSO.dataPointSounds[3], AudioSourceConfig.SoundType.UISound);
    }
}