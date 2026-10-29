using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuizView : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text questionText;

    [SerializeField] private Transform answersContainer;

    [SerializeField] private Button answerButtonPrefab;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button menuButton;

    [SerializeField] private QuizCard resultCard;

    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;

    [Tooltip("Se lanza al terminar el nivel. Para dar recompensas.")]
    [SerializeField] private UnityEvent onLevelCompleted;

    private QuizData quiz;
    private LevelData level;
    private int levelIndex;
    private int questionIndex;
    private int correctCount;
    private List<Button> answerButtons = new List<Button>();
    private List<int> shownOrder = new List<int>();

    private void Start()
    {
        nextButton.onClick.AddListener(NextQuestion);
        menuButton.onClick.AddListener(QuizLauncher.ReturnToMenu);
        nextButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        resultCard.gameObject.SetActive(false);

        quiz = QuizLauncher.SelectedQuiz;

        if (quiz == null)
        {
            QuizLauncher.ReturnToMenu();
            return; //La escena cambia al final del frame: sin este return seguiría y daría error
        }

        titleText.text = quiz.title;

        //El nivel que toca. Si ya están todos hechos, se acaba aquí
        levelIndex = QuizProgress.GetLevelsCompleted(quiz);
        if (levelIndex >= quiz.levels.Count)
        {
            levelText.text = "";
            progressText.text = "";
            questionText.text = "¡Quiz completado!";
            ShowEndButtons();
            return;
        }

        level = quiz.levels[levelIndex];
        levelText.text = level.name;
        ShowQuestion();
    }

    //Enseña la pregunta actual con un botón por respuesta
    private void ShowQuestion()
    {
        ClearAnswers();
        nextButton.gameObject.SetActive(false);

        QuestionData question = level.questions[questionIndex];
        progressText.text = (questionIndex + 1) + " / " + level.questions.Count;
        questionText.text = question.text;

        for (int i = 0; i < question.answers.Count; i++)
        {
            shownOrder.Add(i);
        }

        Shuffle(shownOrder); //Las respuestas se muestran en orden aleatorio

        foreach (int answerIndex in shownOrder)
        {
            Button button = Instantiate(answerButtonPrefab, answersContainer);
            button.GetComponentInChildren<TMP_Text>().text = question.answers[answerIndex];
            button.onClick.AddListener(() => Answer(question, answerIndex));
            answerButtons.Add(button);
        }
    }

    private void Answer(QuestionData question, int answerIndex)
    {
        if (answerIndex == question.correct) correctCount++;

        //La correcta en verde y, si se ha fallado, la pulsada en rojo
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (shownOrder[i] == question.correct) Paint(answerButtons[i], correctColor);
            else if (shownOrder[i] == answerIndex) Paint(answerButtons[i], wrongColor);
            answerButtons[i].interactable = false; //Solo se puede responder una vez
        }

        nextButton.gameObject.SetActive(true);
    }

    private void NextQuestion()
    {
        questionIndex++;
        if (questionIndex < level.questions.Count) ShowQuestion();
        else FinishLevel();
    }

    //4. Fin de la sesión
    private void FinishLevel()
    {
        ClearAnswers();
        nextButton.gameObject.SetActive(false);

        QuizProgress.CompleteLevel(quiz, levelIndex);
        onLevelCompleted.Invoke();

        progressText.text = "";
        questionText.text = level.name + " completado\n" + correctCount + " / " + level.questions.Count + " correctas";
        ShowEndButtons();
    }

    //Al final: la tarjeta del quiz (▶▶ = seguir jugando) y el botón de salir al menú
    private void ShowEndButtons()
    {
        resultCard.gameObject.SetActive(true);
        resultCard.Setup(quiz);
        menuButton.gameObject.SetActive(true);
    }

    private void ClearAnswers()
    {
        foreach (Button button in answerButtons) Destroy(button.gameObject);
        answerButtons.Clear();
        shownOrder.Clear();
    }


    private void Paint(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.disabledColor = color;
        button.colors = colors;
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
