using System.Collections;
using UnityEngine;

public class Turtoise : Unit
{
    Vector2Int direction;
    TurtoiseAnimationHandler animationHandler;
    public AudioClip jumpClip;
    public AudioClip deathClip;

    /*
    bool shouldMove = false;
    private void FixedUpdate()
    {
        if (isMoving)
        {
            return;
        }
        if (Time.time % 10 != 0)
        {
            return;
        }
        if (shouldMove && !isMoving)
        {
            Unit unitinfront = GetNonSelfUnitAtPosition(cellPosition + direction);
            if (unitinfront == null || unitinfront.isJumpable) // can jump on it
            {
                Debug.Log("A");
                this.Move(cellPosition + direction);
            }
            else // going to another unit
            {
                Debug.Log("B");
                shouldMove = false;
            }
        }

    }
    */

    private void Start()
    {
        animationHandler = GetComponentInChildren<TurtoiseAnimationHandler>();
    }
    public override IEnumerator JumpingOn(Unit player)
    {
        yield return base.JumpingOn(player);
        player.BumpAnimation();
        Vector2Int direction = cellPosition - player.cellPosition;
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        player.BumpAnimation();

        // moveto the middle of cellposition and the cell next to it
        yield return StartCoroutine(player.MoveTo(
           (platform.grid.GetWorldPosition(cellPosition - direction) + platform.grid.GetWorldPosition(cellPosition)) / 2
            ));
        // moveto the cell next to it
        StartCoroutine(player.MoveTo(
            platform.grid.GetWorldPosition(cellPosition - direction)
            ));
        player.cellPosition = cellPosition - direction;
        //yield return new WaitForSeconds(.1f);
    }
    public override void JumpedOn(Unit player)
    {
        base.JumpedOn(player);
        //shouldMove = true;
        Vector2Int direction = cellPosition - player.cellPosition;
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        Vector2Int target = cellPosition - direction;
        if (GetUnitAtPosition(player.cellPosition, player) == null) // coming from empty
        {
            //Debug.Log("to rock from Empty");
            player.cellPosition = cellPosition;
            player.Move(target);
        }
        else // coming from another unit
        {
            //Debug.Log("to rock from NonEmpty");
            player.cellPosition = cellPosition;
            player.Move(target);

        }
        //---
        //this.direction = direction;
        //StartCoroutine(ChainMove());
        // call this script's move instead of base.move
        animationHandler.Hide();
        animationHandler.Rotate(true);
        this.Move(cellPosition + direction);
         

    }
    public override void Move(Vector2Int targetPosition, float delay)
    {
        Vector2Int previousDir = direction;
        direction = targetPosition - cellPosition;
            Debug.Log("dreicti " + direction);
        if (direction == Vector2Int.zero)
        {
            direction = -previousDir;
        }
        base.Move(targetPosition, delay);

    }

    public override IEnumerator MoveRoutine(Vector2Int targetPosition, float delay)
    {
        yield return base.MoveRoutine(targetPosition, delay);
        if(GetNonSelfUnitAtPosition(targetPosition) == null) // empty
        {
            Move(targetPosition + direction);
        }
    }
    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
    }

    public override void JumpAnimation()
    {
        base.JumpAnimation();
        //animationHandler.Jump();
    }

    public override void JumpOnAnimation()
    {
        base.JumpOnAnimation();
        //animationHandler.HalfJump();
    }

    public override void JumpOffAnimation()
    {
        base.JumpOffAnimation();
        //animationHandler.AttackJump();
    }

    public override void BumpAnimation()
    {
        base.BumpAnimation();
        //animationHandler.Bump();
    }



    private IEnumerator ChainMove() // deprecated
    {
        Vector2Int nextPosition = cellPosition + direction;

        while (true)
        {
            Unit nextUnit = GetUnitAtPosition(nextPosition);

            if (nextUnit != null && !nextUnit.isJumpable)
            {
                break;
            }

            // Move to the next position
            yield return MoveRoutine(nextPosition, 0f);
            nextPosition = cellPosition + direction;
            /*
            if (!platform.IsInsideGrid(nextPosition))
            {
                break;
            }
            */
        }
    }

}
