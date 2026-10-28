using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public static class QuizSerializer
{
    public static string ToJson(QuizData quiz)
    {
        return JsonUtility.ToJson(quiz, true);
    }

    public static string ToShareCode(QuizData quiz)
    {
        byte[] jsonBytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(quiz));

        //Se comprimen los bytes del JSON con GZip
        MemoryStream output = new MemoryStream();
        using (GZipStream gzip = new GZipStream(output, System.IO.Compression.CompressionLevel.Optimal))
        {
            gzip.Write(jsonBytes, 0, jsonBytes.Length);
        }

        //Base64 convierte los bytes comprimidos en letras y números que se pueden copiar y pegar
        return Convert.ToBase64String(output.ToArray());
    }

    public static QuizData Import(string text)
    {
        try
        {
            text = text.Trim();
            string json = text.StartsWith("{") ? text : ShareCodeToJson(text);

            QuizData quiz = JsonUtility.FromJson<QuizData>(json);
            if (quiz == null || quiz.levels.Count == 0)
            {
                Debug.LogError("El quiz no tiene niveles.");
                return null;
            }
            return quiz;
        }
        catch (Exception e)
        {
            Debug.LogError("No se ha podido leer el quiz: " + e.Message);
            return null;
        }
    }

    private static string ShareCodeToJson(string code)
    {
        byte[] compressed = Convert.FromBase64String(code);
        using (GZipStream gzip = new GZipStream(new MemoryStream(compressed), CompressionMode.Decompress))
        using (StreamReader reader = new StreamReader(gzip, Encoding.UTF8))
        {
            return reader.ReadToEnd();
        }
    }
}
