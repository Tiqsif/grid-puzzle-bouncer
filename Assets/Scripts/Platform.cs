using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class Platform : MonoBehaviour
{
    [HideInInspector] public Grid grid;
    public int width = 4;
    public int height = 4;
    public float cellSize = 1f;

    public Transform unitsHolder;
    public Player player;
    public List<Unit> units;
    private int enemyCount;
    private bool isEnding;
    

    

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

    void Start()
    {
        grid = new Grid(width, height, cellSize);
        units = new List<Unit>();
        if (Application.IsPlaying(this)) // if the game is running
        {
            foreach (Transform child in unitsHolder) // get all children of the object
            {
                units.Add(child.GetComponent<Unit>());
                if (child.TryGetComponent(out Player player))
                {
                    this.player = player;
                }
            }
            foreach (Unit unit in units)
            {
                CheckAndSnap(unit);
            }
        }
        SetGridElements();
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Update()
    {
        // if the game is NOT running (meaning its in the editor) -----------------------------------------------
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
        // -------------------------------------------------------------------------------------------------------------


        if (enemyCount == 0 && !player.isMoving) // if all enemies are cleared and player is not moving game is won
        {
            Win();
        }
        grid.DrawGrid();
    }

   
    
    public Vector2Int CheckAndSnap(Unit u)
    {
        Vector3 closestCell = grid.GetClosestCellWorldPosition(u.transform.position);
        u.transform.position = new Vector3(closestCell.x, closestCell.y, closestCell.z);
        u.cellPosition = grid.GetXY(closestCell);

        grid.SetValue(u.cellPosition, (int)u.type);

        return u.cellPosition;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SetGridElements()
    {
        Debug.Log("Setting grid elements");
        units = new List<Unit>();

        foreach (Transform child in unitsHolder) // get all children of the object
        {
            units.Add(child.GetComponent<Unit>());
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
