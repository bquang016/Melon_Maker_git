using UnityEngine;

public class ShopTester : MonoBehaviour
{
    // Kéo file Skin_Animal vào ô này trên Inspector để test mua thử nó
    public SkinPackData packToTest;

    void Update()
    {
        // Bấm phím B để giả lập hành động user bấm nút Mua
        if (Input.GetKeyDown(KeyCode.B))
        {
            ShopManager.Instance.BuySkin(packToTest);
        }

        // Bấm phím M để ăn gian thêm 500 tiền
        if (Input.GetKeyDown(KeyCode.M))
        {
            ShopManager.Instance.AddMoney(500);
            Debug.Log("Tiền hiện tại: " + ShopManager.Instance.GetCurrentMoney());
        }

        // Bấm phím R để Reset toàn bộ dữ liệu (Xóa hết tiền và skin đã mua)
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Đã xóa sạch Data test!");
        }
        // Bấm phím P để giả lập Game Over với số điểm ngẫu nhiên từ 100 đến 3000
        if (Input.GetKeyDown(KeyCode.P))
        {
            int randomScore = Random.Range(100, 3000);
            LeaderboardManager.Instance.SubmitScore(randomScore);
        }
    }

}