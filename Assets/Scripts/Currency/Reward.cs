using TMPro;
using UnityEngine;

[System.Serializable]
public class RewardType
{
    public Currency currencyType;
    public GameObject currencyImage;
}
public class Reward : MonoBehaviour
{
    public Currency curency;
    [SerializeField] private RewardType[] currencies;
    [SerializeField] private TMP_Text amount;

    private void Awake()
    {
        for (int i = 0; i < currencies.Length; i++)
        {
            currencies[i].currencyImage.SetActive(currencies[i].currencyType == curency);
        }
    }

    public void SetAmount(int i)
    {
        amount.text = NumberFormat.Short(i);
    }


}
