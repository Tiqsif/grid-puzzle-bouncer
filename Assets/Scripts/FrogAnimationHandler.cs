using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAnimationHandler : MonoBehaviour
{
    public Animator animator;
    private Player player;
    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
    }
    public void Jump()
    {
        if(!animator) return;
        animator.SetTrigger("Jump");
        AudioManager.Instance.PlaySFX(player.jumpClip);
    }
    public void Fall()
    {
        if (!animator) return;
        animator.SetTrigger("AttackJump");
        AudioManager.Instance.PlaySFX(player.jumpClip);
        AudioManager.Instance.PlaySFX(player.deathClip, 0.5f);
    }
    public void AttackJump()
    {
        if (!animator) return;
        animator.SetTrigger("AttackJump");
        AudioManager.Instance.PlaySFX(player.jumpClip);
    }

    public void HalfJump()
    {
        if (!animator) return;
        animator.SetTrigger("HalfJump");
        AudioManager.Instance.PlaySFX(player.jumpClip);
    }
}
