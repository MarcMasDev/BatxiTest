using System;
using System.Globalization;

public static class NumberFormat
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture; //Coma para miles y punto para decimales
    private static readonly string[] Suffixes = { "", "K", "M", "B" };

    public static string Short(int value)
    {
        if (value < 10000) return value.ToString("#,0", Culture);

        decimal shown = value; //decimal y no float: así 1.15 es exactamente 1.15 al redondear
        int suffix = 0;
        while (shown >= 1000 && suffix < Suffixes.Length - 1)
        {
            shown /= 1000;
            suffix++;
        }

        //3 cifras: 1.25 / 12.5 / 125
        int decimals = shown < 10 ? 2 : shown < 100 ? 1 : 0;
        decimal factor = (decimal)Math.Pow(10, decimals);
        shown = Math.Floor(shown * factor) / factor;

        return shown.ToString("#,0.##", Culture) + Suffixes[suffix];
    }
}
