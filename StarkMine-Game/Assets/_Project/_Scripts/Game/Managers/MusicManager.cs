using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Game.Managers;
using DG.Tweening;
using UnityEngine;

public class MusicManager : PersistentSingleton<MusicManager>
{
    public static readonly string MusicVolumePlayerPrefs = "MUSIC_VOLUME";
    [SerializeField] private float volume;

    public float Volume
    {
        get => volume;
        set
        {
            volume = value;
            audioSource.volume = volume * downDefaultVolume;
        }
    }

    [SerializeField] private float downDefaultVolume = 0.5f;

    [SerializeField] private AudioSource audioSource;

    public float fadeDuration = 3.0f;

    private void Start()
    {
        volume = GetMusicVolume();
        audioSource.loop = true;
        audioSource.volume = volume;
        GameManager.Instance.OnChangePlanetEventHandler += GameManagerOnChangePlanetEventHandler;
        PlayMusic(GameManager.Instance.CurrentPlanetId.listPlanetMusic);
    }

    private void GameManagerOnChangePlanetEventHandler(object sender, GameManager.OnChangePlanetEventHandlerEventArgs e)
    {
        PlayMusic(e.NewPlanet.listPlanetMusic);
    }

    public void PlayMusic(List<AudioClip> newClips)
    {
        if (newClips == null || newClips.Count == 0)
        {
            StopMusic();
            return;
        }

        if (IsSameCurrentMusic(newClips))
        {
            return;
        }

        AudioClip newClip = newClips[Random.Range(0, newClips.Count)];

        audioSource.DOFade(0, fadeDuration).OnComplete(() =>
        {
            audioSource.Stop();
            audioSource.clip = newClip;
            audioSource.Play();
            audioSource.DOFade(volume * downDefaultVolume, fadeDuration);
        });
    }

    private bool IsSameCurrentMusic(List<AudioClip> newClips)
    {
        return newClips.Any(clip => audioSource.clip == clip);
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (newClip == null || newClip == audioSource.clip)
        {
            Debug.LogWarning("AudioClip is null or same as current music clip");
            return;
        }

        audioSource.DOFade(0, fadeDuration).OnComplete(() =>
        {
            audioSource.Stop();
            audioSource.clip = newClip;
            audioSource.Play();
            audioSource.DOFade(volume * downDefaultVolume, fadeDuration);
        });
    }

    public void StopMusic()
    {
        audioSource.DOFade(0, fadeDuration).OnComplete(() => audioSource.Stop());
    }

    public float GetMusicVolume()
    {
        if (!PlayerPrefs.HasKey(MusicVolumePlayerPrefs))
        {
            PlayerPrefs.SetFloat(MusicVolumePlayerPrefs, 0.5f);
            return 0.5f;
        }

        return PlayerPrefs.GetFloat(MusicVolumePlayerPrefs);
    }
}