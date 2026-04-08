using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameUIManager : MonoBehaviour
{
    // Singleton Pattern an toàn
    public static GameUIManager Instance { get; private set; }
  [Header("Cấu hình hiển thị Top Merge")]
    public RecordItemUI[] recordItems;

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
public TMPro.TextMeshProUGUI dailyBestScoreText;
public TMPro.TextMeshProUGUI totalMergedText;

public void UpdateDailyAndTotalUI(int dailyBest, int totalMerged) {
    if(dailyBestScoreText != null) dailyBestScoreText.text = dailyBest.ToString();
    if(totalMergedText != null) totalMergedText.text = totalMerged.ToString();
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
    public void UpdateMergedFruitsDisplay() {
    // Kiểm tra xem bạn đã kéo đủ 6 prefab vào mảng recordItems chưa
    if (recordItems == null || recordItems.Length < 6) return;

    // Lấy dữ liệu mảng số lượng merge từ DataManager
    int[] counts = DataManager.Instance.currentSaveData.fruitMergeCounts;

    for (int i = 0; i < 6; i++)
    {
        // Công thức tính ngược ID: 10, 9, 8, 7, 6, 5
        int fruitID = 10 - i; 

        // 1. Lấy Sprite của quả tương ứng từ SkinManager
        Sprite icon = SkinManager.Instance.GetSpriteForFruit(fruitID);

        // 2. Lấy số lượng đã merge của quả đó từ dữ liệu đã lưu
        int count = 0;
        if (fruitID < counts.Length)
        {
            count = counts[fruitID];
        }

        // 3. Đổ dữ liệu vào Prefab cố định tại vị trí i
        // recordItems[i] chính là cái prefab bạn đặt ở dòng thứ i trên bảng
        recordItems[i].SetData(icon, count);
        
        // Luôn hiển thị vì đây là bảng cố định
        recordItems[i].gameObject.SetActive(true);
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
    public void OpenRecord() { CloseAllPopups(); EnablePopup(panelRecord); }
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