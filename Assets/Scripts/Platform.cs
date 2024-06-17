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
    private List<Unit> units;
    private int enemyCount;
    private bool isEnding;
    private bool isPlayerDead;
    public delegate void OnPlayerDeath();
    public static event OnPlayerDeath onPlayerDeath;
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

        if (enemyCount == 0 && !player.isMoving) // if all enemies are cleared and player is not moving game is won
        {
            Win();
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
        // make the unit look at the direction it is moving
        unitToMove.transform.right = new Vector3(direction.x, 0, direction.y);
        if (unitToMove != null)
        {
            StartCoroutine(MoveUnitRoutine(unitToMove, direction));
        }
        //SetGridElements(); // if this is called, grid will be updated before the movement to empty cell is completed. only works for empty cells!
    }

    private IEnumerator MoveUnitRoutine(Unit unitToMove, Vector2Int direction)
    {
        Vector2Int currentPosition = unitToMove.cellPosition;
        Vector2Int targetPosition = currentPosition + direction;

        while (true)
        {
            bool isPlayer = false;
            FrogAnimationHandler frogAnimationHandler = null;
            if (unitToMove.TryGetComponent(out Player player))
            {
                isPlayer = true;
                frogAnimationHandler = unitToMove.GetComponentInChildren<FrogAnimationHandler>();
                

            }

            int targetCellValue = grid.GetValue(targetPosition);

            if (targetCellValue == (int)CellType.EmptyCell) // if empty, go
            {
                if (frogAnimationHandler != null)
                {
                    frogAnimationHandler.Jump();
                }

                unitToMove.cellPosition = targetPosition;
                Vector3 targetWorldPosition = grid.GetWorldPosition(targetPosition);
                yield return unitToMove.MoveTo(targetWorldPosition);
                break; // break the loop finish the movement
            }
            else if (targetCellValue == -1) // out of bounds INVALID
            {
                if (frogAnimationHandler != null)
                {
                    frogAnimationHandler.Fall();
                }
                Vector3 targetWorldPosition = grid.GetWorldPosition(targetPosition);
                Debug.Log(targetWorldPosition);
                yield return unitToMove.FallTo(targetWorldPosition);
                Die();
                break; // break the loop finish the movement
            }
            else if (targetCellValue == (int)CellType.EnemyCell) // if enemy, attack and jump to next cell
            {
                Debug.Log("Attacking enemy");
                foreach (Unit enemy in units)
                {
                    if (enemy.type == Type.Enemy && enemy.cellPosition == targetPosition)
                    {
                        if (frogAnimationHandler != null)
                        {
                            frogAnimationHandler.AttackJump();
                        }
                        enemy.GetComponent<Enemy>().Die(0.5f);
                        grid.SetValue(targetPosition, (int)CellType.EmptyCell);
                        break; // breaks the foreach loop !!! not the while loop !!! 
                    }
                }
            }
            else if (targetCellValue == (int)CellType.RockCell) // if rock, jump over it to next cell
            {
                if (frogAnimationHandler != null)
                {
                    frogAnimationHandler.HalfJump();
                }
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
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Die()
    {
        if (isPlayerDead)
        {
            return;
        }
        Debug.Log("Player died");
        isPlayerDead = true;
        onPlayerDeath?.Invoke();
    }
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
            //Debug.Log(unit.type + "to: " + unit.cellPosition);
        }
        Debug.Log("Enemy count: " + enemyCount);
        Debug.Log("---");
        
    }

    void Win()
    {
        if (isEnding || isPlayerDead)
        {
            return;
        }
        AudioManager.Instance.PlaySFX(AudioManager.Instance.winClip);
        LevelManager.Instance.LoadNextLevel(2f);
        isEnding = true;
    }
}
