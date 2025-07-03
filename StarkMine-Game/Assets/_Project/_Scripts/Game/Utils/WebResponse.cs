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
    private bool _isLoadStationLevelsConfigSuccess;
    private bool _isFirstLoad;

    private void Update()
    {
        if (IsLoadBaseDataSuccess() && !_isFirstLoad)
        {
            OnLoadFullBaseData?.Invoke(this, EventArgs.Empty);
            UIManager.Instance.loadingUI.Hide();
            _isFirstLoad = true;
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
               _isLoadMinerLevelConfigSuccess && _isLoadStationLevelsConfigSuccess;
    }

    public void ResetLoadData()
    {
        _isLoadShipDataSuccess = false;
        _isLoadCoreEngineSuccess = false;
        _isLoadStationDataSuccess = false;
        _isLoadMinerLevelConfigSuccess = false;
        _isLoadStationLevelsConfigSuccess = false;
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
            HandleUnSuccess(response.level, response.message);
            return;
        }

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
        if (_isLoadStationDataSuccess) return;

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
}