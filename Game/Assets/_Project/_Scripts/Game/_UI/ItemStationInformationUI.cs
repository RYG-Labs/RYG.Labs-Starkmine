using TMPro;
using UnityEngine;

public class ItemStationInformationUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI minerTypeText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI hashPowerText;

    public void SetUp(string minerName, int amount, float hashPower)
    {
        minerTypeText.text = minerName;
        amountText.text = amount.ToString();
        hashPowerText.text = Helpers.Round(hashPower).ToString();
    }
}