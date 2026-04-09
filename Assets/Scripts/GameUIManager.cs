using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    // Singleton Pattern an toàn
    public static GameUIManager Instance { get; private set; }

    [Header("--- Next Fruit UI ---")]
    [SerializeField] private Image nextFruitImage;

    [Header("--- Core UI / HUD ---")]
    [SerializeField] private GameObject splashDevPanel; 
    [SerializeField] private GameObject panelGameplay;  
    [SerializeField] private TextMeshProUGUI currentScoreText; // Điểm hiện tại (móc từ Kminh)
    [SerializeField] private TextMeshProUGUI bestScoreText;    // Điểm cao nhất

    [Header("--- Overlays & Popups ---")]
    [SerializeField] private GameObject darkOverlay; // Nền đen mờ che gameplay khi bật Popup
    [SerializeField] private GameObject panelSetting;
    [SerializeField] private GameObject panelMusic;
    [SerializeField] private GameObject panelRecord;
    [SerializeField] private GameObject panelRank;
    [SerializeField] private GameObject panelShop;
    [SerializeField] private GameObject panelSkin;
    [SerializeField] private GameObject panelRevive;
    [SerializeField] private GameObject panelGameOver;

    [Header("--- Record Settings ---")]
[SerializeField] private GameObject recordItemPrefab; // Kéo Prefab cột ngang vào đây
[SerializeField] private Transform recordContainer;   // Kéo Content của ScrollView vào đây

public void OpenRecord() 
{ 
    CloseAllPopups(); 
    EnablePopup(panelRecord); 

    // 1. Dọn dẹp các dòng cũ trong Container để làm mới dữ liệu
    foreach (Transform child in recordContainer) 
    {
        Destroy(child.gameObject);
    }

    // 2. Lấy mảng dữ liệu merge từ DataManager
    int[] mergeCounts = DataManager.Instance.currentSaveData.fruitMergeCounts;

    // 3. Vòng lặp lấy 6 loại quả to nhất: 
    // ID 10 là Dưa hấu, giảm dần về 5 (đủ 6 loại)
    for (int i = 10; i >= 5; i--)
    {
        // Tạo ra một bản sao từ Prefab duy nhất bạn đã kéo vào
        GameObject item = Instantiate(recordItemPrefab, recordContainer);
        
        // Lấy script điều khiển trên bản sao đó để nạp dữ liệu
        FruitRecordItem script = item.GetComponent<FruitRecordItem>();
        
        if (script != null)
        {
            // Nạp ID quả và số lượng merge tương ứng từ mảng lưu trữ
            script.Setup(i, mergeCounts[i]);
        }
    }
}

    private void Awake()
    {
            Instance = this;
    }

    private void Start()
    {
        // 1. Trạng thái khởi tạo: Tắt hết mọi thứ, chỉ bật màn hình Dev Splash
        CloseAllPopups();
        panelGameplay.SetActive(false); // Sẽ bật sau khi tắt Splash
        splashDevPanel.SetActive(true);

        // 2. Chạy Coroutine tự động tắt Splash và vào thẳng game
        StartCoroutine(HideSplashScreenRoutine());
    }

    /// <summary>
    /// Coroutine xử lý luồng: Chờ 2 giây -> Tắt Splash -> Bật HUD -> Có thể gọi Kminh bắt đầu game
    /// </summary>
    private IEnumerator HideSplashScreenRoutine()
    {
        yield return new WaitForSeconds(2f); // Thời gian hiển thị logo Dev

        splashDevPanel.SetActive(false);
        panelGameplay.SetActive(true);

        // Reset điểm số HUD về 0
        UpdateCurrentScore(0);

        // TODO: Chỗ này có thể gọi GameManager.Instance.StartGame() của Kminh
    }

    #region PUBLIC API CHO CÁC DEV KHÁC (KMINH, QUANG)

//    public void UpdateCurrentScore(int score)
   // {
    //    if (currentScoreText != null) currentScoreText.text = score.ToString();
  //  }

    public void UpdateCurrentScore(int score)
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = score.ToString();
            Debug.Log($"<color=yellow>UI Đã đổi Text thành: {score}</color>");
        }
        else
        {
            Debug.LogError("LỖI: Biến currentScoreText bị trống. Bạn chưa kéo thả Text vào Inspector hoặc đã kéo nhầm!");
        }
    }

    public void UpdateBestScore(int bestScore)
    {
        if (bestScoreText != null) bestScoreText.text = bestScore.ToString();
    }

    #endregion

    #region QUẢN LÝ POPUP (Gắn vào OnClick của các Button)

    public void OpenSetting() { CloseAllPopups(); EnablePopup(panelSetting); }
    public void OpenMusic() { CloseAllPopups(); EnablePopup(panelMusic); }
    public void OpenRank() { CloseAllPopups(); EnablePopup(panelRank); }
    public void OpenShop() { CloseAllPopups(); EnablePopup(panelShop); }
    public void OpenSkin() { CloseAllPopups(); EnablePopup(panelSkin); }

    // Màn hình hồi sinh (Gọi bởi Kminh khi thua)
    public void ShowRevive() { CloseAllPopups(); EnablePopup(panelRevive); }

    // Màn hình Game Over (Gọi khi bấm No Thanks ở màn Revive)
    public void ShowGameOver() { CloseAllPopups(); EnablePopup(panelGameOver); }

    /// <summary>
    /// Hàm helper để bật 1 popup kèm theo nền đen DarkOverlay
    /// </summary>
    private void EnablePopup(GameObject popup)
    {
        darkOverlay.SetActive(true); // Bật nền đen che dưa hấu
        popup.SetActive(true);
    }

    /// <summary>
    /// HÀM ĐÓNG TẤT CẢ PANEL (Dùng cho các nút X)
    /// </summary>
    public void CloseAllPopups()
    {
        panelSetting.SetActive(false);
        panelMusic.SetActive(false);
        panelRecord.SetActive(false);
        panelRank.SetActive(false);
        panelShop.SetActive(false);
        panelSkin.SetActive(false);
        panelRevive.SetActive(false);
        panelGameOver.SetActive(false);

        // Đừng quên tắt nền đen khi đóng popup
        if (darkOverlay != null) darkOverlay.SetActive(false);
    }

    public void UpdateNextFruit(Sprite fruitSprite)
    {
        if (nextFruitImage != null && fruitSprite != null)
        {
            nextFruitImage.sprite = fruitSprite;
            // Bật Image lên phòng trường hợp nó đang bị tắt
            nextFruitImage.enabled = true;
        }
    }

    #endregion
}