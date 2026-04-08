using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Tạo biến Singleton để có thể gọi AudioManager từ bất kỳ script/scene nào khác
    public static AudioManager Instance;

    [Header("Audio Settings")]
    public AudioSource bgmSource;
    public AudioClip[] bgmTracks; // Mảng chứa danh sách các bài nhạc nền trong game

    private void Awake()
    {
        // Kiểm tra xem đã có AudioManager nào tồn tại chưa
        if (Instance == null)
        {
            Instance = this;
            // Quan trọng: Giữ cho GameObject này không bị huỷ khi load Scene mới
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu chuyển scene mà đã có AudioManager từ scene trước đó, thì xoá bản sao mới đi để tránh phát 2 bài cùng lúc
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Khi bắt đầu game, phát bài nhạc mặc định (track thứ 0)
        PlayMusic(0);
    }

    // Hàm gọi để phát hoặc đổi nhạc nền
    public void PlayMusic(int trackIndex)
    {
        if (trackIndex >= 0 && trackIndex < bgmTracks.Length)
        {
            // Nếu đang phát đúng bài nhạc này rồi thì không cần load lại
            if (bgmSource.clip == bgmTracks[trackIndex]) return;

            bgmSource.Stop();
            bgmSource.clip = bgmTracks[trackIndex];
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy bài nhạc ở index: " + trackIndex);
        }
    }

    // Hàm gọi để thay đổi âm lượng (Dùng cho thanh trượt Slider trong Cài đặt)
    public void SetVolume(float volume)
    {
        bgmSource.volume = volume;
    }
}