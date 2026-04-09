using UnityEngine;
using TMPro;

public class DailyRankItem : MonoBehaviour
{
    public TextMeshProUGUI rankText;  // Số 1, 2, 3...
    public TextMeshProUGUI dateText;  // Ngày
    public TextMeshProUGUI scoreText; // Điểm

    public void Setup(int rank, string date, int score)
    {
        rankText.text = rank.ToString();
        dateText.text = date;
        scoreText.text = score.ToString();
    }
}