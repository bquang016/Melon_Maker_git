using UnityEngine;
using TMPro;

public class DiamondUIUpdater : MonoBehaviour
{
    [Header("Kéo cái Text hiển thị số Kim Cương vào đây")]
    public TextMeshProUGUI diamondText;

    // Hàm này tự động chạy mỗi khi cái Bảng chứa nó (Màn hình Shop) được bật lên (SetActive(true))
    private void OnEnable()
    {
        if (ShopManager.Instance != null)
        {
            // 1. Vừa mở bảng lên là lấy ngay số tiền hiện tại đập vào mặt UI
            UpdateText(ShopManager.Instance.GetDiamonds());

            // 2. Đăng ký "Hóng chuyện": Hễ có ai gọi OnDiamondChanged thì tự động cập nhật lại chữ
            ShopManager.Instance.OnDiamondChanged += UpdateText;
        }
    }

    private void OnDisable()
    {
        // Phải hủy hóng chuyện khi tắt màn hình để tránh lỗi bộ nhớ
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OnDiamondChanged -= UpdateText;
        }
    }

    private void UpdateText(int newAmount)
    {
        if (diamondText != null)
        {
            diamondText.text = newAmount.ToString();
            Debug.Log($"<color=cyan>[DiamondUIUpdater]</color> UI Đã nghe thấy sự kiện OnDiamondChanged! Đã đổi số trên màn hình thành: {newAmount}");
        }
        else
        {
            Debug.LogError("<color=red>[DiamondUIUpdater]</color> LỖI MẤT KẾT NỐI: Script này chưa được Dev 5 kéo cái cục Text vào ô diamondText ở cột Inspector!");
        }
    }
}