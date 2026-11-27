using UnityEngine;
using UnityEngine.UI;

//Botón "Recordármelo" de la tarjeta: añade al calendario el aviso del siguiente repaso de ese test
public class ReminderButton : MonoBehaviour
{
    [SerializeField] private QuizCard card;
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(Remind);
    }

    private void Update()
    {
        if (card.Quiz != null) button.interactable = QuizProgress.SecondsUntilSpaced(card.Quiz) > 0;
    }

    private void Remind()
    {
        Notifications.RemindNextRepetition(card.Quiz);
    }
}
