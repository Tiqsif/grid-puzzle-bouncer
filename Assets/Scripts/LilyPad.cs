using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyPad : Unit
{
    [SerializeField] private GameObject waterPrefab;
    public override void JumpedOn()
    {
        base.JumpedOn();
    }

    public override void JumpedOff()
    {
        base.JumpedOff();
        GameObject waterObj = Instantiate(waterPrefab, transform.position, Quaternion.identity, transform.parent);
        waterObj.GetComponent<Unit>().cellPosition = cellPosition;
        Destroy(gameObject);
    }
}
