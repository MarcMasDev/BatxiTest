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
    [SerializeField] private Image colorDot;
    [SerializeField] private Color color = Color.white;

    [SerializeField] private Transform levelsContainer;
    [SerializeField] private QuizLevelNode levelNodePrefab;

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
        colorDot.color = color;

        foreach (Transform child in levelsContainer) Destroy(child.gameObject);
        for (int i = 0; i < quiz.levels.Count; i++)
        {
            QuizLevelNode node = Instantiate(levelNodePrefab, levelsContainer);
            bool isCompleted = i < completed;
            bool nextCompleted = i + 1 < completed;
            bool isLast = i == quiz.levels.Count - 1;
            node.Setup(i + 1, isCompleted, nextCompleted, isLast, color);
        }

        playButton.gameObject.SetActive(!allCompleted);
        if (completedMark != null) completedMark.SetActive(allCompleted);
    }

    private void Play()
    {
        QuizLauncher.Play(quiz); //Abre la escena del quiz con el siguiente nivel
    }
}
