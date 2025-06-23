using System;
using _Project._Scripts.Game.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemSpaceStationUI : MonoBehaviour, IPointerClickHandler
{
    public event EventHandler<OnClickHandlerEventArgs> OnClickHandler;

    public class OnClickHandlerEventArgs : EventArgs
    {
        public int ItemIndex { get; set; }
        public bool IsEmpty { get; set; }
    }

    [SerializeField] private Transform lineBorderWhite;
    [SerializeField] private Transform lineBorderGreen;
    [SerializeField] private Button addCoreButton;
    [SerializeField] private Button launchButton;
    [SerializeField] private Button callbackButton;
    [SerializeField] private Image shipImage;
    [SerializeField] private ImageAnimation shipAnimation;
    [SerializeField] private Image warningImage;
    public int Index { get; set; } = 0;
    public bool IsEmpty { get; set; }
    private ShipData _shipData;

    public void SetUp(int index, ShipData ship, bool isEmpty)
    {
        Index = index;
        _shipData = ship;
        IsEmpty = isEmpty;
        HideAllButton();
        HideBorder();
        if (isEmpty)
        {
            ShowBorderWhite();
            shipImage.gameObject.SetActive(false);
            warningImage.gameObject.SetActive(false);
            return;
        }

        if (ship.onDuty)
        {
            callbackButton.gameObject.SetActive(true);
            ShowBorderGreen();
        }
        else
        {
            if (ship.CoreEngine == null)
            {
                addCoreButton.gameObject.SetActive(true);
            }
            else
            {
                launchButton.gameObject.SetActive(true);
            }
        }

        warningImage.gameObject.SetActive(ship.maintenanceLevel < 80);
        shipImage.gameObject.SetActive(true);
        shipImage.sprite = ship.shipSO.imageAnimationSO.sprites[0];
        shipImage.SetNativeSize();
        shipAnimation.ImageAnimationSO = ship.shipSO.imageAnimationSO;
        ship.OnShipDataChangedEventHandler += ShipOnOnShipDataChangedEventHandler;
    }

    private void ShipOnOnShipDataChangedEventHandler(object sender, ShipData.OnShipDataChangedEventArgs e)
    {
        _shipData.OnShipDataChangedEventHandler -= ShipOnOnShipDataChangedEventHandler;
        SetUp(Index, _shipData, false);
    }

    private void OnDestroy()
    {
        if (_shipData == null) return;
        _shipData.OnShipDataChangedEventHandler -= ShipOnOnShipDataChangedEventHandler;
    }

    private void Start()
    {
        addCoreButton.onClick.AddListener(OnClickAddCoreButton);
        launchButton.onClick.AddListener(OnClickLaunchButton);
        callbackButton.onClick.AddListener(OnClickCallbackButton);
    }

    private void OnClickCallbackButton()
    {
        bool success = GameManager.Instance.CallbackSpaceShip(_shipData);
        if (success)
        {
            SpaceShipOnCallbackHandler();
        }
    }

    private void OnClickLaunchButton()
    {
        SpaceShipOnDutyHandler();
        GameManager.Instance.LaunchSpaceShip(_shipData);
    }

    private void OnClickAddCoreButton()
    {
        bool isContainCoreEngineRequire =
            DataManager.Instance.IsContainCoreEngineRequireInInventory(_shipData.shipSO.shipType);

        if (!isContainCoreEngineRequire)
        {
            DontHaveRequireCoreEngineUI dontHaveRequireCoreEngineUI = UIManager.Instance.dontHaveRequireCoreEngineUI;
            CoreEngineSO coreEngineRequire = DataManager.Instance.GetCoreEngineRequire(_shipData.shipSO.shipType);
            dontHaveRequireCoreEngineUI.SetUp(coreEngineRequire);
            dontHaveRequireCoreEngineUI.Show();
            return;
        }

        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler += OnYesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler += OnNoButtonClickEventHandler;
        yesNoUI.SetUp(
            $"Use 1 core engine <color=#FEE109>{DataManager.Instance.GetCoreEngineRequire(_shipData.shipSO.shipType).nameCoreEngine}</color> for this spaceship?");
        yesNoUI.Show();
    }

    private void OnYesButtonClickEventHandler(object sender, EventArgs e)
    {
        CoreEngineSO coreEngineSo = DataManager.Instance.GetCoreEngineRequire(_shipData.shipSO.shipType);
        DataManager.Instance.AddCoreEngineToSpaceShip(coreEngineSo, _shipData);

        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= OnYesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= OnNoButtonClickEventHandler;
        SpaceShipOnCallbackHandler();
    }

    private void OnNoButtonClickEventHandler(object sender, EventArgs e)
    {
        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= OnYesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= OnNoButtonClickEventHandler;
    }


    public void ShowBorderWhite()
    {
        lineBorderWhite.gameObject.SetActive(true);
        lineBorderGreen.gameObject.SetActive(false);
    }

    public void ShowBorderGreen()
    {
        lineBorderWhite.gameObject.SetActive(false);
        lineBorderGreen.gameObject.SetActive(true);
    }

    public void HideBorder()
    {
        lineBorderWhite.gameObject.SetActive(false);
        lineBorderGreen.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke(this, new OnClickHandlerEventArgs
        {
            ItemIndex = Index, IsEmpty = IsEmpty
        });
    }

    public void HideAllButton()
    {
        launchButton.gameObject.SetActive(false);
        addCoreButton.gameObject.SetActive(false);
        callbackButton.gameObject.SetActive(false);
    }

    public void SpaceShipOnDutyHandler()
    {
        ShowBorderGreen();
        HideAllButton();
        callbackButton.gameObject.SetActive(true);
    }

    public void SpaceShipOnCallbackHandler()
    {
        HideBorder();
        HideAllButton();
        launchButton.gameObject.SetActive(true);
    }
}