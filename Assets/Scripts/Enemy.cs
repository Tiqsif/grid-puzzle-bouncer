using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    private void Die(float time)
    {
        Destroy(gameObject, time);
    }

    public override void JumpedOn()
    {
        base.JumpedOn();
        Die(0.5f);
    }
}
