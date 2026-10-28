using UnityEngine.SceneManagement;
public static class QuizLauncher
{
    public const string QuizSceneName = "QuizzScene";
    public const string MenuSceneName = "Menu_3D";

    public static QuizData SelectedQuiz;

    public static void Play(QuizData quiz)
    {
        SelectedQuiz = quiz;
        SceneManager.LoadScene(QuizSceneName);
    }

    public static void ReturnToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }
}
