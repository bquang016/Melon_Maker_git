using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    // Cấu trúc Singleton: Giúp gọi AdsManager từ bất kỳ đâu mà không cần FindObject
    public static AdsManager Instance { get; private set; }

    // --- CÁC EVENT (SỰ KIỆN) GIAO TIẾP VỚI DEV KHÁC ---
    // Dev 2 (Data) và Dev 5 (UI) sẽ "nghe" các sự kiện này để cộng tiền hoặc đổi màn hình
    public static event Action<int> OnRewardVideoWatched; // Phát ra khi xem xong video nhận thưởng (truyền theo số tiền)
    public static event Action OnInterstitialClosed;      // Phát ra khi đóng quảng cáo toàn màn hình

    private void Awake()
    {
        // Thiết lập Singleton và giữ cho AdsManager không bị hủy khi chuyển Scene
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
    // CÁC HÀM GIẢ LẬP (MOCK API) CHO TUẦN 1
    // (Tuần 2 chúng ta sẽ thay ruột các hàm này bằng code thật của AdMob)
    // ==========================================

    public void ShowBanner()
    {
        // Hiện thông báo dưới góc console thay vì hiện quảng cáo thật
        Debug.Log("<b>[AdsManager]</b> <color=green>Đã hiển thị Banner ở đáy màn hình.</color>");
    }

    public void HideBanner()
    {
        Debug.Log("<b>[AdsManager]</b> <color=red>Đã ẩn Banner.</color>");
    }

    public void ShowInterstitial()
    {
        Debug.Log("<b>[AdsManager]</b> <color=yellow>Đang hiển thị Quảng cáo toàn màn hình (Interstitial)...</color>");

        // Giả lập việc người chơi bấm dấu X đóng quảng cáo sau 1 giây
        Invoke(nameof(CloseInterstitialMock), 1f);
    }

    private void CloseInterstitialMock()
    {
        Debug.Log("<b>[AdsManager]</b> Đã đóng quảng cáo toàn màn hình.");
        // Phát loa thông báo cho Dev 5 biết để hiện màn hình Game Over
        OnInterstitialClosed?.Invoke();
    }

    public void ShowRewardVideo()
    {
        Debug.Log("<b>[AdsManager]</b> <color=cyan>Đang xem Video nhận thưởng (Reward Video)...</color>");

        // Giả lập việc xem video tốn 2 giây
        Invoke(nameof(FinishRewardVideoMock), 2f);
    }

    private void FinishRewardVideoMock()
    {
        int thuongVang = 50; // Giả sử phần thưởng là 50 vàng
        Debug.Log($"<b>[AdsManager]</b> Tuyệt vời! Đã xem xong video. Thưởng {thuongVang} Vàng.");

        // PHÁT LOA THÔNG BÁO: Bất kỳ script nào (của Dev 2 hay Dev 4) đang nghe sự kiện này 
        // sẽ tự động được kích hoạt và nhận số 50 vào hàm của họ.
        OnRewardVideoWatched?.Invoke(thuongVang);
    }
}