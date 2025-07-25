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

    public ShipData ShipData
    {
        get => _shipData;
        set => _shipData = value;
    }

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
            SpaceShipOnDutyHandler();
        }
        else
        {
            SpaceShipOnCallbackHandler();
        }

        warningImage.gameObject.SetActive(ship.CoreEngineData != null &&
                                          ship.CoreEngineData.GetDurabilityPercentage() * 100 < 80);
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
        // addCoreButton.onClick.AddListener(OnClickAddCoreButton);
        launchButton.onClick.AddListener(OnClickLaunchButton);
        callbackButton.onClick.AddListener(OnClickCallbackButton);
    }

    private void OnClickCallbackButton()
    {
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseExtinguishMinerEventHandler += WebResponseOnResponseExtinguishMinerEventHandler;
        WebResponse.Instance.OnResponseExtinguishMinerFailEventHandler +=
            WebResponseOnResponseExtinguishMinerFailEventHandler;
        WebRequest.CallRequestExtinguishMiner(_shipData.id);
    }

    private void WebResponseOnResponseExtinguishMinerFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseExtinguishMinerEventHandler -= WebResponseOnResponseExtinguishMinerEventHandler;
        WebResponse.Instance.OnResponseExtinguishMinerFailEventHandler -=
            WebResponseOnResponseExtinguishMinerFailEventHandler;
    }

    private void WebResponseOnResponseExtinguishMinerEventHandler(object sender,
        WebResponse.OnResponseExtinguishMinerEventArgs e)
    {
        bool success = GameManager.Instance.CallbackSpaceShip(_shipData);

        SoundManager.Instance.PlayDataPointSound1();
        if (success)
        {
            SpaceShipOnCallbackHandler();
        }

        WebResponse.Instance.OnResponseExtinguishMinerEventHandler -= WebResponseOnResponseExtinguishMinerEventHandler;
        WebResponse.Instance.OnResponseExtinguishMinerFailEventHandler -=
            WebResponseOnResponseExtinguishMinerFailEventHandler;
    }

    private void OnClickLaunchButton()
    {
        bool isContainCoreEngineRequire =
            DataManager.Instance.IsContainCoreEngineUnActiveByShipType(_shipData.shipSO.shipType);
        SoundManager.Instance.PlayConfirmSound3();
        if (!isContainCoreEngineRequire)
        {
            DontHaveRequireCoreEngineUI dontHaveRequireCoreEngineUI = UIManager.Instance.dontHaveRequireCoreEngineUI;
            CoreEngineSO coreEngineRequire = DataManager.Instance.GetCoreEngineByShipType(_shipData.shipSO.shipType);
            dontHaveRequireCoreEngineUI.SetUp(coreEngineRequire);
            dontHaveRequireCoreEngineUI.Show();
            return;
        }

        ChooseCoreEngineUI chooseCoreEngineUI = UIManager.Instance.chooseCoreEngineUI;
        chooseCoreEngineUI.SetUpAndShow(_shipData);
        chooseCoreEngineUI.OnItemSelectEventHandler += ChooseCoreEngineUIOnItemSelectEventHandler;
    }

    private void ChooseCoreEngineUIOnItemSelectEventHandler(object sender, ChooseCoreEngineUI.OnItemSelectEventArgs e)
    {
        SoundManager.Instance.PlayBleepSound1();
        CoreEngineData coreEngineData = e.CoreEngineData;
        WebResponse.Instance.OnResponseIgniteMinerEventHandler += WebResponseOnResponseIgniteMinerEventHandler;
        WebResponse.Instance.OnResponseIgniteMinerFailEventHandler += WebResponseOnResponseIgniteMinerFailEventHandler;
        WebRequest.CallRequestIgniteMiner(_shipData.id, coreEngineData.id);
        UIManager.Instance.loadingUI.Show();

        ChooseCoreEngineUI chooseCoreEngineUI = UIManager.Instance.chooseCoreEngineUI;
        chooseCoreEngineUI.OnItemSelectEventHandler -= ChooseCoreEngineUIOnItemSelectEventHandler;
    }

    private void WebResponseOnResponseIgniteMinerFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseIgniteMinerEventHandler -= WebResponseOnResponseIgniteMinerEventHandler;
        WebResponse.Instance.OnResponseIgniteMinerFailEventHandler -= WebResponseOnResponseIgniteMinerFailEventHandler;
    }

    private void WebResponseOnResponseIgniteMinerEventHandler(object sender,
        WebResponse.OnResponseIgniteMinerEventArgs e)
    {
        // SpaceShipOnCallbackHandler();
        SpaceShipOnDutyHandler();
        CoreEngineData coreEngineData = DataManager.Instance.GetCoreEngineDataById(e.Data.CoreEngineDto.tokenId);
        coreEngineData.lastUsedBlock = e.Data.CoreEngineDto.lastUsedBlock;
        DataManager.Instance.AddCoreEngineToSpaceShip(coreEngineData, _shipData);
        GameManager.Instance.LaunchSpaceShip(_shipData);
        WebResponse.Instance.OnResponseIgniteMinerEventHandler -= WebResponseOnResponseIgniteMinerEventHandler;
        WebResponse.Instance.OnResponseIgniteMinerFailEventHandler -= WebResponseOnResponseIgniteMinerFailEventHandler;
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