using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems; // Để kiểm tra click UI

public class Quang_BoosterManager : MonoBehaviour
{
    public static Quang_BoosterManager Instance { get; private set; }

    [Header("UI Cinematic Bars")]
    public RectTransform topBar;
    public RectTransform bottomBar;
    public float barHeight = 250f;
    public float animationTime = 0.3f;

    [Header("Cài đặt Búa")]
    public GameObject hammerPrefab; // Kéo Prefab Búa mới vào đây
    public GameObject explosionPrefab;
    public AudioClip smashSound;

    [Header("Cài đặt vị trí búa")]
    public Vector3 hammerOffset = new Vector3(0, 1.2f, 0); // Mặc định dịch lên trên 1.2 đơn vị

    private bool isTargeting = false;

    private void Awake()
    {
        Instance = this;
    }

    // Gắn hàm này vào nút Búa trên UI
    public void ActivateHammer()
    {
        if (GameManager.Instance.isGameOver || isTargeting) return;

        // Gọi DataManager để trừ 100 Ruby
        if (DataManager.Instance.UseRuby(100))
        {
            // Nếu trừ tiền thành công mới chạy hiệu ứng
            isTargeting = true;
            GameManager.Instance.isUsingBooster = true;

            if (topBar != null) topBar.gameObject.SetActive(true);
            if (bottomBar != null) bottomBar.gameObject.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(AnimateBars(barHeight));
        }
        else
        {
            // Xử lý khi không đủ tiền (ví dụ: hiện thông báo)
            Debug.Log("Bạn cần 100 Ruby để dùng Búa!");
        }
    }

    private void Update()
    {
        if (!isTargeting) return;

        if (Input.GetMouseButtonDown(0))
        {
            // Chống click xuyên UI
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Fruit"))
            {
                // Trúng trái cây!
                ExecuteSmash(hit.collider.gameObject);
            }
            else
            {
                // Click trượt -> Hủy
                CancelHammer();
            }
        }
    }

    private void ExecuteSmash(GameObject targetFruit)
    {
        isTargeting = false;
        StartCoroutine(AnimateBars(0f));

        // TÍNH TOÁN VỊ TRÍ MỚI: Tâm quả + Khoảng cách lệch
        Vector3 spawnPos = targetFruit.transform.position + hammerOffset;

        // Sinh ra búa ở vị trí đã dịch chuyển
        GameObject hammer = Instantiate(hammerPrefab, spawnPos, Quaternion.identity);

        Quang_HammerEffect hammerFx = hammer.GetComponent<Quang_HammerEffect>();
        if (hammerFx != null)
        {
            hammerFx.PlayEffect(targetFruit, explosionPrefab, smashSound);
        }
    }

    private void CancelHammer()
    {
        isTargeting = false;
        GameManager.Instance.isUsingBooster = false;
        StartCoroutine(AnimateBars(0f));
    }

    private IEnumerator AnimateBars(float targetHeight)
    {
        float elapsedTime = 0f;
        Vector2 topStartSize = topBar.sizeDelta;
        Vector2 bottomStartSize = bottomBar.sizeDelta;

        Vector2 topTargetSize = new Vector2(topStartSize.x, targetHeight);
        Vector2 bottomTargetSize = new Vector2(bottomStartSize.x, targetHeight);

        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            topBar.sizeDelta = Vector2.Lerp(topStartSize, topTargetSize, smoothT);
            bottomBar.sizeDelta = Vector2.Lerp(bottomStartSize, bottomTargetSize, smoothT);
            yield return null;
        }
        topBar.sizeDelta = topTargetSize;
        bottomBar.sizeDelta = bottomTargetSize;

        if (targetHeight <= 0)
        {
            topBar.gameObject.SetActive(false);
            bottomBar.gameObject.SetActive(false);
        }
    }
}