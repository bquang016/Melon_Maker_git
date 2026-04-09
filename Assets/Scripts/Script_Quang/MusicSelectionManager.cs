using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class MusicSelectionManager : MonoBehaviour
{
    [Header("Danh sách bài hát")]
    public List<MusicItem> allMusicItems = new List<MusicItem>();

    private MusicItem currentlyPreviewingItem;
    private MusicItem currentSelectedMusic;

    [Header("--- UI THANH DƯỚI CÙNG ---")]
    public GameObject groupPlayState;
    public GameObject groupUnlockState;
    public TextMeshProUGUI textUnlockPrice;
    public GameObject buyButtonOverlay;

    // KHÔNG CẦN BIẾN currentPoints NỮA VÌ ĐÃ CÓ KÉT SẮT DATAMANAGER

    void Start()
    {
        if (allMusicItems.Count > 0)
        {
            currentSelectedMusic = allMusicItems[0];
            currentSelectedMusic.isUnlocked = true;
        }
    }

    void OnEnable()
    {
        currentlyPreviewingItem = currentSelectedMusic;

        if (AudioManager.Instance != null && currentSelectedMusic != null)
        {
            AudioManager.Instance.PlayMusic(currentSelectedMusic.trackIndex);
        }

        RefreshAllItems();
    }

    // Bắt sự kiện khi Két Sắt nhận được nốt nhạc từ Ads thì tự động làm mới Shop
    void Update()
    {
        // Hàm Update chạy liên tục giúp nút Mua (Buy Overlay) tự động tắt mờ ngay khi xem Ads xong
        if (groupUnlockState.activeSelf)
        {
            CheckBuyButtonOverlay();
        }
    }

    public void OnItemClicked(MusicItem clickedItem)
    {
        currentlyPreviewingItem = clickedItem;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(clickedItem.trackIndex);
        }

        RefreshAllItems();
    }

    private void RefreshAllItems()
    {
        if (currentlyPreviewingItem == null) return;

        // Cập nhật UI cho từng bài hát
        foreach (MusicItem item in allMusicItems)
        {
            if (item == null) continue;
            bool isPreview = (item == currentlyPreviewingItem);
            bool isSelected = (item == currentSelectedMusic);
            item.UpdateUIState(isPreview, isSelected);
        }

        // --- LỚP GIÁP KIỂM TRA INSPECTOR ---
        if (groupPlayState == null || groupUnlockState == null)
        {
            Debug.LogError(" LỖI UNITY INSPECTOR: Bạn chưa kéo 'Group Play State' hoặc 'Group Unlock State' vào Music Selection Manager!");
            return; // Dừng lại ngay để không bị văng lỗi crash game
        }

        // Cập nhật Thanh UI bên dưới (Nút Mua / Nút Chọn)
        if (currentlyPreviewingItem.isUnlocked)
        {
            groupPlayState.SetActive(true);
            groupUnlockState.SetActive(false);
        }
        else
        {
            groupPlayState.SetActive(false);
            groupUnlockState.SetActive(true);

            if (textUnlockPrice != null)
            {
                textUnlockPrice.text = currentlyPreviewingItem.price.ToString();
            }

            CheckBuyButtonOverlay();
        }
    }

    private void CheckBuyButtonOverlay()
    {
        // 1. Kiểm tra an toàn tầng 1 (Tránh lỗi chưa kéo thả Inspector)
        if (buyButtonOverlay == null || currentlyPreviewingItem == null) return;

        // 2. Kiểm tra an toàn tầng 2 (Tránh lỗi chưa có Két Sắt)
        if (DataManager.Instance == null || DataManager.Instance.currentSaveData == null) return;

        // 3. TỰ ĐỘNG SỬA LỖI FILE SAVE CŨ (Nếu file cũ không có metaData thì tạo mới)
        if (DataManager.Instance.currentSaveData.metaData == null)
        {
            DataManager.Instance.currentSaveData.metaData = new GameState();
        }

        // 4. Soi tiền và cập nhật nút
        int tienThat = DataManager.Instance.currentSaveData.metaData.currentMusicNote;

        if (tienThat >= currentlyPreviewingItem.price)
        {
            buyButtonOverlay.SetActive(false); // Đủ tiền -> Tắt mờ nút
        }
        else
        {
            buyButtonOverlay.SetActive(true);  // Thiếu tiền -> Bật mờ nút
        }
    }

    public void OnClick_PlayButton()
    {
        currentSelectedMusic = currentlyPreviewingItem;
        RefreshAllItems();
    }

    public void OnClick_UnlockButton()
    {
        // LẤY TIỀN TỪ KÉT SẮT RA ĐỂ KIỂM TRA
        int tienThat = DataManager.Instance.currentSaveData.metaData.currentMusicNote;

        if (tienThat < currentlyPreviewingItem.price) return;

        // TRỪ TIỀN THẬT VÀ LƯU VÀO Ổ CỨNG
        DataManager.Instance.currentSaveData.metaData.currentMusicNote -= currentlyPreviewingItem.price;
        DataManager.Instance.SaveDataToDisk();

        currentlyPreviewingItem.isUnlocked = true;
        currentSelectedMusic = currentlyPreviewingItem;

        RefreshAllItems();
    }

    public void OnClick_ClosePanel()
    {
        currentlyPreviewingItem = currentSelectedMusic;

        if (AudioManager.Instance != null && currentSelectedMusic != null)
        {
            AudioManager.Instance.PlayMusic(currentSelectedMusic.trackIndex);
        }

        RefreshAllItems();
        gameObject.SetActive(false);
    }

    // NÚT XEM QUẢNG CÁO BÂY GIỜ ĐƯỢC CHUYỂN SANG CHO ADSMANAGER XỬ LÝ NÊN XÓA HÀM CŨ ĐI
}