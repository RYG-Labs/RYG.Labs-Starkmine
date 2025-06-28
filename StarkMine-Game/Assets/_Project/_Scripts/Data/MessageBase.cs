using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

[Serializable]
public class MessageBase<T>
{
    public enum MessageEnum
    {
        INFOR,
        WARNING,
        ERROR,
    }

    public string status;
    public string message;
    public T data;
    public MessageEnum level;

    public bool IsSuccess()
    {
        return status == "success";
    }
}