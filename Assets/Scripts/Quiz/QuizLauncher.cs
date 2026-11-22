using UnityEngine.SceneManagement;
public static class QuizLauncher
{
    public const string QuizSceneName = "QuizzScene";
    public const string MenuSceneName = "Menu_3D";

    public static QuizData SelectedQuiz;

    public static void Play(QuizData quiz)
    {
        if (!QuizProgress.IsSpacedAvailable(quiz)) return; //Mientras dura la espera el quiz no se puede jugar

        SelectedQuiz = quiz;
        SceneManager.LoadScene(QuizSceneName);
    }

    public static void ReturnToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }
}
