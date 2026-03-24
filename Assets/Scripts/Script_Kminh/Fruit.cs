using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("ID của quả (Từ 0 đến 10)")]
    public int fruitID;
    public bool hasMerged = false;

    void Start()
    {
        // Nhờ SkinManager mặc áo
        if (SkinManager.Instance != null)
        {
            Sprite aoMoi = SkinManager.Instance.GetSpriteForFruit(fruitID);
            if (aoMoi != null) GetComponent<SpriteRenderer>().sprite = aoMoi;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fruit"))
        {
            Fruit quaKia = collision.gameObject.GetComponent<Fruit>();

            // Nếu cùng ID, chưa gộp và chưa phải Dưa hấu
            if (quaKia != null && quaKia.fruitID == this.fruitID && !this.hasMerged && !quaKia.hasMerged && this.fruitID < 10)
            {
                if (this.gameObject.GetInstanceID() > quaKia.gameObject.GetInstanceID())
                {
                    this.hasMerged = true;
                    quaKia.hasMerged = true;

                    // Tính điểm (1, 2, 4, 8, 16...)
                    int diemThuong = (int)Mathf.Pow(2, this.fruitID);
                    GameManager.Instance.CongDiem(diemThuong);

                    // Đẻ quả mới
                    Vector3 viTriMoi = (this.transform.position + quaKia.transform.position) / 2f;
                    GameObject prefabQuaMoi = GameManager.Instance.tatCaTraiCay[this.fruitID + 1];
                    GameObject quaMoi = Instantiate(prefabQuaMoi, viTriMoi, Quaternion.identity);

                    // Hiệu ứng nảy nhẹ (Game Juice)
                    Rigidbody2D rbQuaMoi = quaMoi.GetComponent<Rigidbody2D>();
                    if (rbQuaMoi != null)
                    {
                        float lucNay = 0.5f;
                        rbQuaMoi.AddForce(Vector2.up * rbQuaMoi.mass * lucNay, ForceMode2D.Impulse);
                    }

                    // BÁO CÁO LÊN KẾ TOÁN ĐỂ MỞ KHÓA QUẢ MỚI
                    GameManager.Instance.KiemTraMoKhoa(this.fruitID + 1);

                    // Tiêu diệt xác cũ
                    Destroy(this.gameObject);
                    Destroy(quaKia.gameObject);
                }
            }
        }
    }
}