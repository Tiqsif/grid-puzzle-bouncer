using System.Collections;
using UnityEngine;
using DG.Tweening;

// attach to camera
public class CameraStartMove : MonoBehaviour
{
    public bool On = true;
    public bool Snap = false;
    [Range(0.1f,10f)]public float moveDuration = 4f;
    [Range(0.1f, 2f)] public float fastMoveDuration = 0.5f;
    private Vector3 startPosition;
    [Tooltip("Cam rotation before movement")] public Vector3 startRotation;
    private Vector3 endPosition;
    [Tooltip("Cam rotation after movement")] public Vector3 endRotation;
    private bool isMoving = false;
    public Ease easeType = Ease.InOutSine;
  
    public void CamMove(Vector2Int gridSize)
    {
        if (!On) return;
        Debug.Log("CameraStartMove: CamMove: " + gridSize);

        Vector3 pivot = new Vector3((gridSize.x - 1) / 2f, 0, (gridSize.y-2f) / 2f);
        float max = Mathf.Max(gridSize.x, gridSize.y);
        // smooth distance, if max is smaller than 5 a little more distance, if bigger than 7 a little less distance
        float distance = max * 1.75f + 5f - Mathf.Clamp(max, 5f, 7f);
        if (isMoving)
        {
            // if already moving, restart from the beginning

            isMoving = false;
            DOTween.Kill(this);

        }

        startPosition = pivot + Quaternion.Euler(startRotation) * Vector3.forward * -distance;
        SetToStart();


        Vector3 targetDir = Quaternion.Euler(endRotation) * Vector3.forward * -distance;
        endPosition = pivot + targetDir;
        if (Snap)
        {
            transform.position = endPosition;
            transform.rotation = Quaternion.Euler(endRotation);
            return;
        }

        

        isMoving = true;
        // look at pivot
        // use tween to move and rotate the camera
        // when tweening if any input detected, stop the tween, snap to the end position and rotation
        Debug.Log("CameraMoveStart");
        Sequence seq = DOTween.Sequence();
        seq.SetId(this);
        seq.Append(transform.DOMove(endPosition, moveDuration).SetEase(easeType));
        seq.Join(transform.DORotateQuaternion(Quaternion.Euler(endRotation), moveDuration).SetEase(easeType));
        DOTween.To(() => 0f, x =>
        {
            transform.LookAt(pivot);
        }, 1f, moveDuration).SetEase(Ease.Linear).SetId(this);


        seq.OnComplete(() => { isMoving = false;
            Debug.Log("CameraMoveComplete");
        });
        seq.Play();

    }

    private IEnumerator DetectInput(Sequence seq)
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForEndOfFrame();
        while (isMoving)
        {
            if (Input.anyKeyDown)
            {
                // stop the tween
                DOTween.Kill(this);
                // snap to the end position and rotation
                transform.position = endPosition;
                transform.rotation = Quaternion.Euler(endRotation);
                isMoving = false;
                Debug.Log("CameraMoveSkipped");
                yield break;
            }
            yield return null;
        }
    }

    private void SetToStart()
    {
        transform.rotation = Quaternion.Euler(startRotation);
        transform.position = startPosition;
        isMoving = false;
    }
}
