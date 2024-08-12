using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    private void Die(float time)
    {
        Destroy(gameObject, time);
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
        Die(0.0f);
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
    }
}
