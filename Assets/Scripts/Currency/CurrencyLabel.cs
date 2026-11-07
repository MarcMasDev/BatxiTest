using System.Collections;
using TMPro;
using UnityEngine;

public class CurrencyLabel : MonoBehaviour
{
    [SerializeField] private Currency currency;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private float punchScale = 1.25f; //Al ganar, el número crece un momento (1 = sin efecto)

    private int shown;

    private void OnEnable()
    {
        Currencies.Changed += OnCurrencyChanged;
        shown = Currencies.Get(currency);
        amountText.text = NumberFormat.Short(shown);
    }

    private void OnDisable()
    {
        Currencies.Changed -= OnCurrencyChanged;
        amountText.transform.localScale = Vector3.one;
    }

    private void OnCurrencyChanged(Currency changed)
    {
        if (changed != currency) return;

        int amount = Currencies.Get(currency);
        if (amount > shown)
        {
            StopAllCoroutines();
            StartCoroutine(Punch());
        }

        shown = amount;
        amountText.text = NumberFormat.Short(amount);
    }

    private IEnumerator Punch()
    {
        Transform text = amountText.transform;
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / 0.2f)
        {
            text.localScale = Vector3.one * Mathf.Lerp(punchScale, 1f, t);
            yield return null;
        }
        text.localScale = Vector3.one;
    }
}
