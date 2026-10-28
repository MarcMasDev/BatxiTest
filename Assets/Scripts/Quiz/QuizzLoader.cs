using TMPro;
using UnityEngine;

public class QuizzLoader : MonoBehaviour
{
    [SerializeField] private TextAsset quizFile;
    [SerializeField] private TMP_InputField codeInput;

    public void PlayFile()
    {
        QuizData quiz = QuizSerializer.Import(quizFile.text);
        if (quiz != null) QuizLauncher.Play(quiz);
    }

    public void PlayCode()
    {
        QuizData quiz = QuizSerializer.Import(codeInput.text);
        if (quiz != null) QuizLauncher.Play(quiz);
    }

    [ContextMenu("Copiar código del quiz")]
    private void CopyShareCode()
    {
        GUIUtility.systemCopyBuffer = QuizSerializer.ToShareCode(QuizSerializer.Import(quizFile.text));
        Debug.Log("Código copiado al portapapeles");
    }
}
