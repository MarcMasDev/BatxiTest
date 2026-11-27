using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Ventana al volver al juego: la fuerza ganada mientras no estabas no se suma hasta pulsar "Reclamar"
public class AfkClaimPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text awayText;
    [SerializeField] private Button claimButton;
    [SerializeField] private FlyingCurrency flyingStrength;

    [SerializeField] private string amountFormat = "+{0}";
    [SerializeField] private string awayFormat = "Has estado fuera {0}";

    private int pendingStrength;

    private void Awake()
    {
        ActivateCanvas(false);
        claimButton.onClick.AddListener(Claim);
    }

    public void Show(int strength, long secondsAway)
    {
        pendingStrength += strength; //Si vuelve a salir sin reclamar, se acumula en vez de perderse
        amountText.text = string.Format(amountFormat, NumberFormat.Short(pendingStrength));
        awayText.text = string.Format(awayFormat, NumberFormat.Duration(secondsAway));
        ActivateCanvas(true);
    }

    private void Claim()
    {
        Currencies.Add(Currency.Strength, pendingStrength);
        pendingStrength = 0;
        ActivateCanvas(false);

        if (flyingStrength != null) flyingStrength.Fly();
    }

    private void ActivateCanvas(bool active)
    {
        panel.alpha = active ? 1 : 0;
        panel.interactable = active;
        panel.blocksRaycasts = active;
    }
}
