using System.Globalization;
using TMPro;
using UnityEngine;

//Fuerza media por segundo que da el gym (la misma que se usa para el tiempo AFK)
public class StrengthRateLabel : MonoBehaviour
{
    [SerializeField] private ProgressionBalance balance;
    [SerializeField] private TMP_Text rateText;

    private void OnEnable()
    {
        Currencies.Changed += OnCurrencyChanged;
        Refresh();
    }

    private void OnDisable()
    {
        Currencies.Changed -= OnCurrencyChanged;
    }

    //Mejorar una máquina gasta monedas: es justo cuando cambia
    private void OnCurrencyChanged(Currency changed)
    {
        if (changed == Currency.Coins) Refresh();
    }

    private void Refresh()
    {
        //Siempre con 1 decimal: cada mejora de máquina sube la media poco, y sin decimales a veces no se notaría
        float rate = balance.AverageStrengthPerSecond();
        string shown = rate < 10000 ? rate.ToString("#,0.0", CultureInfo.InvariantCulture) : NumberFormat.Short(Mathf.RoundToInt(rate));
        rateText.text = "+" + shown + "/s";
    }
}
