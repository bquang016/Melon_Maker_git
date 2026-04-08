using UnityEngine;
using TMPro;

// Gắn script này thẳng vào các vật thể TextMeshPro (Hiển thị tiền) trên Canvas
public class CurrencyDisplay : MonoBehaviour
{
    [Header("Tick chọn nếu hiển thị Ruby, Bỏ tick nếu hiển thị Nốt Nhạc")]
    public bool isRuby = true;

    private TextMeshProUGUI textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        // Liên tục kiểm tra Két sắt và cập nhật con số lên màn hình
        if (DataManager.Instance != null && textMesh != null)
        {
            if (isRuby)
            {
                textMesh.text = DataManager.Instance.currentSaveData.metaData.currentRuby.ToString();
            }
            else
            {
                textMesh.text = DataManager.Instance.currentSaveData.metaData.currentMusicNote.ToString();
            }
        }
    }
}