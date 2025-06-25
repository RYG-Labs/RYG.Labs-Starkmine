using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CoreEngineSO", menuName = "Scriptable Objects/CoreEngineSO")]
public class CoreEngineSO : ScriptableObject
{
    public enum CoreEngineType
    {
        Basic,
        Elite,
        Pro,
        GIGA
    }

    [SerializeField] public Sprite sprite;
    [SerializeField] public string nameCoreEngine;
    [SerializeField] public CoreEngineType coreEngineType;
    [SerializeField] public int cost;
}