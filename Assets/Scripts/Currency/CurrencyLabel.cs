using TMPro;
using UnityEngine;

public class CurrencyLabel : MonoBehaviour
{
    [SerializeField] private Currency currency;
    [SerializeField] private TMP_Text amountText;

    private void OnEnable()
    {
        Currencies.Changed += OnCurrencyChanged;
        amountText.text = NumberFormat.Short(Currencies.Get(currency));
    }

    private void OnDisable()
    {
        Currencies.Changed -= OnCurrencyChanged;
    }

    private void OnCurrencyChanged(Currency changed)
    {
        if (changed == currency)
        {
            amountText.text = NumberFormat.Short(Currencies.Get(currency));
        }
    }
}
