using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpaceStationSO", menuName = "Scriptable Objects/SpaceStationSO")]
public class SpaceStationSO : ScriptableObject
{
    public int maxLevel;
    public List<float> listMultiplierPerLevel = new();
    public List<int> listCostPerLevel = new();
}