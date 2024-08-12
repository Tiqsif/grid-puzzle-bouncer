
using System.Collections;
using UnityEngine;

public class Rock : Unit
{
    public override IEnumerator JumpingOn(Unit player)
    {
        yield return base.JumpingOn(player);
        player.BumpAnimation();
        yield return new WaitForSeconds(.4f);
    }
    public override void JumpedOn(Unit player)
    {
        base.JumpedOn(player);
        Vector2Int target = player.cellPosition;
        if (GetNonPlayerUnitAtPosition(player.cellPosition) == null) // coming from empty
        {
            //Debug.Log("to rock from Empty");
        }
        else
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
