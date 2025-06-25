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

    public static string FormatCurrencyNumber(double number)
    {
        return number.ToString("#,##0.00");
    }

    public static float Round(float value, int decimalPlaces = 5)
    {
        return (float)Math.Round(value, decimalPlaces);
    }

    // public static string FormatCurrencyNumber(long number, long threshold = 100000, bool useShortFormat = true)
    // {
    //     // Nếu không dùng định dạng viết tắt, chỉ định dạng với dấu phẩy
    //     if (!useShortFormat)
    //     {
    //         return number.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture);
    //     }
    //
    //     // Nếu số nhỏ hơn ngưỡng, định dạng với dấu phẩy
    //     if (Math.Abs(number) < threshold)
    //     {
    //         return number.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture);
    //     }
    //
    //     // Xử lý các đơn vị viết tắt (k, m, b)
    //     string[] suffixes = { "K", "M", "B" };
    //     int suffixIndex = -1;
    //     double shortenedNumber = Math.Abs(number);
    //
    //     // Chia nhỏ số cho đến khi nhỏ hơn 1000
    //     while (shortenedNumber >= 1000 && suffixIndex < suffixes.Length - 1)
    //     {
    //         shortenedNumber /= 1000;
    //         suffixIndex++;
    //     }
    //
    //     // Định dạng số nguyên nếu là số nguyên, nếu không thì giữ tối đa 2 chữ số thập phân
    //     string formattedNumber = shortenedNumber % 1 == 0
    //         ? shortenedNumber.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture)
    //         : shortenedNumber.ToString("#,##0.##", System.Globalization.CultureInfo.InvariantCulture).TrimEnd('0')
    //             .TrimEnd('.');
    //
    //     // Thêm dấu trừ nếu số âm và thêm hậu tố
    //     return (number < 0 ? "-" : "") + formattedNumber + suffixes[suffixIndex];
    // }

    public static string FormatAddress(string address)
    {
        // Kiểm tra nếu địa chỉ không hợp lệ
        if (string.IsNullOrEmpty(address) || address.Length < 10)
        {
            return "Invalid address";
        }

        // Lấy 6 ký tự đầu và 4 ký tự cuối
        string prefix = address.Substring(0, 6); // Lấy 6 ký tự đầu (bao gồm 0x)
        string suffix = address.Substring(address.Length - 4); // Lấy 4 ký tự cuối

        // Trả về định dạng: 6 ký tự đầu + ... + 4 ký tự cuối
        return $"{prefix}...{suffix}";
    }
}