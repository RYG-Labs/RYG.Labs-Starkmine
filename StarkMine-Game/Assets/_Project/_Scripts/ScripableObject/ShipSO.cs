using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ShipSO", menuName = "Scriptable Objects/ShipSO")]
public class ShipSO : ScriptableObject
{
    public enum ShipType
    {
        Basic,
        Elite,
        Pro,
        GIGA
    }

    public ShipType shipType;
    public Sprite baseSprite;
    public string shipName;
    public int shipCost;
    public int maxLevel;
    public int shipDamage;
    public int shipHealth;
    public int fireRate;
    public ImageAnimationSO imageAnimationSO;
    public BulletSO bulletSO;
    public int baseHashPower;
    public int tierBonus;
    public List<int> powerShipPerLevel = new();
    public List<int> costPerLevel = new();
    public ShipType shipTypeRequire;

}