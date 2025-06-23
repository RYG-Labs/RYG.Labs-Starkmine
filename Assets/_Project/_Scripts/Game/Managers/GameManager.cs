using System;
using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Game.Enemies;
using _Project._Scripts.Game.Poolings;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project._Scripts.Game.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        public event EventHandler<OnChangePlanetEventHandlerEventArgs> OnChangePlanetEventHandler;

        public class OnChangePlanetEventHandlerEventArgs : EventArgs
        {
            public PlanetSO CurrentPlanet { get; set; }
            public PlanetSO NewPlanet { get; set; }
            public ShipData[] ListShipInNewPlanet { get; set; }
            public StationData CurrentStation { get; set; }
        }

        [SerializeField] private Ship shipPrefab;
        [SerializeField] private Planet planetPrefab;
        public PlanetSO CurrentPlanetId;

        [SerializeField] private Planet currentPlanet;

        [SerializeField] private List<Ship> ships;
        [SerializeField] private Transform GameHolder;

        private void Start()
        {
            currentPlanet = Instantiate(planetPrefab, new Vector3(0, 0, 0), Quaternion.identity, GameHolder);
        }

        public void MoveToNewPlanet(PlanetSO planetSO)
        {
            BulletPooling.Instance.ClearPool();
            GameHolder.DestroyChildren();
            ships.Clear();
            CurrentPlanetId = planetSO;
            currentPlanet = Instantiate(planetPrefab, new Vector3(0, 0, 0), Quaternion.identity, GameHolder);
            currentPlanet.spriteRenderer.sprite = planetSO.planetSprite;
            Dictionary<PlanetSO, ShipData[]> shipInInventory = DataManager.Instance.PlanetShipDictionary;
            foreach (ShipData shipData in shipInInventory[CurrentPlanetId])
            {
                if (shipData == null || !shipData.onDuty) continue;
                Ship ship = Instantiate(shipPrefab, new Vector3(-2, 0, 0), Quaternion.identity, GameHolder);
                ships.Add(ship);
                ship.SetUp(shipData, currentPlanet);
            }

            OnChangePlanetEventHandler?.Invoke(this, new OnChangePlanetEventHandlerEventArgs
            {
                CurrentPlanet = CurrentPlanetId,
                NewPlanet = planetSO,
                ListShipInNewPlanet = shipInInventory[CurrentPlanetId],
                CurrentStation = GetCurrentStation()
            });
        }

        public void AddShipToCurrentPlanet(ShipData shipData, int index)
        {
            List<ShipData> listShipData = DataManager.Instance.ShipInInventory;
            DataManager.Instance.AddShipToPlanet(CurrentPlanetId, shipData, index);
            listShipData.Remove(shipData);
        }

        public void RemoveShipToCurrentPlanet(ShipData shipData, int index)
        {
            List<ShipData> listShipData = DataManager.Instance.ShipInInventory;
            DataManager.Instance.RemoveShipToPlanet(CurrentPlanetId, shipData, index);
            listShipData.Add(shipData);
        }

        public void LaunchSpaceShip(ShipData shipData)
        {
            Ship ship = Instantiate(shipPrefab, new Vector3(2, 0, 0), Quaternion.identity, GameHolder);
            shipData.onDuty = true;
            ship.SetUp(shipData, currentPlanet);
            ships.Add(ship);
        }

        public bool CallbackSpaceShip(ShipData shipData)
        {
            Ship ship = GetShipInstant(shipData);
            if (ship == null) return false;
            shipData.onDuty = false;
            ships.Remove(ship);
            Destroy(ship.gameObject);
            return true;
        }

        public Ship GetShipInstant(ShipData shipData)
        {
            return ships.FirstOrDefault(ship => ship.ShipData == shipData);
        }

        public ShipData[] GetShipOnCurrentPlanet()
        {
            return DataManager.Instance.PlanetShipDictionary[CurrentPlanetId];
        }

        public StationData GetCurrentStation()
        {
            int index = DataManager.Instance.planets.IndexOf(CurrentPlanetId);
            return DataManager.Instance.listStationData[index];
        }
        // public void DestroyAllShip()
        // {
        //     foreach (var ship in ships)
        //     {
        //         Destroy(ship.gameObject);
        //     }
        //
        //     ships.Clear();
        // }
    }
}