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
    [SerializeField] private TextMeshProUGUI totalFailingAmountText;
    [SerializeField] private TextMeshProUGUI costPerMergeAmountText;
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
        selectSpaceShipInUI.SetUp(DataManager.Instance.ShipInInventory.FindAll(shipData =>
            shipData.shipSO.shipType == e.ShipSo.shipTypeRequire));
    }

    private void OnEnable()
    {
        mergeButton.interactable = false;
        TotalSuccessRate = _successRateDefault;
        costPerMergeAmountText.text = _costPerMerge + "$STRK";

        if (selectSpaceShipResultUI.ShipSOSelected != null)
        {
            selectSpaceShipInUI.SetUp(DataManager.Instance.ShipInInventory.FindAll(shipData =>
                shipData.shipSO.shipType == selectSpaceShipResultUI.ShipSOSelected.shipTypeRequire));
        }
    }

    private void OnClickMergeButton()
    {
        if (!IsSuccess(TotalSuccessRate + selectSpaceShipInUI.TotalSuccessRate))
        {
            SoundManager.Instance.PlayBleepSound4();
            TotalFailing++;
            TotalSuccessRate++;
            return;
        }
        SoundManager.Instance.PlayCompleteSound2();
        ShipData newShipData = new ShipData(selectSpaceShipResultUI.ShipSOSelected);
        List<ShipData> listShipRemove = selectSpaceShipInUI.DestroyItemSelected();
        DataManager.Instance.AddShipToInventory(newShipData);
        DataManager.Instance.RemoveShipFromInventory(listShipRemove);
        // selectSpaceShipResultUI.Refresh();
        TotalFailing = 0;
        TotalSuccessRate = _successRateDefault;
        mergeButton.interactable = false;
    }

    private void SelectSpaceShipInUIOnOnUnSelectSpaceShipEventHandler(object sender, EventArgs e)
    {
        mergeButton.interactable = false;
    }

    private void SelectSpaceShipInUIOnOnSelectSpaceShipEventHandler(object sender,
        SelectSpaceShipInUI.OnSelectSpaceShipEventArgs e)
    {
        // selectSpaceShipResultUI.SetUp(shipSos);
        mergeButton.interactable = true;
    }

    private bool IsSuccess(int totalSuccessRate)
    {
        if (totalSuccessRate == 0) return false;
        int random = UnityEngine.Random.Range(0, 100);
        return random < totalSuccessRate;
    }
}