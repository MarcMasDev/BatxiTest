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
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text timerText;
    
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject completedMark;

    private QuizData quiz;
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
            int coins = balance.coinsPerAttempt.Evaluate(Upgrades.GetLevel(Upgrade.Coins));
            timerText.text = "";
            rewards.alpha = 1;
            coinsReward.SetAmount(coins);
            statusText.text = "";
        }
        else
        {
            rewards.alpha = 0;
            statusText.text = "Próximo repaso en " + FormatTime(seconds);
        }
    }

    //3h 05m / 12m 30s / 45s
    private string FormatTime(long seconds)
    {
        long hours = seconds / 3600;
        long minutes = seconds / 60 % 60;
        long secs = seconds % 60;

        if (hours > 0) return hours + "h " + minutes.ToString("00") + "m";
        if (minutes > 0) return minutes + "m " + secs.ToString("00") + "s";
        return secs + "s";
    }

    private void Play()
    {
        QuizLauncher.Play(quiz); //Abre la escena del quiz
    }
}
