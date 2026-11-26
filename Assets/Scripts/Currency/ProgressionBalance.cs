using System;
using UnityEngine;


[CreateAssetMenu(fileName = "ProgressionBalance", menuName = "Batxi/ProgressionBalance")]
public class ProgressionBalance : ScriptableObject
{
    [Header("Inicio (se da una vez, la primera vez que se abre el gym)")]
    public int startingStrength = 200;
    public int startingCoins = 100;

    [Header("Quiz")]
    public GrowthCurve coinsPerAttempt = new GrowthCurve { baseValue = 20, growth = 1.2f }; //Monedas por repaso espaciado. Sube con la mejora de monedas
    public GrowthCurve brainPerAnswer = new GrowthCurve { baseValue = 10, growth = 1.2f };
    public float defaultCooldownHours = 24; //Espera entre repasos si el quiz no trae la suya
    public int repetitionsToComplete = 3; //Repasos espaciados para completar un quiz
    public int completionMultiplier = 2;

    [Header("Mejoras con fuerza")]
    public GrowthCurve strengthUpgradeCost = new GrowthCurve { baseValue = 100, growth = 1.6f };

    [Header("Personaje")]
    public GrowthCurve brainToLevelUp = new GrowthCurve { baseValue = 50, growth = 1.7f };

    //Cada nivel del personaje desbloquea estos niveles de cada mejora. Sin subir de nivel, la mejora se queda bloqueada
    public int strengthUpgradeLevelsPerCharacterLevel = 1; //Mejoras de inteligencia y monedas (se pagan con fuerza)
    public int machineUpgradeLevelsPerCharacterLevel = 5; //Mejoras de las máquinas (se pagan con monedas)

    //Todas las máquinas suben de precio igual (x1.5). Cambia lo que cuesta entrar y cuánta fuerza suma cada mejora
    [Header("Máquinas del gym")]
    public GrowthCurve machineRunUpgradeCost = new GrowthCurve { baseValue = 20, growth = 1.5f };
    public GrowthCurve machineRunStrength = new GrowthCurve { baseValue = 4, growth = 1.2f };
    public GrowthCurve machineCurlUpgradeCost = new GrowthCurve { baseValue = 60, growth = 1.5f };
    public GrowthCurve machineCurlStrength = new GrowthCurve { baseValue = 5, growth = 1.3f };
    public GrowthCurve machineBenchUpgradeCost = new GrowthCurve { baseValue = 150, growth = 1.5f };
    public GrowthCurve machineBenchStrength = new GrowthCurve { baseValue = 6, growth = 1.4f };
    public GrowthCurve machineLegUpgradeCost = new GrowthCurve { baseValue = 400, growth = 1.5f };
    public GrowthCurve machineLegStrength = new GrowthCurve { baseValue = 8, growth = 1.5f };

    [Header("Ciclo de entrenamiento")]
    public float machineSeconds = 10; //Tiempo en cada máquina
    public float restSeconds = 5; //Descanso entre máquinas
    public float maxAfkHours = 24; //Lo máximo que cuenta el tiempo fuera del juego

    //Fuerza media por segundo de un ciclo
    public float AverageStrengthPerSecond()
    {
        float total = 0;
        foreach (Upgrade machine in new[] { Upgrade.Run, Upgrade.Curl, Upgrade.Bench, Upgrade.Leg })
        {
            total += GetMachineStrength(machine).Evaluate(Upgrades.GetLevel(machine));
        }

        float averageMachine = total / 4;
        return averageMachine * machineSeconds / (machineSeconds + restSeconds);
    }

    //Las curvas de cada máquina, para no repetir este switch en cada script
    public GrowthCurve GetMachineCost(Upgrade machine)
    {
        switch (machine)
        {
            case Upgrade.Run: return machineRunUpgradeCost;
            case Upgrade.Curl: return machineCurlUpgradeCost;
            case Upgrade.Bench: return machineBenchUpgradeCost;
            default: return machineLegUpgradeCost;
        }
    }

    public GrowthCurve GetMachineStrength(Upgrade machine)
    {
        switch (machine)
        {
            case Upgrade.Run: return machineRunStrength;
            case Upgrade.Curl: return machineCurlStrength;
            case Upgrade.Bench: return machineBenchStrength;
            default: return machineLegStrength;
        }
    }

    //El personaje empieza en el nivel 1
    public int CharacterLevel(int totalBrain)
    {
        int level = 1;
        int cost = brainToLevelUp.Evaluate(level - 1);

        while (cost > 0 && totalBrain >= cost)
        {
            totalBrain -= cost;
            level++;
            cost = brainToLevelUp.Evaluate(level - 1);
        }

        return level;
    }
}

[Serializable]
public class GrowthCurve
{
    public float baseValue = 10;
    public float growth = 1.2f;

    public int Evaluate(int level)
    {
        return Mathf.RoundToInt(baseValue * Mathf.Pow(growth, level));
    }
}
