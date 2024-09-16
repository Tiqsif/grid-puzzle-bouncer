using System;
using System.Collections;
using UnityEngine;

public class BugAnimationHandler : MonoBehaviour
{
    public Vector2 rotateTimeMinMax;
    float rotateTime;
    float rotateTimer = 0;
    float rotateSpeed = 180f; // degrees per second
    bool isRotating = false;
    Transform parent;
    Animator animator;
    private void Start()
    {
        rotateTime = UnityEngine.Random.Range(rotateTimeMinMax.x, rotateTimeMinMax.y);
        rotateTimer = rotateTime;
        animator = GetComponent<Animator>();
        parent = transform.parent;
    }

    private void FixedUpdate()
    {
        if (rotateTimer > 0)
        {
            rotateTimer -= Time.deltaTime;
        }
        else
        {
            Rotate();
            
        }
    }

    public void Rotate()
    {
        if (!isRotating)
        {
            float angleToRotate = UnityEngine.Random.Range(-3, 4) * 45;
            //Debug.Log($"Rotating by {angleToRotate} degrees");
            animator.SetTrigger("Shake");
            StartCoroutine(RotateRoutine(angleToRotate));
            

        }
    }

    IEnumerator RotateRoutine(float angle)
    {
        if (isRotating)
        {
            yield break;
        }
        Vector3 startRotation = parent.eulerAngles;
        Vector3 endRotation = startRotation + new Vector3(0, angle, 0);
        float rotationDuration = Mathf.Abs(angle) / rotateSpeed; // time to complete the rotation
        //Debug.Log($"Rotation Duration: {rotationDuration} seconds");
        float elapsedTime = 0f;
        isRotating = true;
        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationDuration;
            parent.eulerAngles = Vector3.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        // ensure final rotation is exactly the end rotation
        parent.eulerAngles = endRotation;
        rotateTime = UnityEngine.Random.Range(rotateTimeMinMax.x, rotateTimeMinMax.y);
        rotateTimer = rotateTime;
        isRotating = false;
    }
}
