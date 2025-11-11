using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Unit
{
    public GameObject filledWaterPrefab;
    public override IEnumerator JumpingOn(Unit jumper)
    {
        yield return base.JumpingOn(jumper);

        jumper.FallAnimation();
        yield return StartCoroutine(jumper.MoveTo(platform.grid.GetWorldPosition(cellPosition)));
    }
    public override void JumpedOn(Unit jumper)
    {
        base.JumpedOn(jumper);
        jumper.cellPosition = cellPosition;
        StartCoroutine(jumper.FallTo(platform.grid.GetWorldPosition(cellPosition)));
        if (jumper.type == Type.MovingRock)
        {
            // spawn filled water, destroy this water
            GameObject filledWaterObj = Instantiate(filledWaterPrefab, transform.position, Quaternion.identity, transform.parent);
            Unit filledWaterUnit = filledWaterObj.GetComponent<Unit>();
            filledWaterUnit.cellPosition = cellPosition;
            filledWaterUnit.transform.rotation = transform.rotation;
            Destroy(jumper.gameObject); // destroy the moving rock
            Destroy(gameObject);

        }
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
    }
}
