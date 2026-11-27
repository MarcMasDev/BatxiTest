using System;
using UnityEngine;

public class StrengthTrainer : MonoBehaviour
{
    [SerializeField] private ActionController actionController;
    [SerializeField] private ProgressionBalance balance;

    [SerializeField] private AfkClaimPopup claimPopup;
    [SerializeField] private float minMinutesForPopup = 10; //Menos tiempo fuera (por ejemplo, volver de un test) se suma sin ventana

    [SerializeField] private FloatingCurrency floatingStrength;
    [SerializeField] private Transform floatingStrengthParent;

    private const string LastSeenKey = "StrengthLastSeen";
    private const long ResumeGapSeconds = 10;

    private float timer;
    private float strengthLeftover; //Los decimales de fuerza que aún no llegan a 1 (se suman el siguiente segundo)
    private long lastTick; 

    private void Start()
    {
        GiveAfkStrength();
        lastTick = Now();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < 1f) return;
        timer -= 1f; //Una vez por segundo (y no cada frame, para no guardar en PlayerPrefs 60 veces por segundo)

        //En la web (y en el móvil) cambiar de app o de pestaña congela el juego sin cerrarlo: al volver no se vuelve a
        //cargar la escena, así que ese hueco se cuenta aquí como tiempo fuera
        long now = Now();
        if (now - lastTick > ResumeGapSeconds) GiveAfkStrength();
        lastTick = now;

        PlayerPrefs.SetString(LastSeenKey, now.ToString()); //Última vez que el gym estaba abierto (para el AFK)

        if (actionController.Current == TrainingAction.Resting) return;

        Upgrade machine = GetMachine(actionController.Current);
        //La máquina da fuerza con decimales (por ejemplo 4.8/s): se suma la parte entera y el resto se guarda
        strengthLeftover += balance.GetMachineStrength(machine).EvaluateExact(Upgrades.GetLevel(machine));
        int strength = (int)strengthLeftover;
        if (strength == 0) return; //Con menos de 1 por segundo, algunos segundos aún no llega a 1: no se suma ni se enseña "+0"

        strengthLeftover -= strength;
        Currencies.Add(Currency.Strength, strength);

        if (floatingStrength != null) floatingStrength.Spawn(floatingStrengthParent, strength);
    }


    private void GiveAfkStrength()
    {
        string lastSeen = PlayerPrefs.GetString(LastSeenKey, "");

        //Primera vez que se abre el gym: no hay AFK, pero sí lo inicial para empezar a mejorar
        if (lastSeen == "")
        {
            Currencies.Add(Currency.Strength, balance.startingStrength);
            Currencies.Add(Currency.Coins, balance.startingCoins);
            PlayerPrefs.SetString(LastSeenKey, Now().ToString()); //Así no se vuelve a dar si se sale antes del primer segundo
            return;
        }

        //El tiempo fuera de verdad se enseña entero, pero la fuerza solo cuenta hasta el máximo de horas:
        //así volver tarde no se castiga y el máximo solo sirve para equilibrar la economía
        long secondsAway = Now() - long.Parse(lastSeen);
        long rewardedSeconds = Math.Min(secondsAway, (long)(balance.maxAfkHours * 3600));

        int strength = (int)(rewardedSeconds * balance.AverageStrengthPerSecond());
        if (strength <= 0) return; //Nada que dar (o el reloj del dispositivo ha ido hacia atrás)

        Debug.Log("AFK: " + secondsAway + " s fuera (cuentan " + rewardedSeconds + "), +" + strength + " fuerza");

        //Si ha estado un buen rato fuera, se reclama en la ventana. Si no, se suma directamente
        if (secondsAway >= minMinutesForPopup * 60) claimPopup.Show(strength, secondsAway);
        else Currencies.Add(Currency.Strength, strength);
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
