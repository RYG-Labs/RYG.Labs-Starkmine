using System;
using UnityEngine;

[Serializable]
public class UserData
{
    private string _address;

    public string Address
    {
        get { return _address; }
        set { _address = value; }
    }

    private double _balance;

    public double Balance
    {
        get { return _balance; }
        set { _balance = value; }
    }
}