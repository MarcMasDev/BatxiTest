using UnityEngine;
using UnityEngine.UI;

public class GoToMenu : MonoBehaviour
{
    private void Start()
    {
        Button b = GetComponent<Button>();
        if (b) b.onClick.AddListener(LoadMenu);
    }
    public void LoadMenu()
    {
        QuizLauncher.ReturnToMenu();
    }
}
