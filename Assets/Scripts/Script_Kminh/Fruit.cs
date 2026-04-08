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

            if (quaKia != null && quaKia.fruitID == this.fruitID && !this.hasMerged && !quaKia.hasMerged && this.fruitID < 10)
            {
                if (this.gameObject.GetInstanceID() > quaKia.gameObject.GetInstanceID())
                {
                    this.hasMerged = true;
                    quaKia.hasMerged = true;

                    Vector3 viTriMoi = (this.transform.position + quaKia.transform.position) / 2f;

                    // 1. TẠO HIỆU ỨNG VỤ NỔ VỚI KÍCH CỠ LINH HOẠT
                    if (explosionPrefab != null)
                    {
                        Vector3 viTriHieuUng = new Vector3(viTriMoi.x, viTriMoi.y, 0f);
                        GameObject hieuUng = Instantiate(explosionPrefab, viTriHieuUng, Quaternion.identity);
                        
                        // LẤY KÍCH CỠ CỦA QUẢ HIỆN TẠI ĐỂ GÁN CHO HIỆU ỨNG
                        // Ta nhân thêm một hệ số (ví dụ 1.5f) để hiệu ứng to hơn quả một chút cho đẹp
                        float tyLeHieuUng = this.transform.localScale.x * 2.5f;
                        hieuUng.transform.localScale = new Vector3(tyLeHieuUng, tyLeHieuUng, 1f);

                        // Đảm bảo hiện lên trên cùng
                        SpriteRenderer srHieuUng = hieuUng.GetComponent<SpriteRenderer>();
                        if (srHieuUng != null) srHieuUng.sortingOrder = 100;
                    }

                    // 2. PHÁT ÂM THANH
                    if (mergeSound != null)
                    {
                        AudioSource.PlayClipAtPoint(mergeSound, Camera.main.transform.position);
                    }

                    // 3. LOGIC CỘNG ĐIỂM VÀ SINH QUẢ MỚI
                    int diemThuong = (int)Mathf.Pow(2, this.fruitID);
                    GameManager.Instance.CongDiem(diemThuong);
                    GameManager.Instance.KiemTraMoKhoa(this.fruitID + 1);

                    GameObject prefabQuaMoi = GameManager.Instance.tatCaTraiCay[this.fruitID + 1];
                    GameObject quaMoi = Instantiate(prefabQuaMoi, viTriMoi, Quaternion.identity);

                    Rigidbody2D rbQuaMoi = quaMoi.GetComponent<Rigidbody2D>();
                    if (rbQuaMoi != null)
                    {
                        rbQuaMoi.AddForce(Vector2.up * rbQuaMoi.mass * 0.5f, ForceMode2D.Impulse);
                    }

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