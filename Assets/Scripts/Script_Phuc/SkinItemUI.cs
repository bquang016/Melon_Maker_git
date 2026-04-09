using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image skinIcon;
    public TextMeshProUGUI skinNameText;

    [Header("Action Controls")]
    public Button actionButton;
    public TextMeshProUGUI actionText;
    public GameObject adIcon;        // Icon xem quảng cáo
    public GameObject checkmarkIcon; // Dấu tick xanh

    [Header("Button Colors")]
    public Color yellowColor = new Color(1f, 0.8f, 0f); // Màu vàng cho Ads
    public Color blueColor = new Color(0f, 0.5f, 1f);   // Màu xanh cho Use

    private SkinPackData mySkinData;

    public void Setup(SkinPackData data)
    {
        mySkinData = data;
        skinNameText.text = data.packName;
        if (data.fruitSprites != null && data.fruitSprites.Length > 0)
            skinIcon.sprite = data.fruitSprites[0];

        UpdateUIState();

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => ShopManager.Instance.OnSkinButtonClicked(mySkinData));
    }

    public void UpdateUIState()
    {
        // 1. Kiểm tra trạng thái sở hữu
        bool isUnlocked = ShopManager.Instance.CheckIsUnlocked(mySkinData.packID) || !mySkinData.requiresAd;
        // 2. Kiểm tra xem có đang trang bị không
        bool isEquipped = (SkinManager.Instance.currentSkinPack == mySkinData);

        if (isEquipped)
        {
            // TRẠNG THÁI: ĐANG TRANG BỊ -> Hiện tick, ẩn nút
            checkmarkIcon.SetActive(true);
            actionButton.gameObject.SetActive(false);
        }
        else
        {
            // TRẠNG THÁI: CHƯA TRANG BỊ -> Hiện nút, ẩn tick
            checkmarkIcon.SetActive(false);
            actionButton.gameObject.SetActive(true);

            if (isUnlocked)
            {
                // Đã sở hữu nhưng chưa dùng -> Nút Xanh + Chữ "Use"
                actionButton.image.color = blueColor;
                actionText.text = "Use";
                actionText.gameObject.SetActive(true);
                adIcon.SetActive(false);
            }
            else
            {
                // Chưa sở hữu -> Nút Vàng + Icon Ads
                actionButton.image.color = yellowColor;
                actionText.gameObject.SetActive(false);
                adIcon.SetActive(true);
            }
        }
    }
}