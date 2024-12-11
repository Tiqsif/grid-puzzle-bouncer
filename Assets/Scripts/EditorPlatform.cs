using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EditorPlatform : Platform
{
    public GameObject placeHolderPrefab; // place holders to click on and select a unit for custom levels
    public GameObject placeHoldersParent;
    public bool isPlaying = false;
    public Grid newGrid; // grid thats made in the editor
    private LevelSpawner levelSpawner;
    private new void Awake()
    {
        grid = new Grid(width, height, cellSize);
        units = new List<Unit>();
        levelSpawner = GetComponent<LevelSpawner>();
    }

    private void OnEnable()
    {
        LevelManager.onLevelChange += OnLevelChange;
        UnitSelect.onUnitSelected += OnUnitButtonSelected;
        
    }

    private void OnDisable()
    {
        LevelManager.onLevelChange -= OnLevelChange;
        UnitSelect.onUnitSelected -= OnUnitButtonSelected;
        
    }
    protected new void OnLevelChange()
    {
        Debug.Log("Level Changed");

        if(player) player.isDead = false;
        isEnding = false;

        if (levelSpawner)
        {
            width = levelSpawner.levelData.currentLevel.gridSize.x;
            height = levelSpawner.levelData.currentLevel.gridSize.y;
            cellSize = levelSpawner.levelData.currentLevel.cellSize;
        }
        grid = new Grid(width, height, cellSize);

        // place holder creation
        if (placeHolderPrefab && placeHoldersParent.transform.childCount == 0)
        {
            Debug.Log("Placeholders created");
            for (int i = 0; i < width * height; i++)
            {

                Vector2Int cellPos = new Vector2Int(i % width, i / width);
                GameObject g = Instantiate(placeHolderPrefab, GetWorldPosition(cellPos.x,cellPos.y), Quaternion.identity);
                g.transform.localScale = new Vector3(cellSize, placeHolderPrefab.transform.localScale.y,cellSize);
                g.transform.SetParent(placeHoldersParent.transform);
                if (g.TryGetComponent(out UnitPlacer unitPlacer))
                {
                    unitPlacer.cellPosition = cellPos; 
                }
            }
        }


        if (enemyCount > 0)
        {
            isEnemyPresentAtStart = true;
        }
        else
        {
            isEnemyPresentAtStart = false;
        }
    }

    [ContextMenu("Set Grid Elements from Editor")]
    public override void SetGridElements()
    {
        Debug.Log("Setting grid elements from Editor");
        if (isPlaying)
        {
            Debug.Log("SetGrid isPlaying true");
            base.SetGridElements();

        }
    }

    private void OnUnitButtonSelected(Vector2Int cellPos, int index)
    {
        if (levelSpawner)
        {
            levelSpawner.SpawnUnit(new SpawnData((Type)index, cellPos, true, 0f));
        }
    }
}
