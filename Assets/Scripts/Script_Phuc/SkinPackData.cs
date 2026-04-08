using UnityEngine;

[CreateAssetMenu(fileName = "New Skin Pack", menuName = "Suika Game/Skin Pack")]
public class SkinPackData : ScriptableObject
{
    [Header("Thông tin Gói Skin")]
    public string packID;
    public string packName;
    public bool requiresAd;        // BẬT cái này lên nếu muốn người chơi phải xem Ads để mở
    public bool isUnlocked;        // Vẫn giữ cái này cho bộ Mặc định

    [Header("Danh sách Hình ảnh")]
    public Sprite[] fruitSprites;
}