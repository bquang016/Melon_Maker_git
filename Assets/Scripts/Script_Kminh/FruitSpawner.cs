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

    private int nextFruitIndex;
    private GameObject currentFruitObject;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        int firstFruitIndex = GetRandomFruitIndex();
        nextFruitIndex = GetRandomFruitIndex();
        SpawnCurrentFruit(firstFruitIndex);
        UpdateNextFruitUI();
    }

    void Update()
    {
        if (thoiGianDemNguoc > 0) thoiGianDemNguoc -= Time.deltaTime;

        Vector3 screenPos = Input.mousePosition;
        if (!float.IsInfinity(screenPos.x) && !float.IsInfinity(screenPos.y) && mainCam != null)
        {
            screenPos.z = 10f;
            Vector3 mousePos = mainCam.ScreenToWorldPoint(screenPos);
            float toaDoX = Mathf.Clamp(mousePos.x, gioiHanTrai, gioiHanPhai);
            transform.position = new Vector3(toaDoX, transform.position.y, 0);
        }

        // THẢ QUẢ - Chỉ để lại 1 block IF duy nhất
        if (Input.GetMouseButtonDown(0) && thoiGianDemNguoc <= 0 && currentFruitObject != null && !GameManager.Instance.isUsingBooster)
        {
            DropFruit();
        }
    }

    private void SpawnCurrentFruit(int index)
    {
        currentFruitObject = Instantiate(fruitPrefabs[index], spawnPoint.position, Quaternion.identity);
        currentFruitObject.transform.SetParent(spawnPoint);
        Rigidbody2D rb = currentFruitObject.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
    }

    private void DropFruit()
    {
        currentFruitObject.transform.SetParent(null);

        Rigidbody2D rb = currentFruitObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;

            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        currentFruitObject = null;
        thoiGianDemNguoc = thoiGianCho;

        Invoke(nameof(PrepareNextTurn), 0.5f);
    }

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
        int maxDroppable = (kyLucHienTai > 0) ? kyLucHienTai - 1 : 0;
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