using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 *Tarjeta de un quiz
 */

public class QuizCard : MonoBehaviour
{
    [SerializeField] private TextAsset quizFile;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text progressText;

    [SerializeField] private Button playButton;
    [SerializeField] private GameObject completedMark;

    private QuizData quiz;

    private void Awake()
    {
        playButton.onClick.AddListener(Play);
    }

    private void Start()
    {
        if (quizFile != null) Setup(QuizSerializer.Import(quizFile.text));
    }

    public void Setup(QuizData quizToShow)
    {
        quiz = quizToShow;
        if (quiz == null) return;

        int completed = QuizProgress.GetLevelsCompleted(quiz);
        bool allCompleted = completed >= quiz.levels.Count;

        titleText.text = quiz.title;
        progressSlider.maxValue = quiz.levels.Count;
        progressSlider.value = completed;
        progressText.text = completed + " / " + quiz.levels.Count;

        playButton.gameObject.SetActive(!allCompleted);
        if (completedMark != null) completedMark.SetActive(allCompleted);
    }

    private void Play()
    {
        QuizLauncher.Play(quiz); //Abre la escena del quiz con el siguiente nivel
    }
}
