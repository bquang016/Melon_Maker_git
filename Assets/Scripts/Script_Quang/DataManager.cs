using System.Collections.Generic;
using UnityEngine;

// File này thay thế hoàn toàn cho MockTest cũ
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    // Biến này ôm toàn bộ dữ liệu khi game đang chạy
    public SaveData currentSaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Khởi động game là phải nạp file save từ ổ cứng lên RAM ngay
        currentSaveData = SaveLoadManager.LoadGame();
        Debug.Log($"[DataManager] Đã tải Data. Ruby: {currentSaveData.metaData.currentRuby} | Nốt nhạc: {currentSaveData.metaData.currentMusicNote}");
    }

    // ==========================================================
    // LIÊN KẾT VỚI ADS MANAGER (Của bạn) ĐỂ NHẬN TIỀN
    // ==========================================================
    private void OnEnable()
    {
        AdsManager.OnRewardRuby += AddRubyFromAds;
        AdsManager.OnRewardMusicNote += AddMusicNoteFromAds;
    }

    private void OnDisable()
    {
        AdsManager.OnRewardRuby -= AddRubyFromAds;
        AdsManager.OnRewardMusicNote -= AddMusicNoteFromAds;
    }

    private void AddRubyFromAds(int amount)
    {
        currentSaveData.metaData.currentRuby += amount;
        Debug.Log($"[DataManager] Bting! Nhận {amount} Ruby. Tổng: {currentSaveData.metaData.currentRuby}");
        SaveDataToDisk();
    }

    private void AddMusicNoteFromAds(int amount)
    {
        currentSaveData.metaData.currentMusicNote += amount;
        Debug.Log($"[DataManager] Bting! Nhận {amount} Nốt Nhạc. Tổng: {currentSaveData.metaData.currentMusicNote}");
        SaveDataToDisk();
    }

    // ==========================================================
    // CÁC HÀM CHO GAMEMANAGER / SHOP GỌI ĐỂ LƯU GAME
    // ==========================================================

    public void SaveDataToDisk()
    {
        SaveLoadManager.SaveGame(currentSaveData);
    }

    // Gọi hàm cập nhật điểm
    public void UpdateBestScore(int newScore)
    {
        if (newScore > currentSaveData.bestScore)
        {
            currentSaveData.bestScore = newScore;
            SaveDataToDisk();
        }
    }
    // Thêm đoạn này vào bên dưới hàm UpdateBestScore
    public void UpdateMaxFruit(int fruitID)
    {
        if (fruitID > currentSaveData.maxUnlockedFruitID)
        {
            currentSaveData.maxUnlockedFruitID = fruitID;
            SaveDataToDisk();
            Debug.Log($"[DataManager] Đã mở khóa trái cây kỷ lục mới: Cấp {fruitID}!");
        }
    }

    // gọi hàm này khi người chơi thoát game để lưu lại bàn chơi dở
    public void SaveSession(List<FruitSaveData> activeFruits)
    {
        currentSaveData.sessionData.Clear();
        currentSaveData.sessionData.AddRange(activeFruits);
        SaveDataToDisk();
    }
}