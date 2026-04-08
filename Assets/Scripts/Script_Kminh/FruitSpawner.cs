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
    public Transform spawnPoint;

    // Quản lý luồng quả
    private int nextFruitIndex;
    private GameObject currentFruitObject;

    // TỐI ƯU HÓA: Cất Camera vào biến để không phải tìm kiếm mỗi frame
    private Camera mainCam;

    void Start()
    {
        // Cache camera lại cho game mượt
        mainCam = Camera.main;

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

        // --- LỚP GIÁP CHỐNG LỖI INFINITY ---
        Vector3 screenPos = Input.mousePosition;

        // Nếu chuột văng ra khỏi màn hình hoặc bị giá trị Vô Cực -> Bỏ qua không xử lý di chuyển
        if (!float.IsInfinity(screenPos.x) && !float.IsInfinity(screenPos.y) && mainCam != null)
        {
            screenPos.z = 10f;
            Vector3 mousePos = mainCam.ScreenToWorldPoint(screenPos);

            float toaDoX = Mathf.Clamp(mousePos.x, gioiHanTrai, gioiHanPhai);
            transform.position = new Vector3(toaDoX, transform.position.y, 0);
        }

        // THẢ QUẢ
        // Điều kiện: Click chuột + Đã hết thời gian chờ + ĐANG CÓ QUẢ TRÊN TAY
        if (Input.GetMouseButtonDown(0) && thoiGianDemNguoc <= 0 && currentFruitObject != null)
        {
            // Tránh thả quả khi click ra ngoài màn hình UI (nếu có)
            DropFruit();
        }
    }

    /// <summary>
    /// Tạo quả lơ lửng dưới đám mây
    /// </summary>
    private void SpawnCurrentFruit(int index)
    {
        currentFruitObject = Instantiate(fruitPrefabs[index], spawnPoint.position, Quaternion.identity);
        currentFruitObject.transform.SetParent(spawnPoint);

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
        currentFruitObject.transform.SetParent(null);

        Rigidbody2D rb = currentFruitObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
        }

        currentFruitObject = null;
        thoiGianDemNguoc = thoiGianCho;

        Invoke(nameof(PrepareNextTurn), 0.5f);
    }

    /// <summary>
    /// Hàm gọi sau khi thả quả xong để nạp đạn mới
    /// </summary>
    private void PrepareNextTurn()
    {
        SpawnCurrentFruit(nextFruitIndex);
        nextFruitIndex = GetRandomFruitIndex();
        UpdateNextFruitUI();
    }

    private int GetRandomFruitIndex()
    {
        if (GameManager.Instance == null) return 0;
        int kyLucHienTai = GameManager.Instance.maxUnlockedFruitID;
        int maxDroppable = 0;
        if (kyLucHienTai > 0) maxDroppable = kyLucHienTai - 1;

        int gioiHanBocTham = Mathf.Min(maxDroppable + 1, fruitPrefabs.Length);
        return Random.Range(0, gioiHanBocTham);
    }

    private void UpdateNextFruitUI()
    {
        SpriteRenderer sr = fruitPrefabs[nextFruitIndex].GetComponent<SpriteRenderer>();
        if (sr != null && GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateNextFruit(sr.sprite);
        }
    }
}