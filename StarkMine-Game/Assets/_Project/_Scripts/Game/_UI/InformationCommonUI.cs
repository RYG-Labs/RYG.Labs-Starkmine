using System;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;

public class InformationCommonUI : MonoBehaviour
{
    [Header("SpaceShip")] [SerializeField] private TextMeshProUGUI countAllSpaceShipText;
    [SerializeField] private TextMeshProUGUI countSpaceShipOnDutyText;
    [SerializeField] private TextMeshProUGUI countSpaceShipTypeBasicText;
    [SerializeField] private TextMeshProUGUI countSpaceShipTypeEliteText;
    [SerializeField] private TextMeshProUGUI countSpaceShipTypeProText;
    [SerializeField] private TextMeshProUGUI countSpaceShipTypeGIGAText;

    [Header("CoreEngine")] [SerializeField]
    private TextMeshProUGUI countAllCoreEngineText;

    [SerializeField] private TextMeshProUGUI countCoreEngineEquippedText;
    [SerializeField] private TextMeshProUGUI countCoreEngineTypeLv1Text;
    [SerializeField] private TextMeshProUGUI countCoreEngineTypeLv2Text;
    [SerializeField] private TextMeshProUGUI countCoreEngineTypeLv3Text;
    [SerializeField] private TextMeshProUGUI countCoreEngineTypeLv4Text;

    private void OnEnable()
    {
        countAllSpaceShipText.text = DataManager.Instance.CountAllSpaceShip().ToString();
        countSpaceShipOnDutyText.text = DataManager.Instance.CountSpaceShipOnDuty().ToString();
        countCoreEngineTypeLv1Text.text =
            DataManager.Instance.CountAllSpaceShipByType(ShipSO.ShipType.Basic).ToString();
        countCoreEngineTypeLv2Text.text =
            DataManager.Instance.CountAllSpaceShipByType(ShipSO.ShipType.Elite).ToString();
        countCoreEngineTypeLv3Text.text =
            DataManager.Instance.CountAllSpaceShipByType(ShipSO.ShipType.Pro).ToString();
        countCoreEngineTypeLv4Text.text =
            DataManager.Instance.CountAllSpaceShipByType(ShipSO.ShipType.GIGA).ToString();

        countAllCoreEngineText.text = DataManager.Instance.CountAllCoreEngine().ToString();
        countCoreEngineEquippedText.text = DataManager.Instance.CountCoreEngineEquipped().ToString();
        countCoreEngineTypeLv1Text.text =
            DataManager.Instance.CountAllCoreEngineByType(CoreEngineSO.CoreEngineType.Basic).ToString();
        countCoreEngineTypeLv2Text.text =
            DataManager.Instance.CountAllCoreEngineByType(CoreEngineSO.CoreEngineType.Elite).ToString();
        countCoreEngineTypeLv3Text.text =
            DataManager.Instance.CountAllCoreEngineByType(CoreEngineSO.CoreEngineType.Pro).ToString();
        countCoreEngineTypeLv4Text.text =
            DataManager.Instance.CountAllCoreEngineByType(CoreEngineSO.CoreEngineType.GIGA).ToString();
    }
}