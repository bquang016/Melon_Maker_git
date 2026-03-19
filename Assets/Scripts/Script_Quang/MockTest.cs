using System.Collections.Generic;
using UnityEngine;

public class MockTest : MonoBehaviour
{
    [Header("Kho chứa Prefab giả")]
    // Bạn nhập "Apple", "Melon" vào mảng này
    public string[] prefabIDs;
    // Bạn kéo các Prefab hình tròn tương ứng vào mảng này
    public GameObject[] fruitPrefabs;

    [Header("Nơi chứa trái cây sinh ra")]
    public Transform fruitContainer;

    private void Update()
    {
        // Giả lập UI bằng phím tắt
        if (Input.GetKeyDown(KeyCode.S)) ExecuteSave();
        if (Input.GetKeyDown(KeyCode.L)) ExecuteLoad();
        if (Input.GetKeyDown(KeyCode.X)) ClearScene(); // Nút xóa lẹ để test Load
    }

    private void ExecuteSave()
    {
        SaveData currentSave = new SaveData();
        currentSave.metaData.highScore = 9999;
        currentSave.metaData.currentCoin = 150;
        currentSave.metaData.unlockedSkins.Add("Skin_Default");

        // Tìm TẤT CẢ các trái cây có trong Scene (không cần quan tâm nó nằm ở đâu)
        MockFruitItem[] activeFruits = FindObjectsOfType<MockFruitItem>();

        foreach (var fruit in activeFruits)
        {
            FruitSaveData fruitData = new FruitSaveData(
                fruit.fruitID,
                fruit.transform.position,
                fruit.transform.rotation.eulerAngles.z
            );
            currentSave.sessionData.Add(fruitData);
        }

        SaveLoadManager.SaveGame(currentSave);
    }

    private void ExecuteLoad()
    {
        ClearScene(); // Quét sạch màn hình trước khi Load
        SaveData loadedSave = SaveLoadManager.LoadGame();

        // Đọc từng trái cây từ file JSON
        foreach (var fruitData in loadedSave.sessionData)
        {
            GameObject prefabToSpawn = GetPrefabByID(fruitData.fruitID);
            if (prefabToSpawn != null)
            {
                // Sinh ra trái cây
                GameObject newFruit = Instantiate(prefabToSpawn, fruitContainer);

                // --- ĐOẠN NÀY CỰC KỲ QUAN TRỌNG CHO GAME SUIKA ---
                // Tạm thời đóng băng vật lý để không bị lỗi văng tung tóe khi set tọa độ
                Rigidbody2D rb = newFruit.GetComponent<Rigidbody2D>();
                if (rb != null) rb.simulated = false;

                // Áp tọa độ và góc xoay từ file Save vào
                newFruit.transform.position = new Vector2(fruitData.posX, fruitData.posY);
                newFruit.transform.rotation = Quaternion.Euler(0, 0, fruitData.rotZ);

                // Mở lại vật lý cho nó rớt tự nhiên
                if (rb != null) rb.simulated = true;
            }
        }
    }

    private void ClearScene()
    {
        // Tìm TẤT CẢ trái cây trong Scene và tiêu diệt chúng
        MockFruitItem[] allFruits = FindObjectsOfType<MockFruitItem>();
        foreach (MockFruitItem fruit in allFruits)
        {
            Destroy(fruit.gameObject);
        }
    }

    // ĐÂY LÀ HÀM BỊ THIẾU ĐÃ ĐƯỢC THÊM VÀO
    private GameObject GetPrefabByID(string id)
    {
        for (int i = 0; i < prefabIDs.Length; i++)
        {
            if (prefabIDs[i] == id) return fruitPrefabs[i];
        }
        return null;
    }
}