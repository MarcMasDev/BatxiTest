using System;
using UnityEngine;

//Progreso de cada quiz: cuántos repasos espaciados se han hecho, su nota y cuándo toca el siguiente
public static class QuizProgress
{
    public static int GetSpacedAttempts(QuizData quiz)
    {
        return PlayerPrefs.GetInt(Key(quiz) + "_Attempts", 0); //0 si nunca se ha jugado
    }

    //Aciertos de un repaso espaciado (el primero es el 0)
    public static int GetScore(QuizData quiz, int attempt)
    {
        return PlayerPrefs.GetInt(Key(quiz) + "_Score" + attempt, 0);
    }

    public static bool IsCompleted(QuizData quiz, ProgressionBalance balance)
    {
        return GetSpacedAttempts(quiz) >= balance.repetitionsToComplete;
    }

    //El siguiente repaso es el que completa el test: sus recompensas se multiplican. Si no, x1
    public static int GetRewardMultiplier(QuizData quiz, ProgressionBalance balance)
    {
        bool completesTest = GetSpacedAttempts(quiz) + 1 == balance.repetitionsToComplete;
        return completesTest ? balance.completionMultiplier : 1;
    }

    //Segundos que faltan para el siguiente repaso espaciado. 0 = ya toca
    public static long SecondsUntilSpaced(QuizData quiz)
    {
        long next = long.Parse(PlayerPrefs.GetString(Key(quiz) + "_NextTime", "0"));
        return Math.Max(0, next - Now());
    }

    public static bool IsSpacedAvailable(QuizData quiz)
    {
        return SecondsUntilSpaced(quiz) == 0;
    }

    //Guarda la nota, suma el repaso y vuelve a empezar la espera
    public static void FinishSpacedAttempt(QuizData quiz, int score, ProgressionBalance balance)
    {
        int attempts = GetSpacedAttempts(quiz);
        long cooldown = (long)(GetCooldownHours(quiz, balance) * 3600);

        PlayerPrefs.SetInt(Key(quiz) + "_Score" + attempts, score);
        PlayerPrefs.SetInt(Key(quiz) + "_Attempts", attempts + 1);
        PlayerPrefs.SetString(Key(quiz) + "_NextTime", (Now() + cooldown).ToString());
        PlayerPrefs.Save();
    }

    //La espera la pone el profesor en el quiz. Si no la pone (o es 0), se usa la del balance
    public static float GetCooldownHours(QuizData quiz, ProgressionBalance balance)
    {
        return quiz.cooldownHours > 0 ? quiz.cooldownHours : balance.defaultCooldownHours;
    }

    public static void Reset(QuizData quiz)
    {
        int attempts = GetSpacedAttempts(quiz);
        for (int i = 0; i < attempts; i++)
        {
            PlayerPrefs.DeleteKey(Key(quiz) + "_Score" + i);
        }
        PlayerPrefs.DeleteKey(Key(quiz) + "_Attempts");
        PlayerPrefs.DeleteKey(Key(quiz) + "_NextTime");
    }

    //Segundos desde 1970 (UTC), igual que el AFK del gym: funciona aunque el juego esté cerrado
    private static long Now()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    private static string Key(QuizData quiz)
    {
        return "Quiz_" + quiz.title + "_" + quiz.created;
    }
}
