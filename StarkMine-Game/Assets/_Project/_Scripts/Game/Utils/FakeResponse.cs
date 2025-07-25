using System.Collections;
using _Project._Scripts.Game.Managers;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class FakeResponse : StaticInstance<FakeResponse>
{
    public void StartFakeResponseAssignMinerToStationCoroutine(int stationId, int minerId, int index)
    {
        StartCoroutine(FakeResponseCoroutine(stationId, minerId, index));
    }

    private IEnumerator FakeResponseCoroutine(int stationId, int minerId, int index)
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.loadingUI.Hide();
        WebResponse.Instance.InvokeResponseAssignMinerToStation(
            new WebResponse.OnResponseAssignMinerToStationEventArgs()
            {
                Data = new AssignMinerToStationDTO()
                {
                    stationId = stationId,
                    minerId = minerId,
                    minerSlot = index
                }
            });
    }

    public void StartFakeResponseRemoveMinerFromStationCoroutine(int stationId, int minerSlot)
    {
        StartCoroutine(FakeResponseRemoveMinerFromStationCoroutine(stationId, minerSlot));
    }

    private IEnumerator FakeResponseRemoveMinerFromStationCoroutine(int stationId, int minerSlot)
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.loadingUI.Hide();
        WebResponse.Instance.InvokeResponseRemoveMinerFromStation(
            new WebResponse.OnResponseRemoveMinerFromStationEventArgs()
            {
                Data = new RemoveMinerFromStationDTO()
                {
                    stationId = stationId,
                    minerSlot = minerSlot,
                }
            });
    }

    public void StartFakeResponseIgniteMinerCoroutine(int minerId, int coreEngineId)
    {
        StartCoroutine(FakeResponseResponseIgniteMinerCoroutine(minerId, coreEngineId));
    }

    private IEnumerator FakeResponseResponseIgniteMinerCoroutine(int minerId, int coreEngineId)
    {
        yield return new WaitForSeconds(0.5f);
        CoreEngineDTO coreEngineDto = new CoreEngineDTO()
        {
            tokenId = coreEngineId,
            lastUsedBlock = DataManager.Instance.CurrentBlock
        };
        UIManager.Instance.loadingUI.Hide();
        WebResponse.Instance.InvokeResponseIgniteMiner(
            new WebResponse.OnResponseIgniteMinerEventArgs
            {
                Data = new ResponseIgniteMinerDTO
                {
                    minerId = minerId,
                    // coreEngineId = coreEngineId

                    coreEngine = JObject.FromObject(coreEngineDto)
                }
            });
    }

    public void StartFakeInvokeResponseExtinguishMinerCoroutine(int minerId)
    {
        StartCoroutine(FakeInvokeResponseExtinguishMinerCoroutine(minerId));
    }

    private IEnumerator FakeInvokeResponseExtinguishMinerCoroutine(int minerId)
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.loadingUI.Hide();
        WebResponse.Instance.InvokeResponseExtinguishMiner(
            new WebResponse.OnResponseExtinguishMinerEventArgs
            {
                Data = new ResponseExtinguishMinerDTO
                {
                    minerId = minerId,
                }
            });
    }

    public void StartFakeInvokeResponseUpgradeStationCoroutine(int stationId, int targetLevel)
    {
        StartCoroutine(FakeInvokeResponseUpgradeStationCoroutine(stationId, targetLevel));
    }

    private IEnumerator FakeInvokeResponseUpgradeStationCoroutine(int stationId, int targetLevel)
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.loadingUI.Hide();
        WebResponse.Instance.InvokeResponseUpgradeStation(
            new WebResponse.OnResponseUpgradeStationEventArgs()
            {
                Data = new ResponseUpgradeStationDTO
                {
                    stationId = stationId,
                    targetLevel = targetLevel
                }
            });
    }

    public void StartFakeResponseMintCoreEngineCoroutine(string engineType)
    {
        StartCoroutine(FakeInvokeResponseMintCoreEngineCoroutine(engineType));
    }

    private IEnumerator FakeInvokeResponseMintCoreEngineCoroutine(string engineType)
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.loadingUI.Hide();
        WebResponse.Instance.InvokeResponseMintCoreEngine(
            new()
            {
                Data = new()
                {
                    engineType = engineType,
                    coreEngineId = DataManager.Instance.listCoreEngineData.Count + 1
                }
            });
    }

    public void StartFakeResponseDefuseEngineCoroutine(int engineId)
    {
        StartCoroutine(FakeInvokeResponseDefuseEngineCoroutine(engineId));
    }

    private IEnumerator FakeInvokeResponseDefuseEngineCoroutine(int engineId)
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.loadingUI.Hide();
        WebResponse.Instance.InvokeResponseDefuseEngine(
            new()
            {
                Data = new()
                {
                    engineId = engineId,
                }
            });
    }

    public void StartFakeResponseUpgradeMinerCoroutine(int minerId)
    {
        StartCoroutine(FakeInvokeResponseUpgradeMinerCoroutine(minerId));
    }

    private IEnumerator FakeInvokeResponseUpgradeMinerCoroutine(int minerId)
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.loadingUI.Hide();
        WebResponse.Instance.InvokeResponseUpgradeMinerEngine(
            new()
            {
                Data = new()
                {
                    minerId = minerId,
                }
            });
    }

    public void StartFakeResponseGetPendingRewardCoroutine()
    {
        StartCoroutine(FakeInvokeResponseGetPendingRewardCoroutine());
    }

    private IEnumerator FakeInvokeResponseGetPendingRewardCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        WebResponse.Instance.InvokeResponseGetPendingReward(
            new()
            {
                Data = new()
                {
                    pendingReward = DataManager.Instance.PendingReward + 100
                }
            });
    }

    public void StartFakeMergeMinerCoroutine(int tokenId1, int tokenId2, string fromTier, string toTier)
    {
        StartCoroutine(FakeInvokeMergeMinerCoroutine(tokenId1, tokenId2, fromTier, toTier));
    }

    private IEnumerator FakeInvokeMergeMinerCoroutine(int tokenId1, int tokenId2, string fromTier, string toTier)
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.loadingUI.Hide();
        WebResponse.Instance.InvokeResponseMergeMiner(
            new()
            {
                Data = new()
                {
                    tokenId1 = tokenId1,
                    tokenId2 = tokenId2,
                    fromTier = fromTier,
                    toTier = toTier,
                    isMergeSuccessful = Random.Range(0, 2) == 0,
                    failureBonus = 1,
                    newTokenId = DataManager.Instance.allShip.Count
                }
            });
    }

    public void StartFakeResponseCurrentBlock()
    {
        StartCoroutine(FakeInvokeResponseCurrentBlockCoroutine());
    }

    private IEnumerator FakeInvokeResponseCurrentBlockCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.loadingUI.Hide();
        WebResponse.Instance.InvokeResponseCurrentBlock(
            new()
            {
                Data = new()
                {
                    currentBlock = DataManager.Instance.CurrentBlock + 2
                }
            });
    }
}