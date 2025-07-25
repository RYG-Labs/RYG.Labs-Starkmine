using UnityEngine;

[CreateAssetMenu(fileName = "CoreEngineDownWithDurabilitySO",
    menuName = "Scriptable Objects/CoreEngineDownWithDurabilitySO")]
public class CoreEngineDownWithDurabilitySO : ScriptableObject
{
    public int downAmount;
    public int minDurability;
    public int maxDurability;
}