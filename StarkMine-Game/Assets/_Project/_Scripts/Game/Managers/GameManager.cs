using System;
using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Game.Enemies;
using _Project._Scripts.Game.Poolings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project._Scripts.Game.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        public event EventHandler<OnChangeStationEventHandlerEventArgs> OnChangeStationEventHandler;

        public class OnChangeStationEventHandlerEventArgs : EventArgs
        {
            public ShipData[] ListShipInNewPlanet { get; set; }
            public StationData CurrentStation { get; set; }
        }

        [SerializeField] private Ship shipPrefab;

        [SerializeField] private Planet planetPrefab;

        // public PlanetSO CurrentPlanetId;
        public StationData CurrentStation;

        [SerializeField] private Planet currentPlanet;

        [SerializeField] private List<Ship> ships;
        [SerializeField] private Transform GameHolder;

        private void Start()
        {
            CurrentStation = DataManager.Instance.listStationData[0];
            currentPlanet = Instantiate(planetPrefab, new Vector3(0, 0, 0), Quaternion.identity, GameHolder);
        }

        public void MoveToNewStation(StationData station)
        {
            BulletPooling.Instance.ClearPool();
            GameHolder.DestroyChildren();
            ships.Clear();
            CurrentStation = station;
            currentPlanet = Instantiate(planetPrefab, new Vector3(0, 0, 0), Quaternion.identity, GameHolder);
            currentPlanet.spriteRenderer.sprite = station.planetSo.planetSprite;
            foreach (ShipData shipData in CurrentStation.ListShipData)
            {
                if (shipData == null || !shipData.onDuty) continue;
                Ship ship = Instantiate(shipPrefab, new Vector3(-2, 0, 0), Quaternion.identity, GameHolder);
                ships.Add(ship);
                ship.SetUp(shipData, currentPlanet);
            }

            OnChangeStationEventHandler?.Invoke(this, new OnChangeStationEventHandlerEventArgs
            {
                ListShipInNewPlanet = CurrentStation.ListShipData,
                CurrentStation = CurrentStation
            });
        }

        public void AddShipToCurrentStation(ShipData shipData, int index)
        {
            List<ShipData> listShipData = DataManager.Instance.ShipInInventory;
            DataManager.Instance.AddShipToStation(CurrentStation, shipData, index);
            listShipData.Remove(shipData);
        }

        public void RemoveShipToCurrentStation(ShipData shipData, int index)
        {
            List<ShipData> listShipData = DataManager.Instance.ShipInInventory;
            DataManager.Instance.RemoveShipToStation(CurrentStation, shipData, index);
            listShipData.Add(shipData);
        }

        public void LaunchSpaceShip(int coreEngineId, int shipId)
        {
            ShipData shipData = DataManager.Instance.AddCoreEngineToSpaceShip(coreEngineId, shipId);
            // Update UI ship launched
            UIManager.Instance.spaceStationUI.LaunchSpaceShip(shipData);
            // Instantiate ship
            LaunchSpaceShip(shipData);
        }

        public void LaunchSpaceShip(ShipData shipData)
        {
            Ship ship = Instantiate(shipPrefab, new Vector3(2, 0, 0), Quaternion.identity, GameHolder);
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
            CoreEngineData coreEngineData = shipData.CoreEngineData;
            coreEngineData.isActive = false;
            DataManager.Instance.AddCoreEngine(coreEngineData);
            shipData.CoreEngineData = null;
            return true;
        }

        public Ship GetShipInstant(ShipData shipData)
        {
            return ships.FirstOrDefault(ship => ship.ShipData == shipData);
        }

        public ShipData[] GetShipOnCurrentPlanet()
        {
            return CurrentStation.ListShipData;
        }

        // public StationData GetCurrentStation()
        // {
        //     // int index = DataManager.Instance.planets.IndexOf(CurrentPlanetId);
        //     return DataManager.Instance.listStationData[index];
        // }
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