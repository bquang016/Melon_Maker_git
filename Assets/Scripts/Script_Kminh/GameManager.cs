using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Kho chứa TOÀN BỘ 11 Prefab Trái Cây")]
    public GameObject[] tatCaTraiCay;

    [Header("Hệ thống Điểm số")]
    public int diemHienTai = 0;

    [Header("Hệ thống Game Over & Revive")]
    public bool isGameOver = false;

    [Header("Hệ thống Mở Khóa (Progression)")]
    public int maxUnlockedFruitID = 0;

    // Biến lưu trữ data hiện tại
    private SaveData currentSaveData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        // 1. Tải dữ liệu ngay khi khởi tạo game
        currentSaveData = SaveLoadManager.LoadGame();
        maxUnlockedFruitID = currentSaveData.maxUnlockedFruitID;

        // 2. Cập nhật Điểm cao nhất lên giao diện UI của Dev 5
        GameUIManager.Instance?.UpdateBestScore(currentSaveData.bestScore);
    }

    public void CongDiem(int diemCongThem)
    {
        if (isGameOver) return;

        diemHienTai += diemCongThem;
        // In ra Console để xem GameManager có nhận được điểm không
        Debug.Log($"<color=green>Đã cộng {diemCongThem} điểm. Tổng: {diemHienTai}</color>");

        if (GameUIManager.Instance == null)
        {
            Debug.LogError("LỖI: GameUIManager.Instance đang NULL! Nên không thể gửi điểm sang UI.");
        }
        else
        {
            // GỌI UI: Cập nhật điểm hiện tại
            GameUIManager.Instance?.UpdateCurrentScore(diemHienTai);
            }
    }

    public void KichHoatGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f; // Dừng mọi vật lý trong game

        // GỌI UI: Bật màn hình Last Chance (Revive)
        GameUIManager.Instance?.ShowRevive();
    }

    public void HoiSinh_Revive()
    {
        isGameOver = false;
        Time.timeScale = 1f; // Tiếp tục vật lý

        // GỌI UI: Đóng màn hình Revive để chơi tiếp
        GameUIManager.Instance?.CloseAllPopups();
    }

    public void TuChoiHoiSinh_ThuaLuon()
    {
        // GỌI UI: Bật màn hình Game Over thực sự
        GameUIManager.Instance?.ShowGameOver();

        // KIỂM TRA & LƯU KỶ LỤC MỚI
        if (diemHienTai > currentSaveData.bestScore)
        {
            currentSaveData.bestScore = diemHienTai;
            SaveLoadManager.SaveGame(currentSaveData);

            // Cập nhật lại UI điểm cao nhất mới
            GameUIManager.Instance?.UpdateBestScore(currentSaveData.bestScore);
        }
    }

    public void KiemTraMoKhoa(int idQuaMoi)
    {
        // Nếu ghép được quả mới to hơn kỷ lục cũ
        if (idQuaMoi > maxUnlockedFruitID)
        {
            maxUnlockedFruitID = idQuaMoi;
            currentSaveData.maxUnlockedFruitID = maxUnlockedFruitID;

            // Lưu lại tiến trình ngay lập tức
            SaveLoadManager.SaveGame(currentSaveData);
            Debug.Log("MỞ KHÓA THÀNH CÔNG QUẢ MỚI: ID " + idQuaMoi);
        }
    }
}