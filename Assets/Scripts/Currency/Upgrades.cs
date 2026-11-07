using UnityEngine;

public enum Upgrade
{
    Brain,
    Coins,
    Curl,
    Run,
    Bench,
    Leg,
}

public static class Upgrades
{
    public static int GetLevel(Upgrade upgrade)
    {
        return PlayerPrefs.GetInt(Key(upgrade), 0);
    }

    public static Currency GetCurrency(Upgrade upgrade)
    {
        if (upgrade == Upgrade.Brain || upgrade == Upgrade.Coins) return Currency.Strength;
        return Currency.Coins;
    }

    public static int GetPrice(Upgrade upgrade, ProgressionBalance balance)
    {
        int level = GetLevel(upgrade);
        if (upgrade == Upgrade.Brain || upgrade == Upgrade.Coins) return balance.strengthUpgradeCost.Evaluate(level);
        return balance.GetMachineCost(upgrade).Evaluate(level);
    }

    //Lo que da una mejora en un nivel: inteligencia o monedas por acierto, o fuerza por segundo de la máquina
    public static int GetGain(Upgrade upgrade, int level, ProgressionBalance balance)
    {
        if (upgrade == Upgrade.Brain) return balance.brainPerAnswer.Evaluate(level);
        if (upgrade == Upgrade.Coins) return balance.coinsPerAnswer.Evaluate(level);
        return balance.GetMachineStrength(upgrade).Evaluate(level);
    }

    public static bool TryBuy(Upgrade upgrade, ProgressionBalance balance)
    {
        int price = GetPrice(upgrade, balance);
        Currency currency = GetCurrency(upgrade);
        if (Currencies.Get(currency) < price) return false;

        PlayerPrefs.SetInt(Key(upgrade), GetLevel(upgrade) + 1);
        Currencies.Spend(currency, price);
        return true;
    }

    private static string Key(Upgrade upgrade)
    {
        return "Upgrade_" + upgrade;
    }
}
