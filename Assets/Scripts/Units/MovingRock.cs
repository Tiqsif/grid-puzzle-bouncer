using System.Collections;
using UnityEngine;

public class MovingRock : Unit
{
    
    public override IEnumerator JumpingOn(Unit jumper)
    {
        yield return base.JumpingOn(jumper);
        Vector2Int direction = cellPosition - jumper.cellPosition;
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        jumper.BumpAnimation();

        // moveto the middle of cellposition and the cell next to it
        yield return StartCoroutine(jumper.MoveTo(
           (platform.grid.GetWorldPosition(cellPosition - direction) + platform.grid.GetWorldPosition(cellPosition)) / 2
            ));
        // moveto the cell next to it
        StartCoroutine(jumper.MoveTo(
            platform.grid.GetWorldPosition(cellPosition - direction)
            ));
        jumper.cellPosition = cellPosition - direction;
        //yield return new WaitForSeconds(.1f);
    }
    public override void JumpedOn(Unit jumper)
    {
        base.JumpedOn(jumper);
        Vector2Int direction = cellPosition - jumper.cellPosition;
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        Vector2Int target = cellPosition - direction;
        if (GetUnitAtPosition(jumper.cellPosition, jumper) == null) // coming from empty
        {
            //Debug.Log("to rock from Empty");
            if (jumper.type == Type.Turtoise)
            {
                jumper.cellPosition = cellPosition;
                jumper.Move(target);

            }
        }
        else // coming from another unit
        {
            //Debug.Log("to rock from NonEmpty");
            jumper.cellPosition = cellPosition;
            jumper.Move(target);

        }
        Unit destinationUnit = GetUnitAtPosition(cellPosition + direction);
        Type destinationType = destinationUnit != null ? destinationUnit.type : Type.Empty;


        if (destinationType == Type.Empty) // going to empty
        {
            Move(cellPosition + direction);
        }
        else // going to another unit
        {
            if (destinationType == Type.Water || destinationType == Type.FilledWater)
            {
                Move(cellPosition + direction);

            }
        }
    }

    public override void JumpedOff(Unit jumper)
    {
        base.JumpedOff(jumper);
    }
}
