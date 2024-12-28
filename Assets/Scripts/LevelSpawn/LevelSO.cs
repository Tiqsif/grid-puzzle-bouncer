using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnData // hold data for every unit
{
    public Type type;
    public Vector2Int cellPosition;
    public bool randomRotation;
    [Tooltip("0: right, 90: down, 180: left, 270: up")]
    public float rotation;

    public SpawnData(Type type, Vector2Int cellPosition, bool randomRotation, float rotation)
    {
        this.type = type;
        this.cellPosition = cellPosition;
        this.randomRotation = randomRotation;
        this.rotation = rotation;
    }
}

[System.Serializable]
[CreateAssetMenu(fileName = "LevelSO", menuName = "Level/LevelSO", order = 1)]
public class LevelSO : ScriptableObject // hold data for level
{
    public float cellSize = 1f;
    public Vector2Int gridSize = new Vector2Int(4,4);
    public List<SpawnData> spawnDataList;

    public static void WriteLevelSO(LevelSO obj, string fileName)
    {
        string jsonData = JsonUtility.ToJson(obj);
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        System.IO.File.WriteAllText(filePath, jsonData);
        Debug.Log($"Saved LevelSO to {filePath}");
    }

    public static LevelSO ReadLevelSO(string fileName)
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        if (System.IO.File.Exists(filePath))
        {
            string jsonData = System.IO.File.ReadAllText(filePath);
            LevelSO obj = ScriptableObject.CreateInstance<LevelSO>();
            JsonUtility.FromJsonOverwrite(jsonData, obj);
            Debug.Log("Loaded LevelSO from " + filePath);
            return obj;
        }
        else
        {
            Debug.LogWarning("File not found: " + filePath);
            return null;
        }
    }
}
