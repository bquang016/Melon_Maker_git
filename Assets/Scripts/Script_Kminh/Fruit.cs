using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("ID của quả (Từ 0 đến 10)")]
    public int fruitID;

    // Cờ chống lỗi gộp đúp
    public bool hasMerged = false;

    void Start()
    {
        // Vừa sinh ra là chạy ngay đi tìm "Chú Bảo Vệ" xin cái áo
        if (SkinManager.Instance != null)
        {
            Sprite aoMoi = SkinManager.Instance.GetSpriteForFruit(fruitID);
            if (aoMoi != null)
            {
                GetComponent<SpriteRenderer>().sprite = aoMoi;
            }
        }
    }

    // HÀM MỚI: Tự động chạy khi có va chạm vật lý xảy ra
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Kiểm tra Tag xem có đúng là chạm vào Trái Cây không
        if (collision.gameObject.CompareTag("Fruit"))
        {
            Fruit quaKia = collision.gameObject.GetComponent<Fruit>();

            // 2. ĐIỀU KIỆN GỘP: Cùng ID + Cả 2 chưa bị gộp + Chưa phải quả to nhất (ID < 10)
            if (quaKia != null && quaKia.fruitID == this.fruitID && !this.hasMerged && !quaKia.hasMerged && this.fruitID < 10)
            {
                // 3. Giải quyết "Lời nguyền sinh đôi"
                if (this.gameObject.GetInstanceID() > quaKia.gameObject.GetInstanceID())
                {
                    this.hasMerged = true;
                    quaKia.hasMerged = true;

                    int diemThuong = (int)Mathf.Pow(2, this.fruitID);
                    GameManager.Instance.CongDiem(diemThuong);

                    // 4. Tính toán vị trí sinh ra quả mới
                    Vector3 viTriMoi = (this.transform.position + quaKia.transform.position) / 2f;

                    // 5. Lấy Prefab quả cấp tiếp theo từ GameManager
                    GameObject prefabQuaMoi = GameManager.Instance.tatCaTraiCay[this.fruitID + 1];

                    // 6. Đẻ quả mới và tóm lấy nó
                    GameObject quaMoi = Instantiate(prefabQuaMoi, viTriMoi, Quaternion.identity);

                    // 7. HIỆU ỨNG NẢY (GAME JUICE)
                    Rigidbody2D rbQuaMoi = quaMoi.GetComponent<Rigidbody2D>();
                    if (rbQuaMoi != null)
                    {
                        // Lực nảy = 3 (bạn có thể tự sửa số này cho nảy to/nhỏ tùy thích)
                        float lucNay = 0.5f;
                        rbQuaMoi.AddForce(Vector2.up * rbQuaMoi.mass * lucNay, ForceMode2D.Impulse);
                    }

                    // 8. Tiêu diệt cả 2 xác cũ
                    Destroy(this.gameObject);
                    Destroy(quaKia.gameObject);
                }
            }
        }
    }
}