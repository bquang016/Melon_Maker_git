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
    // LIÊN KẾT VỚI ADS MANAGER ĐỂ NHẬN THƯỞNG
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
    // HỆ THỐNG ĐIỂM SỐ VÀ KỶ LỤC
    // ==========================================================

    public void UpdateBestScore(int newScore)
    {
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");

        // 1. Kiểm tra nếu là ngày mới thì reset điểm ngày
        if (currentSaveData.lastPlayDate != today) {
            currentSaveData.lastPlayDate = today;
            currentSaveData.dailyBestScore = 0;
        }

        // 2. Cập nhật Best Score (mọi thời đại)
        if (newScore > currentSaveData.bestScore) {
            currentSaveData.bestScore = newScore;
        }

        // 3. Cập nhật Daily Best Score (kỷ lục trong ngày)
        if (newScore > currentSaveData.dailyBestScore) {
            currentSaveData.dailyBestScore = newScore;
        }

        SaveDataToDisk();
    }

    // Cập nhật cấp độ trái cây lớn nhất đạt được
    public void UpdateMaxFruit(int fruitID)
    {
        if (fruitID > currentSaveData.maxUnlockedFruitID)
        {
            currentSaveData.maxUnlockedFruitID = fruitID;
            SaveDataToDisk();
            Debug.Log($"[DataManager] Đã mở khóa trái cây kỷ lục mới: Cấp {fruitID}!");
        }
    }

    // ==========================================================
    // HỆ THỐNG THỐNG KÊ MERGE QUẢ
    // ==========================================================

    // Cộng dồn tổng số quả đã merge từ trước đến nay
    public void AddMergedFruitCount() {
        currentSaveData.totalMergedFruits++;
        SaveDataToDisk();
    }

    // Đếm chi tiết số lần merge cho từng loại quả riêng biệt
    public void AddFruitMergeCount(int fruitID) {
        // Đảm bảo mảng tồn tại để tránh lỗi NullReference
        if (currentSaveData.fruitMergeCounts == null) {
            currentSaveData.fruitMergeCounts = new int[11];
        }

        if (fruitID >= 0 && fruitID < currentSaveData.fruitMergeCounts.Length) {
            currentSaveData.fruitMergeCounts[fruitID]++;
            SaveDataToDisk();
        }
    }

    // ==========================================================
    // CÁC HÀM LƯU TRỮ VÀ HÀNH LANG (SESSION)
    // ==========================================================

    public void SaveDataToDisk()
    {
        SaveLoadManager.SaveGame(currentSaveData);
    }

    // Gọi hàm này khi người chơi thoát game để lưu lại bàn chơi dở
    public void SaveSession(List<FruitSaveData> activeFruits)
    {
        currentSaveData.sessionData.Clear();
        currentSaveData.sessionData.AddRange(activeFruits);
        SaveDataToDisk();
    }
}