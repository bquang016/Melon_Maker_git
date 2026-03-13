using System.IO;
using UnityEngine;

public static class SaveLoadManager
{
    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "suika_save.json");

    public static void SaveGame(SaveData data)
    {
        // Chuyển object thành chuỗi JSON (true để format JSON dễ nhìn trong file)
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SaveFilePath, json);
        Debug.Log($"[SaveLoadManager] Đã lưu game tại: {SaveFilePath}");
    }

    public static SaveData LoadGame()
    {
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("[SaveLoadManager] Tải dữ liệu thành công.");
            return data;
        }

        Debug.LogWarning("[SaveLoadManager] Không tìm thấy file save, tạo dữ liệu mới.");
        return new SaveData(); // Trả về data mặc định nếu chưa có save
    }

    public static void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            Debug.Log("[SaveLoadManager] Đã xóa file save.");
        }
    }
}