using System;
using System.Globalization;
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

    public static string AddSecondToUtcNow(long seconds)
    {
        DateTime currentUtcTime = DateTime.UtcNow;

        // Cộng thêm số giây
        DateTime futureTime = currentUtcTime.AddSeconds(seconds);

        // Định dạng ngày tháng bằng tiếng Anh (US culture)
        string formattedDate = futureTime.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);

        return formattedDate;
    }

    public static int SecondToDay(long seconds)
    {
        return (int)(seconds / 86400);
    }

    public static float Round(float value, int decimalPlaces = 5)
    {
        return (float)Math.Round(value, decimalPlaces);
    }

    public static string FormatCurrencyNumberText(long number, long threshold = 100000, bool useShortFormat = true)
    {
        // Nếu không dùng định dạng viết tắt, chỉ định dạng với dấu phẩy
        if (!useShortFormat)
        {
            return number.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture);
        }

        // Nếu số nhỏ hơn ngưỡng, định dạng với dấu phẩy
        if (Math.Abs(number) < threshold)
        {
            return number.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture);
        }

        // Xử lý các đơn vị viết tắt (k, m, b)
        string[] suffixes = { "K", "M", "B" };
        int suffixIndex = -1;
        double shortenedNumber = Math.Abs(number);

        // Chia nhỏ số cho đến khi nhỏ hơn 1000
        while (shortenedNumber >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            shortenedNumber /= 1000;
            suffixIndex++;
        }

        // Định dạng số nguyên nếu là số nguyên, nếu không thì giữ tối đa 2 chữ số thập phân
        string formattedNumber = shortenedNumber % 1 == 0
            ? shortenedNumber.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture)
            : shortenedNumber.ToString("#,##0.##", System.Globalization.CultureInfo.InvariantCulture).TrimEnd('0')
                .TrimEnd('.');

        // Thêm dấu trừ nếu số âm và thêm hậu tố
        return (number < 0 ? "-" : "") + formattedNumber + suffixes[suffixIndex];
    }

    public static string FormatAddress(string address)
    {
        if (string.IsNullOrEmpty(address) || address.Length < 10)
        {
            return "Invalid address";
        }

        string prefix = address.Substring(0, 6);
        string suffix = address.Substring(address.Length - 4);
        return $"{prefix}...{suffix}";
    }


    public static string TimeFormater(
        long timeSeconds,
        bool showDays = true,
        string daysSuffix = "d",
        string hoursSuffix = "hr",
        string minutesSuffix = "m",
        string secondsSuffix = "s")
    {
        long days = 0;
        long hours = 0;
        long minutes = (timeSeconds % 3600) / 60;
        long seconds = timeSeconds % 60;

        if (showDays)
        {
            // Tính ngày và giờ khi hiển thị ngày
            days = timeSeconds / (24 * 3600);
            hours = (timeSeconds % (24 * 3600)) / 3600;
            return
                $"{days:D2}{daysSuffix}:{hours:D2}{hoursSuffix}:{minutes:D2}{minutesSuffix}:{seconds:D2}{secondsSuffix}";
        }
        else
        {
            // Cộng dồn ngày vào giờ khi không hiển thị ngày
            hours = timeSeconds / 3600;
            return $"{hours:D2}{hoursSuffix}:{minutes:D2}{minutesSuffix}:{seconds:D2}{secondsSuffix}";
        }
    }

    public static void LogCaller(object message = null,
        [System.Runtime.CompilerServices.CallerLineNumber]
        int line = 0
        , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
        , [System.Runtime.CompilerServices.CallerFilePath] string filePath = ""
    )
    {
        // Can replace UnityEngine.Debug.Log with any logging API you want
        UnityEngine.Debug.Log($"{line} :: {memberName} :: {filePath} === {message}");
    }
}