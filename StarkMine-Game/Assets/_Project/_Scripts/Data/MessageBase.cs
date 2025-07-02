using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

[Serializable]
public class MessageBase<T>
{
    public string status;
    public string message;
    public T data;
    public MessageLevel level;

    public bool IsSuccess()
    {
        return status == "success";
    }
}

public enum MessageLevel
{
    INFOR,
    WARNING,
    ERROR,
}