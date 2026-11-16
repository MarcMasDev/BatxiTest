using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuizView : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text questionText;

    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text progressText;

    [SerializeField] private Transform answersContainer;

    [SerializeField] private Button answerButtonPrefab;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button menuButton;

    [SerializeField] private QuizCard resultCard;

    [Header("Feedback al responder")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private Image nextBackground;
    [SerializeField] private TMP_Text nextLabel;
    [SerializeField] private GameObject explanationPanel;
    [SerializeField] private TMP_Text explanationText;

    [Header("Recompensas (se dan al terminar el intento)")]
    [SerializeField] private ProgressionBalance balance;

    [Tooltip("Se lanza al terminar el intento.")]
    [SerializeField] private UnityEvent onLevelCompleted;

    private QuizData quiz;
    private bool spaced; //Repaso espaciado (cuenta y da monedas) o práctica extra (solo inteligencia)
    private int questionIndex;
    private int correctCount;
    private List<Button> answerButtons = new List<Button>();
    private List<int> questionOrder = new List<int>();
    private List<int> shownOrder = new List<int>();
    private Color nextColor; //Color original del botón Siguiente (cuando se falla se queda así)

    private void Start()
    {
        nextColor = nextBackground.color;
        nextButton.onClick.AddListener(NextQuestion);
        menuButton.onClick.AddListener(QuizLauncher.ReturnToMenu);
        nextButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        resultCard.gameObject.SetActive(false);

        quiz = QuizLauncher.SelectedQuiz;

        if (quiz == null)
        {
            QuizLauncher.ReturnToMenu();
            return;
        }

        titleText.text = quiz.title;

        //Si ya ha pasado la espera es un repaso espaciado. Si no, práctica extra
        spaced = QuizProgress.IsSpacedAvailable(quiz);
        levelText.text = GetAttemptName();

        for (int i = 0; i < quiz.questions.Count; i++)
        {
            questionOrder.Add(i);
        }

        Shuffle(questionOrder); //Las mismas preguntas en cada intento, pero en otro orden

        ShowQuestion();
    }

    private string GetAttemptName()
    {
        if (!spaced) return "Práctica extra";
        if (QuizProgress.IsCompleted(quiz, balance)) return "Repaso";
        return "Repaso " + (QuizProgress.GetSpacedAttempts(quiz) + 1) + " / " + balance.repetitionsToComplete;
    }

    //Enseña la pregunta actual con un botón por respuesta
    private void ShowQuestion()
    {
        ClearAnswers();
        nextButton.gameObject.SetActive(false);

        QuestionData question = quiz.questions[questionOrder[questionIndex]];
        progressText.text = (questionIndex + 1) + " / " + quiz.questions.Count;
        progressSlider.maxValue = quiz.questions.Count;
        progressSlider.value = questionIndex + 1;
        questionText.text = question.text;

        for (int i = 0; i < question.answers.Count; i++)
        {
            shownOrder.Add(i);
        }

        Shuffle(shownOrder); //Las respuestas se muestran en orden aleatorio

        for (int i = 0; i < shownOrder.Count; i++)
        {
            int answerIndex = shownOrder[i];
            Button button = Instantiate(answerButtonPrefab, answersContainer);
            QuizQuestion answerText = button.GetComponentInChildren<QuizQuestion>();
            answerText.question.text = question.answers[answerIndex];
            answerText.letter.text = GetLetter(i).ToString(); //La letra va por posición: siempre A, B, C, D de arriba a abajo
            button.onClick.AddListener(() => Answer(question, answerIndex));
            answerButtons.Add(button);
        }
    }

    private void Answer(QuestionData question, int answerIndex)
    {
        bool correct = answerIndex == question.correct;
        if (correct) correctCount++;

        //La pulsada en verde (salta) o en rojo (tiembla), la correcta siempre en verde y el resto apagadas
        for (int i = 0; i < answerButtons.Count; i++)
        {
            QuizQuestion answer = answerButtons[i].GetComponentInChildren<QuizQuestion>();
            answerButtons[i].interactable = false; //Solo se puede responder una vez

            if (shownOrder[i] == question.correct) answer.ShowCorrect(correctColor, correct ? 0f : 0.35f); //Si has fallado, salta después del temblor para que la mires
            else if (shownOrder[i] == answerIndex) answer.ShowWrong(wrongColor);
            else answer.Dim();
        }

        //El botón de Siguiente también lo dice
        char correctLetter = GetLetter(shownOrder.IndexOf(question.correct));
        nextLabel.text = correct ? "¡Correcto!  <size=70%>Siguiente</size>" : "Era la " + correctLetter + "  <size=70%>Siguiente</size>";
        nextBackground.color = correct ? correctColor : nextColor;

        //La explicación (si el quiz la trae), aciertes o falles, para aprender el porqué
        if (!string.IsNullOrEmpty(question.explanation))
        {
            explanationText.text = "<color=#FFD447>¿Por qué?</color> " + question.explanation;
            explanationPanel.SetActive(true);
        }

        nextButton.gameObject.SetActive(true);
    }

    private void NextQuestion()
    {
        questionIndex++;
        if (questionIndex < quiz.questions.Count) ShowQuestion();
        else FinishAttempt();
    }

    private void FinishAttempt()
    {
        ClearAnswers();
        nextButton.gameObject.SetActive(false);

        //Recompensas: la inteligencia siempre, las monedas solo en un repaso espaciado
        int brain = balance.brainPerAnswer.Evaluate(Upgrades.GetLevel(Upgrade.Brain)) * correctCount;
        int coins = spaced ? balance.coinsPerAttempt.Evaluate(Upgrades.GetLevel(Upgrade.Coins)) : 0;
        Currencies.Add(Currency.Brain, brain);
        Currencies.Add(Currency.Coins, coins);

        //El repaso cuenta para completar el quiz y vuelve a empezar la espera
        if (spaced) QuizProgress.FinishSpacedAttempt(quiz, correctCount, balance);
        onLevelCompleted.Invoke();

        string rewards = spaced ? "+" + brain + " inteligencia   +" + coins + " monedas" : "+" + brain + " inteligencia (la práctica extra no da monedas)";

        progressText.text = "";
        progressSlider.gameObject.SetActive(false);
        questionText.text = levelText.text + " terminado\n" + correctCount + " / " + quiz.questions.Count + " correctas\n" + rewards;
        ShowEndButtons();
    }

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
        explanationPanel.SetActive(false);
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

    private char GetLetter(int index)
    {
        switch (index)
        {
            case 0:
                return 'A';
            case 1:
                return 'B';
            case 2:
                return 'C';
            default:
                return 'D';
        }
    }
}
