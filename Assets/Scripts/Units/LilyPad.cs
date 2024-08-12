using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyPad : Unit
{
    [SerializeField] private GameObject waterPrefab;

    public override IEnumerator JumpingOn(Unit player)
    {
        yield return base.JumpingOn(player);

        player.JumpAnimation();
        yield return StartCoroutine(player.MoveTo(platform.grid.GetWorldPosition(cellPosition)));
    }
    public override void JumpedOn(Unit player)
    {
        base.JumpedOn(player);
        Vector2Int direction = cellPosition - player.cellPosition;
        Vector2Int playerTarget = cellPosition + direction;
        player.cellPosition = cellPosition;
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
        GameObject waterObj = Instantiate(waterPrefab, transform.position, Quaternion.identity, transform.parent);
        waterObj.GetComponent<Unit>().cellPosition = cellPosition;
        Destroy(gameObject);
    }
}
