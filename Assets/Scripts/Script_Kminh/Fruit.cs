using UnityEngine;
using System.Collections;

public class Fruit : MonoBehaviour
{
    [Header("Cấu hình quả")]
    public int fruitID;
    public bool hasMerged = false;

    [Header("Hiệu ứng & Âm thanh")]
    // Kéo Prefab Animation vụ nổ (Sprite Sheet) vào đây
    public GameObject explosionPrefab; 
    public AudioClip mergeSound;    

    void Start()
    {
        if (SkinManager.Instance != null)
        {
            Sprite aoMoi = SkinManager.Instance.GetSpriteForFruit(fruitID);
            if (aoMoi != null) GetComponent<SpriteRenderer>().sprite = aoMoi;
        }
        // Hiệu ứng nảy khi xuất hiện
        StartCoroutine(HieuUngXuatHien());
    }

    private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Fruit"))
    {
        Fruit quaKia = collision.gameObject.GetComponent<Fruit>();

        // Kiểm tra điều kiện: Cùng loại, chưa merge, và không phải quả cuối (ID < 10)
        if (quaKia != null && quaKia.fruitID == this.fruitID && !this.hasMerged && !quaKia.hasMerged)
        {
            // Chỉ cho phép quả có ID instance lớn hơn thực hiện logic (tránh nhân đôi quả mới)
            if (this.gameObject.GetInstanceID() > quaKia.gameObject.GetInstanceID())
            {
                // Chặn merge lần nữa cho cả 2 quả cũ
                this.hasMerged = true;
                quaKia.hasMerged = true;

                // 1. Tính toán vị trí và ID quả mới
                Vector3 viTriMoi = (this.transform.position + quaKia.transform.position) / 2f;
                int nextFruitID = this.fruitID + 1;

                // Kiểm tra nếu vượt quá giới hạn mảng trái cây (Dưa hấu là kịch khung)
                if (nextFruitID >= GameManager.Instance.tatCaTraiCay.Length) return;

                // 2. TẠO HIỆU ỨNG VỤ NỔ
                if (explosionPrefab != null)
                {
                    GameObject hieuUng = Instantiate(explosionPrefab, viTriMoi, Quaternion.identity);
                    float tyLeHieuUng = this.transform.localScale.x * 2.5f;
                    hieuUng.transform.localScale = new Vector3(tyLeHieuUng, tyLeHieuUng, 1f);
                    Destroy(hieuUng, 1f); // Tự hủy hiệu ứng sau 1 giây cho sạch bộ nhớ
                }

                // 3. PHÁT ÂM THANH
                if (mergeSound != null)
                {
                    AudioSource.PlayClipAtPoint(mergeSound, Camera.main.transform.position);
                }

                // 4. CẬP NHẬT DỮ LIỆU & UI (PHẦN BẠN YÊU CẦU)
                int diemThuong = (int)Mathf.Pow(2, nextFruitID);
                GameManager.Instance.CongDiem(diemThuong);
                GameManager.Instance.KiemTraMoKhoa(nextFruitID);
                
                DataManager.Instance.AddMergedFruitCount(); // Tổng số quả merge
                DataManager.Instance.AddFruitMergeCount(nextFruitID); // Đếm riêng loại quả mới
                
                // LỆNH QUAN TRỌNG: Cập nhật lại bảng 6 quả trên UI ngay lập tức
                GameUIManager.Instance?.UpdateMergedFruitsDisplay();

                // 5. SINH QUẢ MỚI
                GameObject prefabQuaMoi = GameManager.Instance.tatCaTraiCay[nextFruitID];
                GameObject quaMoi = Instantiate(prefabQuaMoi, viTriMoi, Quaternion.identity);

                // Thêm lực đẩy nhẹ lên trên cho quả mới sinh ra
                Rigidbody2D rbQuaMoi = quaMoi.GetComponent<Rigidbody2D>();
                if (rbQuaMoi != null)
                {
                    rbQuaMoi.AddForce(Vector2.up * rbQuaMoi.mass * 0.5f, ForceMode2D.Impulse);
                }

                // 6. XÓA 2 QUẢ CŨ
                Destroy(this.gameObject);
                Destroy(quaKia.gameObject);
            }
        }
    }
}

    IEnumerator HieuUngXuatHien()
    {
        float thoiGian = 0.2f;
        float dem = 0;
        Vector3 targetScale = transform.localScale; // Lưu lại scale gốc của quả
        transform.localScale = Vector3.zero;

        while (dem < thoiGian)
        {
            dem += Time.deltaTime;
            // Nảy lên từ 0 đến 1.2 lần scale gốc, rồi co về scale gốc
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale * 1.2f, dem / thoiGian);
            yield return null;
        }
        transform.localScale = targetScale;
    }
}