using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipSO", menuName = "Scriptable Objects/ShipSO")]
public class ShipSO : ScriptableObject
{
    public enum ShipType
    {
        Basic,
        Elite,
        Pro,
        Giga
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
    public float hashPower;
    public List<int> powerShipPerLevel = new();
    public List<int> costPerLevel = new();
    public ShipType shipTypeRequire;

}