using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtoiseAnimationHandler : MonoBehaviour
{
    public Animator animator;
    private Turtoise turtoise;
    public bool isPlaying;
    private bool shouldRotate = false;
    Transform parent;
    private void Start()
    {
        animator = GetComponent<Animator>();
        turtoise = GetComponentInParent<Turtoise>();
        parent = transform.parent;
    }

    private void Update()
    {
        if (shouldRotate)
        {
            // rotate the parent 
            parent.Rotate(Vector3.up, 360f * Time.deltaTime);

        }
    }

    public void Rotate(bool shouldRotate)
    {
        this.shouldRotate = shouldRotate;
    }
    public void Hide()
    {

        if (!animator) return;
        animator.SetFloat("Multiplier", 1f);
        animator.SetTrigger("Hide");
        StartCoroutine(UpdateFlagRoutine("TurtoiseHide"));
    }

    public void Show()
    {
        if (!animator) return;
        // reverse the hide animation
        animator.SetFloat("Multiplier", -1f);
        animator.SetTrigger("Hide");
        StartCoroutine(UpdateFlagRoutine("TurtoiseHide"));
    }
    public void Jump()
    {
        if (!animator) return;
        animator.SetTrigger("Jump");
        StartCoroutine(UpdateFlagRoutine("TurtoiseJump"));

        AudioManager.Instance.KillSFX(turtoise.jumpClip);
        AudioManager.Instance.PlaySFX(turtoise.jumpClip);
    }

    public void Fall()
    {
        if (!animator) return;
        animator.SetTrigger("Jump");
        StartCoroutine(UpdateFlagRoutine("TurtoiseJump"));
        AudioManager.Instance.KillSFX(turtoise.jumpClip);
        AudioManager.Instance.PlaySFX(turtoise.jumpClip);
        AudioManager.Instance.PlaySFX(turtoise.deathClip, 0.5f);
    }
   

    public void HalfJump()
    {
        if (!animator) return;
        animator.SetTrigger("HalfJump");
        StartCoroutine(UpdateFlagRoutine("TurtoiseHalfJump"));
        AudioManager.Instance.KillSFX(turtoise.jumpClip);
        AudioManager.Instance.PlaySFX(turtoise.jumpClip);
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
