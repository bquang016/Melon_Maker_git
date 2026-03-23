using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Kho chứa TOÀN BỘ 11 Prefab Trái Cây")]
    public GameObject[] tatCaTraiCay;

    [Header("Hệ thống Điểm số")]
    public int diemHienTai = 0;
    public TextMeshProUGUI textDiemSo;

    [Header("Hệ thống Game Over")]
    public bool isGameOver = false;
    public GameObject panelGameOver; // Kéo dòng chữ Game Over vào đây

    private void Awake()
    {
        if (Instance == null) Instance = this;
        Time.timeScale = 1f; // Đảm bảo game chạy bình thường lúc mới bật
    }

    public void CongDiem(int diemCongThem)
    {
        if (isGameOver) return; // Thua rồi thì nghỉ tính điểm
        diemHienTai += diemCongThem;
        if (textDiemSo != null) textDiemSo.text = diemHienTai.ToString();
    }

    // HÀM MỚI: Gọi khi bị tràn trái cây
    public void KichHoatGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Hiện bảng Game Over lên
        if (panelGameOver != null) panelGameOver.SetActive(true);

        // Đóng băng toàn bộ thời gian trong game (Trái cây lơ lửng dừng rơi luôn)
        Time.timeScale = 0f;
    }
    [Header("Hệ thống Game Over & Revive")]
    public bool isGameOver = false;
    public GameObject panelLastChance; // Thay vì panelGameOver, giờ đổi thành panel Last Chance
    public GameObject panelGameOverThat; // Panel thua thật sự (nếu bấm No Thanks)

    // ... (Giữ nguyên hàm Awake và CongDiem)

    // 1. SỬA LẠI HÀM NÀY: Gọi bảng Last Chance thay vì thua luôn
    public void KichHoatGameOver()
    {
        if (isGameOver) return;
        isGameOver = true; // Tạm thời đánh dấu là thua để ngừng rơi/cộng điểm

        // Hiện bảng Last Chance (Để Dev khác lo làm UI cho cái này)
        if (panelLastChance != null) panelLastChance.SetActive(true);

        Time.timeScale = 0f; // Dừng thời gian
    }

    // 2. THÊM HÀM NÀY: Dành cho nút "Revive (Ads)" để Dev khác gọi vào
    public void HoiSinh_Revive()
    {
        // Tắt bảng Last Chance
        if (panelLastChance != null) panelLastChance.SetActive(false);

        // TODO: Xóa bớt 1-2 quả to nhất ở gần miệng hộp (Cơ chế cây búa - Phần này Dev 1 sẽ làm chi tiết sau)
        // Hướng giải quyết: Tìm tất cả các quả có Tag "Fruit", tìm quả có tọa độ Y cao nhất và Destroy nó.

        isGameOver = false; // Xóa án tử
        Time.timeScale = 1f; // Cho thời gian chạy lại bình thường
    }

    // 3. THÊM HÀM NÀY: Dành cho nút "No Thanks"
    public void TuChoiHoiSinh_ThuaLuon()
    {
        // Tắt bảng Last Chance
        if (panelLastChance != null) panelLastChance.SetActive(false);

        // Bật bảng Game Over thật (Có nút Chơi Lại / Về Trang Chủ)
        if (panelGameOverThat != null) panelGameOverThat.SetActive(true);
    }
}
