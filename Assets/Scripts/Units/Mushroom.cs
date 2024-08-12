using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Mushroom : Unit
{

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
        // moveto animation may be added here
        player.Move(playerTarget);
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
    }
}
