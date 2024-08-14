using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltedFlower : Unit
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
        Vector3 facingDir = transform.right;
        Vector2Int direction = new Vector2Int(Mathf.RoundToInt(facingDir.x), Mathf.RoundToInt(facingDir.z));
        direction *= 2; // double the distance
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
