using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Đừng quên cái này để dùng List
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("--- Record & Stats UI ---")]
    [SerializeField] public RecordItemUI[] recordItems; // Thêm [SerializeField] vào đây
    
    public TextMeshProUGUI dailyBestScoreText;
    public TextMeshProUGUI totalMergedText;

    [Header("Kho chứa TOÀN BỘ 11 Prefab Trái Cây")]
    public GameObject[] tatCaTraiCay;

    [Header("Hệ thống Điểm số")]
    public int diemHienTai = 0;

    [Header("Hệ thống Game Over & Revive")]
    public bool isGameOver = false;

    [Header("Hệ thống Mở Khóa (Progression)")]
    public int maxUnlockedFruitID = 0;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    // ==========================================
    // LIÊN KẾT VỚI ADS MANAGER CỦA DEV 3 ĐỂ NHẬN LỆNH HỒI SINH
    // ==========================================
    private void OnEnable()
    {
        // Khi AdsManager phát loa báo xem xong Video Hồi sinh, tự động gọi hàm HoiSinh_Revive
        AdsManager.OnRewardRevive += HoiSinh_Revive;
    }

    private void OnDisable()
    {
        AdsManager.OnRewardRevive -= HoiSinh_Revive;
    }

    private void Start()
    {
        // 1. Lấy dữ liệu từ cái Két Sắt DataManager
        maxUnlockedFruitID = DataManager.Instance.currentSaveData.maxUnlockedFruitID;

        // 2. Cập nhật Điểm cao nhất lên giao diện HUD
        GameUIManager.Instance?.UpdateBestScore(DataManager.Instance.currentSaveData.bestScore);

        // 3. Cập nhật Điểm Ngày và Bảng đếm quả Merge (Tính năng mới)
        GameUIManager.Instance?.UpdateDailyAndTotalUI(
            DataManager.Instance.currentSaveData.dailyBestScore,
            DataManager.Instance.currentSaveData.totalMergedFruits
        );
        GameUIManager.Instance?.UpdateMergedFruitsDisplay();

        // 4. GỌI HÀM LOAD TRÁI CÂY CHƠI DỞ
        LoadSavedBoard();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) KichHoatGameOver();
    }

    // ==========================================
    // HỆ THỐNG LOAD TRÁI CÂY (TỪ DATA VẼ RA MÀN HÌNH)
    // ==========================================
    private void LoadSavedBoard()
    {
        List<FruitSaveData> savedFruits = DataManager.Instance.currentSaveData.sessionData;
        if (savedFruits == null || savedFruits.Count == 0) return;

        Debug.Log($"[GameManager] Đang khôi phục lại {savedFruits.Count} trái cây đang chơi dở...");

        foreach (var data in savedFruits)
        {
            GameObject prefab = null;
            foreach (var p in tatCaTraiCay)
            {
                if (p.name == data.fruitID) { prefab = p; break; }
            }

            if (prefab != null)
            {
                GameObject newFruit = Instantiate(prefab, new Vector3(data.posX, data.posY, 0), Quaternion.Euler(0, 0, data.rotZ));

                Rigidbody2D rb = newFruit.GetComponent<Rigidbody2D>();
                if (rb != null) rb.simulated = false;
            }
        }

        Invoke(nameof(EnablePhysics), 0.5f);
    }

    private void EnablePhysics()
    {
        GameObject[] fruits = GameObject.FindGameObjectsWithTag("Fruit");
        foreach (var f in fruits)
        {
            Rigidbody2D rb = f.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = true;
        }
    }

    // ==========================================
    // HỆ THỐNG ĐIỂM & GAME OVER
    // ==========================================
    public void CongDiem(int diemCongThem)
    {
        if (isGameOver) return;
        diemHienTai += diemCongThem;
        GameUIManager.Instance?.UpdateCurrentScore(diemHienTai);
        
        // Cập nhật kỷ lục điểm ngay lập tức trong RAM để hiển thị UI
        DataManager.Instance.UpdateBestScore(diemHienTai);
        GameUIManager.Instance?.UpdateDailyAndTotalUI(
            DataManager.Instance.currentSaveData.dailyBestScore,
            DataManager.Instance.currentSaveData.totalMergedFruits
        );
    }

    public void KichHoatGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        GameUIManager.Instance?.ShowRevive();
    }

    public void HoiSinh_Revive()
    {
        Debug.Log("<color=cyan>[GameManager] ĐANG HỒI SINH: Quét dọn trái cây...</color>");

        GameObject[] tatCaQua = GameObject.FindGameObjectsWithTag("Fruit");
        System.Array.Sort(tatCaQua, (a, b) => b.transform.position.y.CompareTo(a.transform.position.y));

        int soQuaCanXoa = Mathf.Min(4, tatCaQua.Length);
        for (int i = 0; i < soQuaCanXoa; i++)
        {
            Destroy(tatCaQua[i]);
        }

        isGameOver = false;
        Time.timeScale = 1f;
        GameUIManager.Instance?.CloseAllPopups();
    }

    public void TuChoiHoiSinh_ThuaLuon()
    {
        GameUIManager.Instance?.ShowGameOver();
        DataManager.Instance.UpdateBestScore(diemHienTai);

        AdsManager.Instance.ShowInterstitial();
    }

    public void KiemTraMoKhoa(int idQuaMoi)
    {
        if (idQuaMoi > maxUnlockedFruitID)
        {
            maxUnlockedFruitID = idQuaMoi;
            DataManager.Instance.UpdateMaxFruit(maxUnlockedFruitID);
        }
    }

    public void ReplayGame()
    {
        DataManager.Instance.UpdateBestScore(diemHienTai);

        GameObject[] tatCaQua = GameObject.FindGameObjectsWithTag("Fruit");
        foreach (GameObject qua in tatCaQua) Destroy(qua);

        diemHienTai = 0;
        isGameOver = false;
        Time.timeScale = 1f;

        GameUIManager.Instance?.UpdateCurrentScore(0);
        GameUIManager.Instance?.UpdateBestScore(DataManager.Instance.currentSaveData.bestScore);
        
        // Reset hiển thị bảng kỷ lục ngày về trạng thái mới
        GameUIManager.Instance?.UpdateDailyAndTotalUI(
            DataManager.Instance.currentSaveData.dailyBestScore,
            DataManager.Instance.currentSaveData.totalMergedFruits
        );

        GameUIManager.Instance?.CloseAllPopups();

        DataManager.Instance.currentSaveData.sessionData.Clear();
        DataManager.Instance.SaveDataToDisk();
    }
}