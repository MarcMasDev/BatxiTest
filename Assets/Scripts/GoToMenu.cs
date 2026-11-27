using UnityEngine;
using UnityEngine.UI;

public class GoToMenu : MonoBehaviour
{
    [Tooltip("Antes de volver, añade al calendario el aviso del siguiente repaso de este test.")]
    [SerializeField] private bool notify;

    private void Start()
    {
        Button b = GetComponent<Button>();
        if (b) b.onClick.AddListener(LoadMenu);
    }
    public void LoadMenu()
    {
        if (notify) Notifications.RemindNextRepetition(QuizLauncher.SelectedQuiz);
        QuizLauncher.ReturnToMenu();
    }
}
