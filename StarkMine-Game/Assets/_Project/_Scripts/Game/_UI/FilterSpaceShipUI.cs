using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterSpaceShipUI : MonoBehaviour
{
    public event EventHandler<OnOptionFilterChangeEventArgs> OnOptionFilterChangeEventHandler;

    public class OnOptionFilterChangeEventArgs : EventArgs
    {
        public List<ShipSO.ShipType> ListToggleSelected;
    }

    [SerializeField] private List<Toggle> listToggle;
    [SerializeField] private List<ShipSO.ShipType> listToggleIndexSelected = new();

    public List<ShipSO.ShipType> ListToggleIndexSelected
    {
        get => listToggleIndexSelected;
    }

    private void Start()
    {
        for (int i = 0; i < listToggle.Count; i++)
        {
            var i1 = i;
            listToggle[i].onValueChanged.AddListener(isOn => OnToggleChanged(i1, listToggle[i1], isOn));
        }
    }

    private void OnEnable()
    {
        listToggleIndexSelected.Add(ShipSO.ShipType.Basic);
        listToggleIndexSelected.Add(ShipSO.ShipType.Elite);
        listToggleIndexSelected.Add(ShipSO.ShipType.Pro);
        listToggleIndexSelected.Add(ShipSO.ShipType.GIGA);
    }

    private void OnDisable()
    {
        listToggleIndexSelected.Clear();
    }

    private void OnToggleChanged(int index, Toggle toggle, bool isOn)
    {
        ShipSO.ShipType shipType = (ShipSO.ShipType)index;
        if (isOn)
        {
            listToggleIndexSelected.Add(shipType);
        }
        else
        {
            listToggleIndexSelected.Remove(shipType);
        }

        OnOptionFilterChangeEventHandler?.Invoke(this,
            new OnOptionFilterChangeEventArgs { ListToggleSelected = listToggleIndexSelected });
    }
}