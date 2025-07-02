using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTabPlanetUI : MonoBehaviour
{
    public event EventHandler<OnUseEventArgs> OnUse;

    public class OnUseEventArgs : EventArgs
    {
        public PlanetSO PlanetSo;
        public StationData StationData;
    }

    [SerializeField] private Button useButton;
    [SerializeField] private Image planetImage;
    [SerializeField] private TextMeshProUGUI planetNameTextMeshPro;
    public int indexButton = -1;
    public PlanetSO PlanetSo;
    public StationData StationData { get; set; }

    public void Setup(PlanetSO planetSo, StationData stationData)
    {
        planetImage.sprite = planetSo.planetSprite;
        planetNameTextMeshPro.text = planetSo.planetName;
        PlanetSo = planetSo;
        StationData = stationData;
    }

    private void Start()
    {
        useButton.onClick.AddListener(OnUseButtonClicked);
    }

    public void OnUseButtonClicked()
    {
        OnUse.Invoke(this, new OnUseEventArgs()
        {
            PlanetSo = PlanetSo,
            StationData = StationData
        });
    }
}