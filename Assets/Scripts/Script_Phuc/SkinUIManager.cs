using UnityEngine;
using System.Collections.Generic;

public class SkinUIManager : MonoBehaviour
{
    [Header("Cài đặt danh sách (Dev 5 kéo vào đây)")]
    public Transform verticalContent; // Kéo cái cục "Content" của Scroll View vào
    public SkinItemUI skinItemPrefab; // Kéo cái Prefab thanh ngang vào

    private List<SkinItemUI> spawnedItems = new List<SkinItemUI>();

    private void Start()
    {
        // Lắng nghe sự kiện: Bất cứ khi nào có 1 skin được trang bị, tự động load lại tất cả các nút!
        ShopManager.Instance.OnSkinEquipped += RefreshAllButtons;

        GenerateVerticalList();
    }

    private void OnDestroy()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OnSkinEquipped -= RefreshAllButtons;
        }
    }

    private void GenerateVerticalList()
    {
        // Quét kho hàng của Dev 4
        foreach (var pack in ShopManager.Instance.allSkinPacks)
        {
            // Đẻ ra 1 thanh Prefab nhét vào Scroll View
            SkinItemUI newItem = Instantiate(skinItemPrefab, verticalContent);

            // Nạp data cho nó
            newItem.Setup(pack);

            // Lưu vào danh sách để quản lý
            spawnedItems.Add(newItem);
        }
    }

    // Hàm này réo toàn bộ các thanh Prefab update lại chữ (Ví dụ: Chuyển nút kia thành "Sử Dụng", nút này thành "Đang Dùng")
    private void RefreshAllButtons(SkinPackData pack)
    {
        foreach (var item in spawnedItems)
        {
            item.UpdateUIState();
        }
    }
}