using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : FrogBase
{
    public bool inputEnabled = true;
    public delegate void OnPlayerDeath();
    public static event OnPlayerDeath onPlayerDeath;

    private void Awake()
    {
        animationHandler = GetComponentInChildren<FrogAnimationHandler>();
        
    }
    
    private void Update()
    {
        if (!isDead && !isMoving && inputEnabled && Time.timeScale > 0)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Move(cellPosition + Vector2Int.up);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Move(cellPosition + Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Move(cellPosition + Vector2Int.down);
            }
            else if (Input.GetKeyDown(KeyCode.D))
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
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        Vector2Int jumperTarget = cellPosition + direction;
        jumper.cellPosition = cellPosition;
        jumper.Move(jumperTarget);
    }

    public override void JumpedOff(Unit jumper)
    {
        base.JumpedOff(jumper);
       
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

    public override void JumpOffAnimation() // not used atm
    {
        base.JumpOffAnimation();
        //animationHandler.AttackJump();
    }

    public override void BumpAnimation()
    {
        if(inputEnabled) StartCoroutine(DisableInputFor(1f));
        base.BumpAnimation();
        //animationHandler.Bump();
    }

    /// <summary>
    /// Fall animation and then invoke Die
    /// </summary>
    public override void FallAnimation()
    {
        if(inputEnabled) StartCoroutine(DisableInputFor(1f));
        base.FallAnimation();
        //animationHandler.Fall();
    }


    private IEnumerator DisableInputFor(float delay)
    {
        inputEnabled = false;
        yield return new WaitForSeconds(delay);
        inputEnabled = true;
    }
}
