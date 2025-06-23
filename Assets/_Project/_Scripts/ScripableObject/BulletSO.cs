using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletSO", menuName = "Scriptable Objects/BulletSO")]
public class BulletSO : ScriptableObject
{
    public RuntimeAnimatorController animatorController;
    public float speed;
    public List<AudioClip> shootSounds;
}