using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : FrogBase
{

    public delegate void OnPlayerDeath();
    public static event OnPlayerDeath onPlayerDeath;

    private void Start()
    {
        animationHandler = GetComponentInChildren<FrogAnimationHandler>();
    }
    private void Update()
    {
        if (!isDead && !isMoving)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Move(cellPosition + Vector2Int.up);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Move(cellPosition + Vector2Int.left);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Move(cellPosition + Vector2Int.down);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Move(cellPosition + Vector2Int.right);
            }

        }
    }
    
    public void Die()
    {
        if (isDead)
        {
            return;
        }
        Debug.Log("Player died");
        isDead = true;
        onPlayerDeath?.Invoke();
    }

    public override IEnumerator JumpingOn(Unit jumper)
    {
        yield return base.JumpingOn(jumper);
        jumper.JumpOnAnimation();
        yield return StartCoroutine(jumper.MoveTo(platform.grid.GetWorldPosition(cellPosition)));
    }
    public override void JumpedOn(Unit jumper)
    {
        base.JumpedOn(jumper);
        Vector2Int direction = cellPosition - jumper.cellPosition;
        Vector2Int playerTarget = cellPosition + direction;
        jumper.cellPosition = cellPosition;
        jumper.Move(playerTarget);
    }

    public override void JumpedOff(Unit jumper)
    {
        base.JumpedOff(jumper);
       
    }
    public override void JumpAnimation()
    {
        base.JumpAnimation();
        animationHandler.Jump();
    }

    public override void JumpOnAnimation()
    {
        base.JumpOnAnimation();
        animationHandler.HalfJump();
    }

    public override void JumpOffAnimation() // not used atm
    {
        base.JumpOffAnimation();
        animationHandler.AttackJump();
    }

    public override void BumpAnimation()
    {
        base.BumpAnimation();
        animationHandler.Bump();
    }

    /// <summary>
    /// Fall animation and then invoke Die
    /// </summary>
    public override void FallAnimation()
    {
        base.FallAnimation();
        animationHandler.Fall();
    }
}
