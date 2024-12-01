using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Sunflower : Unit
{
    Vector2Int direction;
    public AudioPackSO[] rotateAudioPacks;
    public GameObject rotateParticle;
    private void OnEnable()
    {
        Vector3 facingDir = transform.right;
        direction = new Vector2Int(Mathf.RoundToInt(facingDir.x), Mathf.RoundToInt(facingDir.z));
        onUnitMove += OnUnitMoved;
    }
    private void OnDestroy()
    {
        onUnitMove -= OnUnitMoved;
    }

    private void OnDisable()
    {
        onUnitMove -= OnUnitMoved;
    }

    private void OnUnitMoved(Unit mover, Vector2Int direction, Unit unitMovedTo)
    {
        if (unitMovedTo == null && mover.type == Type.Player) // only if the mover jumped on an empty cell
        {
            this.direction = direction;
            this.direction = new Vector2Int(Mathf.Clamp(this.direction.x, -1, 1), Mathf.Clamp(this.direction.y, -1, 1));
            StartCoroutine(RotateRoutine(direction));
        }
    }

    IEnumerator RotateRoutine(Vector2Int dir)
    {
        if (dir == Vector2Int.zero)
        {
            yield break;
        }
        yield return new WaitForSeconds(0.4f);

        if (rotateParticle != null)
        {
            Instantiate(rotateParticle, transform.position, Quaternion.identity);
        }
        foreach (AudioClip clip in AudioPackManager.GetRandomClipFromEachPack(rotateAudioPacks))
        {
            AudioManager.Instance.KillSFX(clip);
            AudioManager.Instance.PlaySFX(clip);
        }

        // slowly rotate, transform.right = new Vector3(dir.x, 0, dir.y)
        float t = 0;
        Quaternion start = transform.rotation;
        Debug.Log(dir);
        Quaternion end = Quaternion.LookRotation(new Vector3(-dir.y, 0, dir.x), Vector3.up);
        
        while (t < 1)
        {
            t += Time.deltaTime * 2;
            transform.rotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }

    }
    public override IEnumerator JumpingOn(Unit player)
    {
        yield return base.JumpingOn(player);
        player.JumpOnAnimation();
        //Debug.Log(player==null);
        yield return StartCoroutine(player.MoveTo(platform.grid.GetWorldPosition(cellPosition)));
    }
    public override void JumpedOn(Unit player)
    {
        base.JumpedOn(player);
        Vector2Int playerTarget = cellPosition + this.direction;
        player.cellPosition = cellPosition;
        player.Move(playerTarget);
    }

    public override void JumpedOff(Unit player)
    {
        base.JumpedOff(player);
    }
}
