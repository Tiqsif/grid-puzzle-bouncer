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


    public delegate void OnEditorIsPlayingUpdated(bool isPlaying);
    public static event OnEditorIsPlayingUpdated onEditorIsPlayingUpdated;
    private new void Awake()
    {
        grid = new Grid(width, height, cellSize);
        newGrid = new Grid(width, height, cellSize);
        units = new List<Unit>();
        levelSpawner = GetComponent<LevelSpawner>();
    }

    private void OnEnable()
    {
        LevelManager.onLevelChange += OnLevelChange;
        UnitSelect.onUnitSelected += OnUnitButtonSelected;

        UnitSelect.onEditorPlayStopClicked += OnEditorPlayStopClicked;
        UnitSelect.onEditorSaveClicked += OnEditorSaveClicked;

        Player.onPlayerDeath += _OnPlayerDeath;
    }

    private void OnDisable()
    {
        LevelManager.onLevelChange -= OnLevelChange;
        UnitSelect.onUnitSelected -= OnUnitButtonSelected;

        UnitSelect.onEditorPlayStopClicked -= OnEditorPlayStopClicked;
        UnitSelect.onEditorSaveClicked -= OnEditorSaveClicked;

        Player.onPlayerDeath -= _OnPlayerDeath;

    }
    protected new void OnLevelChange()
    {
        Debug.Log("EditorPlatform: OnLevelChange");

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
            Debug.Log("EditorPlatform: Placeholders created");
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
        if (isPlaying)
        {
            Debug.Log("EditorPlatform: SetGridElements: isPlaying true");
            base.SetGridElements();
        }
        else
        {
            
            base.SetGridElements();
            foreach (Unit u in units)
            {
                u.enabled = false;
            }
            Debug.Log("EditorPlatform: SetGridElements: isPlaying false");
        }
    }

    public void ResetGridElements()
    {
        Debug.Log("EditorPlatform: ResetGridElements");
        if (!isPlaying)
        {
            levelSpawner.SpawnFromGrid(newGrid);
            // disable all unit scripts
            foreach (Unit u in units)
            {
                u.enabled = false;
            }
        }
    }

    private void OnUnitButtonSelected(Vector2Int cellPos, int index) // creates a unit at the cell position
    {
        int finalIndex = index + 1; // +1 because Type.Empty is 0 and its not a unit
        newGrid.SetValue(cellPos, finalIndex); // the editor grid
        if (finalIndex == 0) // empty selected, clear the unit at the cell
        {
            ClearUnitAt(cellPos);
            return;

        }
        if (levelSpawner)
        {
            ClearUnitAt(cellPos);
            levelSpawner.SpawnUnit(new SpawnData((Type)(finalIndex), cellPos, true, 0f)); // spawns the unit, doesnt set grid value
        }

    }

    private void OnEditorPlayStopClicked()
    {
        // check if player and enemy are added
        if (!newGrid.HasValue((int)Type.Player))
        {
            Debug.Log("EditorPlatform: OnEditorPlayStopClicked: Player not added");
            return;
        }
        if (!newGrid.HasValue((int)Type.Enemy))
        {
            Debug.Log("EditorPlatform: OnEditorPlayStopClicked: Enemy not added");
            return;
        }
        SetIsPlaying(!isPlaying);
        if (isPlaying)
        {
            SetGridElements(); // reset unit list from objects in holder, reset grid from every units cellpos, enable all unit scripts
            newGrid = grid.DeepCopy();
        }
        else
        {
            SetGridElements();
            ResetGridElements(); // return to initial edited grid. spawn from grid. disable all unit scripts
        }
    }


    private void OnEditorSaveClicked()
    {
        if (levelSpawner)
        {
            //levelSpawner.SaveLevel(newGrid);
        }
    }

    private void DrawInitialGrid()
    {
        if (newGrid == null) return;
        Vector3 origin = new Vector3(0, 5, 0);
        newGrid.DrawGrid(origin);
        for (int x = 0; x < newGrid.width; x++)
        {
            for (int y = 0; y < newGrid.height; y++)
            {
                if (newGrid.GetValue(x, y) != (int)Type.Empty)
                {
                    Debug.DrawLine(new Vector3(x * cellSize, 0, y * cellSize) + origin, new Vector3(x * cellSize, cellSize, y * cellSize) + origin, Color.red, 0.1f);
                }
            }
        }
    }

    private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        DrawInitialGrid();
    }

    override public void _OnPlayerDeath()
    {
        if (isPlaying)
        {
            SetIsPlaying(false);
            ResetGridElements();
            SetGridElements();
        }
    }

    void SetIsPlaying(bool b)
    {
        isPlaying = b;
        if (isPlaying)
        {
            placeHoldersParent.SetActive(false);
        }
        else
        {
            placeHoldersParent.SetActive(true);
        }
        onEditorIsPlayingUpdated?.Invoke(isPlaying);
    }
}
