using System;
using UnityEngine;

[Serializable]
public class TicketData
{
    private int _id;

    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public TicketData(int id)
    {
        _id = id;
    }
}