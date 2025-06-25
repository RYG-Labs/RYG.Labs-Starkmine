using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetSO", menuName = "Scriptable Objects/PlanetSO")]
public class PlanetSO : ScriptableObject
{
    [SerializeField] public string planetName;
    [SerializeField] public Sprite planetSprite;
    [SerializeField] public int maxShip;
    [SerializeField] public List<AudioClip> listPlanetMusic;
}