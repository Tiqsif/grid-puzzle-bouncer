using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAnimationHandler : MonoBehaviour
{
    public Animator animator;
    private FrogBase frog;
    public bool isPlaying;
    public GameObject jumpParticle;
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

        PlayJumpAudio();
        if (jumpParticle != null)
        {
            Instantiate(jumpParticle, transform.parent.position, Quaternion.identity);
        }
            
    }

    public void Fall()
    {
        if (!animator) return;
        animator.SetTrigger("AttackJump");
        StartCoroutine(UpdateFlagRoutine("FrogAttackJump"));
        PlayJumpAudio();

        foreach (AudioClip clip in AudioPackManager.GetRandomClipFromEachPack(frog.deathAudioPacks))
        {
            AudioManager.Instance.PlaySFX(clip, 0.5f);
        }
    }
    public void AttackJump()
    {
        if (!animator) return;
        animator.SetTrigger("AttackJump");
        StartCoroutine(UpdateFlagRoutine("FrogAttackJump"));

        PlayJumpAudio();
    }

    public void HalfJump()
    {
        if (!animator) return;
        animator.SetTrigger("HalfJump");
        StartCoroutine(UpdateFlagRoutine("FrogHalfJump"));
        PlayJumpAudio();
       

    }
   

    public void Bump()
    {
        if (!animator) return;
        animator.SetTrigger("Bump");
        StartCoroutine(UpdateFlagRoutine("FrogBump"));
        //AudioManager.Instance.PlaySFX(player.bumpClip);
    }

    public void PlayJumpAudio()
    {
        foreach (AudioClip clip in AudioPackManager.GetRandomClipFromEachPack(frog.jumpAudioPacks))
        {
            AudioManager.Instance.KillSFX(clip);
            AudioManager.Instance.PlaySFX(clip);
        }
    }

    private IEnumerator UpdateFlagRoutine(string animationName)
    {
        isPlaying = true;
        //Debug.Log("Animation start: " + animationName);
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
        //Debug.Log("Animation finished: " + animationName);
    }
}
