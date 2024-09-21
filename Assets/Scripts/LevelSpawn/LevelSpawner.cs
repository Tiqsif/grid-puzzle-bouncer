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


    public void SpawnLevel()
    {
        propSpawner.CreatePlatform(levelData.currentLevel);
        SetupLevel();
    }
    private void SetupLevel()
    {
        if (levelData.currentLevel == null)
        {
            Debug.LogError("No current level found");
            LevelManager.Instance.LoadMainMenu();
            return;
        }
        Platform platform = FindAnyObjectByType<Platform>();
        platform.ClearUnits();
        foreach (SpawnData spawnData in levelData.currentLevel.spawnDataList)
        {
            if(spawnData.type == Type.Empty || !unitDictionary.ContainsKey(spawnData.type))
            {
                continue;
            }
            Vector3 pos = platform.GetWorldPosition(spawnData.cellPosition.x, spawnData.cellPosition.y);
            float rot;
            if (spawnData.randomRotation)
            {
                rot = Random.Range(0, 4) * 90;
            }
            else
            {
                rot = spawnData.rotation;
            }
            GameObject unit = Instantiate(unitDictionary[spawnData.type], pos, Quaternion.Euler(0, rot, 0), unitsHolder);
            unit.TryGetComponent(out Unit unitComponent);
            unitComponent.cellPosition = spawnData.cellPosition;
        }
        platform.SetGridElements();
    }

    
}
