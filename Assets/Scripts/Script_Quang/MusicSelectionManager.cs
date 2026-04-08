using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class MusicSelectionManager : MonoBehaviour
{
    [Header("Danh sách bài hát")]
    public List<MusicItem> allMusicItems = new List<MusicItem>();

    private MusicItem currentlyPreviewingItem;
    private MusicItem currentSelectedMusic;

    [Header("--- HỆ THỐNG ĐIỂM NHẠC ---")]
    public int currentPoints = 0;
    public TextMeshProUGUI textPointsDisplay;

    [Header("--- UI THANH DƯỚI CÙNG ---")]
    public GameObject groupPlayState;
    public GameObject groupUnlockState;
    public TextMeshProUGUI textUnlockPrice;

    // THÊM DÒNG NÀY ĐỂ NHẬN DIỆN CÁI OVERLAY CỦA NÚT MUA
    public GameObject buyButtonOverlay;

    void Start()
    {
        if (allMusicItems.Count > 0)
        {
            currentSelectedMusic = allMusicItems[0];
            currentSelectedMusic.isUnlocked = true;
        }
        UpdatePointsUI();
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
        // 1. Cập nhật 14 bài hát
        foreach (MusicItem item in allMusicItems)
        {
            item.SetupItem(this);
            bool isPreview = (item == currentlyPreviewingItem);
            bool isSelected = (item == currentSelectedMusic);
            item.UpdateUIState(isPreview, isSelected);
        }

        // 2. Cập nhật Thanh UI bên dưới
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

            // --- LOGIC MỚI: BẬT/TẮT LỚP PHỦ NÚT MUA TÙY THEO TIỀN ---
            if (buyButtonOverlay != null)
            {
                if (currentPoints >= currentlyPreviewingItem.price)
                {
                    buyButtonOverlay.SetActive(false); // Đủ tiền -> Tắt mờ, nút sáng lên
                }
                else
                {
                    buyButtonOverlay.SetActive(true);  // Thiếu tiền -> Bật mờ
                }
            }
        }
    }

    public void OnClick_PlayButton()
    {
        currentSelectedMusic = currentlyPreviewingItem;
        RefreshAllItems();
    }

    public void OnClick_UnlockButton()
    {
        // Nếu cố tình bấm khi không đủ tiền thì bỏ qua
        if (currentPoints < currentlyPreviewingItem.price) return;

        // Trừ tiền
        currentPoints -= currentlyPreviewingItem.price;
        UpdatePointsUI();

        // Mở khóa bài hát & tự động chọn
        currentlyPreviewingItem.isUnlocked = true;
        currentSelectedMusic = currentlyPreviewingItem;

        // Refresh lại UI
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

    public void OnClick_WatchAdButton()
    {
        currentPoints += 500;
        UpdatePointsUI();

        // --- GỌI LẠI REFRESH Ở ĐÂY ---
        // Để ngay khi nhận tiền, nếu đủ mua thì lớp mờ của nút tự động bay màu ngay lập tức
        RefreshAllItems();
    }

    private void UpdatePointsUI()
    {
        if (textPointsDisplay != null)
        {
            textPointsDisplay.text = currentPoints.ToString();
        }
    }
}