using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Kho chứa 5 quả đầu (từ 0 đến 4)")]
    public GameObject[] fruitPrefabs;

    [Header("Giới hạn di chuyển đám mây (Thu hẹp số này lại)")]
    public float gioiHanTrai = -1.5f;
    public float gioiHanPhai = 1.5f;

    [Header("Thời gian chờ giữa 2 lần thả (Giây)")]
    public float thoiGianCho = 1f; // 1 giây mới được thả quả tiếp

    // Biến ngầm để đồng hồ đếm ngược hoạt động
    private float thoiGianDemNguoc = 0f;

    void Update()
    {
        // 1. HỆ THỐNG ĐỒNG HỒ ĐẾM NGƯỢC
        if (thoiGianDemNguoc > 0)
        {
            thoiGianDemNguoc -= Time.deltaTime; // Trừ dần thời gian đi
        }

        // 2. DI CHUYỂN ĐÁM MÂY (Vẫn giữ như cũ)
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = 10f;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(screenPos);

        // Ép tọa độ X không được vượt quá 2 viền giới hạn
        float toaDoX = Mathf.Clamp(mousePos.x, gioiHanTrai, gioiHanPhai);
        transform.position = new Vector3(toaDoX, transform.position.y, 0);

        // 3. THẢ QUẢ (Đã thêm điều kiện: Chỉ thả khi đồng hồ đã đếm về <= 0)
        if (Input.GetMouseButtonDown(0) && thoiGianDemNguoc <= 0)
        {
            int randomIndex = Random.Range(0, fruitPrefabs.Length);
            Instantiate(fruitPrefabs[randomIndex], transform.position, Quaternion.identity);

            // Thả xong thì reset lại đồng hồ (Bắt đầu bắt người chơi đợi 1 giây)
            thoiGianDemNguoc = thoiGianCho;
        }
    }
}