using System.Collections;
using UnityEngine;

public class MovingRock : Unit
{
    
    public override IEnumerator JumpingOn(Unit player)
    {
        yield return base.JumpingOn(player);
        player.BumpAnimation();
        Vector2Int direction = cellPosition - player.cellPosition;
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        player.BumpAnimation();

        // moveto the middle of cellposition and the cell next to it
        yield return StartCoroutine(player.MoveTo(
           (platform.grid.GetWorldPosition(cellPosition - direction) + platform.grid.GetWorldPosition(cellPosition)) / 2
            ));
        // moveto the cell next to it
        StartCoroutine(player.MoveTo(
            platform.grid.GetWorldPosition(cellPosition - direction)
            ));
        player.cellPosition = cellPosition - direction;
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


        if (GetUnitAtPosition(cellPosition + direction) == null) // going to empty
        {
            Move(cellPosition + direction);
        }
        else // going to another unit
        {
        }
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
    }
}
