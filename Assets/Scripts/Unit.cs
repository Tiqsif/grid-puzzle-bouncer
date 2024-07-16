using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector2Int cellPosition;
    public Color color;
    public Type type;
    public AudioClip[] onJumpedClip;
    public float moveSpeed = 5f;
    public int jumpAmount = 1;
    public Queue<Vector3> moveQueue = new Queue<Vector3>();
    public bool isJumpable = true;
    public bool isMoving = false;
    public IEnumerator MoveTo(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y; // keep the same height
        moveQueue.Enqueue(targetPosition);
        if (!isMoving)
        {
            yield return StartCoroutine(ProcessMoveQueue());
        }
    }

    public virtual void JumpedOn()
    {
        foreach (var clip in onJumpedClip)
        {
            AudioManager.Instance.PlaySFX(clip,0.5f);
        }
        //Debug.Log("Jumped on");
    }

    public virtual void JumpedOff()
    {
        Debug.Log("Jumped off");
    }

    public IEnumerator FallTo(Vector3 targetPosition)
    {
        // called when the player falls down to the water out of bounds
        moveQueue.Enqueue(targetPosition);
        if (!isMoving)
        {
            yield return StartCoroutine(ProcessMoveQueue());
        }
        targetPosition.y = transform.position.y - 5f;
        moveQueue.Enqueue(targetPosition);
        
            yield return StartCoroutine(ProcessMoveQueue());
        
    }

    private IEnumerator ProcessMoveQueue()
    {
        isMoving = true;
        while (moveQueue.Count > 0)
        {
            Vector3 targetPosition = moveQueue.Dequeue();
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        isMoving = false;
    }
    
}
