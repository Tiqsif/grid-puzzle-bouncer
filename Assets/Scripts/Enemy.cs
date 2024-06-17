using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public AudioClip[] deathClips;
    public void Die(float time)
    {
        foreach (var clip in deathClips)
        {
            AudioManager.Instance.PlaySFX(clip);
        }
        Destroy(gameObject, time);
    }
}
