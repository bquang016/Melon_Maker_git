using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecordItemUI : MonoBehaviour
{
    [Header("Cấu hình UI")]
    public Image fruitIcon; // Kéo Image hiển thị hình quả vào đây
    public TextMeshProUGUI countText; // Kéo Text (TMP) hiển thị số lượng vào đây

    /// <summary>
    /// Cập nhật dữ liệu hiển thị cho dòng này
    /// </summary>
    public void SetData(Sprite icon, int count)
    {
        if (fruitIcon != null) fruitIcon.sprite = icon;
        if (countText != null) countText.text = count.ToString();
    }
}