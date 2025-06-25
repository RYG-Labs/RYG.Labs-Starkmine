using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AudioClipRefsSO", menuName = "Scriptable Objects/AudioClipRefsSO")]
public class AudioClipRefsSO : ScriptableObject
{
    [SerializeField] public List<AudioClip> bleepSounds;
    [SerializeField] public List<AudioClip> clickSounds;
    [SerializeField] public List<AudioClip> completeSounds;
    [SerializeField] public List<AudioClip> confirmSounds;
    [SerializeField] public List<AudioClip> dataPointSounds;
    [SerializeField] public List<AudioClip> deniedSounds;
    [SerializeField] public List<AudioClip> executeSounds;
    [SerializeField] public List<AudioClip> sequenceSounds;
}