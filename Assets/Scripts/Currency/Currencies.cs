using System;
using UnityEngine;

public enum Currency
{
    Brain,
    Strength,
    Coins,
}

public static class Currencies
{
    public static event Action<Currency> Changed;

    public static int Get(Currency currency)
    {
        return PlayerPrefs.GetInt(Key(currency), 0);
    }

    public static void Add(Currency currency, int amount)
    {
        Set(currency, Get(currency) + amount);
    }
    public static bool Spend(Currency currency, int amount)
    {
        if (Get(currency) < amount) return false;

        Set(currency, Get(currency) - amount);
        return true;
    }

    private static void Set(Currency currency, int amount)
    {
        PlayerPrefs.SetInt(Key(currency), amount);
        PlayerPrefs.Save();
        Changed?.Invoke(currency);
    }

    private static string Key(Currency currency)
    {
        return "Currency_" + currency;
    }
}
