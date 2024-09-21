using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static LevelManager;

//[ExecuteAlways]
public class Platform : MonoBehaviour
{
    [HideInInspector] public Grid grid;
    [HideInInspector] public int width = 4;
    [HideInInspector] public int height = 4;
    [HideInInspector] public float cellSize = 1f;

    public Transform unitsHolder;
    [HideInInspector] public Player player;
    [HideInInspector] public List<Unit> units;
    protected int enemyCount;
    protected bool isEnding;

    protected bool isEnemyPresentAtStart;
    

    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            grid.DrawGrid();
        }

        foreach (Unit unit in units)
        {
            Color color = unit.color;
            Debug.DrawLine(GetWorldPosition(unit.cellPosition.x, unit.cellPosition.y), GetWorldPosition(unit.cellPosition.x, unit.cellPosition.y) + Vector3.up, color);
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        LevelManager.onLevelChange += OnLevelChange;
    }

    private void OnDisable()
    {
        LevelManager.onLevelChange -= OnLevelChange;
    }
    void Start()
    {
        grid = new Grid(width, height, cellSize);
        units = new List<Unit>();
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Update()
    {
        /*/ if the game is NOT running (meaning its in the editor) -----------------------------------------------
        if (!Application.IsPlaying(this)) // EDITOR
        {
            if (grid == null)
            {
                grid = new Grid(width, height, cellSize);
            }
            units = new List<Unit>();

            foreach (Transform child in unitsHolder) // get all children of the object
            {
                units.Add(child.GetComponent<Unit>());
                if (child.TryGetComponent(out Player player))
                {
                    this.player = player;
                }
            }

            // if an object is close to a cell in the grid of the platform, snap it to that cell
            foreach (Unit unit in units)
            {
                CheckAndSnap(unit);
            }
            SetGridElements();
        } // /EDITOR
        // -------------------------------------------------------------------------------------------------------------*/

        //Debug.Log(units.Count);
        if (player != null && enemyCount == 0 && !player.isMoving && isEnemyPresentAtStart) // if all enemies are cleared and player is not moving game is won
        {
            if (Application.isPlaying)
            {
                Win();
            }
        }
        grid.DrawGrid();
    }

   
    private void OnLevelChange()
    {
        player.isDead = false;
        isEnding = false;

        if (TryGetComponent(out LevelSpawner levelSpawner))
        {
            width = levelSpawner.levelData.currentLevel.gridSize.x;
            height = levelSpawner.levelData.currentLevel.gridSize.y;
            cellSize = levelSpawner.levelData.currentLevel.cellSize;
        }
        grid = new Grid(width, height, cellSize);
        
        
        
        if (enemyCount > 0)
        {
            isEnemyPresentAtStart = true;
        }
        else
        {
            isEnemyPresentAtStart = false;
        }
        
        
    }
    public Vector2Int CheckAndSnap(Unit u) // sets the vec3 position and the cellPosition of the unit to the closest cell in the grid
    {
        Vector3 closestCell = grid.GetClosestCellWorldPosition(u.transform.position);
        u.transform.position = new Vector3(closestCell.x, closestCell.y, closestCell.z); // snap Vec3 position
        u.cellPosition = grid.GetXY(closestCell); // update cellPosition

        grid.SetValue(u.cellPosition, (int)u.type); // update grid

        return u.cellPosition;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SetGridElements()
    {
        Debug.Log("Setting grid elements");
        //Debug.Log("Before: " + units.Count);
        units = new List<Unit>();
        units.Clear();
        foreach (Transform child in unitsHolder) // get all children of the object
        {
            units.Add(child.GetComponent<Unit>());
            if (child.TryGetComponent(out Player player))
            {
                this.player = player;
            }
        }
        enemyCount = 0;
        grid.Clear();
        foreach (Unit unit in units)
        {
            if (unit == null || !unit.gameObject.activeInHierarchy)
            {
                continue;
            }
            if (unit.type == Type.Enemy)
            {
                enemyCount++;
            }

            grid.SetValue(unit.cellPosition, (int)unit.type);
        }
        //Debug.Log("After: " + units.Count);
    }
    public void ClearUnits()
    {
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < unitsHolder.childCount; i++)
        {
            children.Add(unitsHolder.GetChild(i));
        }

        foreach (var child in children)
        {
            DestroyImmediate(child.gameObject);
        }

        units.Clear();
        //Debug.Log("unitsHolder.childCount: " + unitsHolder.childCount);
        //Debug.Log("units.Count: " + units.Count);
    }
    public Vector3 GetWorldPosition(int x, int y) // NOT SECURED (no check if x and y are in the grid)
    {
        return new Vector3(x, 0, y) * cellSize;
    }

    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    public bool IsInsideGrid(Vector2Int vec)
    {
        return IsInsideGrid(vec.x, vec.y);
    }
    void Win()
    {
        if (isEnding || player.isDead)
        {
            return;
        }
        AudioManager.Instance.PlaySFX(AudioManager.Instance.winClip);
        LevelManager.Instance.LoadNextLevel(2f);
        isEnding = true;
    }
}
