using UnityEngine;
using System;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Kho hàng Skin (Kéo các file Skin vào đây)")]
    public List<SkinPackData> allSkinPacks;

    // ----- SỰ KIỆN (EVENTS) ĐỂ DEV 5 CẬP NHẬT GIAO DIỆN -----
    public Action<int> OnDiamondChanged;       // Khi số Kim Cương thay đổi
    public Action<int> OnHammerCountChanged;   // Khi số lượng Búa thay đổi
    public Action<int> OnShakeCountChanged;    // Khi số lượng Lắc thay đổi
    public Action<SkinPackData> OnSkinEquipped;// Để Dev 5 di chuyển cái dấu Tick V xanh
    public Action<string> OnNotify;            // Để hiện chữ thông báo (Popup Text)

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // ==================================================
    // PHẦN 1: MÀN HÌNH SHOP (MUA BOOSTER & NHẬN KIM CƯƠNG)
    // ==================================================

    public int GetDiamonds() => PlayerPrefs.GetInt("PlayerDiamonds", 0);
    public int GetHammerCount() => PlayerPrefs.GetInt("Item_Hammer", 0);
    public int GetShakeCount() => PlayerPrefs.GetInt("Item_Shake", 0);

    // Dùng cho nút [Xem Ads nhận Kim Cương] trên UI Shop
    public void WatchAdForDiamonds()
    {
        // Ghi chú cho Dev 3: Chèn code gọi SDK AdMob vào đây!
        Debug.Log("[Ads] Đang bật video quảng cáo để nhận 50 Kim Cương...");

        // Giả lập user xem video thành công:
        AddDiamonds(50);
        OnNotify?.Invoke("Nhận thành công 50 Kim Cương!");
    }

    public void AddDiamonds(int amount)
    {
        int newTotal = GetDiamonds() + amount;
        PlayerPrefs.SetInt("PlayerDiamonds", newTotal);
        PlayerPrefs.Save();
        OnDiamondChanged?.Invoke(newTotal); // Báo UI nhảy số
    }

    // Dùng cho nút [Mua Búa] trên UI Shop
    public void BuyHammer(int cost = 100)
    {
        if (GetDiamonds() >= cost)
        {
            PlayerPrefs.SetInt("PlayerDiamonds", GetDiamonds() - cost);
            PlayerPrefs.SetInt("Item_Hammer", GetHammerCount() + 1);
            PlayerPrefs.Save();

            OnDiamondChanged?.Invoke(GetDiamonds());
            OnHammerCountChanged?.Invoke(GetHammerCount());
            OnNotify?.Invoke("Đã mua 1 Búa!");
        }
        else OnNotify?.Invoke("Không đủ Kim Cương! Hãy xem Ads nhé.");
    }

    // Dùng cho nút [Mua Lắc Hộp] trên UI Shop
    public void BuyShake(int cost = 50)
    {
        if (GetDiamonds() >= cost)
        {
            PlayerPrefs.SetInt("PlayerDiamonds", GetDiamonds() - cost);
            PlayerPrefs.SetInt("Item_Shake", GetShakeCount() + 1);
            PlayerPrefs.Save();

            OnDiamondChanged?.Invoke(GetDiamonds());
            OnShakeCountChanged?.Invoke(GetShakeCount());
            OnNotify?.Invoke("Đã mua 1 lượt Lắc Hộp!");
        }
        else OnNotify?.Invoke("Không đủ Kim Cương! Hãy xem Ads nhé.");
    }


    // ==================================================
    // PHẦN 2: MÀN HÌNH SKIN (ĐỔI GIAO DIỆN)
    // ==================================================

    // Dev 5 sẽ gắn hàm này vào TẤT CẢ các nút bấm của từng ô Skin
    public void OnSkinButtonClicked(SkinPackData pack)
    {
        // 1. Nếu bộ này đã mở khóa (hoặc là bộ mặc định) -> Mặc luôn
        if (CheckIsUnlocked(pack.packID) || !pack.requiresAd)
        {
            EquipSkin(pack);
        }
        // 2. Nếu chưa mở khóa -> Bắt xem quảng cáo
        else
        {
            WatchAdToUnlockSkin(pack);
        }
    }

    private void WatchAdToUnlockSkin(SkinPackData pack)
    {
        // Ghi chú cho Dev 3: Chèn code gọi SDK AdMob vào đây!
        Debug.Log($"[Ads] Đang bật video quảng cáo để mở khóa bộ {pack.packName}...");

        // Giả lập user xem video thành công:
        UnlockSkin(pack.packID);
        EquipSkin(pack);
        OnNotify?.Invoke($"Mở khóa thành công bộ {pack.packName}!");
    }

    private void EquipSkin(SkinPackData packToEquip)
    {
        if (SkinManager.Instance != null)
        {
            SkinManager.Instance.currentSkinPack = packToEquip;
            Debug.Log($"[Skin] Đang sử dụng bộ: {packToEquip.packName}");

            // Hét lên cho Dev 5 biết để dời cái dấu CheckMark (V) vào ô vừa bấm
            OnSkinEquipped?.Invoke(packToEquip);
        }
    }

    private void UnlockSkin(string packID)
    {
        PlayerPrefs.SetInt("Unlocked_" + packID, 1);
        PlayerPrefs.Save();
    }

    public bool CheckIsUnlocked(string packID)
    {
        if (packID == "default") return true;
        return PlayerPrefs.GetInt("Unlocked_" + packID, 0) == 1;
    }
}