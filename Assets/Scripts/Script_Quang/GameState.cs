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
public class SaveData
{
    public int bestScore = 0;
    public int maxUnlockedFruitID = 0;

    // Thêm mảng này để lưu tổng số lần merge của mỗi loại quả
    public int[] fruitMergeCounts = new int[11]; 

    public GameState metaData = new GameState();
    public List<FruitSaveData> sessionData = new List<FruitSaveData>();
    public List<DailyScoreRecord> dailyScoreRecords = new List<DailyScoreRecord>();
}
[System.Serializable]
public class DailyScoreRecord
{
    public string date; // Định dạng "dd/MM/yyyy"
    public int score;
}
[System.Serializable]
public class FruitSaveData
{
    public string fruitID;
    public float posX;
    public float posY;
    public float rotZ;

    // Constructor tiện ích
    public FruitSaveData(string id, Vector2 pos, float rot)
    {
        fruitID = id;
        posX = pos.x;
        posY = pos.y;
        rotZ = rot;
    }
}

