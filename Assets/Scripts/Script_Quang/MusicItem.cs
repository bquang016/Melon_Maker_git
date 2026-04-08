using UnityEngine;
using TMPro;

public class MusicItem : MonoBehaviour
{
    [Header("Thông tin Bài hát")]
    public string songName;
    public int trackIndex; // SỐ THỨ TỰ của bài hát trong AudioManager (0, 1, 2...)
    public bool isUnlocked;
    public int price;

    [Header("UI Elements")]
    public TextMeshProUGUI textSongName;
    public GameObject iconMusic;
    public GameObject iconLock;
    public GameObject iconCheckmark;
    public GameObject imageDarkOverlay;

    private MusicSelectionManager manager;

    void Start()
    {
        if (textSongName != null) textSongName.text = songName;
    }

    public void SetupItem(MusicSelectionManager mgr)
    {
        manager = mgr;
    }

    public void UpdateUIState(bool isBeingPreviewed, bool isCurrentlySelectedMusic)
    {
        // 0. Reset lại các icon
        iconMusic.SetActive(false);
        iconLock.SetActive(false);
        iconCheckmark.SetActive(false);

        // 1. XỬ LÝ ICON: Mở khóa thì hiện nốt nhạc, Khóa thì hiện ổ khóa
        if (isUnlocked)
        {
            iconMusic.SetActive(true);
        }
        else
        {
            iconLock.SetActive(true);
        }

        // 2. XỬ LÝ TICK XANH: Chỉ hiện nếu bài này đang được cài làm nhạc nền
        if (isCurrentlySelectedMusic)
        {
            iconCheckmark.SetActive(true);
        }

        // 3. XỬ LÝ LỚP PHỦ MỜ (Logic mới của bạn)
        if (isBeingPreviewed)
        {
            // Nếu ĐANG ĐƯỢC CLICK (nghe thử/chọn) -> Tắt lớp mờ để thanh này SÁNG lên
            imageDarkOverlay.SetActive(false);
        }
        else
        {
            // TẤT CẢ các thanh KHÔNG ĐƯỢC CLICK -> Bật lớp mờ để TỐI đi
            imageDarkOverlay.SetActive(true);
        }
    }

    public void OnClickItem()
    {
        // 1. Dòng này để kiểm tra xem Nút có gọi được Code không
        Debug.Log("👉 Đã bấm vào bài: " + songName);

        // 2. Kiểm tra xem có bị mất liên lạc với "Trưởng phòng" không
        if (manager == null)
        {
            Debug.LogError("❌ LỖI: Bài hát này chưa được Manager nhận diện!");
            return;
        }

        manager.OnItemClicked(this);
    }
}