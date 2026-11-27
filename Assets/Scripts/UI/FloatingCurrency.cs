using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingCurrency : MonoBehaviour
{
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private string amountFormat = "+{0}";
    [SerializeField] private float rise = 120f; 
    [SerializeField] private float duration = 0.8f;


    public void Spawn(Transform parent, int amount, float yOffset = 0)
    {
        FloatingCurrency copy = Instantiate(this, parent);
        copy.gameObject.SetActive(true);
        copy.amountText.text = string.Format(amountFormat, NumberFormat.Short(amount));

        RectTransform rect = (RectTransform)copy.transform;
        rect.anchoredPosition += new Vector2(0, yOffset);

        copy.StartCoroutine(copy.Float());
    }

    private IEnumerator Float()
    {
        RectTransform rect = (RectTransform)transform;
        Vector2 start = rect.anchoredPosition;

        for (float i = 0f; i < 1f; i += Time.unscaledDeltaTime / duration)
        {
            rect.anchoredPosition = start + Vector2.up * rise * (1f - (1f - i) * (1f - i));
            group.alpha = 1f - i * i;
            yield return null;
        }

        Destroy(gameObject);
    }
}
