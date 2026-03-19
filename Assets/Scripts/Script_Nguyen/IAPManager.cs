using System;
using System.Collections;
using UnityEngine;

public class IAPManager : MonoBehaviour
{
    // Lại là Singleton để gọi mọi lúc mọi nơi
    public static IAPManager Instance { get; private set; }

    // --- CÁC EVENT GIAO TIẾP VỚI DEV KHÁC ---
    // Dev 2 (Data) nghe sự kiện này để cộng tiền hoặc mở khóa vĩnh viễn
    public static event Action<string> OnPurchaseSuccess;
    public static event Action<string> OnPurchaseFailed;

    // Khai báo sẵn các ID gói hàng (Sau này lấy từ Google Play Console)
    public const string PRODUCT_REMOVE_ADS = "com.team.suika.removeads";
    public const string PRODUCT_1000_COINS = "com.team.suika.1000coins";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ==========================================
    // HÀM GIẢ LẬP MUA HÀNG (MOCK IAP) CHO TUẦN 1
    // ==========================================

    // Dev 5 (Làm UI) sẽ gọi hàm này khi người chơi bấm nút "Mua 0.99$"
    public void BuyProduct(string productId)
    {
        Debug.Log($"<b>[IAPManager]</b> <color=orange>Đang kết nối tới Google Play để thanh toán gói: {productId}...</color>");

        // Gọi Coroutine giả lập thời gian chờ mạng internet
        StartCoroutine(MockPurchaseRoutine(productId));
    }

    private IEnumerator MockPurchaseRoutine(string productId)
    {
        // Giả lập giao dịch mất 1.5 giây
        yield return new WaitForSeconds(1.5f);

        // Giả lập: 90% mua thành công, 10% khách hàng hủy thẻ/lỗi mạng
        float randomChance = UnityEngine.Random.Range(0f, 100f);

        if (randomChance > 10f)
        {
            Debug.Log($"<b>[IAPManager]</b> <color=green>Giao dịch THÀNH CÔNG: Mua xong {productId}!</color>");

            // Phát loa thông báo cho Dev 2 (Data) lưu lại đồ vừa mua
            OnPurchaseSuccess?.Invoke(productId);
        }
        else
        {
            Debug.Log($"<b>[IAPManager]</b> <color=red>Giao dịch THẤT BẠI hoặc người dùng bấm Hủy: {productId}</color>");
            OnPurchaseFailed?.Invoke(productId);
        }
    }
}