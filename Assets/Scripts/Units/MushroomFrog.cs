using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomFrog : Unit
{
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip deathClip;

    private FrogAnimationHandler animationHandler;


    private void Start()
    {
        animationHandler = GetComponentInChildren<FrogAnimationHandler>();
    }
    

    public override IEnumerator JumpingOn(Unit player)
    {
        yield return base.JumpingOn(player);
        player.JumpOnAnimation();
        yield return StartCoroutine(player.MoveTo(platform.grid.GetWorldPosition(cellPosition)));
    }
    public override void JumpedOn(Unit player)
    {
        base.JumpedOn(player);
        Vector2Int direction = cellPosition - player.cellPosition;
        Vector2Int playerTarget = cellPosition + direction;
        player.cellPosition = cellPosition;
        player.Move(playerTarget);
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
        Vector3 facingDir = transform.right;
        Vector2Int direction = new Vector2Int(Mathf.RoundToInt(facingDir.x), Mathf.RoundToInt(facingDir.z));
        if (!isDead && !isMoving)
        {
            Debug.Log(platform.grid.GetValue(cellPosition + direction));
            Move(cellPosition + direction, 0.2f);
        }
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
