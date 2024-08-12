using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip deathClip;

    private FrogAnimationHandler animationHandler;

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
