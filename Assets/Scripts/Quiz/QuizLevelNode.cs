using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizLevelNode : MonoBehaviour
{
    [SerializeField] private Image circle;
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private Image line;

    [SerializeField] private Color pendingCircleColor = Color.gray2;
    [SerializeField] private Color pendingLineColor = Color.green;

    public void Setup(int number, bool completed, bool nextCompleted, bool isLast, Color color)
    {
        numberText.text = number.ToString();
        circle.color = completed ? color : pendingCircleColor;
        line.color = nextCompleted ? color : pendingLineColor;
        line.gameObject.SetActive(!isLast);
    }
}
