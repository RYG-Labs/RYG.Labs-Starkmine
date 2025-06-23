using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Game.Enemies;
using _Project._Scripts.Game.Managers;
using UnityEngine;

public class TabPlanetUI : BasePopup
{
    [SerializeField] private ItemTabPlanetUI itemTabPlanetUIPrefab;
    [SerializeField] private Transform itemContainer;

    protected override void Start()
    {
        base.Start();
        List<PlanetSO> planetSoList = DataManager.Instance.PlanetShipDictionary.Keys.ToList();
        foreach (PlanetSO planetSo in planetSoList)
        {
            ItemTabPlanetUI itemTabPlanetUI = Instantiate(itemTabPlanetUIPrefab, itemContainer);
            itemTabPlanetUI.Setup(planetSo.planetSprite, planetSo.planetName, planetSo);
            itemTabPlanetUI.OnUse += ItemTabPlanetUIOnOnUse;
        }
    }

    private void ItemTabPlanetUIOnOnUse(object sender, ItemTabPlanetUI.OnUseEventArgs e)
    {
        GameManager.Instance.MoveToNewPlanet(e.PlanetSo);
    }
}