using System;

[Serializable]
public class CoreEngineData
{
    public int id;
    public CoreEngineSO coreEngineSO;
    public bool isActive;
    public CoreEngineData(int id, CoreEngineSO coreEngineSO, bool isActive)
    {
        this.id = id;
        this.coreEngineSO = coreEngineSO;
        this.isActive = isActive;
    }
}