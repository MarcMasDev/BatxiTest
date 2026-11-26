using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Nivel del personaje según la inteligencia total: "Nv. 3" y la barra con lo que falta para el siguiente
public class CharacterLevelDisplay : MonoBehaviour
{
    [SerializeField] private ProgressionBalance balance;

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text progressText;

    private void OnEnable()
    {
        Currencies.Changed += OnCurrencyChanged;
        Refresh();
    }

    private void OnDisable()
    {
        Currencies.Changed -= OnCurrencyChanged;
    }

    private void OnCurrencyChanged(Currency changed)
    {
        if (changed == Currency.XP) Refresh();
    }

    private void Refresh()
    {
        int brain = Currencies.Get(Currency.XP);
        int level = 1; //El personaje empieza en el nivel 1
        int toReach = balance.brainToLevelUp.Evaluate(level - 1);

        //Se resta lo de cada nivel superado; lo que sobra es el progreso del nivel actual
        while (toReach > 0 && brain >= toReach)
        {
            brain -= toReach;
            level++;
            toReach = balance.brainToLevelUp.Evaluate(level - 1);
        }

        levelText.text = level.ToString();
        progressSlider.maxValue = toReach;
        progressSlider.value = brain;
        progressText.text = NumberFormat.Short(brain) + " / " + NumberFormat.Short(toReach);
    }
}
