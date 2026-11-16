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
    public float cooldownHours; //Espera entre repasos espaciados. Lo pone el profesor; 0 = la del balance
    public List<QuestionData> questions = new List<QuestionData>();
}

[Serializable]
public class QuestionData
{
    public string text = "";
    public List<string> answers = new List<string>();
    public int correct;
    public string explanation = ""; //Opcional: por qué es la correcta. Se enseña al responder
}
