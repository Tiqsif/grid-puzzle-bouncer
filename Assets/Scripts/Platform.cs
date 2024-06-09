using System.Collections;
using System.Collections.Generic;
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
    private List<Unit> units;
    public enum CellType
    {
        EmptyCell,
        PlayerCell,
        EnemyCell,
        RockCell
    }

    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            grid.DrawGrid();
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

        if (player != null)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveUnit(player, Vector2Int.up);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                MoveUnit(player, Vector2Int.left);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                MoveUnit(player, Vector2Int.down);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                MoveUnit(player, Vector2Int.right);
            }
        }
        grid.DrawGrid();
    }

    // --------------------------------------------------------------- MOVE UNIT -----------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------- MOVE UNIT -----------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------- MOVE UNIT -----------------------------------------------------------------------------------------------------
    public void MoveUnit(Unit unitToMove, Vector2Int direction)
    {
        if (unitToMove == null)
        {
            Debug.LogWarning("unitToMove not found");
            return;
        }
        if (unitToMove.isMoving)
        {
            return;
        }

        StartCoroutine(MoveUnitRoutine(unitToMove, direction));
        //SetGridElements(); // if this is called, grid will be updated before the movement to empty cell is completed. only works for empty cells!
    }

    private IEnumerator MoveUnitRoutine(Unit unitToMove, Vector2Int direction)
    {
        Vector2Int currentPosition = unitToMove.cellPosition;
        Vector2Int targetPosition = currentPosition + direction;

        while (true)
        {
            int targetCellValue = grid.GetValue(targetPosition);

            if (targetCellValue == (int)CellType.EmptyCell) // if empty, go
            {
                unitToMove.cellPosition = targetPosition;
                Vector3 targetWorldPosition = grid.GetWorldPosition(targetPosition);
                yield return unitToMove.MoveTo(targetWorldPosition);
                break; // break the loop finish the movement
            }
            else if (targetCellValue == -1) // out of bounds INVALID
            {
                Debug.LogWarning("Out of bounds");
                break; // break the loop finish the movement
            }
            else if (targetCellValue == (int)CellType.EnemyCell) // if enemy, attack and jump to next cell
            {
                Debug.Log("Attacking enemy");
                foreach (Unit enemy in units)
                {
                    if (enemy.type == Type.Enemy && enemy.cellPosition == targetPosition)
                    {
                        enemy.gameObject.SetActive(false);
                        grid.SetValue(targetPosition, (int)CellType.EmptyCell);
                        break; // breaks the foreach loop !!! not the while loop !!! 
                    }
                }
            }
            else if (targetCellValue == (int)CellType.RockCell) // if rock, jump over it to next cell
            {
                Debug.Log($"Jumping over rock at: {targetPosition.x}, {targetPosition.y}");
            }
            else
            {
                Debug.LogWarning("Unexpected cell type");
                break; // break the loop finish the movement
            }

            // update current position
            currentPosition = targetPosition;
            targetPosition = currentPosition + direction;

            // move to the next cell
            Vector3 nextWorldPosition = grid.GetWorldPosition(currentPosition);
            yield return unitToMove.MoveTo(nextWorldPosition); // start MoveTo coroutine and wait for it to finish

            // delay to make sure MoveTo has completed before continuing
            while (unitToMove.isMoving)
            {
                yield return null;
            }
        }
        SetGridElements();
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Vector2Int CheckAndSnap(Unit u)
    {
        Vector3 closestCell = grid.GetClosestCellWorldPosition(u.transform.position);
        u.transform.position = new Vector3(closestCell.x, u.transform.position.y, closestCell.z);
        u.cellPosition = grid.GetXY(closestCell);

        grid.SetValue(u.cellPosition, (int)u.type);

        return u.cellPosition;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SetGridElements()
    {
        grid.Clear();
        foreach (Unit unit in units)
        {
            if (unit == null || !unit.gameObject.activeInHierarchy)
            {
                continue;
            }
            grid.SetValue(unit.cellPosition, (int)unit.type);
            Debug.Log(unit.type + "to: " + unit.cellPosition);
        }
        Debug.Log("---");
    }
}
