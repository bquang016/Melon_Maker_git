using UnityEngine;
using UnityEngine.UI;

public class UITabController : MonoBehaviour
{
    [Header("--- TAB BUTTONS ---")]
    public Button btnTab1;
    public Button btnTab2;

    [Header("--- TAB CONTENTS (SCROLL VIEWS) ---")]
    public GameObject viewTab1;
    public GameObject viewTab2;

    [Header("--- TAB COLORS ---")]
    public Color activeColor = new Color(1f, 0.8f, 0f, 1f);   // Màu Vàng (Đang chọn)
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.8f, 1f); // Màu Xanh/Xám (Không chọn)

    void Start()
    {
        // Gắn sự kiện tự động khi game bắt đầu (Không cần kéo thả trong Inspector nữa cho nhàn)
        btnTab1.onClick.AddListener(ShowTab1);
        btnTab2.onClick.AddListener(ShowTab2);

        // Mặc định hiển thị Tab 1 lúc mới mở
        ShowTab1();
    }

    public void ShowTab1()
    {
        // 1. Bật ruột 1, tắt ruột 2
        viewTab1.SetActive(true);
        viewTab2.SetActive(false);

        // 2. Đổi màu nút
        btnTab1.GetComponent<Image>().color = activeColor;
        btnTab2.GetComponent<Image>().color = inactiveColor;
    }

    public void ShowTab2()
    {
        // 1. Bật ruột 2, tắt ruột 1
        viewTab1.SetActive(false);
        viewTab2.SetActive(true);

        // 2. Đổi màu nút
        btnTab1.GetComponent<Image>().color = inactiveColor;
        btnTab2.GetComponent<Image>().color = activeColor;
    }
}