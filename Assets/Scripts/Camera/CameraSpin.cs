using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attach to camera
public class CameraSpin : MonoBehaviour
{
    public float spinDuration = 4f;
    [Range(1f, 10f)]public float resetSpeed = 2f;
    private bool spinning = false;
    Quaternion startRotation;
    Vector3 startPosition;
    public void Spin(Vector3 pivot)
    {
        if (spinning)
        {
            StopAllCoroutines();
            ResetTransform();
        }
        StartCoroutine(SpinRoutine(pivot));

    }

    private IEnumerator SpinRoutine(Vector3 pivotPoint)
    {
        startRotation = transform.rotation; // Initial rotation of the camera
        startPosition = transform.position; // Initial position of the camera

        float duration = spinDuration; // Total duration for a full rotation 
        float elapsed = 0f;
        Vector3 initialOffset = transform.position - pivotPoint; // Initial offset from the pivot
        Vector3 axis = Vector3.up; // Y-axis for horizontal rotation

        spinning = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration); // Smoothing factor

            // Calculate the angle smoothly
            float angle = Mathf.Lerp(0, 360, t);

            // Apply the rotation
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Vector3 newOffset = rotation * initialOffset;

            // Update the camera's position smoothly
            transform.position = pivotPoint + newOffset;
            transform.LookAt(pivotPoint);

            if (elapsed > 0.25f && Input.anyKey)
            {
                break;
            }
            yield return null;
        }
        // Ensure the camera finishes exactly at the 360-degree point
        while(Vector3.Distance(transform.position, pivotPoint + initialOffset) > 0.02f || Quaternion.Angle(transform.rotation, startRotation) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime * resetSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, Time.deltaTime * resetSpeed);
            yield return null;
        }
        ResetTransform();
        spinning = false;
    }

    private void ResetTransform()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
