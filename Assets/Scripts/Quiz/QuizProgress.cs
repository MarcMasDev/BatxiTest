using UnityEngine;

public static class QuizProgress
{
    public static int GetLevelsCompleted(QuizData quiz)
    {
        return PlayerPrefs.GetInt(Key(quiz), 0); //0 si nunca se ha jugado
    }

    public static void CompleteLevel(QuizData quiz, int levelIndex)
    {
        int completed = levelIndex + 1;
        if (completed > GetLevelsCompleted(quiz)) //Así repetir un nivel anterior no hace retroceder el progreso
        {
            PlayerPrefs.SetInt(Key(quiz), completed);
            PlayerPrefs.Save();
        }
    }

    public static void Reset(QuizData quiz)
    {
        PlayerPrefs.DeleteKey(Key(quiz));
    }

    private static string Key(QuizData quiz)
    {
        return "QuizLevelsCompleted_" + quiz.title + "_" + quiz.created;
    }
}
