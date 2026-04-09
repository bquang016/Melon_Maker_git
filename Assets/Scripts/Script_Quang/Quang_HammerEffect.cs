using UnityEngine;
using System.Collections;

public class Quang_HammerEffect : MonoBehaviour
{
    private GameObject targetFruit;
    private GameObject explosionPrefab;
    private AudioClip smashSound;

    [Header("Thông số lực đập (Đã cân chỉnh)")]
    // Lấy đà sang Phải (Số Âm)
    public float windUpAngle = -45f;
    // Đập sang Trái - hướng xuống (Số Dương)
    public float smashAngle = 80f;

    // Tăng thời gian lấy đà để nhìn rõ hơn (Gốc là 0.3)
    public float windUpTime = 0.5f;
    // Tăng thời gian gõ xuống cho đầm tay hơn (Gốc là 0.1)
    public float smashTime = 0.2f;

    public void PlayEffect(GameObject fruit, GameObject explosion, AudioClip sound)
    {
        targetFruit = fruit;
        explosionPrefab = explosion;
        smashSound = sound;
        StartCoroutine(SwingAndSmash());
    }

    private IEnumerator SwingAndSmash()
    {
        // 1. LẤY ĐÀ (Ngả sang phải)
        float elapsed = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion windUpRot = Quaternion.Euler(0, 0, windUpAngle);

        while (elapsed < windUpTime)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, windUpRot, elapsed / windUpTime);
            yield return null;
        }

        // 2. PHANG XUỐNG
        elapsed = 0f;
        Quaternion smashRot = Quaternion.Euler(0, 0, smashAngle);

        while (elapsed < smashTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / smashTime;
            transform.rotation = Quaternion.Lerp(windUpRot, smashRot, t * t);
            yield return null;
        }

        // THÊM DÒNG NÀY: Chờ thêm một tẹo (0.05s) để búa thực sự "chạm" vào vỏ quả
        yield return new WaitForSeconds(0.05f);

        // 3. VA CHẠM (Xóa quả, tạo nổ)
        if (targetFruit != null)
        {
            Vector3 pos = targetFruit.transform.position;
            Destroy(targetFruit);

            if (explosionPrefab != null) Instantiate(explosionPrefab, pos, Quaternion.identity);
            if (smashSound != null) AudioSource.PlayClipAtPoint(smashSound, Camera.main.transform.position);

            // Bật rung camera
            if (Quang_CameraShake.Instance != null) Quang_CameraShake.Instance.Shake(0.2f, 0.15f);
        }

        // 4. MỜ DẦN VÀ BIẾN MẤT (Chờ 1 chút để thấy rõ búa đập xong mới mờ)
        yield return new WaitForSeconds(0.1f);

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        elapsed = 0f;
        float fadeTime = 0.3f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            if (sr != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            }
            yield return null;
        }

        // Mở khóa Spawner
        if (GameManager.Instance != null) GameManager.Instance.isUsingBooster = false;
        Destroy(gameObject);
    }
}