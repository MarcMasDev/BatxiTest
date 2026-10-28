using System;
using System.Collections.Generic;

/*
 * Qué ES un quiz. Solo datos: es lo que se guarda en el .json y lo que se comparte con el código.
 */

[Serializable]
public class QuizData
{
    public string title = "";
    public string created = "";
    public List<LevelData> levels = new List<LevelData>();
}

[Serializable]
public class LevelData
{
    public string name = "";
    public List<QuestionData> questions = new List<QuestionData>();
}

[Serializable]
public class QuestionData
{
    public string text = "";
    public List<string> answers = new List<string>();
    public int correct;
}