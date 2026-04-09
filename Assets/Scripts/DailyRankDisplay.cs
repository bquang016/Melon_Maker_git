using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DailyRankDisplay : MonoBehaviour 
{
    [Header("--- Cấu hình UI ---")]
    public GameObject itemPrefab; // Kéo cái mẫu dòng điểm (Prefab) vào đây
    public Transform content;     // Kéo cái Content của ScrollView vào đây

    [Header("--- Tùy chọn ---")]
    public bool refreshOnEnable = true;

    private void OnEnable()
    {
        if (refreshOnEnable)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        Debug.Log("<color=white><b>[DailyRankDisplay]</b> Đang làm mới danh sách điểm ngày...</color>");

        if (itemPrefab == null || content == null)
        {
            Debug.LogError("[DailyRankDisplay] LỖI: Bạn chưa kéo thả Prefab hoặc Content vào Inspector!");
            return;
        }

        // 1. Xóa toàn bộ các dòng cũ để tránh bị lặp
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 2. Lấy dữ liệu từ "Két sắt" DataManager
        if (DataManager.Instance == null || DataManager.Instance.currentSaveData == null)
        {
            Debug.LogError("[DailyRankDisplay] LỖI: Không tìm thấy DataManager hoặc Dữ liệu save!");
            return;
        }

        var records = DataManager.Instance.currentSaveData.dailyScoreRecords;

        if (records == null || records.Count == 0)
        {
            Debug.LogWarning("[DailyRankDisplay] Hiện tại chưa có bản ghi điểm nào trong máy.");
            return;
        }

        Debug.Log($"[DailyRankDisplay] Tìm thấy {records.Count} bản ghi. Đang vẽ ra màn hình...");

        // 3. Vòng lặp tạo ra các dòng điểm
        for (int i = 0; i < records.Count; i++)
        {
            GameObject go = Instantiate(itemPrefab, content);
            
            // Đảm bảo Item vừa tạo ra có tỷ lệ chuẩn (tránh lỗi tàng hình)
            go.transform.localScale = Vector3.one;

            // Lấy script Setup trên Item để nạp dữ liệu
            DailyRankItem itemScript = go.GetComponent<DailyRankItem>();
            if (itemScript != null)
            {
                itemScript.Setup(i + 1, records[i].date, records[i].score);
            }
            else
            {
                Debug.LogError("[DailyRankDisplay] LỖI: Cái Prefab bạn kéo vào không có script 'DailyRankItem'!");
            }
        }

        // 4. Ép Layout cập nhật lại (Quan trọng để nó hiện ra ngay)
        Canvas.ForceUpdateCanvases();
        if (content.TryGetComponent<VerticalLayoutGroup>(out var layout))
        {
            layout.enabled = false;
            layout.enabled = true;
        }
    }
}
