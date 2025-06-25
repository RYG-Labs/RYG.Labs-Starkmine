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

    private string _balance;

    public string Balance
    {
        get { return _balance; }
        set { _balance = value; }
    }
}