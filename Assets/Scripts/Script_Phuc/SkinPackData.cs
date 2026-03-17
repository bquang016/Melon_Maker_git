using UnityEngine;

[CreateAssetMenu(fileName = "New Skin Pack", menuName = "Suika Game/Skin Pack")]
public class SkinPackData : ScriptableObject
{
    [Header("Thông tin Gói Skin")]
    public string packID;          // Mã ID (Ví dụ: "default", "animal", "planet")
    public string packName;        // Tên hiển thị trong Shop (Ví dụ: "Bộ Gốc")
    public int price;              // Giá tiền để mua gói này
    public bool isUnlocked;        // Gói này có được mở sẵn từ đầu không?

    [Header("Danh sách Hình ảnh (Kéo 11 ảnh từ nhỏ đến lớn vào đây)")]
    // Mảng này sẽ chứa 11 hình ảnh tương ứng với 11 trái cây của bộ skin này
    public Sprite[] fruitSprites;
}