using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;
    public Action<int> OnNewHighScore;

    // KHAI BÁO 2 ID BẢNG XẾP HẠNG
    public const string BOARD_HIGHSCORE = "HighScoreBoard";
    public const string BOARD_FRUITS_MERGED = "FruitsMergedBoard";

    public bool isReady = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private async void Start()
    {
        await InitializeUGSAsync();
    }

    private async Task InitializeUGSAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"[UGS] Đăng nhập ẩn danh thành công! ID: {AuthenticationService.Instance.PlayerId}");
            }

            isReady = true;
        }
        catch (Exception e) { Debug.LogError($"[UGS] Lỗi: {e.Message}"); }
    }

    // GỌI HÀM NÀY ĐỂ ĐẨY ĐIỂM
    public void SubmitScore(int finalScore)
    {
        if (finalScore > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", finalScore);
            OnNewHighScore?.Invoke(finalScore);
        }
        PushDataToUGS(BOARD_HIGHSCORE, finalScore);
    }

    // GỌI HÀM NÀY ĐỂ ĐẨY SỐ HOA QUẢ MERGE ĐƯỢC
    public void SubmitFruitsMerged(int totalFruits)
    {
        PushDataToUGS(BOARD_FRUITS_MERGED, totalFruits);
    }

    private async void PushDataToUGS(string boardId, int value)
    {
        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                await LeaderboardsService.Instance.AddPlayerScoreAsync(boardId, value);
            }
        }
        catch (Exception e) { Debug.LogError($"[Leaderboard] Lỗi đẩy {boardId}: {e.Message}"); }
    }

    // LẤY DANH SÁCH TOP NGƯỜI CHƠI (Hỗ trợ chọn Board)
    public async Task<List<LeaderboardEntry>> FetchTopScores(string boardId, int limit = 100)
    {
        List<LeaderboardEntry> topScores = new List<LeaderboardEntry>();
        if (!AuthenticationService.Instance.IsSignedIn) return topScores;

        try
        {
            var response = await LeaderboardsService.Instance.GetScoresAsync(boardId, new GetScoresOptions { Limit = limit });
            foreach (var entry in response.Results)
            {
                topScores.Add(new LeaderboardEntry
                {
                    rank = entry.Rank + 1,
                    playerName = entry.PlayerName ?? "Player_" + entry.PlayerId.Substring(0, 5),
                    score = (int)entry.Score
                });
            }
        }
        catch (Exception e) { Debug.LogError($"[Leaderboard] Lỗi kéo bảng {boardId}: {e.Message}"); }

        return topScores;
    }

    // LẤY HẠNG CỦA CHÍNH BẢN THÂN NGƯỜI CHƠI NÀY
    // LẤY HẠNG CỦA CHÍNH BẢN THÂN NGƯỜI CHƠI NÀY
    public async Task<LeaderboardEntry> FetchPlayerScore(string boardId)
    {
        if (!AuthenticationService.Instance.IsSignedIn) return null;

        try
        {
            var response = await LeaderboardsService.Instance.GetPlayerScoreAsync(boardId);
            return new LeaderboardEntry
            {
                rank = response.Rank + 1,
                playerName = response.PlayerName ?? "Player_" + response.PlayerId.Substring(0, 5),
                score = (int)response.Score
            };
        }
        catch (LeaderboardsException e)
        {
            // Nếu API trả về lỗi, phần lớn là do người chơi chưa từng có điểm trên bảng này
            Debug.Log($"[Leaderboard] Player chưa có điểm ở bảng {boardId} (hoặc mã lỗi: {e.Reason}).");
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Leaderboard] Lỗi mạng khi tải điểm cá nhân: {e.Message}");
            return null;
        }
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public int rank;
    public string playerName;
    public int score;
}