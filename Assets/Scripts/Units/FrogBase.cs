using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogBase : Unit
{
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip deathClip;

    protected FrogAnimationHandler animationHandler;


    private void Awake()
    {
        animationHandler = GetComponentInChildren<FrogAnimationHandler>();
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
