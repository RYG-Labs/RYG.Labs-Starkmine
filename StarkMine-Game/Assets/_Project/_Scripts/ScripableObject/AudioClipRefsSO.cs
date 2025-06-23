using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipRefsSO", menuName = "Scriptable Objects/AudioClipRefsSO")]
public class AudioClipRefsSO : ScriptableObject
{
    [SerializeField] public AudioClip[] lazerSounds;
}