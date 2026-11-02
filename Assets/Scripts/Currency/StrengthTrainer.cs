using System;
using UnityEngine;

public class StrengthTrainer : MonoBehaviour
{
    [SerializeField] private ActionController actionController;
    [SerializeField] private ProgressionBalance balance;

    private const string LastSeenKey = "StrengthLastSeen";

    private float timer;

    private void Start()
    {
        GiveAfkStrength();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < 1f) return;
        timer -= 1f; //Una vez por segundo (y no cada frame, para no guardar en PlayerPrefs 60 veces por segundo)

        PlayerPrefs.SetString(LastSeenKey, Now().ToString()); //Última vez que el gym estaba abierto (para el AFK)

        if (actionController.Current == TrainingAction.Resting) return;

        Upgrade machine = GetMachine(actionController.Current);
        int strength = balance.GetMachineStrength(machine).Evaluate(Upgrades.GetLevel(machine));
        Currencies.Add(Currency.Strength, strength);
    }

    //Al volver: segundos fuera * fuerza media por segundo
    private void GiveAfkStrength()
    {
        string lastSeen = PlayerPrefs.GetString(LastSeenKey, "");
        if (lastSeen == "") return; //Primera vez que se abre el gym

        long seconds = Now() - long.Parse(lastSeen);
        seconds = Math.Min(seconds, (long)(balance.maxAfkHours * 3600));

        int strength = (int)(seconds * balance.AverageStrengthPerSecond());
        if (strength <= 0) return; //Nada que dar (o el reloj del dispositivo ha ido hacia atrás)

        Currencies.Add(Currency.Strength, strength);
        Debug.Log("AFK: " + seconds + " s fuera, +" + strength + " fuerza");
    }

    //Segundos desde 1970 (UTC): un número que solo sube, fácil de guardar y restar
    private long Now()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    private Upgrade GetMachine(TrainingAction action)
    {
        switch (action)
        {
            case TrainingAction.Curl: return Upgrade.Curl;
            case TrainingAction.Run: return Upgrade.Run;
            case TrainingAction.BenchPress: return Upgrade.Bench;
            default: return Upgrade.Leg;
        }
    }
}
