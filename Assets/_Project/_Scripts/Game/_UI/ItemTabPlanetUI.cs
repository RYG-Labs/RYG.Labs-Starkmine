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
    }

    [SerializeField] private Button useButton;
    [SerializeField] private Image planetImage;
    [SerializeField] private TextMeshProUGUI planetNameTextMeshPro;
    public int indexButton = -1;
    public PlanetSO PlanetSo;

    public void Setup(Sprite planetImageSprite, string planetName, PlanetSO planetSo)
    {
        planetImage.sprite = planetImageSprite;
        planetNameTextMeshPro.text = planetName;
        PlanetSo = planetSo;
    }

    private void Start()
    {
        useButton.onClick.AddListener(OnUseButtonClicked);
    }

    public void OnUseButtonClicked()
    {
        OnUse.Invoke(this, new OnUseEventArgs() { PlanetSo = PlanetSo });
    }
}