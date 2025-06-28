using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project._Scripts.Game.Managers
{
    public class DataManager : PersistentSingleton<DataManager>
    {
        public event EventHandler<OnAddShipToInventoryEventArgs> OnAddShipToInventoryEventHandler;

        public class OnAddShipToInventoryEventArgs : EventArgs
        {
            public ShipData NewShip;
            public List<ShipData> Ships;
        }

        public event EventHandler<OnRemoveShipToInventoryEventArgs> OnRemoveShipToInventoryEventHandler;

        public class OnRemoveShipToInventoryEventArgs : EventArgs
        {
            public ShipData ShipRemoved;
            public List<ShipData> ListShipRemoved = new();
            public List<ShipData> Ships = new();
        }

        public event EventHandler<OnAddShipToPlanetHandlerEventArgs> OnAddShipToPlanetHandler;

        public class OnAddShipToPlanetHandlerEventArgs : EventArgs
        {
            public PlanetSO PlanetSo;
            public ShipData NewShipData;
        }

        public event EventHandler<OnRemoveShipToPlanetEventArgs> OnRemoveShipToPlanetHandler;

        public class OnRemoveShipToPlanetEventArgs : EventArgs
        {
            public PlanetSO PlanetSo;
            public ShipData ShipRemoved;
        }

        public event EventHandler<OnUserDataChangedEventArgs> OnUserDataChangedEventHandler;

        public class OnUserDataChangedEventArgs : EventArgs
        {
            public UserData NewUserData;
        }

        // User Data
        [SerializeField] private UserData userData = new();

        public UserData UserData
        {
            get => userData;
            set
            {
                userData = value;
                OnUserDataChangedEventHandler?.Invoke(this, new OnUserDataChangedEventArgs { NewUserData = value });
            }
        }


        //ship Inventory
        [SerializeField] private List<ShipData> shipInInventory = new();

        public List<ShipData> ShipInInventory
        {
            get => shipInInventory;
            set => shipInInventory = value;
        }

        public void AddShipToInventory(ShipData ship)
        {
            shipInInventory.Add(ship);
            OnAddShipToInventoryEventHandler?.Invoke(this, new OnAddShipToInventoryEventArgs
            {
                NewShip = ship,
                Ships = shipInInventory
            });
        }

        public void RemoveShipFromInventory(ShipData ship)
        {
            shipInInventory.Remove(ship);
            OnRemoveShipToInventoryEventHandler?.Invoke(this, new OnRemoveShipToInventoryEventArgs
            {
                ShipRemoved = ship,
                Ships = shipInInventory
            });
        }

        public void RemoveShipFromInventory(List<ShipData> ships)
        {
            foreach (ShipData shipData in ships)
            {
                shipInInventory.Remove(shipData);
            }

            OnRemoveShipToInventoryEventHandler?.Invoke(this, new OnRemoveShipToInventoryEventArgs
            {
                ListShipRemoved = ships,
                Ships = shipInInventory
            });
        }

        // int : planetId
        private Dictionary<PlanetSO, ShipData[]> _planetShipDictionary = new();

        public Dictionary<PlanetSO, ShipData[]> PlanetShipDictionary
        {
            get => _planetShipDictionary;
            set => _planetShipDictionary = value;
        }

        // private string walletAddress = "0x0034...3456";
        //
        // public string WalletAddress
        // {
        //     get => walletAddress;
        //     set => walletAddress = value;
        // }

        public event EventHandler<OnMineCoinUpdateEventArgs> OnMineCoinUpdate;

        public class OnMineCoinUpdateEventArgs : EventArgs
        {
            public double NewMineCoin;
        }

        [SerializeField] private double _mineCoin = 100000;

        public double MineCoin
        {
            get => _mineCoin;
            set
            {
                _mineCoin = value;
                Debug.Log($"MineCoin: {_mineCoin}");
                OnMineCoinUpdate?.Invoke(this, new OnMineCoinUpdateEventArgs { NewMineCoin = value });
            }
        }

        [SerializeField] public List<PlanetSO> planets = new();
        [SerializeField] public List<StationData> listStationData = new();

        private void Start()
        {
            foreach (PlanetSO planet in planets)
            {
                _planetShipDictionary.Add(planet, new ShipData[6]);
            }
        }

        public void AddShipToPlanet(PlanetSO planetSo, ShipData shipData, int index)
        {
            PlanetShipDictionary[planetSo][index] = shipData;
            OnAddShipToPlanetHandler?.Invoke(this, new OnAddShipToPlanetHandlerEventArgs
            {
                PlanetSo = planetSo,
                NewShipData = shipData
            });
        }

        public void RemoveShipToPlanet(PlanetSO planetSo, ShipData shipData, int index)
        {
            PlanetShipDictionary[planetSo][index] = null;
            OnRemoveShipToPlanetHandler?.Invoke(this, new OnRemoveShipToPlanetEventArgs()
            {
                PlanetSo = planetSo,
                ShipRemoved = shipData
            });
        }

        //Core Engine

        public event EventHandler<OnUpdateAmountCoreEngineHandlerEventArgs> OnUpdateAmountCoreEngineHandler;

        public class OnUpdateAmountCoreEngineHandlerEventArgs : EventArgs
        {
            public int NewAmount;
            public CoreEngineSO CoreEngine;
        }

        public List<CoreEngineSO> listCoreEngineSO;
        public List<CoreEngineAmountData> listCoreEngineAmountInInventory;
        public List<CoreEngineData> listCoreEngineData;

        public CoreEngineData GetCoreEngineDataUnActiveByType(CoreEngineSO.CoreEngineType type)
        {
            return listCoreEngineData.Find(coreEngineData =>
                coreEngineData.coreEngineSO.coreEngineType == type && !coreEngineData.isActive);
        }

        public CoreEngineData GetCoreEngineDataById(int id)
        {
            return listCoreEngineData.Find(coreEngineData => coreEngineData.id == id);
        }

        public void ResetListCoreEngineAmountInInventory()
        {
            foreach (CoreEngineAmountData coreEngineAmountData in listCoreEngineAmountInInventory)
            {
                coreEngineAmountData.amount = 0;
            }
        }

        public void AddCoreEngine(CoreEngineSO coreEngineSO, int amount)
        {
            CoreEngineAmountData coreEngineAmountData =
                listCoreEngineAmountInInventory.Find(item => item.coreEngineSO == coreEngineSO);

            coreEngineAmountData.amount += amount;

            OnUpdateAmountCoreEngineHandler?.Invoke(this, new OnUpdateAmountCoreEngineHandlerEventArgs
            {
                NewAmount = coreEngineAmountData.amount,
                CoreEngine = coreEngineAmountData.coreEngineSO
            });
        }

        public bool RemoveCoreEngine(CoreEngineSO coreEngineSO, int amount)
        {
            CoreEngineAmountData coreEngineAmountData =
                listCoreEngineAmountInInventory.Find(item => item.coreEngineSO == coreEngineSO);
            if (coreEngineAmountData.amount < amount)
            {
                return false;
            }

            coreEngineAmountData.amount -= amount;

            OnUpdateAmountCoreEngineHandler?.Invoke(this, new OnUpdateAmountCoreEngineHandlerEventArgs
            {
                NewAmount = coreEngineAmountData.amount,
                CoreEngine = coreEngineAmountData.coreEngineSO
            });
            return true;
        }

        public bool IsContainCoreEngineInInventory(CoreEngineSO.CoreEngineType coreEngineType)
        {
            CoreEngineAmountData coreEngineAmountData =
                listCoreEngineAmountInInventory.Find(item => item.coreEngineSO.coreEngineType == coreEngineType);
            return coreEngineAmountData is { amount: > 0 };
        }

        public bool IsContainCoreEngineRequireInInventory(ShipSO.ShipType shipType)
        {
            switch (shipType)
            {
                case ShipSO.ShipType.Basic:
                {
                    return IsContainCoreEngineInInventory(CoreEngineSO.CoreEngineType.Basic);
                }
                case ShipSO.ShipType.Elite:
                {
                    return IsContainCoreEngineInInventory(CoreEngineSO.CoreEngineType.Elite);
                }
                case ShipSO.ShipType.GIGA:
                {
                    return IsContainCoreEngineInInventory(CoreEngineSO.CoreEngineType.Pro);
                }
                case ShipSO.ShipType.Pro:
                {
                    return IsContainCoreEngineInInventory(CoreEngineSO.CoreEngineType.GIGA);
                }
            }

            return false;
        }

        public void AddCoreEngineToSpaceShip(CoreEngineSO coreEngine, ShipData shipData)
        {
            foreach (PlanetSO planet in _planetShipDictionary.Keys)
            {
                foreach (ShipData item in _planetShipDictionary[planet])
                {
                    if (item == shipData)
                    {
                        item.CoreEngine = coreEngine;
                        RemoveCoreEngine(coreEngine, 1);
                    }
                }
            }
        }

        public ShipData AddCoreEngineToSpaceShip(int coreEngineId, int shipId)
        {
            CoreEngineData coreEngineData = GetCoreEngineDataById(coreEngineId);
            ShipData shipData = GetShipDataById(shipId);
            AddCoreEngineToSpaceShip(coreEngineData.coreEngineSO, shipData);
            return shipData;
        }

        public ShipData GetShipDataById(int id)
        {
            return null;
        }

        public int SumAmountOfTypeShipAllPlanet(ShipSO.ShipType shipType)
        {
            return planets.Sum(planets => SumAmountOfTypeShipInPlanet(shipType, planets));
        }

        public int SumAmountOfTypeShipInPlanet(ShipSO.ShipType shipType, PlanetSO planet)
        {
            return _planetShipDictionary[planet].Count(item => item != null && item.shipSO.shipType == shipType);
        }

        public float SumHashPowerOfTypeShipInPlanet(ShipSO.ShipType shipType, PlanetSO planet)
        {
            return _planetShipDictionary[planet]
                .Where(shipData => shipData != null && shipType == shipData.shipSO.shipType)
                .Sum(shipData => shipData.GetHashPower());
        }

        public float SumHashPowerInAllPlanet(ShipSO.ShipType shipType, PlanetSO planet)
        {
            return planets.Sum(planets => SumHashPowerOfTypeShipInPlanet(shipType, planets));
        }

        public CoreEngineSO GetCoreEngineRequire(ShipSO.ShipType shipType)
        {
            switch (shipType)
            {
                case ShipSO.ShipType.Basic: return listCoreEngineSO[0];
                case ShipSO.ShipType.Elite: return listCoreEngineSO[1];
                case ShipSO.ShipType.GIGA: return listCoreEngineSO[2];
                case ShipSO.ShipType.Pro: return listCoreEngineSO[3];
                default: return listCoreEngineSO[0];
            }
        }

        public AllShipLevel allShipLevel;


        public int CountAllSpaceShip()
        {
            return shipInInventory.Count + CountSpaceShipAddedStation();
        }

        public int CountSpaceShipAddedStation()
        {
            return _planetShipDictionary.Values.SelectMany(shipDataArray => shipDataArray)
                .Count(shipData => shipData != null);
        }

        public int CountSpaceShipOnDuty()
        {
            return _planetShipDictionary.Values.SelectMany(shipDataArray => shipDataArray)
                .Count(shipData => shipData != null && shipData.onDuty);
        }

        public int CountAllSpaceShipByType(ShipSO.ShipType shipType)
        {
            return CountAllSpaceShipInInventoryByType(shipType) + CountSpaceShipAddedStationByType(shipType);
        }

        public int CountAllSpaceShipInInventoryByType(ShipSO.ShipType shipType)
        {
            return shipInInventory.Count(shipData => shipData != null && shipData.shipSO.shipType == shipType);
        }

        public int CountSpaceShipAddedStationByType(ShipSO.ShipType shipType)
        {
            return _planetShipDictionary.Values.SelectMany(shipDataArray => shipDataArray)
                .Count(shipData => shipData != null && shipData.shipSO.shipType == shipType);
        }


        public int CountAllCoreEngine()
        {
            return CountCoreEngineEquipped() + CountCoreEngineUnequipped();
        }

        public int CountCoreEngineUnequipped()
        {
            return listCoreEngineAmountInInventory.Sum(coreEngineAmountData => coreEngineAmountData.amount);
        }

        public int CountCoreEngineEquipped()
        {
            return _planetShipDictionary.Values.SelectMany(shipDataArray => shipDataArray)
                .Count(shipData => shipData != null && shipData.CoreEngine != null);
        }

        public int CountCoreEngineUnequippedByType(CoreEngineSO.CoreEngineType coreEngineType)
        {
            return listCoreEngineAmountInInventory
                .Where(coreEngineAmountData => coreEngineAmountData.coreEngineSO.coreEngineType == coreEngineType)
                .Sum(coreEngineAmountData => coreEngineAmountData.amount);
        }

        public int CountCoreEngineEquippedByType(CoreEngineSO.CoreEngineType coreEngineType)
        {
            return _planetShipDictionary.Values.SelectMany(shipDataArray => shipDataArray)
                .Count(shipData => shipData != null && shipData.CoreEngine != null &&
                                   shipData.CoreEngine.coreEngineType == coreEngineType);
        }

        public int CountAllCoreEngineByType(CoreEngineSO.CoreEngineType coreEngineType)
        {
            return CountCoreEngineUnequippedByType(coreEngineType) + CountCoreEngineEquippedByType(coreEngineType);
        }


        public List<ShipData> ShipDataInInventoryFilter(List<ShipSO.ShipType> shipTypes, int page, int size,
            bool isAll = false, bool isAllType = false)
        {
            List<ShipData> result = new List<ShipData>();
            if (isAllType)
            {
                result = ShipInInventory;
            }
            else
            {
                result.AddRange(shipInInventory.Where(shipData => shipTypes.Contains(shipData.shipSO.shipType)));
            }

            if (isAll)
            {
                return result;
            }

            int startIndex = (page - 1) * size;
            int count = Mathf.Min(size, result.Count - startIndex);
            if (startIndex < 0 || startIndex >= result.Count)
            {
                return new List<ShipData>();
            }

            return result.GetRange(startIndex, count);
        }

        public List<ShipSO> AllShipSO;

        [DllImport("__Internal")]
        private static extern void RequestMinersData();

        [DllImport("__Internal")]
        private static extern void RequestCoreEnginesData();

        private void ResponseMinersData(string responseString)
        {
            Debug.Log("ResponseMinersData" + responseString);
            MessageBase<JArray> response = JsonConvert.DeserializeObject<MessageBase<JArray>>(responseString);
            if (!response.IsSuccess()) return;

            shipInInventory.Clear();
            List<JObject> listJObject = response.data.ToObject<List<JObject>>();
            Debug.Log("ResponseMinersData To Array Success" + listJObject.Count);
            foreach (JObject jObject in listJObject)
            {
                ShipDTO shipDto = jObject.ToObject<ShipDTO>();
                Debug.Log(shipDto.ToString());
                ShipData shipData = new ShipData(shipDto.tokenId, GetShipSoByType(shipDto.tier), shipDto.level,
                    shipDto.hashPower, shipDto.isIgnited);
                ShipInInventory.Add(shipData);
            }
        }

        private void ResponseCoreEnginesData(string responseString)
        {
            Debug.Log("ResponseCoreEnginesData" + responseString);
            MessageBase<JArray> response = JsonConvert.DeserializeObject<MessageBase<JArray>>(responseString);
            if (!response.IsSuccess()) return;

            Debug.Log("ResponseCoreEnginesData To Object Success");
            ResetListCoreEngineAmountInInventory();
            List<JObject> listJObject = response.data.ToObject<List<JObject>>();
            foreach (JObject jObject in listJObject)
            {
                CoreEngineDTO coreEngineDto = jObject.ToObject<CoreEngineDTO>();
                CoreEngineSO coreEngineSo = GetCoreEngineByType(coreEngineDto.engineType);
                CoreEngineData coreEngineData =
                    new CoreEngineData(coreEngineDto.tokenId, coreEngineSo, coreEngineDto.isActive);
                listCoreEngineData.Add(coreEngineData);
                AddCoreEngine(coreEngineSo, 1);
            }
        }

        public ShipSO GetShipSoByType(string type)
        {
            switch (type)
            {
                case "Basic": return AllShipSO[0];
                case "Elite": return AllShipSO[1];
                case "Pro": return AllShipSO[2];
                case "GIGA": return AllShipSO[3];
            }

            return AllShipSO[0];
        }

        public CoreEngineSO GetCoreEngineByType(string type)
        {
            switch (type)
            {
                case "Basic": return listCoreEngineSO[0];
                case "Elite": return listCoreEngineSO[1];
                case "Pro": return listCoreEngineSO[2];
                case "GIGA": return listCoreEngineSO[3];
            }

            return listCoreEngineSO[0];
        }

        public void ResponseStationsData(string responseString)
        {
            Debug.Log("ResponseStationsData" + responseString);
            MessageBase<JArray> response = JsonConvert.DeserializeObject<MessageBase<JArray>>(responseString);
            if (!response.IsSuccess()) return;
            // listStationData.Clear();
            List<JObject> listJObject = response.data.ToObject<List<JObject>>();
            for (int i = 0; i < listJObject.Count; i++)
            {
                StationDTO stationDto = listJObject[i].ToObject<StationDTO>();
                
                listStationData[i].level = stationDto.level;
            }
        }
    }
}