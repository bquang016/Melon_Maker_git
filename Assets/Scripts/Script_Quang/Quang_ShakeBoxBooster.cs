using UnityEngine;
using System.Collections;

public class Quang_ShakeBoxBooster : MonoBehaviour
{
    public static Quang_ShakeBoxBooster Instance { get; private set; }

    [Header("Cài đặt UI & Box")]
    public GameObject mainCanvas;
    public Transform boxTransform;
    public float shrinkFactor = 0.85f;
    public float animationSpeed = 0.4f;

    [Header("Thông số Lắc")]
    public float shakeAngle = 12f; // Tăng nhẹ góc lắc cho rõ ràng
    public float shakePower = 6f;  // Lực đẩy trái cây

    private Vector3 originalBoxScale;
    private bool isShaking = false;

    private void Awake()
    {
        Instance = this;
        if (boxTransform != null) originalBoxScale = boxTransform.localScale;
    }

    public void ActivateShakeBooster()
    {
        if (isShaking || GameManager.Instance.isGameOver) return;
        StartCoroutine(ShakeSequence());
    }

    private IEnumerator ShakeSequence()
    {
        isShaking = true;
        GameManager.Instance.isUsingBooster = true; // Khóa thả quả

        if (mainCanvas != null) mainCanvas.SetActive(false);

        // THU NHỎ HỘP
        float elapsed = 0;
        Vector3 targetScale = originalBoxScale * shrinkFactor;
        while (elapsed < animationSpeed)
        {
            elapsed += Time.deltaTime;
            boxTransform.localScale = Vector3.Lerp(originalBoxScale, targetScale, elapsed / animationSpeed);
            yield return null;
        }

        // --- BẮT ĐẦU LẮC THEO NHỊP ---

        // 1. Trái qua Phải (1 lần): Xoay Trái (+), rồi qua Phải (-)
        yield return StartCoroutine(DoShakeStep(shakeAngle, 0.15f));
        yield return StartCoroutine(DoShakeStep(-shakeAngle, 0.15f));

        // 2. Phải qua Trái (2 lần xen kẽ): Xoay Phải (-), rồi qua Trái (+)
        for (int i = 0; i < 2; i++)
        {
            yield return StartCoroutine(DoShakeStep(-shakeAngle, 0.12f));
            yield return StartCoroutine(DoShakeStep(shakeAngle, 0.12f));
        }

        PushAllFruits(); // Tác động lực vật lý

        yield return new WaitForSeconds(0.6f);

        // HỒI PHỤC TRẠNG THÁI
        elapsed = 0;
        boxTransform.rotation = Quaternion.identity;
        while (elapsed < animationSpeed)
        {
            elapsed += Time.deltaTime;
            boxTransform.localScale = Vector3.Lerp(targetScale, originalBoxScale, elapsed / animationSpeed);
            yield return null;
        }

        if (mainCanvas != null) mainCanvas.SetActive(true);

        isShaking = false;
        GameManager.Instance.isUsingBooster = false; // Mở khóa thả quả
    }

    private IEnumerator DoShakeStep(float targetAngle, float duration)
    {
        float elapsed = 0;
        Quaternion startRot = boxTransform.rotation;
        Quaternion endRot = Quaternion.Euler(0, 0, targetAngle);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            boxTransform.rotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            yield return null;
        }
    }

    private void PushAllFruits()
    {
        GameObject[] fruits = GameObject.FindGameObjectsWithTag("Fruit");
        foreach (GameObject f in fruits)
        {
            Rigidbody2D rb = f.GetComponent<Rigidbody2D>();

            // SỬA DÒNG NÀY: Chỉ đẩy những quả đang simulated (đang nằm trong hộp hoặc đang rơi)
            // Quả trên mây có rb.simulated = false nên sẽ bị bỏ qua.
            if (rb != null && rb.simulated)
            {
                Vector2 force = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(1f, 2f)) * shakePower;
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }

        if (Quang_CameraShake.Instance != null) Quang_CameraShake.Instance.Shake(0.4f, 0.15f);
    }

    // Safety Reset: Đề phòng trường hợp lỗi hệ thống
    private void OnDisable()
    {
        if (isShaking)
        {
            GameManager.Instance.isUsingBooster = false;
            isShaking = false;
        }
    }
}