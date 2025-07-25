using System;
using System.Collections;
using _Project._Scripts.Game.Enemies;
using _Project._Scripts.Game.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenTicketUI : BasePopup
{
    [SerializeField] private Button openTicketButton;
    [SerializeField] private Button buyTicketButton;
    [SerializeField] private Button rotateButton;
    [SerializeField] private TextMeshProUGUI amountOrbText;

    [SerializeField] private Transform cardTransform;
    [SerializeField] private float scaleDuration = 2f; // Thời gian scale to
    [SerializeField] private float initialScale = 0.1f; // Kích thước ban đầu
    [SerializeField] private float targetScale = 1f; // Kích thước mục tiêu
    [SerializeField] private Ease easeType = Ease.OutQuad; // Kiểu easing

    [Header("Card Properties")] [SerializeField]
    private ImageAnimation shipImageAnimation;

    [SerializeField] private Image shipImage;

    [SerializeField] private TextMeshProUGUI shipNameText;
    [SerializeField] private TextMeshProUGUI shipIdText;
    [SerializeField] private TextMeshProUGUI hashPowerText;
    [SerializeField] private CardGroupUI cardGroupUI;

    [SerializeField] private GameObject openingVideo;
    [SerializeField] private GameObject waitingVideo;
    [SerializeField] private GameObject endingVideo;
    [SerializeField] private Button claimButton;
    Sequence _openQuantumOrbSequence;

    protected override void Start()
    {
        base.Start();
        openTicketButton.onClick.AddListener(OnClickOpenTicketButton);
        buyTicketButton.onClick.AddListener(OnClickBuyTicketButton);
        claimButton.onClick.AddListener(OnClickClaimButton);
        // CreateEffectOpenQuantumOrb();
    }

    private void OnClickClaimButton()
    {
        endingVideo.SetActive(false);
        StartCoroutine(OpenPopupCoroutine());
    }
    private void OnEnable()
    {
        StartCoroutine(OpenPopupCoroutine());
        SetUp();
    }

    public IEnumerator OpenPopupCoroutine()
    {
        openingVideo.SetActive(true);
        yield return new WaitForSeconds(3f);
        openingVideo.SetActive(false);
        waitingVideo.SetActive(true);
    }

    public void SetUp()
    {
        amountOrbText.text = $"Quantum Orb: {DataManager.Instance.ListTicketData.Count}";
    }

    private void OnClickBuyTicketButton()
    {
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseMintTicketEventHandler += InstanceOnOnResponseMintTicketEventHandler;
        WebResponse.Instance.OnResponseMintTicketFailEventHandler += InstanceOnOnResponseMintTicketFailEventHandler;
        WebRequest.CallRequestMintTicket();
    }

    private void InstanceOnOnResponseMintTicketFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseMintTicketEventHandler -= InstanceOnOnResponseMintTicketEventHandler;
        WebResponse.Instance.OnResponseMintTicketFailEventHandler -= InstanceOnOnResponseMintTicketFailEventHandler;
    }

    private void InstanceOnOnResponseMintTicketEventHandler(object sender, WebResponse.OnResponseMintTicketEventArgs e)
    {
        TicketData ticketData = new TicketData(e.Data.ticketId);
        DataManager.Instance.ListTicketData.Add(ticketData);
        SetUp();
        WebResponse.Instance.OnResponseMintTicketEventHandler -= InstanceOnOnResponseMintTicketEventHandler;
        WebResponse.Instance.OnResponseMintTicketFailEventHandler -= InstanceOnOnResponseMintTicketFailEventHandler;
    }

    private void OnClickOpenTicketButton()
    {
        if (DataManager.Instance.ListTicketData.Count <= 0)
        {
            UIManager.Instance.showNotificationUI.SetUpAndShow("You don't have any quantum orb to open.");
            return;
        }

        UIManager.Instance.loadingUI.Show();
        TicketData ticketData = DataManager.Instance.GetLastTicketData();
        WebResponse.Instance.OnResponseOpenTicketEventHandler += InstanceOnOnResponseOpenTicketEventHandler;
        WebResponse.Instance.OnResponseOpenTicketFailEventHandler += InstanceOnOnResponseOpenTicketFailEventHandler;
        WebRequest.CallRequestOpenTicket(ticketData.Id);
    }

    private void InstanceOnOnResponseOpenTicketFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseOpenTicketEventHandler -= InstanceOnOnResponseOpenTicketEventHandler;
        WebResponse.Instance.OnResponseOpenTicketFailEventHandler -= InstanceOnOnResponseOpenTicketFailEventHandler;
    }

    private void InstanceOnOnResponseOpenTicketEventHandler(object sender, WebResponse.OnResponseOpenTicketEventArgs e)
    {
        TicketData ticketData = DataManager.Instance.GetLastTicketData();
        DataManager.Instance.ListTicketData.Remove(ticketData);
        SetUp();
        ShipSO shipSo = DataManager.Instance.GetShipSoByType(e.Data.tier);
        ShipData shipData = new ShipData(e.Data.tokenId, shipSo);
        DataManager.Instance.AddShipToInventory(shipData);
        SetupCard(shipData);
        // PlayEffectOpenQuantumOrb();
        StartCoroutine(PlayEffectCoroutine());
        WebResponse.Instance.OnResponseOpenTicketEventHandler -= InstanceOnOnResponseOpenTicketEventHandler;
        WebResponse.Instance.OnResponseOpenTicketFailEventHandler -= InstanceOnOnResponseOpenTicketFailEventHandler;
    }

    public IEnumerator PlayEffectCoroutine()
    {
        waitingVideo.SetActive(false);
        endingVideo.SetActive(true);
        yield return new WaitForSeconds(12);
        cardGroupUI.Show();
        cardTransform.localScale = Vector3.zero;
        cardTransform.DOScale(targetScale, scaleDuration).SetEase(easeType);
    }

    public void SetupCard(ShipData shipData)
    {
        shipImage.sprite = shipData.shipSO.baseSprite;
        shipImage.SetNativeSize();
        shipImageAnimation.ImageAnimationSO = shipData.shipSO.imageAnimationSO;
        shipNameText.text = shipData.shipSO.shipName;
        shipIdText.text = $"#{shipData.id}";
        hashPowerText.text = $"{shipData.shipSO.baseHashPower} Hash Power";
    }
}