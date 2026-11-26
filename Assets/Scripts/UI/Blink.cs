using System.Collections;
using UnityEngine;

//Hace parpadear un CanvasGroup: de 0 a 1 y vuelta a 0, tantas veces como se diga
public class Blink : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private int times = 3; //Parpadeos (0 → 1 → 0 cuenta como uno)
    [SerializeField] private float duration = 0.5f; //Segundos de cada parpadeo: más bajo = más rápido

    public void Play()
    {
        StopAllCoroutines(); //Si ya estaba parpadeando, vuelve a empezar
        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        for (int i = 0; i < times; i++)
        {
            for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
            {
                group.alpha = Mathf.PingPong(t * 2f, 1f); //Sube en la primera mitad y baja en la segunda
                yield return null;
            }
        }

        group.alpha = 0f;
    }
}
