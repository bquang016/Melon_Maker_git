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

    [Header("Hệ thống Game Over & Revive")]
    public bool isGameOver = false;
    public GameObject panelLastChance;
    public GameObject panelGameOverThat;

    [Header("Hệ thống Mở Khóa (Progression)")]
    public int maxUnlockedFruitID = 0; // Bắt đầu game, người chơi chỉ được thả quả ID 0 (Việt quất)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        Time.timeScale = 1f;
    }

    public void CongDiem(int diemCongThem)
    {
        if (isGameOver) return;
        diemHienTai += diemCongThem;
        if (textDiemSo != null) textDiemSo.text = diemHienTai.ToString();
    }

    public void KichHoatGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (panelLastChance != null) panelLastChance.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HoiSinh_Revive()
    {
        if (panelLastChance != null) panelLastChance.SetActive(false);
        // TODO: Cây búa đập quả cao nhất sẽ làm sau
        isGameOver = false;
        Time.timeScale = 1f;
    }

    public void TuChoiHoiSinh_ThuaLuon()
    {
        if (panelLastChance != null) panelLastChance.SetActive(false);
        if (panelGameOverThat != null) panelGameOverThat.SetActive(true);
    }

    // HÀM MỚI: Cập nhật kỷ lục khi có quả mới được gộp thành công
    public void KiemTraMoKhoa(int idQuaMoi)
    {
        if (idQuaMoi > maxUnlockedFruitID)
        {
            maxUnlockedFruitID = idQuaMoi;
            Debug.Log("MỞ KHÓA THÀNH CÔNG QUẢ MỚI: ID " + idQuaMoi);
        }
    }
}