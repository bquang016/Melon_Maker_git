using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Kho chứa 5 quả đầu (từ 0 đến 4)")]
    public GameObject[] fruitPrefabs;

    [Header("Giới hạn di chuyển đám mây")]
    public float gioiHanTrai = -1.2f;
    public float gioiHanPhai = 1.2f;

    [Header("Thời gian chờ giữa 2 lần thả (Giây)")]
    public float thoiGianCho = 1f;
    private float thoiGianDemNguoc = 0f;

    [Header("--- Vị Trí Hiển Thị Quả Đang Cầm ---")]
    public Transform spawnPoint; // Nơi quả lơ lửng (nên nằm hơi thấp hơn Đám Mây một chút)

    // Quản lý luồng quả
    private int nextFruitIndex;
    private GameObject currentFruitObject; // Biến lưu giữ "thực thể" quả đang được cầm trên tay

    void Start()
    {
        // 1. Khởi tạo 2 quả (hiện tại và tiếp theo)
        int firstFruitIndex = GetRandomFruitIndex();
        nextFruitIndex = GetRandomFruitIndex();

        // 2. Tạo quả cầm trên tay
        SpawnCurrentFruit(firstFruitIndex);

        // 3. Gửi quả tiếp theo lên UI
        UpdateNextFruitUI();
    }

    void Update()
    {
        // Đếm ngược thời gian chờ
        if (thoiGianDemNguoc > 0)
        {
            thoiGianDemNguoc -= Time.deltaTime;
        }

        // Di chuyển đám mây theo chuột (Chỉ di chuyển khi đang có quả)
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = 10f;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(screenPos);

        float toaDoX = Mathf.Clamp(mousePos.x, gioiHanTrai, gioiHanPhai);
        transform.position = new Vector3(toaDoX, transform.position.y, 0);

        // THẢ QUẢ
        // Điều kiện: Click chuột + Đã hết thời gian chờ + ĐANG CÓ QUẢ TRÊN TAY
        if (Input.GetMouseButtonDown(0) && thoiGianDemNguoc <= 0 && currentFruitObject != null)
        {
            DropFruit();
        }
    }

    /// <summary>
    /// Tạo quả lơ lửng dưới đám mây
    /// </summary>
    private void SpawnCurrentFruit(int index)
    {
        // Tạo quả và set nó làm con của spawnPoint để nó di chuyển theo Đám Mây
        currentFruitObject = Instantiate(fruitPrefabs[index], spawnPoint.position, Quaternion.identity);
        currentFruitObject.transform.SetParent(spawnPoint);

        // TẮT MÔ PHỎNG VẬT LÝ để quả không rớt và không va chạm bậy bạ
        Rigidbody2D rb = currentFruitObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
        }
    }

    /// <summary>
    /// Thực hiện hành động thả
    /// </summary>
    private void DropFruit()
    {
        // 1. Tháo quả ra khỏi mây (tách parent) để nó đứng lại tại chỗ đó
        currentFruitObject.transform.SetParent(null);

        // 2. BẬT LẠI VẬT LÝ để trọng lực kéo nó rơi xuống
        Rigidbody2D rb = currentFruitObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
        }

        // 3. Quên quả này đi (vì nó đã rơi xuống giỏ)
        currentFruitObject = null;

        // 4. Bắt đầu thời gian đếm ngược
        thoiGianDemNguoc = thoiGianCho;

        // 5. MẸO CỦA SENIOR: Trong game xịn, quả tiếp theo không hiện ra ngay lập tức.
        // Dùng Invoke để delay nửa giây (0.5s) rồi mới tạo quả mới trên tay, nhìn sẽ mượt hơn rất nhiều!
        Invoke(nameof(PrepareNextTurn), 0.5f);
    }

    /// <summary>
    /// Hàm gọi sau khi thả quả xong để nạp đạn mới
    /// </summary>
    private void PrepareNextTurn()
    {
        // Quả Next biến thành quả đang cầm
        SpawnCurrentFruit(nextFruitIndex);

        // Random ra quả Next mới tinh
        nextFruitIndex = GetRandomFruitIndex();

        // Cập nhật lại UI Next Fruit
        UpdateNextFruitUI();
    }

    // (Giữ nguyên logic random cũ)
    private int GetRandomFruitIndex()
    {
        if (GameManager.Instance == null) return 0;
        int kyLucHienTai = GameManager.Instance.maxUnlockedFruitID;
        int maxDroppable = 0;
        if (kyLucHienTai > 0) maxDroppable = kyLucHienTai - 1;

        int gioiHanBocTham = Mathf.Min(maxDroppable + 1, fruitPrefabs.Length);
        return Random.Range(0, gioiHanBocTham);
    }

    // (Giữ nguyên logic gọi UI cũ)
    private void UpdateNextFruitUI()
    {
        SpriteRenderer sr = fruitPrefabs[nextFruitIndex].GetComponent<SpriteRenderer>();
        if (sr != null && GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateNextFruit(sr.sprite);
        }
    }
}