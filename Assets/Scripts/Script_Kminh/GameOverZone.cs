using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

public class GameOverZone : MonoBehaviour
{
    [Header("Thời gian cho phép trào ra (Giây)")]
    public float thoiGianChiuDung = 2f; // Cho phép trào 2 giây, sang giây thứ 3 là thua

    private float timer = 0f;
    private List<GameObject> traiCayTrongVung = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fruit") && !traiCayTrongVung.Contains(collision.gameObject))
        {
            traiCayTrongVung.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Fruit"))
        {
            traiCayTrongVung.Remove(collision.gameObject);
        }
    }

    private void Update()
    {
        // Dọn dẹp lỡ có quả nào bị xóa do vừa gộp thành quả khác
        traiCayTrongVung.RemoveAll(item => item == null);

        // Bắt đầu đếm ngược nếu có trái cây chạm vạch
        if (traiCayTrongVung.Count > 0)
        {
            timer += Time.deltaTime;
            if (timer >= thoiGianChiuDung)
            {
                GameManager.Instance.KichHoatGameOver();
            }
        }
        else
        {
            timer = 0f; // Trái cây rơi xuống thoát khỏi vạch thì reset đồng hồ
        }
    }
}
