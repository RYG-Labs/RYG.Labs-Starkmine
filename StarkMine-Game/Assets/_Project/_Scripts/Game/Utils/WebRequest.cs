using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebRequest
{
    [DllImport("__Internal")]
    private static extern void RequestConnectWallet();

    [DllImport("__Internal")]
    private static extern void RequestDisconnectWallet();

    [DllImport("__Internal")]
    private static extern void RequestMinersData();

    [DllImport("__Internal")]
    private static extern void RequestCoreEnginesData();

    [DllImport("__Internal")]
    private static extern void RequestAssignMinerToStation(int stationId, int minerId, int index);

    [DllImport("__Internal")]
    private static extern void RequestRemoveMinerFromStation(int stationId, int minerSlot);

    [DllImport("__Internal")]
    private static extern void RequestIgniteMiner(int minerId, int coreEngineId);

    [DllImport("__Internal")]
    private static extern void RequestExtinguishMiner(int minerId);

    [DllImport("__Internal")]
    private static extern void RequestUpgradeStation(int stationId, int targetLevel);

    [DllImport("__Internal")]
    private static extern void RequestMintCoreEngine(string engineType);

    public static void CallRequestConnectWallet()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestConnectWallet();
#endif
    }

    public static void CallRequestDisconnectWallet()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestDisconnectWallet();
#endif
    }

    public static void CallRequestMinersData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestMinersData();
#endif
    }

    public static void CallRequestCoreEnginesData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestCoreEnginesData();
#endif
    }

    public static void CallRequestAssignMinerToStation(int stationId, int minerId, int index)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestAssignMinerToStation(stationId, minerId, index);
#else
        FakeResponse.Instance.StartFakeResponseAssignMinerToStationCoroutine(stationId, minerId, index);
#endif
    }

    public static void CallRequestRemoveMinerFromStation(int stationId, int minerSlot)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestRemoveMinerFromStation(stationId, minerSlot +1);
#else
        FakeResponse.Instance.StartFakeResponseRemoveMinerFromStationCoroutine(stationId, minerSlot);
#endif
    }

    public static void CallRequestIgniteMiner(int minerId, int coreEngineId)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestIgniteMiner(minerId, coreEngineId);
#else
        FakeResponse.Instance.StartFakeResponseIgniteMinerCoroutine(minerId, coreEngineId);
#endif
    }

    public static void CallRequestExtinguishMiner(int minerId)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestExtinguishMiner(minerId);
#else
        FakeResponse.Instance.StartFakeInvokeResponseExtinguishMinerCoroutine(minerId);
#endif
    }

    public static void CallRequestUpgradeStation(int stationId, int targetLevel)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestUpgradeStation(stationId, targetLevel);
#else
        FakeResponse.Instance.StartFakeInvokeResponseUpgradeStationCoroutine(stationId, targetLevel);
#endif
    }

    public static void CallRequestMintCoreEngine(string engineType)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestMintCoreEngine(engineType);
#else
        // FakeResponse.Instance.StartFakeInvokeResponseUpgradeStationCoroutine(stationId, targetLevel);
#endif
    }
}