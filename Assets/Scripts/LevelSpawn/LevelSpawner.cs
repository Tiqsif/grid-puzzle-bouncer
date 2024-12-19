using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public LevelData levelData;
    [SerializeField] private Transform unitsHolder;
    [SerializeField] private AllUnitsSO allUnitsSO;
    private List<GameObject> allUnits;
    private Dictionary<Type, GameObject> unitDictionary = new Dictionary<Type, GameObject>();

    private PropSpawner propSpawner;
    private Platform platform;
    [HideInInspector] public Grid currentGrid;
    private void Awake()
    {
        propSpawner = GetComponent<PropSpawner>();
        platform = FindAnyObjectByType<Platform>();
        allUnits = allUnitsSO.allUnits;
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
        if (currentGrid == null)
        {

            if (propSpawner)
            {
                propSpawner.CreatePlatform(levelData.currentLevel);
            }
            SetupLevel();
        }
        else
        {
            SpawnFromGrid(currentGrid);
        }
    }
    private void SetupLevel()
    {
        if (levelData.currentLevel == null)
        {
            Debug.LogError("No current level found");
            LevelManager.Instance.LoadMainMenu();
            return;
        }
        platform = platform != null ? platform : FindAnyObjectByType<Platform>();

        platform.ClearUnits();
        foreach (SpawnData spawnData in levelData.currentLevel.spawnDataList)
        {
           SpawnUnit(spawnData);
        }
        platform.SetGridElements();
    }

    public void SpawnFromGrid(Grid grid)
    {
        Debug.Log("SpawnFromGrid");
        SetCurrentGrid(grid);
        platform = platform != null ? platform : FindAnyObjectByType<Platform>();
        platform.ClearUnits();
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                if (grid.GetValue(x,y) != (int)Type.Empty)
                {
                    //Debug.Log("Spawning " + (Type)grid.GetValue(x, y) + " at " + x + ", " + y);
                    SpawnData spawnData = new SpawnData((Type)grid.GetValue(x,y), new Vector2Int(x, y), true, 0);
                    SpawnUnit(spawnData);
                }
            }
        }
        platform.SetGridElements();
    }

    public void SetCurrentGrid(Grid grid)
    {
        this.currentGrid = grid;
    }
    public void SpawnUnit(SpawnData spawnData)
    {
        if (spawnData.type == Type.Empty || !unitDictionary.ContainsKey(spawnData.type))
        {
            return;
        }
        Vector3 pos = platform.GetWorldPosition(spawnData.cellPosition.x, spawnData.cellPosition.y);
        float rot = spawnData.randomRotation ? Random.Range(0, 4) * 90 : spawnData.rotation;

        GameObject unit = Instantiate(unitDictionary[spawnData.type], pos, Quaternion.Euler(0, rot, 0), unitsHolder);
        unit.TryGetComponent(out Unit unitComponent);
        unitComponent.cellPosition = spawnData.cellPosition;
        unitComponent.enabled = false; // will be enabled in platform.setgridelements

    }
}
