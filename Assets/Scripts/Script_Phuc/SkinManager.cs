using UnityEngine;

public class SkinManager : MonoBehaviour
{
    // Singleton giúp Dev 1 gọi biến này ở bất cứ đâu
    public static SkinManager Instance;

    [Header("Bỏ file Skin Pack mà user đang chọn vào đây")]
    public SkinPackData currentSkinPack;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Dev 1 sẽ gọi hàm này để lấy đúng cái ảnh gắn vào quả rơi xuống
    public Sprite GetSpriteForFruit(int fruitID)
    {
        if (currentSkinPack == null || currentSkinPack.fruitSprites.Length <= fruitID)
        {
            Debug.LogError("Lỗi: Chưa set Skin Pack hoặc mảng hình ảnh bị thiếu!");
            return null;
        }

        // Trả về đúng hình ảnh của trái cây đó
        return currentSkinPack.fruitSprites[fruitID];
    }
}