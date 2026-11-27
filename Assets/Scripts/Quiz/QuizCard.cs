using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 *Tarjeta de un quiz
 */

public class QuizCard : MonoBehaviour
{
    [SerializeField] private TextAsset quizFile;
    [SerializeField] private ProgressionBalance balance;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text progressText;

    [SerializeField] private CanvasGroup rewards;
    [SerializeField] private Reward coinsReward; //Monedas que da el siguiente repaso
    [SerializeField] private Reward xpReward; //Inteligencia por cada pregunta acertada
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject[] notAvailable;
    
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject playButtonVisuals;
    [SerializeField] private GameObject completedMark;

    private QuizData quiz;
    public QuizData Quiz => quiz; //El test que enseña la tarjeta (lo usa el botón de recordatorio)
    private float timer;

    private void Awake()
    {
        playButton.onClick.AddListener(Play);
    }

    private void Start()
    {
        if (quizFile != null) Setup(QuizSerializer.Import(quizFile.text));
    }

    private void Update()
    {
        if (quiz == null) return;

        timer += Time.deltaTime;
        if (timer < 1f) return;
        timer -= 1f; //Una vez por segundo basta para la cuenta atrás

        RefreshStatus();
    }

    public void Setup(QuizData quizToShow)
    {
        quiz = quizToShow;
        if (quiz == null) return;

        int attempts = QuizProgress.GetSpacedAttempts(quiz);
        int toComplete = balance.repetitionsToComplete;

        titleText.text = quiz.title;
        progressSlider.maxValue = toComplete;
        progressSlider.value = attempts;
        progressText.text = Mathf.Min(attempts, toComplete) + " / " + toComplete;

        //Completado no bloquea nada: se puede seguir repasando
        if (completedMark != null) completedMark.SetActive(QuizProgress.IsCompleted(quiz, balance));

        RefreshStatus();
    }

    private void RefreshStatus()
    {
        long seconds = QuizProgress.SecondsUntilSpaced(quiz);

        //Mientras dura la espera el quiz no se puede jugar
        playButton.interactable = seconds == 0;

        if (seconds == 0)
        {
            int multiplier = QuizProgress.GetRewardMultiplier(quiz, balance); //x2 si este repaso completa el test
            int coins = balance.coinsPerAttempt.Evaluate(Upgrades.GetLevel(Upgrade.Coins)) * multiplier;
            int xpPerAnswer = balance.brainPerAnswer.Evaluate(Upgrades.GetLevel(Upgrade.Brain)) * multiplier;
            timerText.text = "";
            rewards.alpha = 1;
            coinsReward.SetAmount(coins);
            xpReward.SetAmount(xpPerAnswer);
            playButtonVisuals.gameObject.SetActive(true);
            for (int i = 0; i < notAvailable.Length; i++)
            {
                notAvailable[i].SetActive(false);
            }
        }
        else
        {
            rewards.alpha = 0;
            playButtonVisuals.gameObject.SetActive(false);
            for (int i = 0; i < notAvailable.Length; i++)
            {
                notAvailable[i].SetActive(true);
            }
            timerText.text = NumberFormat.Duration(seconds);
        }
    }

    public void Play()
    {
        QuizLauncher.Play(quiz); //Abre la escena del quiz
    }
}
