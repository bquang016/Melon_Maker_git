using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FruitRecordItem : MonoBehaviour
{
    public Image fruitIcon;
    public TextMeshProUGUI countText;

    public void Setup(int fruitID, int count)
    {
        // Lấy hình ảnh từ SkinManager dựa trên ID quả
        if (SkinManager.Instance != null)
        {
            fruitIcon.sprite = SkinManager.Instance.GetSpriteForFruit(fruitID);
        }
        
        // Hiển thị số lượng đã merge
        countText.text = count.ToString();
    }
}