using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EditorPlatform : Platform
{
    public GameObject placeHolderPrefab; // place holders to click on and select a unit for custom levels
    public GameObject placeHoldersParent;
    public bool isPlaying = false;
    public LevelSpawner levelSpawner;

    [HideInInspector] public Grid newGrid; // grid thats made in the editor, slowly take this out and use spawnDataList
    [HideInInspector] public List<SpawnData> spawnDataList; // list of spawn data made in the editor


    public delegate void OnEditorIsPlayingUpdated(bool isPlaying);
    public static event OnEditorIsPlayingUpdated onEditorIsPlayingUpdated;
    private new void Awake()
    {
        grid = new Grid(width, height, cellSize);

        newGrid = new Grid(width, height, cellSize);
        spawnDataList = new List<SpawnData>();


        units = new List<Unit>();
        levelSpawner = GetComponent<LevelSpawner>();
        LevelSO readLevel = LevelSO.ReadLevelSO("CustomLevel.json");
        if (readLevel != null)
        {
            LevelManager.Instance.levelData.currentLevel.cellSize = readLevel.cellSize;
            LevelManager.Instance.levelData.currentLevel.gridSize = readLevel.gridSize;
            LevelManager.Instance.levelData.currentLevel.spawnDataList = readLevel.spawnDataList;
            spawnDataList = readLevel.spawnDataList;
            Debug.Log(spawnDataList.Count);
        }
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

    private new void Update()
    {
        if (isPlaying)
        {
            base.Update();
        }
    }
    protected override void HandleLevelManagerButtons()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //LevelManager.Instance.ReloadLevel();
            OnEditorPlayStopClicked(); // act as stop pressed
        }

        /*
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LevelManager.Instance.LoadPreviousLevel();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            LevelManager.Instance.LoadNextLevel();
        }
        */
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
        Debug.Log("EditorPlatform: OnLevelChange: " + width + " " + height + " " + cellSize);
        // place holder creation
        if (placeHolderPrefab && placeHoldersParent.transform.childCount != width * height)
        {
            for (int i = placeHoldersParent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(placeHoldersParent.transform.GetChild(i).gameObject);
            }
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
            Debug.Log("EditorPlatform: Placeholders created");
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
            //levelSpawner.SpawnFromGrid(newGrid);
            levelSpawner.SpawnFromCustomSpawnDataList(spawnDataList);
            // disable all unit scripts
            foreach (Unit u in units)
            {
                u.enabled = false;
            }
        }
    }

    private void OnUnitButtonSelected(UnitPlacer selectedPlacer, int index) // creates a unit at the cell position
    {
        
        int finalIndex = index + 1; // +1 because Type.Empty is 0 and its not a unit
        
        ClearUnitAt(selectedPlacer.cellPosition);
        if (finalIndex > 0) // if not empty
        {
            newGrid.SetValue(selectedPlacer.cellPosition, finalIndex); // the editor grid
            SpawnData newSpawnData = new SpawnData((Type)(finalIndex), selectedPlacer.cellPosition, false, selectedPlacer.rotationAngle);
            spawnDataList.Add(newSpawnData); // create and add spawn data to list, TODO: custom rotation
            if (levelSpawner) levelSpawner.SpawnUnit(newSpawnData); // spawns the unit, doesnt set grid value
        }
        

    }

    private void OnEditorPlayStopClicked()
    {
        // check if player and enemy are added
        if (!isPlaying)
        {
            if (!HasTypeInSpawnDataList(Type.Player))
            {
                Debug.Log("EditorPlatform: OnEditorPlayStopClicked: Player not added");
                FloatingTextManager.Instance.CreateFloatingText("Player not added", 1f, 1f, 1f);
                return;
            }
            if (!HasTypeInSpawnDataList(Type.Enemy))
            {
                Debug.Log("EditorPlatform: OnEditorPlayStopClicked: Enemy not added");
                FloatingTextManager.Instance.CreateFloatingText("Bug not added", 1f, 1f, 1f);
                return;
            }
        }
        SetIsPlaying(!isPlaying);
        if (isPlaying)
        {
            SetGridElements(); // reset unit list from objects in holder, reset grid from every units cellpos, enable all unit scripts
            //newGrid = grid.DeepCopy();

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

            // log the spawn datas for now
            foreach (SpawnData spawnData in spawnDataList)
            {
                Debug.Log(spawnData.type + " at " + spawnData.cellPosition + " with rotation " + spawnData.rotation);
            }
        }

        LevelSO currentCustomLevel = ScriptableObject.CreateInstance<LevelSO>();
        currentCustomLevel.cellSize = cellSize;
        currentCustomLevel.gridSize =  levelSpawner.levelData.currentLevel.gridSize;
        currentCustomLevel.spawnDataList = spawnDataList;
        LevelSO.WriteLevelSO(currentCustomLevel, "CustomLevel.json");
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
        //DrawInitialGrid();
        Vector3 origin = new Vector3(0, 5, 0);
        if (spawnDataList != null && spawnDataList.Count > 0 && newGrid != null)
        {
            newGrid.DrawGrid(origin);
            foreach (SpawnData spawnData in spawnDataList)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Vector3(spawnData.cellPosition.x * cellSize, 0, spawnData.cellPosition.y * cellSize) + origin, Vector3.up * cellSize);
            }
        }
    }

    override public void _OnPlayerDeath()
    {
        if (isPlaying)
        {
            /*
            SetIsPlaying(false);
            ResetGridElements();
            SetGridElements();
            */
            OnEditorPlayStopClicked(); // act as stop pressed
        }
    }

    protected override void Win()
    {
        if (isEnding || player.isDead || !isPlaying)
        {
            return;
        }
        AudioManager.Instance.PlaySFX(AudioManager.Instance.winClip);
        OnEditorPlayStopClicked(); // act as stop pressed
        isEnding = true;
    }

    void SetIsPlaying(bool b)
    {
        isPlaying = b;
        if (isPlaying)
        {
            placeHoldersParent.SetActive(false);
            isEnding = false;
            FloatingTextManager.Instance.CreateFloatingText("R to stop", 1f, 0.5f, 5f);
        }
        else
        {
            placeHoldersParent.SetActive(true);
            isEnding = false;
        }
        onEditorIsPlayingUpdated?.Invoke(isPlaying);
    }

    private bool HasTypeInSpawnDataList(Type type)
    {
        Debug.Log("HasTypeInSpawnDataList: " + type);
        foreach (SpawnData spawnData in spawnDataList)
        {
            if (spawnData.type == type)
            {
                return true;
            }
        }
        return false;

    }

    public override void ClearUnitAt(Vector2Int cellPos)
    {
        base.ClearUnitAt(cellPos);
        newGrid.SetValue(cellPos, (int)Type.Empty);
        for (int i = spawnDataList.Count - 1; i >= 0; i--)
        {
            if (spawnDataList[i].cellPosition == cellPos)
            {
                spawnDataList.RemoveAt(i);
            }
        }
    }


    // grid size update buttons onclick
    /*
    public void IncreaseX()
    {
        if (isPlaying) return;
        levelSpawner.levelData.currentLevel.gridSize.x++;
        LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevelIndex);
    }
    public void DecreaseX()
    {
        if (isPlaying) return;
        levelSpawner.levelData.currentLevel.gridSize.x--;
        LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevelIndex);
    }
    public void IncreaseY()
    {
        if (isPlaying) return;
        levelSpawner.levelData.currentLevel.gridSize.y++;
        LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevelIndex);
    }
    public void DecreaseY()
    {
        if (isPlaying) return;
        levelSpawner.levelData.currentLevel.gridSize.y--;
        LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevelIndex);
    }
    */
}
