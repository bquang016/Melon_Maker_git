using UnityEngine;
using System;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Kho hàng của Shop (Kéo thả các SO vào đây)")]
    public List<SkinPackData> allSkinPacks;

    // ----- CÁC SỰ KIỆN (EVENTS) ĐỂ DEV 5 CẬP NHẬT UI -----
    public Action<int> OnMoneyChanged;      // Bắn ra khi tiền thay đổi (để update text Tiền)
    public Action<string> OnBuySuccess;     // Bắn ra khi mua thành công (để hiện Popup Chúc mừng)
    public Action<string> OnBuyFailed;      // Bắn ra khi mua thất bại (để hiện Popup Lỗi)

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // Khởi tạo trạng thái Unlock cho các món hàng khi vừa mở game
        LoadUnlockedStatus();
    }

    // ----------------------------------------------------
    // GIẢ LẬP TIỀN TỆ (Sau này Dev 2 sẽ thay bằng Data thật)
    // ----------------------------------------------------
    public int GetCurrentMoney()
    {
        // Tạm thời lưu tiền bằng PlayerPrefs. Cho sẵn 1000 vàng để test.
        return PlayerPrefs.GetInt("PlayerMoney", 1000);
    }

    public void AddMoney(int amount)
    {
        int newMoney = GetCurrentMoney() + amount;
        PlayerPrefs.SetInt("PlayerMoney", newMoney);
        PlayerPrefs.Save();

        OnMoneyChanged?.Invoke(newMoney); // Hét lên cho UI biết tiền đã đổi!
    }

    private void DeductMoney(int amount)
    {
        int newMoney = GetCurrentMoney() - amount;
        PlayerPrefs.SetInt("PlayerMoney", newMoney);
        PlayerPrefs.Save();

        OnMoneyChanged?.Invoke(newMoney);
    }

    // ----------------------------------------------------
    // LOGIC MUA BÁN VÀ TRANG BỊ
    // ----------------------------------------------------

    // Hàm này sẽ được gọi khi User bấm vào nút MUA ở 1 món hàng
    public void BuySkin(SkinPackData packToBuy)
    {
        // 1. Check xem đã có chưa?
        if (CheckIsUnlocked(packToBuy.packID))
        {
            EquipSkin(packToBuy); // Có rồi thì mặc luôn, không trừ tiền
            return;
        }

        // 2. Check xem đủ tiền không?
        if (GetCurrentMoney() >= packToBuy.price)
        {
            // Đủ tiền -> Trừ tiền
            DeductMoney(packToBuy.price);

            // Mở khóa skin
            UnlockSkin(packToBuy.packID);

            // Trang bị luôn bộ vừa mua
            EquipSkin(packToBuy);

            // Bắn Event thành công
            OnBuySuccess?.Invoke($"Bạn đã mua thành công {packToBuy.packName}!");
        }
        else
        {
            // Không đủ tiền
            OnBuyFailed?.Invoke("Không đủ vàng! Hãy chơi thêm hoặc xem quảng cáo nhé.");
        }
    }

    // Hàm trang bị Skin (Gửi dữ liệu sang SkinManager của bạn ở bài trước)
    public void EquipSkin(SkinPackData packToEquip)
    {
        if (SkinManager.Instance != null)
        {
            SkinManager.Instance.currentSkinPack = packToEquip;
            Debug.Log($"Đã trang bị bộ: {packToEquip.packName}");
        }
    }

    // ----------------------------------------------------
    // LOGIC LƯU TRỮ TRẠNG THÁI MỞ KHÓA (Dùng PlayerPrefs tạm)
    // ----------------------------------------------------
    private void UnlockSkin(string packID)
    {
        // Lưu 1 biến có tên là "Unlocked_animal" với giá trị = 1 (1 là true, 0 là false)
        PlayerPrefs.SetInt("Unlocked_" + packID, 1);
        PlayerPrefs.Save();
    }

    public bool CheckIsUnlocked(string packID)
    {
        // Luôn luôn mở khóa bộ Mặc Định
        if (packID == "default") return true;

        // Trả về true nếu giá trị lưu là 1
        return PlayerPrefs.GetInt("Unlocked_" + packID, 0) == 1;
    }

    private void LoadUnlockedStatus()
    {
        // Quét qua toàn bộ gói skin, nếu trong PlayerPrefs có báo đã mua thì đánh dấu mở khóa
        foreach (var pack in allSkinPacks)
        {
            if (CheckIsUnlocked(pack.packID))
            {
                pack.isUnlocked = true;
            }
        }
    }
}