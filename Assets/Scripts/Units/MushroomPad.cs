using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPad : Unit
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
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        Vector2Int playerTarget = cellPosition + direction;
        player.cellPosition = cellPosition;
        player.Move(playerTarget);
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
        Unit waterUnit= Instantiate(waterPrefab, transform.position, Quaternion.identity, transform.parent).GetComponent<Unit>();
        waterUnit.cellPosition = cellPosition;
        waterUnit.transform.rotation = transform.rotation;
        Destroy(gameObject);
    }
}
