using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LeaderboardUI : MonoBehaviour
{
    [Header("--- CHẾ ĐỘ TEST UI ---")]
    [Tooltip("Bật cái này lên để tự đẻ ra 50 người chơi giả, tắt đi để lấy điểm mạng thật")]
    public bool useMockData = true;

    [Header("UI Containers (Nơi chứa danh sách)")]
    public Transform contentScore;
    public Transform contentFruit;

    [Header("Prefabs - Bảng Điểm (Score)")]
    public GameObject scoreTop1Prefab;
    public GameObject scoreTop2Prefab;
    public GameObject scoreTop3Prefab;
    public GameObject scoreNormalPrefab;

    [Header("Prefabs - Bảng Hoa Quả (Fruit)")]
    public GameObject fruitTop1Prefab;
    public GameObject fruitTop2Prefab;
    public GameObject fruitTop3Prefab;
    public GameObject fruitNormalPrefab;

    [Header("Thanh Player Cố Định - BẢNG ĐIỂM")]
    public GameObject playerBarScoreObj;
    public TextMeshProUGUI playerScoreRankText;
    public TextMeshProUGUI playerScoreNameText;
    public TextMeshProUGUI playerScoreValueText;

    [Header("Thanh Player Cố Định - BẢNG HOA QUẢ")]
    public GameObject playerBarFruitObj;
    public TextMeshProUGUI playerFruitRankText;
    public TextMeshProUGUI playerFruitNameText;
    public TextMeshProUGUI playerFruitValueText;

    private async void Start()
    {
        // Nếu không dùng Mock Data thì phải đợi mạng thông mới chạy
        if (!useMockData)
        {
            while (LeaderboardManager.Instance == null || !LeaderboardManager.Instance.isReady)
            {
                await Task.Delay(100);
            }
        }

        LoadAndDisplay(LeaderboardManager.BOARD_HIGHSCORE, contentScore);
        LoadAndDisplay(LeaderboardManager.BOARD_FRUITS_MERGED, contentFruit);

        OnTabScoreClicked();
    }

    public void OnTabScoreClicked()
    {
        if (!useMockData && (LeaderboardManager.Instance == null || !LeaderboardManager.Instance.isReady)) return;

        if (playerBarScoreObj != null) playerBarScoreObj.SetActive(true);
        if (playerBarFruitObj != null) playerBarFruitObj.SetActive(false);

        UpdatePlayerBar(LeaderboardManager.BOARD_HIGHSCORE);
    }

    public void OnTabFruitClicked()
    {
        if (!useMockData && (LeaderboardManager.Instance == null || !LeaderboardManager.Instance.isReady)) return;

        if (playerBarScoreObj != null) playerBarScoreObj.SetActive(false);
        if (playerBarFruitObj != null) playerBarFruitObj.SetActive(true);

        UpdatePlayerBar(LeaderboardManager.BOARD_FRUITS_MERGED);
    }

    private async void LoadAndDisplay(string boardId, Transform container)
    {
        foreach (Transform child in container) { Destroy(child.gameObject); }

        List<LeaderboardEntry> topPlayers;

        // KIỂM TRA: Lấy dữ liệu giả hay dữ liệu thật?
        if (useMockData)
        {
            topPlayers = GenerateMockList(boardId);
        }
        else
        {
            topPlayers = await LeaderboardManager.Instance.FetchTopScores(boardId, 100);
        }

        bool isScoreBoard = boardId == LeaderboardManager.BOARD_HIGHSCORE;

        foreach (var player in topPlayers)
        {
            GameObject prefabToSpawn;

            if (isScoreBoard)
            {
                if (player.rank == 1) prefabToSpawn = scoreTop1Prefab;
                else if (player.rank == 2) prefabToSpawn = scoreTop2Prefab;
                else if (player.rank == 3) prefabToSpawn = scoreTop3Prefab;
                else prefabToSpawn = scoreNormalPrefab;
            }
            else
            {
                if (player.rank == 1) prefabToSpawn = fruitTop1Prefab;
                else if (player.rank == 2) prefabToSpawn = fruitTop2Prefab;
                else if (player.rank == 3) prefabToSpawn = fruitTop3Prefab;
                else prefabToSpawn = fruitNormalPrefab;
            }

            GameObject newRow = Instantiate(prefabToSpawn, container);

            Transform nameObj = newRow.transform.Find("NameText");
            if (nameObj != null) nameObj.GetComponent<TextMeshProUGUI>().text = player.playerName;

            Transform scoreObj = newRow.transform.Find("Group_Point/ScoreText");
            if (scoreObj != null) scoreObj.GetComponent<TextMeshProUGUI>().text = player.score.ToString();

            Transform rankObj = newRow.transform.Find("Image/RankText");
            if (rankObj != null) rankObj.GetComponent<TextMeshProUGUI>().text = player.rank.ToString();
        }
    }

    private async void UpdatePlayerBar(string boardId)
    {
        bool isScoreBoard = boardId == LeaderboardManager.BOARD_HIGHSCORE;

        if (isScoreBoard)
        {
            playerScoreNameText.text = "Loading...";
            playerScoreValueText.text = "-";
            if (playerScoreRankText != null) playerScoreRankText.text = "-";
        }
        else
        {
            playerFruitNameText.text = "Loading...";
            playerFruitValueText.text = "-";
            if (playerFruitRankText != null) playerFruitRankText.text = "-";
        }

        LeaderboardEntry myScore;

        // KIỂM TRA: Lấy điểm của mình từ Mock hay mạng thật?
        if (useMockData)
        {
            myScore = GetMockPlayer(boardId);
        }
        else
        {
            myScore = await LeaderboardManager.Instance.FetchPlayerScore(boardId);
        }

        if (isScoreBoard)
        {
            if (myScore != null)
            {
                if (playerScoreRankText != null) playerScoreRankText.text = myScore.rank.ToString();
                playerScoreNameText.text = myScore.playerName;
                playerScoreValueText.text = myScore.score.ToString();
            }
        }
        else
        {
            if (myScore != null)
            {
                if (playerFruitRankText != null) playerFruitRankText.text = myScore.rank.ToString();
                playerFruitNameText.text = myScore.playerName;
                playerFruitValueText.text = myScore.score.ToString();
            }
        }
    }

    // ==========================================
    // CÁC HÀM TẠO DỮ LIỆU GIẢ LẬP (MOCK DATA)
    // ==========================================
    private List<LeaderboardEntry> GenerateMockList(string boardId)
    {
        List<LeaderboardEntry> mockList = new List<LeaderboardEntry>();
        int startScore = boardId == LeaderboardManager.BOARD_HIGHSCORE ? 15000 : 350;
        int decreaseStep = boardId == LeaderboardManager.BOARD_HIGHSCORE ? 250 : 5;

        for (int i = 1; i <= 50; i++)
        {
            mockList.Add(new LeaderboardEntry
            {
                rank = i,
                playerName = "Bot_" + Random.Range(1000, 9999),
                score = startScore - (i * decreaseStep)
            });
        }
        return mockList;
    }

    private LeaderboardEntry GetMockPlayer(string boardId)
    {
        return new LeaderboardEntry
        {
            rank = 42,
            playerName = "Quang_Dev",
            score = boardId == LeaderboardManager.BOARD_HIGHSCORE ? 4500 : 140
        };
    }

    // ==========================================
    // HÀM ĐIỀU KHIỂN BẬT/TẮT PANEL
    // ==========================================
    public void ClosePanel()
    {
        // Ẩn chính cái Panel đang chứa script này đi
        gameObject.SetActive(false);
    }
}