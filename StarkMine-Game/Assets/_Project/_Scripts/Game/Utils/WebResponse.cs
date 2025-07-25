using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class WebResponse : StaticInstance<WebResponse>
{
    #region Common

    public event EventHandler OnLoadFullBaseData;

    private bool _isLoadShipDataSuccess;
    private bool _isLoadCoreEngineSuccess;
    private bool _isLoadStationDataSuccess;
    private bool _isLoadMinerLevelConfigSuccess;
    private bool _isLoadCoreEngineConfigSuccess;
    private bool _isLoadStationLevelsConfigSuccess;
    private bool _isLoadLoginStreakSuccess;
    private bool _isFirstLoad;

    private void Update()
    {
        if (IsLoadBaseDataSuccess())
        {
            OnLoadFullBaseData?.Invoke(this, EventArgs.Empty);
            UIManager.Instance.loadingUI.Hide();
            ResetLoadData();
            // _isFirstLoad = true;
            Debug.Log("================Load Full Data===================");
        }
    }

    private MessageBase<T> DeserializeMessage<T>(string responseString) where T : JToken
    {
        return JsonConvert.DeserializeObject<MessageBase<T>>(responseString);
    }

    private void HandleUnSuccess(MessageLevel level, string message)
    {
        switch (level)
        {
            case MessageLevel.WARNING:
                UIManager.Instance.showNotificationUI.SetUp(message);
                UIManager.Instance.showNotificationUI.Show();
                break;
            case MessageLevel.ERROR:
                UIManager.Instance.showNotificationCantOffUI.SetUp(message);
                UIManager.Instance.showNotificationCantOffUI.Show();
                break;
        }
    }

    private bool IsLoadBaseDataSuccess()
    {
        return _isLoadShipDataSuccess && _isLoadCoreEngineSuccess && _isLoadStationDataSuccess &&
               _isLoadMinerLevelConfigSuccess && _isLoadStationLevelsConfigSuccess && _isLoadCoreEngineConfigSuccess &&
               _isLoadLoginStreakSuccess;
    }

    public void ResetLoadData()
    {
        _isLoadShipDataSuccess = false;
        _isLoadCoreEngineSuccess = false;
        _isLoadStationDataSuccess = false;
        _isLoadMinerLevelConfigSuccess = false;
        _isLoadCoreEngineConfigSuccess = false;
        _isLoadStationLevelsConfigSuccess = false;
        _isLoadLoginStreakSuccess = false;
        Debug.Log("================Reset Load Data===================");
    }

    #endregion

    #region ResponseConnectWallet

    public event EventHandler<OnResponseConnectWalletEventArgs> OnResponseConnectWalletEventHandler;

    public class OnResponseConnectWalletEventArgs : EventArgs
    {
        public UserDTO Data { get; set; }
    }

    private void ResponseConnectWallet(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.showNotificationCantOffUI.Hide();
        UIManager.Instance.showNotificationUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            return;
        }

        OnResponseConnectWalletEventHandler?.Invoke(this,
            new OnResponseConnectWalletEventArgs { Data = response.data.ToObject<UserDTO>() });
    }

    #endregion

    #region ResponseGetPendingReward

    public event EventHandler<OnResponseGetPendingRewardEventArgs> OnResponseGetPendingRewardEventHandler;

    public class OnResponseGetPendingRewardEventArgs : EventArgs
    {
        public ResponseGetPendingRewardDTO Data { get; set; }
    }

    private void ResponseGetPendingReward(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);

        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            return;
        }

        OnResponseGetPendingRewardEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseGetPendingRewardDTO>() });
    }

    public void InvokeResponseGetPendingReward(
        OnResponseGetPendingRewardEventArgs onResponseGetPendingRewardEventArgs)
    {
        OnResponseGetPendingRewardEventHandler?.Invoke(this, onResponseGetPendingRewardEventArgs);
    }

    #endregion

    #region ResponseLoginStreak

    public event EventHandler<OnResponseLoginStreakEventArgs> OnResponseLoginStreakEventHandler;

    public class OnResponseLoginStreakEventArgs : EventArgs
    {
        public ResponseLoginStreakDTO Data { get; set; }
    }

    private void ResponseLoginStreak(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);

        if (_isLoadLoginStreakSuccess) return;
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);

        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            return;
        }

        _isLoadLoginStreakSuccess = true;
        OnResponseLoginStreakEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseLoginStreakDTO>() });
    }

    #endregion

    #region ResponseGetPendingReward

    public event EventHandler<OnResponseClaimPendingRewardEventArgs> OnResponseClaimPendingRewardEventHandler;

    public class OnResponseClaimPendingRewardEventArgs : EventArgs
    {
        public ResponseClaimPendingRewardDTO Data { get; set; }
    }

    public event EventHandler OnResponseClaimPendingRewardFailEventHandler;

    private void ResponseClaimPendingReward(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();

        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseClaimPendingRewardFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseClaimPendingRewardEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseClaimPendingRewardDTO>() });
    }

    #endregion

    #region ResponseMinersData

    public event EventHandler<OnResponseMinersDataEventArgs> OnResponseMinersDataEventHandler;

    public class OnResponseMinersDataEventArgs : EventArgs
    {
        public List<ShipDTO> Data { get; set; }
    }

    private void ResponseMinersData(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);

        if (_isLoadShipDataSuccess) return;
        MessageBase<JArray> response = DeserializeMessage<JArray>(responseString);

        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            return;
        }

        _isLoadShipDataSuccess = true;
        OnResponseMinersDataEventHandler?.Invoke(this,
            new OnResponseMinersDataEventArgs { Data = response.data.ToObject<List<ShipDTO>>() });
    }

    #endregion

    #region ResponseCoreEnginesData

    public event EventHandler<OnResponseCoreEnginesDataEventArgs> OnResponseCoreEnginesDataEventHandler;

    public class OnResponseCoreEnginesDataEventArgs : EventArgs
    {
        public List<CoreEngineDTO> Data { get; set; }
    }

    private void ResponseCoreEnginesData(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);

        if (_isLoadCoreEngineSuccess) return;

        MessageBase<JArray> response = DeserializeMessage<JArray>(responseString);

        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            return;
        }

        _isLoadCoreEngineSuccess = true;
        OnResponseCoreEnginesDataEventHandler?.Invoke(this,
            new OnResponseCoreEnginesDataEventArgs
                { Data = response.data.ToObject<List<CoreEngineDTO>>() });
    }

    #endregion

    #region ResponseStationsData

    public event EventHandler<OnResponseStationsDataEventArgs> OnResponseStationsDataEventHandler;

    public class OnResponseStationsDataEventArgs : EventArgs
    {
        public List<StationDTO> Data { get; set; }
    }

    private void ResponseStationsData(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);

        if (_isLoadStationDataSuccess) return;

        MessageBase<JArray> response = DeserializeMessage<JArray>(responseString);
        if (!response.IsSuccess())
        {
            UIManager.Instance.initStationUI.Show();
            UIManager.Instance.loadingUI.Hide();

            // HandleUnSuccess(response.level, response.message);
            return;
        }

        UIManager.Instance.initStationUI.Hide();
        _isLoadStationDataSuccess = true;
        OnResponseStationsDataEventHandler?.Invoke(this,
            new OnResponseStationsDataEventArgs { Data = response.data.ToObject<List<StationDTO>>() });
    }

    #endregion

    #region ResponseMinerLevelsConfig

    public event EventHandler<OnResponseMinerLevelsConfigEventArgs> OnResponseMinerLevelsConfigHandler;

    public class OnResponseMinerLevelsConfigEventArgs : EventArgs
    {
        public List<ResponseMinerLevelsConfigDTO> Data { get; set; }
    }

    private void ResponseMinerLevelsConfig(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);

        if (_isLoadMinerLevelConfigSuccess) return;

        MessageBase<JArray> response = DeserializeMessage<JArray>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            return;
        }

        _isLoadMinerLevelConfigSuccess = true;
        OnResponseMinerLevelsConfigHandler?.Invoke(this,
            new OnResponseMinerLevelsConfigEventArgs
                { Data = response.data.ToObject<List<ResponseMinerLevelsConfigDTO>>() });
    }

    #endregion

    #region ResponseEngineConfigs

    public event EventHandler<OnResponseEngineConfigsEventArgs> OnResponseEngineConfigsHandler;

    public class OnResponseEngineConfigsEventArgs : EventArgs
    {
        public List<EngineConfigDTO> Data { get; set; }
    }

    private void ResponseEngineConfigs(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);

        if (_isLoadCoreEngineConfigSuccess) return;

        MessageBase<JArray> response = DeserializeMessage<JArray>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            return;
        }

        _isLoadCoreEngineConfigSuccess = true;
        OnResponseEngineConfigsHandler?.Invoke(this,
            new() { Data = response.data.ToObject<List<EngineConfigDTO>>() });
    }

    #endregion

    #region ResponseStationLevelsConfig

    public event EventHandler<OnResponseStationLevelsConfigEventArgs> OnResponseStationLevelsConfigHandler;

    public class OnResponseStationLevelsConfigEventArgs : EventArgs
    {
        public List<ResponseStationLevelsConfigDTO> Data { get; set; }
    }

    private void ResponseStationLevelsConfig(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);

        if (_isLoadStationLevelsConfigSuccess) return;

        MessageBase<JArray> response = DeserializeMessage<JArray>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            return;
        }

        _isLoadStationLevelsConfigSuccess = true;
        OnResponseStationLevelsConfigHandler?.Invoke(this,
            new OnResponseStationLevelsConfigEventArgs
                { Data = response.data.ToObject<List<ResponseStationLevelsConfigDTO>>() });
    }

    #endregion

    #region ResponseAssignMinerToStation

    public event EventHandler<OnResponseAssignMinerToStationEventArgs> OnResponseAssignMinerToStationEventHandler;

    public class OnResponseAssignMinerToStationEventArgs : EventArgs
    {
        public AssignMinerToStationDTO Data;
    }

    private void ResponseAssignMinerToStation(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);

        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            return;
        }

        OnResponseAssignMinerToStationEventHandler?.Invoke(this,
            new OnResponseAssignMinerToStationEventArgs { Data = response.data.ToObject<AssignMinerToStationDTO>() });
    }

    public void InvokeResponseAssignMinerToStation(
        OnResponseAssignMinerToStationEventArgs onResponseAssignMinerToStationEventArgs)
    {
        OnResponseAssignMinerToStationEventHandler?.Invoke(this, onResponseAssignMinerToStationEventArgs);
    }

    #endregion

    #region ResponseRemoveMinerFromStation

    public event EventHandler<OnResponseRemoveMinerFromStationEventArgs> OnResponseRemoveMinerFromStationEventHandler;

    public class OnResponseRemoveMinerFromStationEventArgs : EventArgs
    {
        public RemoveMinerFromStationDTO Data;
    }

    private void ResponseRemoveMinerFromStation(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            return;
        }

        OnResponseRemoveMinerFromStationEventHandler?.Invoke(this,
            new OnResponseRemoveMinerFromStationEventArgs
                { Data = response.data.ToObject<RemoveMinerFromStationDTO>() });
    }

    public void InvokeResponseRemoveMinerFromStation(
        OnResponseRemoveMinerFromStationEventArgs onResponseRemoveMinerFromStationEventArgs)
    {
        OnResponseRemoveMinerFromStationEventHandler?.Invoke(this, onResponseRemoveMinerFromStationEventArgs);
    }

    #endregion

    #region ResponseIgniteMiner

    public event EventHandler<OnResponseIgniteMinerEventArgs> OnResponseIgniteMinerEventHandler;
    public event EventHandler OnResponseIgniteMinerFailEventHandler;

    public class OnResponseIgniteMinerEventArgs : EventArgs
    {
        public ResponseIgniteMinerDTO Data;
    }

    private void ResponseIgniteMiner(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseIgniteMinerFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseIgniteMinerEventHandler?.Invoke(this,
            new OnResponseIgniteMinerEventArgs { Data = response.data.ToObject<ResponseIgniteMinerDTO>() });
    }

    public void InvokeResponseIgniteMiner(
        OnResponseIgniteMinerEventArgs onResponseIgniteMinerEventArgs)
    {
        OnResponseIgniteMinerEventHandler?.Invoke(this, onResponseIgniteMinerEventArgs);
    }

    #endregion

    #region ResponseExtinguishMiner

    public event EventHandler<OnResponseExtinguishMinerEventArgs> OnResponseExtinguishMinerEventHandler;

    public class OnResponseExtinguishMinerEventArgs : EventArgs
    {
        public ResponseExtinguishMinerDTO Data;
    }

    public event EventHandler OnResponseExtinguishMinerFailEventHandler;

    private void ResponseExtinguishMiner(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseExtinguishMinerFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseExtinguishMinerEventHandler?.Invoke(this,
            new OnResponseExtinguishMinerEventArgs { Data = response.data.ToObject<ResponseExtinguishMinerDTO>() });
    }

    public void InvokeResponseExtinguishMiner(
        OnResponseExtinguishMinerEventArgs onResponseExtinguishMinerEventArgs)
    {
        OnResponseExtinguishMinerEventHandler?.Invoke(this, onResponseExtinguishMinerEventArgs);
    }

    #endregion

    #region ResponseUpgradeStation

    public event EventHandler<OnResponseUpgradeStationEventArgs> OnResponseUpgradeStationEventHandler;

    public class OnResponseUpgradeStationEventArgs : EventArgs
    {
        public ResponseUpgradeStationDTO Data;
    }

    public event EventHandler OnResponseUpgradeStationFailEventHandler;

    private void ResponseUpgradeStation(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseUpgradeStationFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseUpgradeStationEventHandler?.Invoke(this,
            new OnResponseUpgradeStationEventArgs { Data = response.data.ToObject<ResponseUpgradeStationDTO>() });
    }

    public void InvokeResponseUpgradeStation(
        OnResponseUpgradeStationEventArgs onResponseUpgradeStationEventArgs)
    {
        OnResponseUpgradeStationEventHandler?.Invoke(this, onResponseUpgradeStationEventArgs);
    }

    #endregion

    #region ResponseUpgradeStation

    public event EventHandler<OnResponseRequestDowngradeStationEventArgs> OnResponseRequestDowngradeStationEventHandler;

    public class OnResponseRequestDowngradeStationEventArgs : EventArgs
    {
        public ResponseDowngradeStationDTO Data;
    }

    public event EventHandler OnResponseRequestDowngradeStationFailEventHandler;

    private void ResponseRequestDowngradeStation(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseRequestDowngradeStationFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseRequestDowngradeStationEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseDowngradeStationDTO>() });
    }

    public void InvokeResponseRequestDowngradeStation(
        OnResponseRequestDowngradeStationEventArgs onResponseRequestDowngradeStationEventArgs)
    {
        OnResponseRequestDowngradeStationEventHandler?.Invoke(this, onResponseRequestDowngradeStationEventArgs);
    }

    #endregion

    #region ResponseMintCoreEngine

    public event EventHandler<OnResponseMintCoreEngineEventArgs> OnResponseMintCoreEngineEventHandler;

    public class OnResponseMintCoreEngineEventArgs : EventArgs
    {
        public ResponseMintCoreEngineDTO Data;
    }

    public event EventHandler OnResponseMintCoreEngineFailEventHandler;

    private void ResponseMintCoreEngine(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseMintCoreEngineFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseMintCoreEngineEventHandler?.Invoke(this,
            new OnResponseMintCoreEngineEventArgs { Data = response.data.ToObject<ResponseMintCoreEngineDTO>() });
    }

    public void InvokeResponseMintCoreEngine(
        OnResponseMintCoreEngineEventArgs onResponseMintCoreEngineEventArgs)
    {
        OnResponseMintCoreEngineEventHandler?.Invoke(this, onResponseMintCoreEngineEventArgs);
    }

    #endregion

    #region ResponseDefuseEngine

    public event EventHandler<OnResponseDefuseEngineEventArgs> OnResponseDefuseEngineEventHandler;

    public class OnResponseDefuseEngineEventArgs : EventArgs
    {
        public ResponseDefuseEngineDTO Data;
    }

    public event EventHandler OnResponseDefuseEngineFailEventHandler;

    private void ResponseDefuseEngine(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseDefuseEngineFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseDefuseEngineEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseDefuseEngineDTO>() });
    }

    public void InvokeResponseDefuseEngine(
        OnResponseDefuseEngineEventArgs onResponseMintCoreEngineEventArgs)
    {
        OnResponseDefuseEngineEventHandler?.Invoke(this, onResponseMintCoreEngineEventArgs);
    }

    #endregion

    #region ResponseUpgradeMiner

    public event EventHandler<OnResponseUpgradeMinerEventArgs> OnResponseUpgradeMinerEventHandler;

    public class OnResponseUpgradeMinerEventArgs : EventArgs
    {
        public ResponseUpgradeMinerDTO Data;
    }

    public event EventHandler OnResponseUpgradeMinerFailEventHandler;

    private void ResponseUpgradeMiner(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseUpgradeMinerFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseUpgradeMinerEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseUpgradeMinerDTO>() });
    }

    public void InvokeResponseUpgradeMinerEngine(
        OnResponseUpgradeMinerEventArgs onResponseUpgradeMinerEventArgs)
    {
        OnResponseUpgradeMinerEventHandler?.Invoke(this, onResponseUpgradeMinerEventArgs);
    }

    #endregion

    #region ResponseMergeMiner

    public event EventHandler<OnResponseMergeMinerEventArgs> OnResponseMergeMinerEventHandler;

    public class OnResponseMergeMinerEventArgs : EventArgs
    {
        public ResponseMergeMinerDTO Data;
    }

    public event EventHandler OnResponseMergeMinerFailEventHandler;

    private void ResponseMergeMiner(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseMergeMinerFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseMergeMinerEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseMergeMinerDTO>() });
    }

    public void InvokeResponseMergeMiner(
        OnResponseMergeMinerEventArgs onResponseMergeMinerEventArgs)
    {
        OnResponseMergeMinerEventHandler?.Invoke(this, onResponseMergeMinerEventArgs);
    }

    #endregion

    #region ResponseMergeMiner

    public event EventHandler<OnResponseCurrentSuccessRateEventArgs> OnResponseCurrentMergeStatusByUserEventHandler;

    public class OnResponseCurrentSuccessRateEventArgs : EventArgs
    {
        public ResponseCurrentMergeStatusByUserDTO Data;
    }

    public event EventHandler OnResponseCurrentMergeStatusByUserFailEventHandler;

    private void ResponseCurrentMergeStatusByUser(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseCurrentMergeStatusByUserFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseCurrentMergeStatusByUserEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseCurrentMergeStatusByUserDTO>() });
    }

    public void InvokeResponseCurrentSuccessRate(
        OnResponseCurrentSuccessRateEventArgs onResponseCurrentSuccessRateEventArgs)
    {
        OnResponseCurrentMergeStatusByUserEventHandler?.Invoke(this, onResponseCurrentSuccessRateEventArgs);
    }

    #endregion

    #region ResponseExecuteDowngrade

    public event EventHandler<OnResponseExecuteDowngradeEventArgs> OnResponseExecuteDowngradeEventHandler;

    public class OnResponseExecuteDowngradeEventArgs : EventArgs
    {
        public ResponseExecuteDowngradeDTO Data;
    }

    public event EventHandler OnResponseExecuteDowngradeFailEventHandler;

    private void ResponseExecuteDowngrade(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseExecuteDowngradeFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseExecuteDowngradeEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseExecuteDowngradeDTO>() });
    }

    public void InvokeResponseExecuteDowngrade(
        OnResponseExecuteDowngradeEventArgs response)
    {
        OnResponseExecuteDowngradeEventHandler?.Invoke(this, response);
    }

    #endregion

    #region ResponseRepairCoreEngine

    public event EventHandler<OnResponseRepairCoreEngineEventArgs> OnResponseRepairCoreEngineEventHandler;

    public class OnResponseRepairCoreEngineEventArgs : EventArgs
    {
        public ResponseRepairCoreEngineDTO Data;
    }

    public event EventHandler OnResponseRepairCoreEngineFailEventHandler;

    private void ResponseRepairCoreEngine(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        UIManager.Instance.loadingUI.Hide();
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseRepairCoreEngineFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseRepairCoreEngineEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseRepairCoreEngineDTO>() });
    }

    public void InvokeResponseRepairCoreEngine(
        OnResponseRepairCoreEngineEventArgs onResponseRepairCoreEngineEventArgs)
    {
        OnResponseRepairCoreEngineEventHandler?.Invoke(this, onResponseRepairCoreEngineEventArgs);
    }

    #endregion

    #region ResponseTotalHashPower

    public event EventHandler<OnResponseTotalHashPowerEventArgs> OnResponseTotalHashPowerEventHandler;

    public class OnResponseTotalHashPowerEventArgs : EventArgs
    {
        public ResponseTotalHashPowerDTO Data;
    }

    public event EventHandler OnResponseTotalHashPowerFailEventHandler;

    private void ResponseTotalHashPower(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            // HandleUnSuccess(response.level, response.message);
            OnResponseTotalHashPowerFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseTotalHashPowerEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseTotalHashPowerDTO>() });
    }

    #endregion

    #region ResponseUserHashPower

    public event EventHandler<OnResponseUserHashPowerEventArgs> OnResponseUserHashPowerEventHandler;

    public class OnResponseUserHashPowerEventArgs : EventArgs
    {
        public ResponseUserHashPowerDTO Data;
    }

    public event EventHandler OnResponseUserHashPowerFailEventHandler;

    private void ResponseUserHashPower(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            // HandleUnSuccess(response.level, response.message);
            OnResponseUserHashPowerFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseUserHashPowerEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseUserHashPowerDTO>() });
    }

    #endregion

    #region ResponseRemainingBlockForHaving

    public event EventHandler<OnResponseRemainingBlockForHavingEventArgs> OnResponseRemainingBlockForHavingEventHandler;

    public class OnResponseRemainingBlockForHavingEventArgs : EventArgs
    {
        public ResponseRemainingBlockForHavingDTO Data;
    }

    public event EventHandler OnResponseRemainingBlockForHavingFailEventHandler;

    private void ResponseRemainingBlockForHaving(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            // HandleUnSuccess(response.level, response.message);
            OnResponseRemainingBlockForHavingFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseRemainingBlockForHavingEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseRemainingBlockForHavingDTO>() });
    }

    #endregion

    #region ResponseCancelDowngrade

    public event EventHandler<OnResponseCancelDowngradeEventArgs> OnResponseCancelDowngradeEventHandler;

    public class OnResponseCancelDowngradeEventArgs : EventArgs
    {
        public ResponseCancelDowngradeDTO Data;
    }

    public event EventHandler OnResponseCancelDowngradeFailEventHandler;

    private void ResponseCancelDowngrade(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        UIManager.Instance.loadingUI.Hide();
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseCancelDowngradeFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseCancelDowngradeEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseCancelDowngradeDTO>() });
    }

    #endregion

    #region ResponseCurrentBlock

    public event EventHandler<OnResponseCurrentBlockEventArgs> OnResponseCurrentBlockEventHandler;

    public class OnResponseCurrentBlockEventArgs : EventArgs
    {
        public ResponseCurrentBlockDTO Data;
    }

    private void ResponseCurrentBlock(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        if (!response.IsSuccess())
        {
            return;
        }

        OnResponseCurrentBlockEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseCurrentBlockDTO>() });
    }

    public void InvokeResponseCurrentBlock(
        OnResponseCurrentBlockEventArgs responseData)
    {
        OnResponseCurrentBlockEventHandler?.Invoke(this, responseData);
    }

    #endregion

    #region ResponseRecordLogin

    public event EventHandler<OnResponseRecordLoginEventArgs> OnResponseRecordLoginEventHandler;
    public event EventHandler OnResponseRecordLoginFailEventHandler;

    public class OnResponseRecordLoginEventArgs : EventArgs
    {
        public ResponseRecordLoginDTO Data;
    }

    private void ResponseRecordLogin(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        UIManager.Instance.loadingUI.Hide();
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseRecordLoginFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseRecordLoginEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseRecordLoginDTO>() });
    }

    #endregion

    #region ResponseMintTicket

    public event EventHandler<OnResponseMintTicketEventArgs> OnResponseMintTicketEventHandler;
    public event EventHandler OnResponseMintTicketFailEventHandler;

    public class OnResponseMintTicketEventArgs : EventArgs
    {
        public ResponseMintTicketDTO Data;
    }

    private void ResponseMintTicket(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        UIManager.Instance.loadingUI.Hide();
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseMintTicketFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseMintTicketEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseMintTicketDTO>() });
    }

    public void InvokeResponseMintTicket(
        OnResponseMintTicketEventArgs responseData)
    {
        OnResponseMintTicketEventHandler?.Invoke(this, responseData);
    }

    #endregion
    #region ResponseOpenTicket

    public event EventHandler<OnResponseOpenTicketEventArgs> OnResponseOpenTicketEventHandler;
    public event EventHandler OnResponseOpenTicketFailEventHandler;

    public class OnResponseOpenTicketEventArgs : EventArgs
    {
        public ResponseOpenTicketDTO Data;
    }

    private void ResponseOpenTicket(string responseString)
    {
        Debug.Log(MethodBase.GetCurrentMethod()?.Name + " " + responseString);
        MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
        UIManager.Instance.loadingUI.Hide();
        if (!response.IsSuccess())
        {
            HandleUnSuccess(response.level, response.message);
            OnResponseOpenTicketFailEventHandler?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnResponseOpenTicketEventHandler?.Invoke(this,
            new() { Data = response.data.ToObject<ResponseOpenTicketDTO>() });
    }

    public void InvokeResponseOpenTicket(
        OnResponseOpenTicketEventArgs responseData)
    {
        OnResponseOpenTicketEventHandler?.Invoke(this, responseData);
    }

    #endregion
}