using System;
using UnityEngine;

//Lee un test desde el texto de su archivo .json
public static class QuizSerializer
{
    public static QuizData Import(string json)
    {
        try
        {
            QuizData quiz = JsonUtility.FromJson<QuizData>(json);
            if (quiz == null || quiz.questions.Count == 0)
            {
                Debug.LogError("El test no tiene preguntas.");
                return null;
            }
            return quiz;
        }
        catch (Exception e)
        {
            Debug.LogError("No se ha podido leer el test: " + e.Message);
            return null;
        }
    }
}
