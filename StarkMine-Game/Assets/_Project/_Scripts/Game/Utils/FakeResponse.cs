using System.Collections;
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
                    index = index
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
}