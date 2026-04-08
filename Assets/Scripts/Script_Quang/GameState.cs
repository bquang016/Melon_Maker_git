using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    // TIỀN TỆ TRONG GAME
    public int currentRuby = 0;
    public int currentMusicNote = 0;

    // LƯU TRỮ SHOP
    public List<string> unlockedSkins = new List<string>();
    public List<string> unlockedMusic = new List<string>();
}

[System.Serializable]
public class FruitSaveData
{
    public string fruitID;
    public float posX;
    public float posY;
    public float rotZ;

    public FruitSaveData(string id, Vector2 pos, float rot)
    {
        fruitID = id;
        posX = pos.x;
        posY = pos.y;
        rotZ = rot;
    }
}

[System.Serializable]
public class SaveData
{
    // CÁC KỶ LỤC CỦA NGƯỜI CHƠI
    public int bestScore = 0;
    public int dailyBestScore = 0; // THÊM MỚI: Điểm cao nhất trong ngày
    public string lastPlayDate = ""; // THÊM MỚI: Ngày cuối cùng chơi để reset điểm ngày
    
    public int maxUnlockedFruitID = 0;

    // THỐNG KÊ MERGE
    public int totalMergedFruits = 0; // THÊM MỚI: Tổng số quả đã merge
    public int[] fruitMergeCounts = new int[11]; // THÊM MỚI: Mảng đếm cho từng loại quả

    // DỮ LIỆU KINH TẾ & TRẠNG THÁI BÀN CHƠI
    public GameState metaData = new GameState();
    public List<FruitSaveData> sessionData = new List<FruitSaveData>();
}