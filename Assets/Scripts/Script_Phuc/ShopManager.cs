using UnityEngine;
using System;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    private SkinPackData pendingSkinToUnlock;

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

    // Lấy số Kim Cương (Ruby) trực tiếp từ DataManager thay vì PlayerPrefs
    public int GetDiamonds()
    {
        if (DataManager.Instance != null && DataManager.Instance.currentSaveData != null)
        {
            return DataManager.Instance.currentSaveData.metaData.currentRuby;
        }
        return 0; // Trả về 0 nếu DataManager chưa kịp load
    }

    public int GetHammerCount() => PlayerPrefs.GetInt("Item_Hammer", 0);
    public int GetShakeCount() => PlayerPrefs.GetInt("Item_Shake", 0);

    // Dùng cho nút [Xem Ads nhận Kim Cương] trên UI Shop
    public void WatchAdForDiamonds()
    {
        Debug.Log("[Ads] Đang bật video quảng cáo để nhận 50 Kim Cương...");
        // Ở đây đáng lẽ gọi AdsManager, nhưng để test nhanh cứ cộng luôn:
        AddDiamonds(50);
        OnNotify?.Invoke("Nhận thành công 50 Kim Cương!");
    }

    // Hàm quan trọng nhất: Cộng/Trừ tiền nối thẳng vào két sắt của DataManager
    // Hàm quan trọng nhất: Cộng/Trừ tiền nối thẳng vào két sắt của DataManager
    public void AddDiamonds(int amount)
    {
        Debug.Log($"<color=yellow>[ShopManager]</color> NHẬN ĐƯỢC LỆNH TỪ NÚT BẤM IAP: Yêu cầu cộng {amount} Kim Cương!");

        if (DataManager.Instance != null && DataManager.Instance.currentSaveData != null)
        {
            // Cộng (hoặc trừ) tiền vào Ruby
            DataManager.Instance.currentSaveData.metaData.currentRuby += amount;

            Debug.Log($"<color=green>[ShopManager]</color> Đã nhét tiền vào két! Tổng tiền trong DataManager hiện đang là: {DataManager.Instance.currentSaveData.metaData.currentRuby}");

            // Lưu lại ngay lập tức xuống ổ cứng
            DataManager.Instance.SaveDataToDisk();

            // Hét lên cho UI Shop cập nhật lại số tiền hiển thị
            OnDiamondChanged?.Invoke(DataManager.Instance.currentSaveData.metaData.currentRuby);
        }
        else
        {
            Debug.LogError("<color=red>[ShopManager]</color> LỖI CỰC MẠNH: Thằng DataManager chưa được tạo ra! Két sắt không tồn tại để cất tiền!");
        }
    }

    // Dùng cho nút [Mua Búa] trên UI Shop
    public void BuyHammer(int cost = 100)
    {
        if (GetDiamonds() >= cost)
        {
            AddDiamonds(-cost); // Trừ tiền bằng DataManager

            PlayerPrefs.SetInt("Item_Hammer", GetHammerCount() + 1);
            PlayerPrefs.Save();

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
            AddDiamonds(-cost); // Trừ tiền bằng DataManager

            PlayerPrefs.SetInt("Item_Shake", GetShakeCount() + 1);
            PlayerPrefs.Save();

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
        pendingSkinToUnlock = pack;

        // 1. Đăng ký nghe sự kiện OnRewardSkin từ AdsManager
        AdsManager.OnRewardSkin += OnAdsFinished;

        // 2. Gọi hàm hiện quảng cáo với tham số RewardType.Skin
        AdsManager.Instance.ShowRewardVideo(RewardType.Skin);

        Debug.Log($"[Shop] Đang gọi AdMob để mở khóa: {pack.packName}");
    }

    private void OnAdsFinished() // Bỏ tham số int vì OnRewardSkin không truyền số
    {
        AdsManager.OnRewardSkin -= OnAdsFinished; // Hủy đăng ký ngay

        if (pendingSkinToUnlock != null)
        {
            UnlockSkin(pendingSkinToUnlock.packID);
            EquipSkin(pendingSkinToUnlock);
            OnNotify?.Invoke($"Mở khóa thành công bộ {pendingSkinToUnlock.packName}!");
            pendingSkinToUnlock = null;
        }
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