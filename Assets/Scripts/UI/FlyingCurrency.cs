using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCurrency : MonoBehaviour
{
    [SerializeField] private Currency currency;
    [SerializeField] private RectTransform icon;
    [SerializeField] private RectTransform target;
    [SerializeField] private int count = 6;
    [SerializeField] private float duration = 0.6f;

    private static readonly Dictionary<Currency, int> lastAmounts = new Dictionary<Currency, int>();
    private int amountOnLoad;

    private void Awake()
    {
        amountOnLoad = Currencies.Get(currency);
    }

    private IEnumerator Start()
    {
        yield return null; //Un frame para que el AFK y lo inicial ya se hayan sumado

        int before = lastAmounts.TryGetValue(currency, out int last) ? last : amountOnLoad;
        if (Currencies.Get(currency) > before) Fly();

        lastAmounts[currency] = Currencies.Get(currency);
        Currencies.Changed += OnCurrencyChanged;
    }

    private void OnDestroy()
    {
        Currencies.Changed -= OnCurrencyChanged;
    }

    private void OnCurrencyChanged(Currency changed)
    {
        if (changed == currency) lastAmounts[currency] = Currencies.Get(currency);
    }

    public void Fly()
    {
        for (int i = 0; i < count; i++)
        {
            StartCoroutine(FlyOne(i * 0.06f));
        }
    }

    private IEnumerator FlyOne(float delay)
    {
        RectTransform copy = Instantiate(icon, transform);
        copy.gameObject.SetActive(true);

        //Salen un poco repartidos alrededor de este objeto
        Vector3 start = transform.position + (Vector3)(Random.insideUnitCircle * Screen.height * 0.08f);
        copy.position = start;

        yield return new WaitForSecondsRealtime(delay);

        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
        {
            copy.position = Vector3.Lerp(start, target.position, t * t); //Empieza lento y acelera hacia el contador
            yield return null;
        }

        Destroy(copy.gameObject);
    }
}
