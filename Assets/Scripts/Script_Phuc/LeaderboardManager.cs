using UnityEngine;
using System;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    // Sự kiện để báo cho Dev 5 (UI) biết có kỷ lục mới để hiện hiệu ứng pháo hoa
    public Action<int> OnNewHighScore;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Lấy điểm cao nhất hiện tại đã lưu trên máy
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    // Dev 5 (hoặc Dev 1) sẽ gọi hàm này khi người chơi Game Over
    public void SubmitScore(int finalScore)
    {
        Debug.Log($"[Leaderboard] Game Over! Điểm trận này: {finalScore}");

        // 1. Kiểm tra xem có phá kỷ lục local không?
        int currentHighScore = GetHighScore();
        if (finalScore > currentHighScore)
        {
            // Lưu kỷ lục mới
            PlayerPrefs.SetInt("HighScore", finalScore);
            PlayerPrefs.Save();

            Debug.Log($"[Leaderboard] KỶ LỤC MỚI: {finalScore}!");

            // Hét lên cho Dev 5 biết để bật UI chúc mừng
            OnNewHighScore?.Invoke(finalScore);
        }

        // 2. Giả lập việc bắn điểm lên Server (Google Play hoặc Unity Services)
        PushScoreToServerMock(finalScore);
    }

    private void PushScoreToServerMock(int score)
    {
        // Tuần sau chúng ta sẽ thay code gọi API mạng thật vào đây
        Debug.Log($"[Leaderboard] Đang đồng bộ {score} điểm lên Server đám mây...");
        Debug.Log($"[Leaderboard] Đồng bộ thành công!");
    }
}