using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public int highScore;
    public int currentCoin;
    public List<string> unlockedSkins = new List<string>();
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

[System.Serializable]
public class SaveData
{
    public GameState metaData = new GameState();
    public List<FruitSaveData> sessionData = new List<FruitSaveData>();
}