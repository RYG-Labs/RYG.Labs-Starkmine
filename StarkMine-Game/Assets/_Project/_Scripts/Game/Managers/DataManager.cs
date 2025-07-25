using System;
using System.Collections;
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

        [SerializeField] public List<StationData> listStationData = new();
        [SerializeField] public List<StationDTO> listStationDto = new();

        public void AddShipToPlanet(PlanetSO planetSo, ShipData shipData, int index)
        {
            PlanetShipDictionary[planetSo][index] = shipData;
            OnAddShipToPlanetHandler?.Invoke(this, new OnAddShipToPlanetHandlerEventArgs
            {
                NewShipData = shipData
            });
        }

        public void AddShipToStation(StationData stationData, ShipData shipData, int index)
        {
            stationData.ListShipData[index] = shipData;
            OnAddShipToPlanetHandler?.Invoke(this, new OnAddShipToPlanetHandlerEventArgs
            {
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

        public void RemoveShipToStation(StationData stationData, ShipData shipData, int index)
        {
            // PlanetShipDictionary[planetSo][index] = null;
            // stationData.ListShipData.Remove(shipData);
            stationData.ListShipData[index] = null;

            OnRemoveShipToPlanetHandler?.Invoke(this, new OnRemoveShipToPlanetEventArgs()
            {
                // PlanetSo = planetSo,
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
        public List<ShipData> allShip;

        public CoreEngineData GetCoreEngineDataUnActiveByType(CoreEngineSO.CoreEngineType type)
        {
            return listCoreEngineData.Find(coreEngineData =>
                coreEngineData.coreEngineSO.coreEngineType == type && !coreEngineData.isActive);
        }

        public List<CoreEngineData> GetCoreEngineDataUnActiveByListType(List<CoreEngineSO.CoreEngineType> types)
        {
            return listCoreEngineData.FindAll(coreEngineData =>
                types.Contains(coreEngineData.coreEngineSO.coreEngineType) && !coreEngineData.isActive);
        }

        public List<CoreEngineData> GetCoreEngineDataUnActive()
        {
            return listCoreEngineData.FindAll(coreEngineData => coreEngineData != null && !coreEngineData.isActive);
        }

        public List<CoreEngineData> GetCoreEngineDataActive()
        {
            return listCoreEngineData.FindAll(coreEngineData => coreEngineData != null && coreEngineData.isActive);
        }

        public CoreEngineData GetCoreEngineDataById(int id)
        {
            return listCoreEngineData.Find(coreEngineData => coreEngineData.id == id);
        }

        public int CountCoreEngineIsActive()
        {
            return listCoreEngineData.Count(coreEngineData => coreEngineData.isActive);
        }

        public void ResetListCoreEngineAmountInInventory()
        {
            foreach (CoreEngineAmountData coreEngineAmountData in listCoreEngineAmountInInventory)
            {
                coreEngineAmountData.amount = 0;
            }
        }

        public void AddCoreEngine(CoreEngineData coreEngineData)
        {
            CoreEngineAmountData coreEngineAmountData =
                listCoreEngineAmountInInventory.Find(item => item.coreEngineSO == coreEngineData.coreEngineSO);
            coreEngineAmountData.amount++;
            // listCoreEngineData.Add(coreEngineData);
            OnUpdateAmountCoreEngineHandler?.Invoke(this, new OnUpdateAmountCoreEngineHandlerEventArgs
            {
                NewAmount = coreEngineAmountData.amount,
                CoreEngine = coreEngineAmountData.coreEngineSO
            });
        }

        public void CreateCoreEngine(CoreEngineData coreEngineData)
        {
            CoreEngineAmountData coreEngineAmountData =
                listCoreEngineAmountInInventory.Find(item => item.coreEngineSO == coreEngineData.coreEngineSO);
            coreEngineAmountData.amount++;
            listCoreEngineData.Add(coreEngineData);
            OnUpdateAmountCoreEngineHandler?.Invoke(this, new OnUpdateAmountCoreEngineHandlerEventArgs
            {
                NewAmount = coreEngineAmountData.amount,
                CoreEngine = coreEngineAmountData.coreEngineSO
            });
        }

        public bool RemoveCoreEngine(CoreEngineData coreEngineData)
        {
            CoreEngineAmountData coreEngineAmountData =
                listCoreEngineAmountInInventory.Find(item => item.coreEngineSO == coreEngineData.coreEngineSO);
            if (coreEngineAmountData.amount < 1)
            {
                return false;
            }

            coreEngineAmountData.amount -= 1;

            listCoreEngineData.Remove(coreEngineData);

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
                case ShipSO.ShipType.Pro:
                {
                    return IsContainCoreEngineInInventory(CoreEngineSO.CoreEngineType.Pro);
                }
                case ShipSO.ShipType.GIGA:
                {
                    return IsContainCoreEngineInInventory(CoreEngineSO.CoreEngineType.GIGA);
                }
            }

            return false;
        }

        public void AddCoreEngineToSpaceShip(CoreEngineData coreEngine, ShipData shipData)
        {
            shipData.onDuty = true;
            coreEngine.isActive = true;
            shipData.CoreEngineData = coreEngine;
        }

        public ShipData AddCoreEngineToSpaceShip(int coreEngineId, int shipId)
        {
            CoreEngineData coreEngineData = GetCoreEngineDataById(coreEngineId);
            ShipData shipData = GetShipDataById(shipId);
            AddCoreEngineToSpaceShip(coreEngineData, shipData);
            return shipData;
        }

        public ShipData GetShipDataById(int id)
        {
            return allShip.Find(data => data.id == id);
        }

        // public int SumAmountOfTypeShipAllPlanet(ShipSO.ShipType shipType)
        // {
        //     return planets.Sum(planets => SumAmountOfTypeShipInPlanet(shipType, planets));
        // }

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

        // public float SumHashPowerInAllPlanet(ShipSO.ShipType shipType, PlanetSO planet)
        // {
        //     return planets.Sum(planets => SumHashPowerOfTypeShipInPlanet(shipType, planets));
        // }

        public CoreEngineSO GetCoreEngineByShipType(ShipSO.ShipType shipType)
        {
            switch (shipType)
            {
                case ShipSO.ShipType.Basic: return listCoreEngineSO[0];
                case ShipSO.ShipType.Elite: return listCoreEngineSO[1];
                case ShipSO.ShipType.Pro: return listCoreEngineSO[2];
                case ShipSO.ShipType.GIGA: return listCoreEngineSO[3];
                default: return listCoreEngineSO[0];
            }
        }

        public CoreEngineData GetCoreEngineRandomByShipType(ShipSO.ShipType shipType)
        {
            CoreEngineSO coreEngineSO = GetCoreEngineByShipType(shipType);
            return listCoreEngineData.Find(data =>
                !data.isActive && data.coreEngineSO.coreEngineType == coreEngineSO.coreEngineType);
        }

        public CoreEngineData GetCoreEngineRandomByType(CoreEngineSO.CoreEngineType type)
        {
            return listCoreEngineData.Find(data =>
                !data.isActive && data.coreEngineSO.coreEngineType == type);
        }

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
            return allShip.Count(shipData => shipData != null && shipData.onDuty);
        }

        public int CountAllSpaceShipByType(ShipSO.ShipType shipType)
        {
            return CountAllSpaceShipInInventoryByType(shipType) + CountSpaceShipAddedStationByType(shipType);
        }

        public int CountAllSpaceShipInInventoryByType(ShipSO.ShipType shipType)
        {
            return shipInInventory.Count(shipData => shipData != null && shipData.shipSO.shipType == shipType);
        }

        public int CountAllCoreEngineInInventoryByType(CoreEngineSO.CoreEngineType coreEngineType, bool isActive)
        {
            return listCoreEngineData.Count(coreEngineData =>
                coreEngineData != null && coreEngineData.coreEngineSO.coreEngineType == coreEngineType &&
                coreEngineData.isActive == isActive);
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
                .Count(shipData => shipData != null && shipData.CoreEngineData != null);
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
                .Count(shipData => shipData != null && shipData.CoreEngineData != null &&
                                   shipData.CoreEngineData.coreEngineSO.coreEngineType == coreEngineType);
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

        public List<CoreEngineData> GetListCoreEngineByType(CoreEngineSO.CoreEngineType coreEngineType)
        {
            return listCoreEngineData.FindAll(coreEngineData =>
                coreEngineData.coreEngineSO.coreEngineType == coreEngineType);
        }

        public List<CoreEngineData> GetListCoreEngineByType(string type)
        {
            CoreEngineSO coreEngineSo = GetCoreEngineByType(type);
            return listCoreEngineData.FindAll(coreEngineData =>
                coreEngineData.coreEngineSO.coreEngineType == coreEngineSo.coreEngineType);
        }

        public List<CoreEngineData> GetListCoreEngineUnActiveByShipType(ShipSO.ShipType shipType)
        {
            CoreEngineSO coreEngineSo = GetCoreEngineByShipType(shipType);
            return listCoreEngineData.FindAll(coreEngineData =>
                coreEngineData.coreEngineSO.coreEngineType == coreEngineSo.coreEngineType && !coreEngineData.isActive);
        }

        public bool IsContainCoreEngineUnActiveByShipType(ShipSO.ShipType shipType)
        {
            List<CoreEngineData> listCoreEngine = GetListCoreEngineUnActiveByShipType(shipType);
            return listCoreEngine.Count > 0;
        }


        private void WebResponseOnResponseEngineConfigsHandler(object sender,
            WebResponse.OnResponseEngineConfigsEventArgs e)
        {
            List<EngineConfigDTO> engineConfigDto = e.Data;
            for (int i = 0; i < engineConfigDto.Count; i++)
            {
                CoreEngineSO coreEngineSo = GetCoreEngineByType(engineConfigDto[i].engineType);
                coreEngineSo.cost = engineConfigDto[i].mintCost;
                coreEngineSo.repairCostBase = engineConfigDto[i].repairCostBase;
                coreEngineSo.durabilityBlock = engineConfigDto[i].durability;
                coreEngineSo.efficiencyBonus = engineConfigDto[i].efficiencyBonus;
            }
        }


        private void InstanceOnOnResponseRemainingBlockForHavingEventHandler(object sender,
            WebResponse.OnResponseRemainingBlockForHavingEventArgs e)
        {
            _nextHavingSecond = e.Data.estimateSeconds;
        }

        private void InstanceOnOnResponseUserHashPowerEventHandler(object sender,
            WebResponse.OnResponseUserHashPowerEventArgs e)
        {
            YourPower = e.Data.userHashPower;
        }

        private void InstanceOnOnResponseTotalHashPowerEventHandler(object sender,
            WebResponse.OnResponseTotalHashPowerEventArgs e)
        {
            GlobalHashPower = e.Data.totalHashPower;
        }

        private void WebResponseOnResponseGetPendingRewardEventHandler(object sender,
            WebResponse.OnResponseGetPendingRewardEventArgs e)
        {
            PendingReward = e.Data.pendingReward;
        }

        [SerializeField] private SpaceStationSO spaceStationSo;

        private void WebResponseOnResponseStationLevelsConfigHandler(object sender,
            WebResponse.OnResponseStationLevelsConfigEventArgs e)
        {
            List<ResponseStationLevelsConfigDTO> responseMinerLevelConfigs = e.Data;

            spaceStationSo.listCostPerLevel.Clear();
            spaceStationSo.maxLevel = responseMinerLevelConfigs.Count;
            foreach (ResponseStationLevelsConfigDTO responseMinerLevelsConfig in responseMinerLevelConfigs)
            {
                spaceStationSo.listCostPerLevel.Add(responseMinerLevelsConfig.mineRequired);
            }
        }

        private void WebResponseOnResponseMinerLevelsConfigHandler(object sender,
            WebResponse.OnResponseMinerLevelsConfigEventArgs e)
        {
            List<ResponseMinerLevelsConfigDTO> responseMinerLevelConfigs = e.Data;
            foreach (ShipSO shipSO in AllShipSO)
            {
                shipSO.costPerLevel.Clear();
                foreach (ResponseMinerLevelsConfigDTO responseMinerLevelsConfig in responseMinerLevelConfigs)
                {
                    shipSO.costPerLevel.Add(responseMinerLevelsConfig.mineRequired);
                }
            }
        }

        private void WebResponseOnLoadFullBaseData(object sender, EventArgs e)
        {
            shipInInventory.Clear();
            List<ShipData> listShipIgnore = new List<ShipData>();
            for (int i = 0; i < listStationDto.Count; i++)
            {
                StationDTO stationDto = listStationDto[i];
                foreach (MinerAssigned minerAssigned in stationDto.MinersAssigned)
                {
                    ShipData shipData = allShip.Find(shipData => shipData.id == minerAssigned.tokenId);
                    listStationData[i].ListShipData[minerAssigned.slot - 1] = shipData;
                    listShipIgnore.Add(shipData);
                }
            }

            foreach (ShipData shipData in allShip)
            {
                if (listShipIgnore.Contains(shipData)) continue;
                shipInInventory.Add(shipData);
            }

            foreach (var shipDto in _listShipDto)
            {
                if (!shipDto.isIgnited) continue;

                CoreEngineData coreEngineData = GetCoreEngineDataById(shipDto.coreEngineId);
                ShipData shipData = GetShipDataById(shipDto.tokenId);
                shipData.CoreEngineData = coreEngineData;
                shipData.onDuty = true;
            }

            GameManager.Instance.MoveToNewStation(listStationData[0]);
            int countLostDurability = 0;
            foreach (var coreEngineData in listCoreEngineData)
            {
                if (coreEngineData.IsLostDurability())
                {
                    countLostDurability++;
                }
            }

            if (countLostDurability > 0)
            {
                UIManager.Instance.showNotificationUI.SetUpAndShow(countLostDurability == 1
                    ? "Your one core engine has lost its durability"
                    : $"Your {countLostDurability} core engines have lost their durability.");
            }

            if (!IsEnoughStreak() && RemainingTimeToRecordLogin == 0)
            {
                UIManager.Instance.checkInStreakUI.Show();
            }

            StartCountDownRemainingTimeToRecordLoginCoroutine();

            UIManager.Instance.tabPlanetUI.Show();
        }

        private void WebResponseOnResponseStationsDataEventHandler(object sender,
            WebResponse.OnResponseStationsDataEventArgs e)
        {
            listStationDto = e.Data;

            for (int i = 0; i < listStationDto.Count; i++)
            {
                listStationData[i].level = listStationDto[i].level;
                listStationData[i].id = listStationDto[i].id;
                listStationData[i].pendingMineTime = listStationDto[i].estimateSecond;
                listStationData[i].pendingDownGrade = listStationDto[i].pendingDowngrade;
                listStationData[i].ResetShipData();
            }
        }

        private void WebResponseOnResponseCoreEnginesDataEventHandler(object sender,
            WebResponse.OnResponseCoreEnginesDataEventArgs e)
        {
            listCoreEngineData.Clear();
            ResetListCoreEngineAmountInInventory();
            List<CoreEngineDTO> listCoreEngineDtos = e.Data;
            foreach (CoreEngineDTO coreEngineDto in listCoreEngineDtos)
            {
                CoreEngineSO coreEngineSo = GetCoreEngineByType(coreEngineDto.engineType);
                CoreEngineData coreEngineData =
                    new CoreEngineData(coreEngineDto.tokenId, coreEngineSo, coreEngineDto.isActive,
                        coreEngineDto.blocksUsed, coreEngineDto.currentEfficiencyBonus,
                        coreEngineDto.lastUsedBlock);
                CreateCoreEngine(coreEngineData);
            }
        }

        private List<ShipDTO> _listShipDto = new();

        private void WebResponseOnResponseMinersDataEventHandler(object sender,
            WebResponse.OnResponseMinersDataEventArgs e)
        {
            shipInInventory.Clear();
            allShip.Clear();
            _listShipDto = e.Data;
            foreach (ShipDTO shipDto in _listShipDto)
            {
                ShipData shipData = new ShipData(shipDto.tokenId, GetShipSoByType(shipDto.tier), shipDto.level,
                    shipDto.hashPower, shipDto.efficiency, shipDto.isIgnited);
                // ShipInInventory.Add(shipData);
                allShip.Add(shipData);
            }
        }

        public event EventHandler<OnPendingRewardChangeEventArgs> OnPendingRewardChangeEventHandler;

        public class OnPendingRewardChangeEventArgs : EventArgs
        {
            public int newValue { get; set; }
        }

        private int _pendingReward;

        public int PendingReward
        {
            get => _pendingReward;
            set
            {
                _pendingReward = value;
                OnPendingRewardChangeEventHandler?.Invoke(this, new() { newValue = value });
            }
        }

        public IEnumerator RefreshPendingRewardCoroutine()
        {
            while (true)
            {
                if (UserData != null)
                {
                    WebRequest.CallRequestGetPendingReward();
                }

                yield return new WaitForSeconds(30);
            }
        }

        public EventHandler OnCountDowngradeStationChangeEventHandler;

        public IEnumerator CountDownDowngradeStationCoroutine()
        {
            while (true)
            {
                // if (UserData == null)
                // {
                foreach (StationData stationData in listStationData)
                {
                    if (stationData.pendingMineTime > 0)
                    {
                        stationData.pendingMineTime--;
                    }
                }

                OnCountDowngradeStationChangeEventHandler?.Invoke(this, EventArgs.Empty);
                // }
                yield return new WaitForSeconds(1);
            }
        }

        public event EventHandler OnMonthlyPoolChangeEventHandler;
        private int _monthlyPool;

        public int MonthlyPool
        {
            get => _monthlyPool;
            set
            {
                _monthlyPool = value;
                OnMonthlyPoolChangeEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler OnGlobalHashPowerChangeEventHandler;
        private float _globalHashPower;

        public float GlobalHashPower
        {
            get => _globalHashPower;
            set
            {
                _globalHashPower = value;
                OnGlobalHashPowerChangeEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public IEnumerator RefreshGlobalHashPowerCoroutine()
        {
            while (true)
            {
                WebRequest.CallRequestTotalHashPower();
                yield return new WaitForSeconds(30);
            }
        }

        public event EventHandler OnNextHavingSecondChangeEventHandler;
        private int _nextHavingSecond;

        public int NextHavingSecond
        {
            get => _nextHavingSecond;
            set
            {
                _nextHavingSecond = value;
                OnNextHavingSecondChangeEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler OnYourPowerChangeEventHandler;
        private float _yourPower;

        public float YourPower
        {
            get => _yourPower;
            set
            {
                _yourPower = value;
                OnYourPowerChangeEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        [SerializeField] private int currentBlock;

        public int CurrentBlock
        {
            get => currentBlock;
        }

        public IEnumerator RefreshCurrentBlockCoroutine()
        {
            while (true)
            {
                WebRequest.CallRequestCurrentBlock();
                yield return new WaitForSeconds(15);
            }
        }

        private void InstanceOnOnResponseCurrentBlockEventHandler(object sender,
            WebResponse.OnResponseCurrentBlockEventArgs e)
        {
            currentBlock = e.Data.currentBlock;
            // Helpers.LogCaller(currentBlock);
            foreach (CoreEngineData coreEngineData in listCoreEngineData)
            {
                if (coreEngineData.isActive && coreEngineData.IsLostDurability(currentBlock))
                {
                    ShipData shipData = GetShipDataByCoreEngineId(coreEngineData.id);
                    if (shipData != null)
                    {
                        coreEngineData.ResetBlockUsed();
                        GameManager.Instance.CallbackSpaceShip(shipData);
                    }
                }
            }
        }

        private int _streak = 0;

        public int Streak
        {
            get => _streak;
            set => _streak = value;
        }

        private int _remainingTimeToRecordLogin = 0;

        public int RemainingTimeToRecordLogin
        {
            get => _remainingTimeToRecordLogin;
            set => _remainingTimeToRecordLogin = value;
        }

        public IEnumerator CountDownRemainingTimeToRecordLoginCoroutine()
        {
            while (_remainingTimeToRecordLogin > 0)
            {
                _remainingTimeToRecordLogin--;
                yield return new WaitForSeconds(1);
            }

            if (StreakToClaimReward != 0)
            {
                Helpers.LogCaller(_remainingTimeToRecordLogin);
                UIManager.Instance.checkInStreakUI.Show();
            }
        }

        public void StartCountDownRemainingTimeToRecordLoginCoroutine()
        {
            StartCoroutine(CountDownRemainingTimeToRecordLoginCoroutine());
        }

        private int _streakToClaimReward = 0;

        public int StreakToClaimReward
        {
            get => _streakToClaimReward;
            set => _streakToClaimReward = value;
        }

        public bool IsEnoughStreak()
        {
            return Streak >= StreakToClaimReward;
        }

        public int GetTimeLeftStreak()
        {
            return StreakToClaimReward - Streak;
        }

        private void InstanceOnOnResponseLoginStreakEventHandler(object sender,
            WebResponse.OnResponseLoginStreakEventArgs e)
        {
            ResponseLoginStreakDTO responseLoginStreakDTO = e.Data;
            Streak = responseLoginStreakDTO.currentStreak;
            RemainingTimeToRecordLogin = responseLoginStreakDTO.remainingTimeToRecordLogin;
            StreakToClaimReward = responseLoginStreakDTO.streakToClaimReward;
        }

        public ShipData GetShipDataByCoreEngineId(int coreEngineId)
        {
            return allShip.FirstOrDefault(shipData =>
                shipData.CoreEngineData != null && shipData.CoreEngineData.id == coreEngineId);
        }

        private List<TicketData> _listTicketData = new();

        public List<TicketData> ListTicketData
        {
            get => _listTicketData;
            set => _listTicketData = value;
        }

        public TicketData GetLastTicketData()
        {
            return _listTicketData.LastOrDefault();
        }

        private void Start()
        {
#if UNITY_EDITOR
            for (int i = 0; i < ShipInInventory.Count; i++)
            {
                shipInInventory[i].id = i;
            }

            foreach (var shipData in shipInInventory)
            {
                allShip.Add(shipData);
            }

            foreach (CoreEngineData coreEngineData in listCoreEngineData)
            {
                AddCoreEngine(coreEngineData);
            }
#endif
            StartCoroutine(RefreshPendingRewardCoroutine());
            StartCoroutine(CountDownDowngradeStationCoroutine());
            StartCoroutine(RefreshGlobalHashPowerCoroutine());
            StartCoroutine(RefreshCurrentBlockCoroutine());
            WebResponse.Instance.OnResponseCurrentBlockEventHandler += InstanceOnOnResponseCurrentBlockEventHandler;
            WebResponse.Instance.OnResponseUserHashPowerEventHandler += InstanceOnOnResponseUserHashPowerEventHandler;
            WebResponse.Instance.OnResponseTotalHashPowerEventHandler += InstanceOnOnResponseTotalHashPowerEventHandler;
            WebResponse.Instance.OnResponseGetPendingRewardEventHandler +=
                WebResponseOnResponseGetPendingRewardEventHandler;
            WebResponse.Instance.OnResponseLoginStreakEventHandler += InstanceOnOnResponseLoginStreakEventHandler;
            WebResponse.Instance.OnResponseMinersDataEventHandler += WebResponseOnResponseMinersDataEventHandler;
            WebResponse.Instance.OnResponseCoreEnginesDataEventHandler +=
                WebResponseOnResponseCoreEnginesDataEventHandler;
            WebResponse.Instance.OnResponseStationsDataEventHandler += WebResponseOnResponseStationsDataEventHandler;
            WebResponse.Instance.OnResponseMinerLevelsConfigHandler += WebResponseOnResponseMinerLevelsConfigHandler;
            WebResponse.Instance.OnResponseEngineConfigsHandler += WebResponseOnResponseEngineConfigsHandler;
            WebResponse.Instance.OnResponseStationLevelsConfigHandler +=
                WebResponseOnResponseStationLevelsConfigHandler;

            WebRequest.CallRequestRemainingBlockForHaving();
            WebResponse.Instance.OnResponseRemainingBlockForHavingEventHandler +=
                InstanceOnOnResponseRemainingBlockForHavingEventHandler;
            WebResponse.Instance.OnLoadFullBaseData += WebResponseOnLoadFullBaseData;
        }
    }
}