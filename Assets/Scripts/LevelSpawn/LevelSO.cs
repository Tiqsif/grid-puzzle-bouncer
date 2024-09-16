using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnData // hold data for every unit
{
    public Type type;
    public Vector2Int cellPosition;
    [Tooltip("0: right, 90: down, 180: left, 270: up")]
    public float rotation;


}

[CreateAssetMenu(fileName = "LevelSO", menuName = "Level/LevelSO", order = 1)]
public class LevelSO : ScriptableObject // hold data for level
{
    public float cellSize = 1f;
    public Vector2Int gridSize = new Vector2Int(4,4); // TODO
    public List<SpawnData> spawnDataList;
}
