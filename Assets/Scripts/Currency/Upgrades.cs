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

    //Lo que da una mejora en un nivel: inteligencia por acierto, monedas por repaso o fuerza por segundo de la máquina
    public static int GetGain(Upgrade upgrade, int level, ProgressionBalance balance)
    {
        if (upgrade == Upgrade.Brain) return balance.brainPerAnswer.Evaluate(level);
        if (upgrade == Upgrade.Coins) return balance.coinsPerAttempt.Evaluate(level);
        return balance.GetMachineStrength(upgrade).Evaluate(level);
    }

    //Nivel del personaje que hace falta para comprar el siguiente nivel de la mejora.
    //Con 1 por nivel: el nivel 1 de la mejora pide nivel 1, el 2 nivel 2... Con 5: los niveles 1-5 piden nivel 1, los 6-10 nivel 2...
    public static int GetRequiredCharacterLevel(Upgrade upgrade, ProgressionBalance balance)
    {
        int levelsPerCharacterLevel = GetCurrency(upgrade) == Currency.Strength
            ? balance.strengthUpgradeLevelsPerCharacterLevel
            : balance.machineUpgradeLevelsPerCharacterLevel;
        return 1 + GetLevel(upgrade) / levelsPerCharacterLevel;
    }

    public static bool IsUnlocked(Upgrade upgrade, ProgressionBalance balance)
    {
        int characterLevel = balance.CharacterLevel(Currencies.Get(Currency.XP));
        return characterLevel >= GetRequiredCharacterLevel(upgrade, balance);
    }

    public static bool TryBuy(Upgrade upgrade, ProgressionBalance balance)
    {
        int price = GetPrice(upgrade, balance);
        Currency currency = GetCurrency(upgrade);
        if (!IsUnlocked(upgrade, balance)) return false;
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
