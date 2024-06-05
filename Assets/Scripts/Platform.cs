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
    public List<Enemy> enemies;
    private List<Unit> units;
    public enum CellType
    {
        Empty,
        Player,
        Enemy,
        Rock
    }

    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            grid.DrawGrid();
        }
    }
    void Start()
    {
        grid = new Grid(width, height, cellSize);
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
        //grid.SetValue(1, 1, (int)CellType.Player);
    }

    void Update()
    {
        if (grid == null)
        {
            grid = new Grid(width, height, cellSize);
        }
        units = new List<Unit>();

        if (!Application.IsPlaying(this)) // if the game is not running (meaning its in the editor)
        {

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
                Snap(unit);
            }
        }

        if (player != null)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                MovePlayer(Vector2Int.up);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                MovePlayer(Vector2Int.left);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                MovePlayer(Vector2Int.down);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                MovePlayer(Vector2Int.right);
            }
        }
    }

    public void MovePlayer(Vector2Int direction)
    {
        if (player == null)
        {
            Debug.LogWarning("Player not found");
            return;
        }
        
        Vector2Int targetPosition = player.cellPosition + direction;
        if (grid.GetValue(targetPosition) == (int)CellType.Empty)
        {
            player.cellPosition = targetPosition;
            //player.MoveTo(grid.GetWorldPosition(targetPosition)); // maybe can move world positions in the units themselves
            Vector3 targetWorldPosition = grid.GetWorldPosition(targetPosition);
            player.transform.position = new Vector3(targetWorldPosition.x, player.transform.position.y, targetWorldPosition.z);
        }
    }

    public void Snap(Unit u)
    {
        Vector3 closestCell = grid.GetClosestCellWorldPosition(u.transform.position);
        u.transform.position = new Vector3(closestCell.x, u.transform.position.y, closestCell.z);
        u.cellPosition = grid.GetXY(closestCell);
    }
}
