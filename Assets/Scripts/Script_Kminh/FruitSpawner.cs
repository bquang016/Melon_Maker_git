using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Kho chứa 5 quả đầu (từ 0 đến 4)")]
    public GameObject[] fruitPrefabs;

    [Header("Giới hạn di chuyển đám mây")]
    public float gioiHanTrai = -1.2f;
    public float gioiHanPhai = 1.2f;

    [Header("Thời gian chờ giữa 2 lần thả (Giây)")]
    public float thoiGianCho = 1f;
    private float thoiGianDemNguoc = 0f;

    void Update()
    {
        // 1. Đồng hồ đếm ngược chờ thả
        if (thoiGianDemNguoc > 0)
        {
            thoiGianDemNguoc -= Time.deltaTime;
        }

        // 2. Di chuyển mây giới hạn trong hộp
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = 10f;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(screenPos);

        float toaDoX = Mathf.Clamp(mousePos.x, gioiHanTrai, gioiHanPhai);
        transform.position = new Vector3(toaDoX, transform.position.y, 0);

        // 3. THẢ QUẢ (LOGIC MỚI: Dưới kỷ lục 1 bậc)
        if (Input.GetMouseButtonDown(0) && thoiGianDemNguoc <= 0)
        {
            int kyLucHienTai = GameManager.Instance.maxUnlockedFruitID;

            // XÁC ĐỊNH MỨC QUẢ CAO NHẤT ĐƯỢC PHÉP THẢ:
            int maxDroppable = 0;
            if (kyLucHienTai > 0)
            {
                // Nếu đã có kỷ lục (từ quả 1 trở lên), chỉ cho thả tối đa là quả dưới đó 1 bậc
                maxDroppable = kyLucHienTai - 1;
            }

            // Tính giới hạn bốc thăm (Đảm bảo không vượt quá sức chứa của Đám Mây - thường là 5 quả)
            int gioiHanBocTham = Mathf.Min(maxDroppable + 1, fruitPrefabs.Length);

            // Hàm Random.Range(0, n) đối với số nguyên sẽ chỉ lấy từ 0 đến n-1
            int randomIndex = Random.Range(0, gioiHanBocTham);
            Instantiate(fruitPrefabs[randomIndex], transform.position, Quaternion.identity);

            // Bắt người chơi đợi lượt thả tiếp theo
            thoiGianDemNguoc = thoiGianCho;
        }
    }
}