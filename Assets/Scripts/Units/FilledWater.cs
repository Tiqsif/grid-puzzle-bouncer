using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilledWater : Unit
{

    public override IEnumerator JumpingOn(Unit player)
    {
        yield return base.JumpingOn(player);

        player.JumpAnimation();
        yield return StartCoroutine(player.MoveTo(platform.grid.GetWorldPosition(cellPosition)));
    }
    public override void JumpedOn(Unit player)
    {
        base.JumpedOn(player);
        player.cellPosition = cellPosition;
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
    }
}
