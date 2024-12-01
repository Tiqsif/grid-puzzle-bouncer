using System.Collections;
using UnityEngine;

public class TiltedMushroom : Unit
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
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        Vector2Int playerTarget = cellPosition + direction;
        player.cellPosition = cellPosition;
        player.Move(playerTarget);
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
    }
}
