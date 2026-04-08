using UnityEngine;
using UnityEngine.UI;
using TMPro; // Dùng TextMeshPro cho mượt

public class SkinItemUI : MonoBehaviour
{
    [Header("Dev 5 kéo các thành phần trong Prefab vào đây:")]
    public Image skinIcon;
    public TextMeshProUGUI skinNameText;
    public TextMeshProUGUI actionText; // Chữ trên nút ("Sử Dụng", "Xem Ads", "Đang Dùng")
    public Button actionButton;

    private SkinPackData mySkinData;

    // Hàm này được gọi khi màn hình sinh ra cái ô này
    public void Setup(SkinPackData data)
    {
        mySkinData = data;
        skinNameText.text = data.packName;

        // Lấy ảnh đầu tiên của bộ làm Icon
        if (data.fruitSprites != null && data.fruitSprites.Length > 0)
        {
            skinIcon.sprite = data.fruitSprites[0];
        }

        UpdateUIState();

        // Cài đặt sự kiện khi người chơi bấm nút
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() =>
        {
            ShopManager.Instance.OnSkinButtonClicked(mySkinData);
        });
    }

    // Hàm này để đổi trạng thái của nút bấm (Tự động chạy theo Data của Dev 4)
    public void UpdateUIState()
    {
        bool isUnlocked = ShopManager.Instance.CheckIsUnlocked(mySkinData.packID) || !mySkinData.requiresAd;

        if (isUnlocked)
        {
            // Nếu đã mở khóa -> Check xem có đang mặc không
            if (SkinManager.Instance.currentSkinPack == mySkinData)
            {
                actionText.text = "Đang Dùng";
                actionButton.interactable = false; // Tối màu nút lại, không cho bấm nữa
            }
            else
            {
                actionText.text = "Sử Dụng";
                actionButton.interactable = true; // Sáng lên cho bấm
            }
        }
        else
        {
            // Nếu chưa mở khóa
            actionText.text = "Xem Ads";
            actionButton.interactable = true;
        }
    }
}