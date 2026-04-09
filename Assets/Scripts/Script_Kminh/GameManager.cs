using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Đừng quên cái này để dùng List

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

    [Header("Hệ thống Booster của Quang")]
    public bool isUsingBooster = false;

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

        // 2. Cập nhật Điểm cao nhất lên giao diện UI
        GameUIManager.Instance?.UpdateBestScore(DataManager.Instance.currentSaveData.bestScore);

        // GỌI HIỆN BANNER NGAY KHI VÀO GAME
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowBanner();
        }

        // 3. GỌI HÀM LOAD TRÁI CÂY CHƠI DỞ (Tuyệt chiêu đóng băng vật lý)
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
            // Tìm đúng prefab theo ID (Giả sử tên prefab chứa ID ở cuối, VD: "Fruit_0")
            GameObject prefab = null;
            foreach (var p in tatCaTraiCay)
            {
                if (p.name == data.fruitID) { prefab = p; break; }
            }

            if (prefab != null)
            {
                GameObject newFruit = Instantiate(prefab, new Vector3(data.posX, data.posY, 0), Quaternion.Euler(0, 0, data.rotZ));

                // Tắt vật lý để chống nổ Big Bang
                Rigidbody2D rb = newFruit.GetComponent<Rigidbody2D>();
                if (rb != null) rb.simulated = false;
            }
        }

        // Bật lại vật lý sau 0.5s
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
    // HỆ THỐNG ĐIỂM & GAME OVER (ĐÃ FIX LỖI)
    // ==========================================
    public void CongDiem(int diemCongThem)
    {
        if (isGameOver) return;
        diemHienTai += diemCongThem;
        GameUIManager.Instance?.UpdateCurrentScore(diemHienTai);
    }

    public void KichHoatGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        GameUIManager.Instance?.ShowRevive();
    }

    // HÀM NÀY SẼ CHẠY KHI BẠN XEM XONG ADS REWARD
    public void HoiSinh_Revive()
    {
        Debug.Log("<color=cyan>[GameManager] ĐANG HỒI SINH: Quét dọn trái cây...</color>");

        // 1. Tìm toàn bộ trái cây
        GameObject[] tatCaQua = GameObject.FindGameObjectsWithTag("Fruit");

        // 2. Sắp xếp mảng: Quả nào nằm CAO NHẤT (trục Y lớn nhất) đưa lên đầu
        System.Array.Sort(tatCaQua, (a, b) => b.transform.position.y.CompareTo(a.transform.position.y));

        // 3. Tiêu diệt 4 quả cao nhất để lấy không gian trống trong hộp
        int soQuaCanXoa = Mathf.Min(4, tatCaQua.Length);
        for (int i = 0; i < soQuaCanXoa; i++)
        {
            // Có thể kèm theo hiệu ứng nổ nhỏ ở đây cho đẹp
            Destroy(tatCaQua[i]);
        }

        // 4. Cho phép game chạy lại
        isGameOver = false;
        Time.timeScale = 1f;
        GameUIManager.Instance?.CloseAllPopups();
    }

    public void TuChoiHoiSinh_ThuaLuon()
    {
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowInterstitial();
        }
        GameUIManager.Instance?.ShowGameOver();
        DataManager.Instance.UpdateBestScore(diemHienTai); // Gọi két sắt lưu điểm
    }

    public void KiemTraMoKhoa(int idQuaMoi)
    {
        if (idQuaMoi > maxUnlockedFruitID)
        {
            maxUnlockedFruitID = idQuaMoi;
            DataManager.Instance.UpdateMaxFruit(maxUnlockedFruitID); // Gọi két sắt lưu kỷ lục
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
        GameUIManager.Instance?.CloseAllPopups();

        // Dọn dẹp sạch sẽ danh sách lưu tạm thời trong RAM
        DataManager.Instance.currentSaveData.sessionData.Clear();
        DataManager.Instance.SaveDataToDisk();
    }
}