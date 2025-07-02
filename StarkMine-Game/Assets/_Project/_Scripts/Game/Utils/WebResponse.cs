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

    private void Update()
    {
        if (_isLoadShipDataSuccess && _isLoadCoreEngineSuccess && _isLoadStationDataSuccess)
        {
            OnLoadFullBaseData?.Invoke(this, EventArgs.Empty);
            UIManager.Instance.loadingUI.Hide();
            ResetLoadData();
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

    private void ResetLoadData()
    {
        _isLoadShipDataSuccess = false;
        _isLoadCoreEngineSuccess = false;
        _isLoadStationDataSuccess = false;
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
        Debug.Log("===ResponseConnectWallet===:" + responseString);
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
        Debug.Log("ResponseMinersData" + responseString);
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
        Debug.Log("ResponseCoreEnginesData" + responseString);
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
        Debug.Log("ResponseStationsData" + responseString);
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

        _isLoadStationDataSuccess = true;
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

    // public event EventHandler<OnResponseIgniteMinerEventArgs> OnResponseIgniteMinerEventHandler;
    //
    // public class OnResponseIgniteMinerEventArgs : EventArgs
    // {
    //     public ResponseIgniteMinerDTO Data;
    // }
    //
    // private void ResponseIgniteMiner(string responseString)
    // {
    //     Debug.Log("ResponseAssignMinerToStation" + responseString);
    //     UIManager.Instance.loadingUI.Hide();
    //     MessageBase<JObject> response = DeserializeMessage<JObject>(responseString);
    //     if (!response.IsSuccess())
    //     {
    //         HandleUnSuccess(response.level, response.message);
    //         return;
    //     }
    //
    //     _isLoadStationDataSuccess = true;
    //     OnResponseAssignMinerToStationEventHandler?.Invoke(this,
    //         new OnResponseAssignMinerToStationEventArgs { Data = response.data.ToObject<AssignMinerToStationDTO>() });
    // }
    //
    // public void StartFakeResponseIgniteMinerCoroutine(int stationId, int minerId, int index)
    // {
    //     StartCoroutine(FakeResponseCoroutine(stationId, minerId, index));
    // }
    //
    // private IEnumerator FakeResponseCoroutine(int stationId, int minerId, int index)
    // {
    //     yield return new WaitForSeconds(0.5f);
    //     UIManager.Instance.loadingUI.Hide();
    //     OnResponseAssignMinerToStationEventHandler?.Invoke(this,
    //         new OnResponseAssignMinerToStationEventArgs()
    //         {
    //             Data = new AssignMinerToStationDTO()
    //             {
    //                 stationId = stationId,
    //                 minerId = minerId,
    //                 index = index
    //             }
    //         });
    // }

    #endregion
}