using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector2Int cellPosition;
    public Color color;
    public Type type;
    public AudioClip[] onJumpedClip;
    public float moveSpeed = 5f;
    private float currentSpeed;
    public Queue<Vector3> moveQueue = new Queue<Vector3>();
    public bool isJumpable = true;
    public bool isMoving = false;
    public bool isDead = false;
    protected Platform platform;
    private void Awake()
    {
        platform = FindObjectOfType<Platform>();
    }
    
    public void Move(Vector2Int targetPosition)
    {
        StartCoroutine(MoveRoutine(targetPosition));
    }
    public IEnumerator MoveRoutine(Vector2Int targetPosition)
    {
        /*
        if (Time.time - lastInputTime < inputDelay)
        {
            return;
        }
        lastInputTime = Time.time;
        */
        if (isDead)
        {
            yield break;
        }
        while (isMoving)
        {
            yield return null;
        }
        Debug.Log("Move");
        Vector2Int direction = targetPosition - cellPosition;
        Vector2 directionF = targetPosition - cellPosition;
        float mag = directionF.magnitude;
        currentSpeed = moveSpeed * mag;
        transform.right = new Vector3(direction.x, 0, direction.y);

        Unit targetUnit = GetNonPlayerUnitAtPosition(targetPosition);
        Unit fromUnit = GetNonPlayerUnitAtPosition(cellPosition);

        if (fromUnit != null) // non player unit jumped from
        {
            //JumpOffAnimation();
            fromUnit.JumpedOff(this);
            platform.SetGridElements();
        }
        if (targetUnit != null) // if a unit is in the target position
        {
            yield return targetUnit.JumpingOn(this);
            /*
            if (targetUnit.isJumpable)
            {
                JumpOnAnimation();
                yield return StartCoroutine(MoveTo(platform.grid.GetWorldPosition(targetPosition)));
            }
            else
            {
                BumpAnimation();
            }
            */
            platform.SetGridElements();
            targetUnit.JumpedOn(this);
        }
        else // if no unit is in the target position
        {
            cellPosition = targetPosition;
            if (platform.IsInsideGrid(cellPosition)) // if in the grid
            {
                JumpAnimation();
                yield return StartCoroutine(MoveTo(platform.grid.GetWorldPosition(cellPosition)));

            }
            else // if out of the grid
            {
                FallAnimation();
                yield return StartCoroutine(FallTo(platform.grid.GetWorldPosition(cellPosition)));
            }
        }


        platform.SetGridElements();
    }
    public IEnumerator MoveTo(Vector3 targetPosition)
    {
        if (isDead)
        {
            yield break;
        }
        while (isMoving)
        {
            yield return null;
        }
        targetPosition.y = transform.position.y; // keep the same height
        moveQueue.Enqueue(targetPosition);
        if (!isMoving)
        {
            yield return StartCoroutine(ProcessMoveQueue());
        }
    }

    private IEnumerator ProcessMoveQueue()
    {
        if (isDead)
        {
            yield break;
        }
        while (isMoving)
        {
            yield return null;
        }
        isMoving = true;
        while (moveQueue.Count > 0)
        {
            Vector3 targetPosition = moveQueue.Dequeue();
            Debug.Log("Moving from" + transform.position + " to " + targetPosition);
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
                yield return null;
            }
        }
        isMoving = false;
    }

    protected Unit GetUnitAtPosition(Vector2Int position)
    {
        
        if (platform == null || platform.units == null)
        {
            Debug.Log(platform.units == null);
            Debug.LogWarning("Platform not found");
            return null;
        }
        
        foreach (Unit unit in platform.units)
        {
            if (unit.cellPosition == position)
            {
                return unit;
            }
        }
        return null;
    }
    protected Unit GetNonPlayerUnitAtPosition(Vector2Int position)
    {
        if (platform == null || platform.units == null)
        {
            Debug.LogWarning("Platform not found");
            return null;
        }
        foreach (Unit unit in platform.units)
        {
            if (unit.cellPosition == position && unit.type != Type.Player)
            {
                return unit;
            }
        }
        return null;
    }

    public virtual IEnumerator JumpingOn(Unit player)
    {
        Debug.Log("Jumping on " + type);
        yield return null;
    }
    public virtual void JumpedOn(Unit player)
    {
        Debug.Log("Jumped on " + type);
        foreach (var clip in onJumpedClip)
        {
            //Debug.Log("Playing Audio: " + clip.name);
            AudioManager.Instance.KillSFX(clip);
            AudioManager.Instance.PlaySFX(clip);
        }
    }

    public virtual void JumpedOff(Unit player)
    {
        Debug.Log("Jumped off " + type);
    }

    public IEnumerator FallTo(Vector3 targetPosition)
    {
        // called when the player falls down to the water out of bounds
        if (isDead)
        {
            yield break;
        }
        
        moveQueue.Enqueue(targetPosition);
        yield return StartCoroutine(ProcessMoveQueue());
        targetPosition.y = transform.position.y - 4f;
        moveQueue.Enqueue(targetPosition);
        Debug.Log("Falling to " + targetPosition);
        yield return StartCoroutine(ProcessMoveQueue());
        if (TryGetComponent(out Player playerScript))
        {
            playerScript.Die();
        }
    }

    public virtual void JumpOnAnimation()
    {
        // animation for jumping on a unit
    }
    public virtual void JumpOffAnimation()
    {
        // animation for jumping off a unit
    }
    public virtual void JumpAnimation()
    {
        // animation for jumping forward
    }
    
    public virtual void BumpAnimation()
    {
        // animation for bumping into a wall
    }

    public virtual void FallAnimation()
    {
        // animation for falling down
    }
    /*
    public Vector2Int GetBounce(Vector2Int direction)
    {

        // Normalize the direction vector
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));

        if (direction == Vector2Int.up)
        {
            return bounceAmount;
        }
        else if (direction == Vector2Int.down)
        {
            return new Vector2Int(-bounceAmount.x, -bounceAmount.y);
        }
        else if (direction == Vector2Int.left)
        {
            return new Vector2Int(-bounceAmount.y, bounceAmount.x);
        }
        else if (direction == Vector2Int.right)
        {
            return new Vector2Int(bounceAmount.y, -bounceAmount.x);
        }

        // Default case (no rotation)
        return bounceAmount;
    }
    */
}