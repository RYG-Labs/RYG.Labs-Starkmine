using System;
using _Project._Scripts.Game.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Helpers
{
    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }

    public static string FormatCurrencyNumber(long number)
    {
        return number.ToString("N0");
    }

    public static float Round(float value, int decimalPlaces = 5)
    {
        return (float)Math.Round(value, decimalPlaces);
    }
}