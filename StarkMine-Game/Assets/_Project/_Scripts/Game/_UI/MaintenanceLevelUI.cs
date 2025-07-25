using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaintenanceLevelUI : MonoBehaviour
{
    private float _maintenanceLevelPercent = 100;
    [SerializeField] private List<Image> maintenanceLevels = new();
    [SerializeField] private TextMeshProUGUI maintenanceLevelPercentText;
    [SerializeField] private TextMeshProUGUI earningRateText;
    [SerializeField] private Image earningRateBackground;

    public void SetUp(float maintenanceLevelPercent, float earningRate)
    {
        _maintenanceLevelPercent = maintenanceLevelPercent;
        maintenanceLevelPercentText.text = maintenanceLevelPercent + "%";
        earningRateText.text = "earnings " + earningRate + "%";
        Refresh();
    }

    public void Refresh()
    {
        HideAll();
        Color color = GetColor();
        maintenanceLevelPercentText.color = color;
        earningRateText.color = color;
        earningRateBackground.color = new Color(color.r, color.g, color.b, 0.125f);
        // earningRateBackground.gameObject.SetActive(_maintenanceLevelPercent <= 70);

        for (int i = 0; i < 10; i++)
        {
            if (i < _maintenanceLevelPercent / 10)
            {
                maintenanceLevels[i].color = color;
                maintenanceLevels[i].gameObject.SetActive(true);
            }
            else
            {
                maintenanceLevels[i].color = new Color(88, 93, 110);
            }
        }
    }

    private void HideAll()
    {
        foreach (var star in maintenanceLevels)
        {
            star.gameObject.SetActive(false);
        }
    }

    private Color GetColor()
    {
        if (_maintenanceLevelPercent <= 15) return new Color32(255, 89, 91, 255);
        if (_maintenanceLevelPercent <= 40) return new Color32(255, 89, 194, 255);
        if (_maintenanceLevelPercent <= 70) return new Color32(255, 180, 51, 255);
        return new Color32(50, 222, 171, 255);
    }
}