[System.Serializable]
public class UserDTO
{
    public string address;
    public double balance;
}

public class ShipDTO
{
    public int tokenId;
    public string tier;
    public double hashPower;
    public int level;
    public int efficiency;
    public string lastMaintenance;
    public string coreEngineId;
    public bool isIgnited;

    public override string ToString()
    {
        return
            $"ShipDTO {{ tokenId={tokenId}, tier={tier}, hashPower={hashPower}, level={level}, efficiency={efficiency}, lastMaintenance={lastMaintenance}, coreEngineId={coreEngineId}, isIgnited={isIgnited} }}";
    }
}