using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAnimationHandler : MonoBehaviour
{
    public Animator animator;
    private FrogBase frog;
    public bool isPlaying;
    private void Start()
    {
        animator = GetComponent<Animator>();
        frog = GetComponentInParent<FrogBase>();
    }
    public void Jump()
    {
        if(!animator) return;
        animator.SetTrigger("Jump");
        StartCoroutine(UpdateFlagRoutine("FrogJump"));

        AudioManager.Instance.KillSFX(frog.jumpClip);
        AudioManager.Instance.PlaySFX(frog.jumpClip);
    }

    public void Fall()
    {
        if (!animator) return;
        animator.SetTrigger("AttackJump");
        StartCoroutine(UpdateFlagRoutine("FrogAttackJump"));
        AudioManager.Instance.KillSFX(frog.jumpClip);
        AudioManager.Instance.PlaySFX(frog.jumpClip);
        AudioManager.Instance.PlaySFX(frog.deathClip, 0.5f);
    }
    public void AttackJump()
    {
        if (!animator) return;
        animator.SetTrigger("AttackJump");
        StartCoroutine(UpdateFlagRoutine("FrogAttackJump"));
        AudioManager.Instance.KillSFX(frog.jumpClip);
        AudioManager.Instance.PlaySFX(frog.jumpClip);
    }

    public void HalfJump()
    {
        if (!animator) return;
        animator.SetTrigger("HalfJump");
        StartCoroutine(UpdateFlagRoutine("FrogHalfJump"));
        AudioManager.Instance.KillSFX(frog.jumpClip);
        AudioManager.Instance.PlaySFX(frog.jumpClip);
    }
   

    public void Bump()
    {
        if (!animator) return;
        animator.SetTrigger("Bump");
        StartCoroutine(UpdateFlagRoutine("FrogBump"));
        //AudioManager.Instance.PlaySFX(player.bumpClip);
    }


    private IEnumerator UpdateFlagRoutine(string animationName)
    {
        isPlaying = true;
        Debug.Log("Animation start: " + animationName);
        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                break;
            }

            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            //Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            yield return null;
        }
        isPlaying = false;
        Debug.Log("Animation finished: " + animationName);
    }
}
