using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAnimationHandler : MonoBehaviour
{
    public Animator animator;
    private Player player;
    public bool isPlaying;
    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
    }
    public void Jump()
    {
        if(!animator) return;
        animator.SetTrigger("Jump");
        StartCoroutine(UpdateFlagRoutine("Jump"));
        AudioManager.Instance.PlaySFX(player.jumpClip);
    }

    public void Fall()
    {
        if (!animator) return;
        animator.SetTrigger("AttackJump");
        StartCoroutine(UpdateFlagRoutine("Jump"));
        AudioManager.Instance.PlaySFX(player.jumpClip);
        AudioManager.Instance.PlaySFX(player.deathClip, 0.5f);
    }
    public void AttackJump()
    {
        if (!animator) return;
        animator.SetTrigger("AttackJump");
        StartCoroutine(UpdateFlagRoutine("AttackJump"));
        AudioManager.Instance.PlaySFX(player.jumpClip);
    }

    public void HalfJump()
    {
        if (!animator) return;
        animator.SetTrigger("HalfJump");
        StartCoroutine(UpdateFlagRoutine("HalfJump"));
        AudioManager.Instance.PlaySFX(player.jumpClip);
    }
    public void HalfJump(int amount)
    {
        if (!animator) return;
        if (amount ==1 ) { 
            animator.SetTrigger("HalfJump");
            StartCoroutine(UpdateFlagRoutine("HalfJump"));
        }
        else if (amount == 2)
        {
            animator.SetTrigger("HalfDoubleJump");
            StartCoroutine(UpdateFlagRoutine("HalfDoubleJump"));
        }
        else 
        {
            Debug.LogWarning("Invalid amount for HalfJump");
        }
        AudioManager.Instance.PlaySFX(player.jumpClip);
    }

    public void Bump()
    {
        if (!animator) return;
        animator.SetTrigger("Bump");
        StartCoroutine(UpdateFlagRoutine("Bump"));
        //AudioManager.Instance.PlaySFX(player.bumpClip);
    }


    private IEnumerator UpdateFlagRoutine(string animationName)
    {
        isPlaying = true;
        Debug.Log("Playing " + animationName);
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < animator.GetCurrentAnimatorStateInfo(0).length)
        {
            yield return null;
        }
        isPlaying = false;
        Debug.Log("Finished " + animationName);
    }
}
