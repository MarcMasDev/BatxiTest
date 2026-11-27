using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Upgrade upgrade;
    [SerializeField] private ProgressionBalance balance;

    [SerializeField] private Button button;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text priceText;

    [Header("Lo que da ahora la mejora (opcional)")]
    [SerializeField] private TMP_Text gainText;

    [SerializeField] private GameObject[] upgradable;

    [Header("Bloqueo por nivel del personaje")]
    [SerializeField] private GameObject lockedMark; 
    [SerializeField] private TMP_Text requiredLevelText;

    private void Awake()
    {
        button.onClick.AddListener(Buy);
    }

    private void OnEnable()
    {
        Currencies.Changed += OnCurrencyChanged;
        Refresh();
    }

    private void OnDisable()
    {
        Currencies.Changed -= OnCurrencyChanged;
    }

    private void Buy()
    {
        Upgrades.TryBuy(upgrade, balance);
    }

    //Cualquier cambio de dinero, de inteligencia (nivel) o una compra puede cambiar si el botón se puede pulsar
    private void OnCurrencyChanged(Currency changed)
    {
        Refresh();
    }

    private void Refresh()
    {
        int price = Upgrades.GetPrice(upgrade, balance);
        Currency currency = Upgrades.GetCurrency(upgrade);

        int level = Upgrades.GetLevel(upgrade);
        levelText.text = (level + 1).ToString(); //Se guarda desde 0, pero al jugador se le enseña desde el nivel 1
        priceText.text = NumberFormat.Short(price);

        if (gainText != null)
        {
            //Cada botón sabe su mejora: las máquinas (se pagan con monedas) dan fuerza con decimales, el resto enteros
            bool isMachine = currency == Currency.Coins;
            gainText.text = isMachine
                ? "+" + balance.GetMachineStrength(upgrade).EvaluateExact(level).ToString("0.0", CultureInfo.InvariantCulture) + "/s"
                : Upgrades.GetGain(upgrade, level, balance).ToString();
        }

        bool unlocked = Upgrades.IsUnlocked(upgrade, balance);
        bool canPurchase = unlocked && Currencies.Get(currency) >= price;

        lockedMark.SetActive(!unlocked);
        requiredLevelText.text = Upgrades.GetRequiredCharacterLevel(upgrade, balance).ToString();

        button.interactable = canPurchase;
        for (int i = 0; i < upgradable.Length; i++)
        {
            upgradable[i].SetActive(canPurchase);
        }
    }
}
