using System;
using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MergeSpaceshipUI : BasePopup
{
    [SerializeField] private SelectSpaceShipResultUI selectSpaceShipResultUI;
    [SerializeField] private SelectSpaceShipInUI selectSpaceShipInUI;
    [SerializeField] private Button mergeButton;
    [SerializeField] private TextMeshProUGUI totalSuccessRateAmountText;
    [SerializeField] private TextMeshProUGUI totalSuccessRateFailingAmountText;
    [SerializeField] private TextMeshProUGUI totalFailingAmountText;
    [SerializeField] private TextMeshProUGUI costPerMergeAmountText;
    [SerializeField] private TextMeshProUGUI successRatePerShipText;
    [SerializeField] private TextMeshProUGUI hashPowerText;
    [SerializeField] private TextMeshProUGUI availableAmountText;

    [SerializeField] private GameObject availableGo;
    [SerializeField] private GameObject unavailableGo;
    [SerializeField] private TextMeshProUGUI requirementText;

    [SerializeField] private List<ShipSO> listShipResult;
    private int _totalFailing;
    private int _successRateDefault = 5;
    private int _totalSuccessRate;

    public int TotalFailing
    {
        get { return _totalFailing; }
        set
        {
            _totalFailing = value;
            totalFailingAmountText.text = $"({value} failing)";
            totalSuccessRateFailingAmountText.text = $"{value}%";
        }
    }

    public int TotalSuccessRate
    {
        get { return _totalSuccessRate; }
        set
        {
            _totalSuccessRate = value;
            totalSuccessRateAmountText.text = value + "%";
        }
    }

    private int _costPerMerge = 125;

    protected override void Start()
    {
        base.Start();
        selectSpaceShipInUI.OnSelectSpaceShipEventHandler += SelectSpaceShipInUIOnOnSelectSpaceShipEventHandler;
        selectSpaceShipInUI.OnUnSelectSpaceShipEventHandler += SelectSpaceShipInUIOnOnUnSelectSpaceShipEventHandler;
        mergeButton.onClick.AddListener(OnClickMergeButton);

        selectSpaceShipResultUI.SetUp(listShipResult);
        selectSpaceShipResultUI.OnSelectSpaceShipResultEventHandler +=
            SelectSpaceShipResultUIOnOnSelectSpaceShipResultEventHandler;
        selectSpaceShipInUI.SetUp(DataManager.Instance.ShipInInventory.FindAll(shipData =>
            shipData.shipSO.shipType == selectSpaceShipResultUI.ShipSOSelected.shipTypeRequire));
    }

    private void SelectSpaceShipResultUIOnOnSelectSpaceShipResultEventHandler(object sender,
        SelectSpaceShipResultUI.OnSelectSpaceShipResultEventArgs e)
    {
        CallRequestCurrentSuccessRate(e.ShipSo.shipName == "Pro" ? "Elite" : "Pro", e.ShipSo.shipName);
    }

    private void OnEnable()
    {
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseCurrentMergeStatusByUserEventHandler +=
            WebResponseOnResponseCurrentMergeStatusByUserEventHandler;
        WebResponse.Instance.OnResponseCurrentMergeStatusByUserFailEventHandler +=
            WebResponseOnResponseCurrentMergeStatusByUserFailEventHandler;
        CallRequestCurrentSuccessRate("Elite", "Pro");
    }

    public void CallRequestCurrentSuccessRate(string fromTier, string toTier)
    {
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseCurrentMergeStatusByUserEventHandler +=
            WebResponseOnResponseCurrentMergeStatusByUserEventHandler;
        WebResponse.Instance.OnResponseCurrentMergeStatusByUserFailEventHandler +=
            WebResponseOnResponseCurrentMergeStatusByUserFailEventHandler;
        WebRequest.CallRequestCurrentMergeStatusByUser(fromTier, toTier);
    }

    private void WebResponseOnResponseCurrentMergeStatusByUserFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseCurrentMergeStatusByUserEventHandler -=
            WebResponseOnResponseCurrentMergeStatusByUserEventHandler;
        WebResponse.Instance.OnResponseCurrentMergeStatusByUserFailEventHandler -=
            WebResponseOnResponseCurrentMergeStatusByUserFailEventHandler;
    }

    private void WebResponseOnResponseCurrentMergeStatusByUserEventHandler(object sender,
        WebResponse.OnResponseCurrentSuccessRateEventArgs e)
    {
        ResponseCurrentMergeStatusByUserDTO data = e.Data;
        _successRateDefault = data.baseSuccessRate;
        successRatePerShipText.text =
            $"Success rate: <color=#FEE109>{_successRateDefault}%</color> and increase <color=#FEE109>+1%</color> per failing";
        mergeButton.interactable = false;
        TotalSuccessRate = _successRateDefault;
        TierConfig tierConfig = data.TierConfig;
        hashPowerText.text = $"{tierConfig.baseHashPower} + {tierConfig.tierBonus}% extra bonus reward";
        availableAmountText.text = $"<color=#FEE109>{tierConfig.mintedCount}</color> /{tierConfig.supplyLimit} ";
        SetAvailableStatus(tierConfig.mintedCount < tierConfig.supplyLimit);
        requirementText.text = $"x2 {data.fromTier} Spaceship";
        costPerMergeAmountText.text = data.costStrk + "$STRK";
        TotalFailing = data.successRateBonus;
        if (selectSpaceShipResultUI.ShipSOSelected != null)
        {
            selectSpaceShipInUI.SetUp(DataManager.Instance.ShipInInventory.FindAll(shipData =>
                shipData.shipSO.shipType == selectSpaceShipResultUI.ShipSOSelected.shipTypeRequire));
        }

        WebResponse.Instance.OnResponseCurrentMergeStatusByUserEventHandler -=
            WebResponseOnResponseCurrentMergeStatusByUserEventHandler;
        WebResponse.Instance.OnResponseCurrentMergeStatusByUserFailEventHandler -=
            WebResponseOnResponseCurrentMergeStatusByUserFailEventHandler;
    }

    private void OnClickMergeButton()
    {
#if UNITY_EDITOR
        if (!IsSuccess(TotalSuccessRate + selectSpaceShipInUI.TotalSuccessRate))
        {
            TotalFailing++;
            TotalSuccessRate++;
            return;
        }
#endif
        SoundManager.Instance.PlayBleepSound4();
        UIManager.Instance.loadingUI.Show();
        List<ShipData> listShipSelected = selectSpaceShipInUI.GetShipSelected();
        WebResponse.Instance.OnResponseMergeMinerEventHandler += WebResponseOnResponseMergeMinerEventHandler;
        WebResponse.Instance.OnResponseMergeMinerFailEventHandler += WebResponseOnResponseMergeMinerFailEventHandler;
        WebRequest.CallRequestMergeMiner(listShipSelected[0].id, listShipSelected[1].id,
            listShipSelected[0].shipSO.shipName, selectSpaceShipResultUI.ShipSOSelected.shipName);
    }

    private void WebResponseOnResponseMergeMinerFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseMergeMinerEventHandler -= WebResponseOnResponseMergeMinerEventHandler;
        WebResponse.Instance.OnResponseMergeMinerFailEventHandler -= WebResponseOnResponseMergeMinerFailEventHandler;
    }

    private void WebResponseOnResponseMergeMinerEventHandler(object sender, WebResponse.OnResponseMergeMinerEventArgs e)
    {
        ResponseMergeMinerDTO responseMergeMinerDTO = e.Data;
        if (!responseMergeMinerDTO.isMergeSuccessful)
        {
            TotalFailing = responseMergeMinerDTO.failureBonus;
            _successRateDefault = responseMergeMinerDTO.baseSuccessRate;
            return;
        }

        SoundManager.Instance.PlayCompleteSound2();
        ShipData newShipData = new ShipData(selectSpaceShipResultUI.ShipSOSelected);
        newShipData.id = responseMergeMinerDTO.newTokenId;
        List<ShipData> listShipRemove = selectSpaceShipInUI.DestroyItemSelected();
        DataManager.Instance.AddShipToInventory(newShipData);
        DataManager.Instance.RemoveShipFromInventory(listShipRemove);
        // selectSpaceShipResultUI.Refresh();
        TotalFailing = 0;
        TotalSuccessRate = _successRateDefault;
        mergeButton.interactable = false;

        WebResponse.Instance.OnResponseMergeMinerEventHandler -= WebResponseOnResponseMergeMinerEventHandler;
        WebResponse.Instance.OnResponseMergeMinerFailEventHandler -= WebResponseOnResponseMergeMinerFailEventHandler;
    }

    private void SelectSpaceShipInUIOnOnUnSelectSpaceShipEventHandler(object sender, EventArgs e)
    {
        TotalSuccessRate = _successRateDefault;
        mergeButton.interactable = false;
    }

    private void SelectSpaceShipInUIOnOnSelectSpaceShipEventHandler(object sender,
        SelectSpaceShipInUI.OnSelectSpaceShipEventArgs e)
    {
        // selectSpaceShipResultUI.SetUp(shipSos);
        TotalSuccessRate = _successRateDefault;
        foreach (var shipData in e.ListShipSelected)
        {
            TotalSuccessRate += shipData.level;
        }

        mergeButton.interactable = true;
    }

    private bool IsSuccess(int totalSuccessRate)
    {
        if (totalSuccessRate == 0) return false;
        int random = UnityEngine.Random.Range(0, 100);
        return random < totalSuccessRate;
    }

    private void SetAvailableStatus(bool available)
    {
        availableGo.SetActive(available);
        unavailableGo.SetActive(!available);
    }
}