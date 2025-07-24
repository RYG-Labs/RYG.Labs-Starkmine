using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterCoreEngineUI : MonoBehaviour
{
    public event EventHandler<OnOptionFilterChangeEventArgs> OnOptionFilterChangeEventHandler;

    public class OnOptionFilterChangeEventArgs : EventArgs
    {
        public List<CoreEngineSO.CoreEngineType> ListToggleSelected;
    }

    [SerializeField] private List<Toggle> listToggle;
    [SerializeField] private List<CoreEngineSO.CoreEngineType> listToggleIndexSelected = new();

    public List<CoreEngineSO.CoreEngineType> ListToggleIndexSelected
    {
        get => listToggleIndexSelected;
    }

    private void Start()
    {
        if (listToggle[0].isOn) listToggleIndexSelected.Add(CoreEngineSO.CoreEngineType.Basic);
        if (listToggle[1].isOn) listToggleIndexSelected.Add(CoreEngineSO.CoreEngineType.Elite);
        if (listToggle[2].isOn) listToggleIndexSelected.Add(CoreEngineSO.CoreEngineType.Pro);
        if (listToggle[3].isOn) listToggleIndexSelected.Add(CoreEngineSO.CoreEngineType.GIGA);
        for (int i = 0; i < listToggle.Count; i++)
        {
            var i1 = i;
            listToggle[i].onValueChanged.AddListener(isOn => OnToggleChanged(i1, listToggle[i1], isOn));
        }
    }

    private void OnToggleChanged(int index, Toggle toggle, bool isOn)
    {
        CoreEngineSO.CoreEngineType shipType = (CoreEngineSO.CoreEngineType)index;
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