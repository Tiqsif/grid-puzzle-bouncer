using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Unit
{
    public override IEnumerator JumpingOn(Unit player)
    {
        yield return base.JumpingOn(player);

        player.FallAnimation();
        yield return StartCoroutine(player.MoveTo(platform.grid.GetWorldPosition(cellPosition)));
    }
    public override void JumpedOn(Unit player)
    {
        base.JumpedOn(player);
        Vector2Int direction = cellPosition - player.cellPosition;
        player.cellPosition = cellPosition;
        StartCoroutine(player.FallTo(platform.grid.GetWorldPosition(cellPosition)));
        
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
    }
}
