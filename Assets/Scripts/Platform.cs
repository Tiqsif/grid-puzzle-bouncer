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
    private float playerSpeed;
    private List<Unit> units;
    private int enemyCount;
    private bool isEnding;
    private bool isPlayerDead;
    private float inputDelay = 0.7f;
    private float lastInputTime;

    public delegate void OnPlayerDeath();
    public static event OnPlayerDeath onPlayerDeath;


    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            grid.DrawGrid();
        }

        foreach (Unit unit in units)
        {
            Color color = unit.color;
            Debug.DrawLine(GetWorldPosition(unit.cellPosition.x,unit.cellPosition.y), GetWorldPosition(unit.cellPosition.x, unit.cellPosition.y ) + Vector3.up, color);
            
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
                    playerSpeed = player.moveSpeed;
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
        if (unitToMove.isMoving || isEnding || isPlayerDead)
        {
            return;
        }
        if (Time.time - lastInputTime < inputDelay)
        {
            return;
        }
        lastInputTime = Time.time;
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
        player.moveSpeed = playerSpeed;

        //Vector2Int previousDirection = direction;
        //bool movingBackwards = false;
        Debug.Log("routine");
        while (true)
        {
            Debug.Log("while");
            bool isPlayer = false;
            FrogAnimationHandler frogAnimationHandler = null;
            if (unitToMove.TryGetComponent(out Player player))
            {
                isPlayer = true;
                frogAnimationHandler = unitToMove.GetComponentInChildren<FrogAnimationHandler>();

                while (frogAnimationHandler.isPlaying)
                {
                    yield return null;
                }
            }
            Unit unitJumpedOn = null;
            int targetCellValue = grid.GetValue(targetPosition);
            int jumpAmount;
            if (targetCellValue == (int)Type.Empty) // if empty, go ----------------------------------- EMPTY -----------------------------------
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
            else if (targetCellValue == -1) // out of bounds  ----------------------------------- OUT OF GRID -----------------------------------
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
            else // ------------------------------------------------------- IF UNIT ------------------------------------------------------
            {
                // ------------------------------------------------------- ALL UNITs ------------------------------------------------------
                
                foreach (Unit unit in units)
                {
                    if (unit.cellPosition == targetPosition)
                    {
                        unitJumpedOn = unit;
                        break; // not while but foreach
                    }
                }
                jumpAmount = unitJumpedOn.jumpAmount;
                if (frogAnimationHandler != null) // ----------------------------------- PLAYER ANIMATION -----------------------------------
                {
                    while (frogAnimationHandler.isPlaying)
                    {
                        yield return null;
                    }
                    if (unitJumpedOn.isJumpable)
                    {
                        frogAnimationHandler.HalfJump();
                    }
                    else
                    {
                        frogAnimationHandler.Bump();
                    }
                }
                unitJumpedOn.JumpedOn();

                // ------------------------------------------------------- /ALL UNITs ------------------------------------------------------

                if (targetCellValue == (int)Type.Enemy) // if enemy jumps to next cell ----------------------------------- ENEMY -----------------------------------
                {
                    Debug.Log("Attacking enemy");
                    grid.SetValue(targetPosition, (int)Type.Empty);
                }
                else if (targetCellValue == (int)Type.Mushroom) // if mushroom jumps to next cell ----------------------------------- MUSHROOM -----------------------------------
                {
                    Debug.Log($"Jumping over mush at: {targetPosition.x}, {targetPosition.y}");
                }
                else if (targetCellValue == (int)Type.Flower) // ----------------------------------- FLOWER -----------------------------------
                {
                    Debug.Log("Jumping over flower");
                }
                else if (targetCellValue == (int)Type.Rock) // ----------------------------------- ROCK -----------------------------------
                {
                    Debug.Log("Headbumping the rock");
                    Unit unitJumpedFrom = null;
                    Debug.Log("current" + currentPosition);
                    Debug.Log("target" + targetPosition);
                    foreach (Unit unit in units)
                    {
                        if (unit.cellPosition == currentPosition)
                        {
                            unitJumpedFrom = unit;
                            break; // not while but foreach
                        }
                    }
                    if (unitJumpedFrom == null || unitJumpedFrom.jumpAmount <= 0) // from empty cell
                    {
                        Debug.Log("from empty break!");
                        unitToMove.cellPosition = currentPosition;

                        break; // break the loop finish the movement
                    }
                    else
                    {
                        Debug.Log("else change dir");
                        // Move back to the original position
                        direction = -direction;
                        // normalize direction
                        unitToMove.transform.right = new Vector3(direction.x, 0, direction.y);
                        currentPosition = targetPosition;
                        targetPosition = currentPosition + direction;

                        Vector3 next = grid.GetWorldPosition(targetPosition);
                        Debug.Log("TARGET:" + targetPosition);
                        //yield return unitToMove.MoveTo(next); // start MoveTo coroutine and wait for it to finish

                        Debug.Log("current" + currentPosition);
                    }
                    

                    //break;
                }
                else if (targetCellValue == (int)Type.Water) // ----------------------------------- WATER -----------------------------------
                {
                    Vector3 targetWorldPosition = grid.GetWorldPosition(targetPosition);
                    Debug.Log(targetWorldPosition);
                    yield return unitToMove.FallTo(targetWorldPosition);
                    Die();
                    break; // break the loop finish the movement
                }
                else if (targetCellValue == (int)Type.LilyPad) // ----------------------------------- LILY PAD -----------------------------------
                {
                    Debug.Log("Jumping on lily pad");

                }
                else if (targetCellValue == (int)Type.Player) // ----------------------------------- PLAYER -----------------------------------
                {
                    unitToMove.cellPosition = targetPosition;
                    Vector3 targetWorldPosition = grid.GetWorldPosition(targetPosition);
                    yield return unitToMove.MoveTo(targetWorldPosition);
                    Debug.LogWarning("Jumping on player");
                    break; // break the loop finish the movement
                }
                else // ----------------------------------- UNIDENTIFIED CELL -----------------------------------
                {
                    Debug.LogWarning("Unexpected cell type");
                    break; // break the loop finish the movement
                }
                
            }
            // --------------------------------------------------------- /UNIT ------------------------------------------------------

            // update current position

            // move to the next cell
            Vector3 nextWorldPosition = grid.GetWorldPosition(targetPosition);
            Debug.Log("TARGET:" + targetPosition);
            yield return unitToMove.MoveTo(nextWorldPosition); // start MoveTo coroutine and wait for it to finish

            // delay to make sure MoveTo has completed before continuing
            while (unitToMove.isMoving)
            {
                yield return null;
            }
            if (unitJumpedOn != null)
            {
                unitJumpedOn.JumpedOff();
            }
            player.moveSpeed = playerSpeed * jumpAmount;
            
            currentPosition = targetPosition;
            targetPosition = currentPosition + (direction * jumpAmount);

        } // /while
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
        u.transform.position = new Vector3(closestCell.x, closestCell.y, closestCell.z);
        u.cellPosition = grid.GetXY(closestCell);

        grid.SetValue(u.cellPosition, (int)u.type);

        return u.cellPosition;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SetGridElements()
    {
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
            //Debug.Log(unit.type + "to: " + unit.cellPosition);
        }
        //Debug.Log("Enemy count: " + enemyCount);
        //Debug.Log("---");
        
    }
    public Vector3 GetWorldPosition(int x, int y) // NOT SECURED (no check if x and y are in the grid)
    {
        return new Vector3(x, 0, y) * cellSize;
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
