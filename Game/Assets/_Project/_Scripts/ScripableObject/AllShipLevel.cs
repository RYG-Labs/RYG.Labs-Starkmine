using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllShipLevel", menuName = "Scriptable Objects/AllShipLevel")]
public class AllShipLevel : ScriptableObject
{
    [SerializeField] private List<ShipSO> listBasicShip = new();
    [SerializeField] private List<ShipSO> listEliteShip = new();
    [SerializeField] private List<ShipSO> listProShip = new();
    [SerializeField] private List<ShipSO> listGigaShip = new();

    public List<ShipSO> GetNextShips(ShipSO.ShipType shipType)
    {
        switch (shipType)
        {
            case ShipSO.ShipType.Basic: return listEliteShip;
            case ShipSO.ShipType.Elite: return listProShip;
            case ShipSO.ShipType.Pro: return listGigaShip;
        }

        return listBasicShip;
    }
}