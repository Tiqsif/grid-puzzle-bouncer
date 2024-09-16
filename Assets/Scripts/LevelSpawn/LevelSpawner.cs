using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PropSpawner))]
public class LevelSpawner : MonoBehaviour
{
    public LevelData levelData;
    [SerializeField] private Transform unitsHolder;
    [SerializeField] private List<GameObject> allUnits;
    private Dictionary<Type, GameObject> unitDictionary = new Dictionary<Type, GameObject>();

    private PropSpawner propSpawner;
    private void Awake()
    {
        propSpawner = GetComponent<PropSpawner>();
        foreach (var unit in allUnits)
        {
            if(unit.TryGetComponent(out Unit unitComponent))
            {
                unitDictionary.Add(unitComponent.type, unit);
            }
           
        }
        if (levelData.allLevels.Count == 0)
        {
            Debug.LogError("No levels found");
            LevelManager.Instance.LoadMainMenu();
            return;
        }
    }

    private void Start()
    {
        Debug.Log(levelData.currentLevel.gridSize);
        propSpawner.CreatePlatform(levelData.currentLevel.gridSize, levelData.currentLevel.cellSize);
        SetupLevel();
    }

    private void SetupLevel()
    {
        ClearUnits();
        if (levelData.currentLevel == null)
        {
            Debug.LogError("No current level found");
            LevelManager.Instance.LoadMainMenu();
            return;
        }
        Platform platform = FindAnyObjectByType<Platform>();
        foreach (var spawnData in levelData.currentLevel.spawnDataList)
        {
            Vector3 pos = platform.GetWorldPosition(spawnData.cellPosition.x, spawnData.cellPosition.y);
            GameObject unit = Instantiate(unitDictionary[spawnData.type], pos, Quaternion.Euler(0, spawnData.rotation, 0), unitsHolder);
            unit.TryGetComponent(out Unit unitComponent);
            unitComponent.cellPosition = spawnData.cellPosition;
        }
        platform.SetGridElements();
    }

    private void ClearUnits()
    {
        foreach (Transform child in unitsHolder)
        {
            Destroy(child.gameObject);
        }
    }
}
