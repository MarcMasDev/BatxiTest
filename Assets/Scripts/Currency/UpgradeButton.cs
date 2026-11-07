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

    [SerializeField] private GameObject[] upgradable;

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

    //Cualquier cambio de dinero (o una compra) puede cambiar si el botón se puede pulsar
    private void OnCurrencyChanged(Currency changed)
    {
        Refresh();
    }

    private void Refresh()
    {
        int price = Upgrades.GetPrice(upgrade, balance);
        Currency currency = Upgrades.GetCurrency(upgrade);

        int level = Upgrades.GetLevel(upgrade);
        levelText.text = level.ToString();
        priceText.text = NumberFormat.Short(price);

        bool canPurchase = Currencies.Get(currency) >= price;

        button.interactable = canPurchase;
        for (int i = 0; i < upgradable.Length; i++)
        {
            upgradable[i].SetActive(canPurchase);
        }
    }
}
