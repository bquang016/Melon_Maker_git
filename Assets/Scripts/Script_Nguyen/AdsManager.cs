using System;
using UnityEngine;
using GoogleMobileAds.Api;

// ĐÃ SỬA: Đổi Coin thành Ruby
public enum RewardType
{
    Ruby,   // Xem để nhận Ruby
    Revive,  // Xem để hồi sinh (Last Chance)
    MusicNote,
    Skin
}

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    // ==========================================
    // CÁC SỰ KIỆN (EVENTS) ĐỂ GIAO TIẾP VỚI CÁC DEV KHÁC
    // ==========================================
    public static event Action OnInterstitialClosed;
    public static event Action<int> OnRewardRuby;        
    public static event Action OnRewardRevive;
    public static event Action<int> OnRewardMusicNote;
    public static event Action OnRewardSkin;

    // ==========================================
    // ID TEST CỦA GOOGLE
    // ==========================================
    private readonly string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    private readonly string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    private readonly string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private RewardType currentRewardType;

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

    private void Start()
    {
        Debug.Log("[AdsManager] Đang khởi tạo SDK AdMob...");
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("<color=green>[AdsManager] Khởi tạo AdMob THÀNH CÔNG!</color>");
            LoadInterstitialAd();
            LoadRewardedAd();
        });
    }

    // ==========================================
    // 1. BANNER ADS
    // ==========================================
    public void ShowBanner()
    {
        if (bannerView != null) bannerView.Destroy();

        AdRequest request = new AdRequest();
        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        bannerView.LoadAd(request);
    }

    public void HideBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }

    // ==========================================
    // 2. INTERSTITIAL ADS 
    // ==========================================
    public void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        AdRequest request = new AdRequest();
        InterstitialAd.Load(interstitialAdUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null) return;
            interstitialAd = ad;

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                LoadInterstitialAd();
                OnInterstitialClosed?.Invoke();
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError e) =>
            {
                LoadInterstitialAd();
                OnInterstitialClosed?.Invoke();
            };
        });
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            OnInterstitialClosed?.Invoke();
        }
    }

    // ==========================================
    // 3. REWARDED VIDEO 
    // ==========================================
    public void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        AdRequest request = new AdRequest();
        RewardedAd.Load(rewardedAdUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null) return;
            rewardedAd = ad;

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                LoadRewardedAd();
            };

            rewardedAd.OnAdFullScreenContentFailed += (AdError e) =>
            {
                LoadRewardedAd();
            };
        });
    }

    public void ShowRewardVideo(RewardType type)
    {
        currentRewardType = type;

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            Debug.Log($"[AdsManager] Đang bật Video Reward cho mục đích: {type}");
            rewardedAd.Show(RewardUser);
        }
        else
        {
            Debug.LogWarning("[AdsManager] Video chưa tải xong! Hãy thử lại sau.");
        }
    }

    private void RewardUser(Reward reward)
    {
        Debug.Log("<color=cyan>[AdsManager] User đã xem hết video! Chuẩn bị phát thưởng...</color>");

        // ĐÃ SỬA: Check Ruby và phát sự kiện OnRewardRuby
        if (currentRewardType == RewardType.Ruby)
        {
            int ruby = 100; // Thưởng 100 Ruby
            OnRewardRuby?.Invoke(ruby);
        }
        else if (currentRewardType == RewardType.Revive)
        {
            OnRewardRevive?.Invoke();
        }
        else if (currentRewardType == RewardType.MusicNote)
        {
            int musicnote = 500; 
            OnRewardMusicNote?.Invoke(musicnote);
        }
        else if (currentRewardType == RewardType.Skin)
        {
            OnRewardSkin?.Invoke(); // Phát tín hiệu đã xem xong để mở skin
        }
    }

    // ==========================================
    // HÀM MỒI CHO NÚT BẤM UI (UNITY INSPECTOR)
    // ==========================================

    // 1. Kéo hàm này vào Nút "Xem Ads nhận Ruby"
    public void Button_WatchAdForRuby()
    {
        ShowRewardVideo(RewardType.Ruby);
    }

    // 2. Kéo hàm này vào Nút "Xem Ads để Hồi sinh" (Last Chance)
    public void Button_WatchAdForRevive()
    {
        ShowRewardVideo(RewardType.Revive);
    }
    // 3. Kéo hàm này vào Nút "Xem Ads để nhận Music Note"
    public void Button_WatchAdForMusicNote()
    {
        ShowRewardVideo(RewardType.MusicNote);
    }
}