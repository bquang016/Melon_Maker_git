using UnityEngine;

public class MenuSliding : MonoBehaviour
{
    public RectTransform menuBar; // Kéo Panel_MenuBar vào đây
    public float speed = 10f;

    public Vector2 hiddenPosition;  // Vị trí khi ẩn
    public Vector2 visiblePosition; // Vị trí khi hiện

    private bool isOpen = false;
    private Vector2 targetPosition;

    void Start()
    {
        // Mặc định lúc đầu là ẩn
        menuBar.anchoredPosition = hiddenPosition;
        targetPosition = hiddenPosition;
    }

    void Update()
    {
        // Di chuyển mượt mà đến vị trí đích
        menuBar.anchoredPosition = Vector2.Lerp(menuBar.anchoredPosition, targetPosition, Time.deltaTime * speed);
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        targetPosition = isOpen ? visiblePosition : hiddenPosition;

        // Thêm tiếng động nếu muốn
        // SoundManager.Instance.PlaySfx("Button_Click");
    }
}