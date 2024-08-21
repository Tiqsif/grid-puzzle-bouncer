
using System.Collections;
using UnityEngine;

public class Rock : Unit
{
    
    public override IEnumerator JumpingOn(Unit player)
    {
        yield return base.JumpingOn(player);
        Vector2Int direction = cellPosition - player.cellPosition;
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        player.BumpAnimation();
        yield return StartCoroutine(player.MoveTo(
           ( platform.grid.GetWorldPosition(cellPosition - direction) + platform.grid.GetWorldPosition(cellPosition) ) / 2
            ));
        StartCoroutine(player.MoveTo(
            platform.grid.GetWorldPosition(cellPosition - direction)
            ));
        //yield return new WaitForSeconds(.1f);
    }
    public override void JumpedOn(Unit player)
    {
        base.JumpedOn(player);
        Vector2Int direction = cellPosition - player.cellPosition;
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        Vector2Int target = cellPosition - direction;
        if (GetUnitAtPosition(player.cellPosition, player) == null) // coming from empty
        {
            //Debug.Log("to rock from Empty");
        }
        else // coming from another unit
        {
            //Debug.Log("to rock from NonEmpty");
            player.cellPosition = cellPosition;
            player.Move(target);

        }
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
    }
}
