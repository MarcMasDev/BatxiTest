using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Una respuesta del quiz: su letra, su texto y cómo reacciona al responder
public class QuizQuestion : MonoBehaviour
{
    public TMP_Text letter;
    public TMP_Text question;

    [SerializeField] private Image background;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private ButtonFeel feel;

    //Se pinta de verde y da un salto
    public void ShowCorrect(Color color, float delay)
    {
        Paint(color);
        StartCoroutine(PunchAfter(delay));
    }

    //Se pinta de rojo y tiembla
    public void ShowWrong(Color color)
    {
        Paint(color);
        StartCoroutine(Shake());
    }

    //Las demás se apagan para que se vea bien cuál era la buena
    public void Dim()
    {
        group.alpha = 0.35f;
    }

    private void Paint(Color color)
    {
        background.color = color;
        letter.color = Color.white;
        question.color = Color.white;
    }

    private IEnumerator PunchAfter(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        feel.Punch(3f);
    }

    private IEnumerator Shake()
    {
        Vector3 start = transform.localPosition;
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / 0.4f)
        {
            transform.localPosition = start + Vector3.right * Mathf.Sin(t * 40f) * 18f * (1f - t);
            yield return null;
        }
        transform.localPosition = start;
    }
}
