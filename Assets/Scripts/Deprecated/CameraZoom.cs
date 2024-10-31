using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// deprecated
public class CameraZoom : MonoBehaviour
{
    [Tooltip("Camera will zoom to targets if no input is given in waitTime seconds")]
    public float waitTime = 5.0f;
    public Vector2 zoomInOutSpeed = new Vector2(1f,2f);
    public Transform[] targets;
    private float counter;
    private Camera mainCamera;
    private Vector3 initialPos;
    private bool isMoving = false;
    private void Awake()
    {
        mainCamera = Camera.main;
        initialPos = mainCamera.transform.position;
    }
    private void Start()
    {
        counter = waitTime;
        
    }
    
    void Update()
    {
        if (Input.anyKey)
        {
            counter = 5.0f;
            StopAllCoroutines();
            ResetTransform();
        }
        else
        {
            if (isMoving) return;
            counter -= Time.deltaTime;
            if (counter <= 0)
            {
                counter = waitTime;
                StartCoroutine(ZoomToTargets());
            }
        }
        
    }

    IEnumerator ZoomToTargets()
    {
        Enemy[] e = FindObjectsOfType<Enemy>();
        if (e.Length == 0)
        {
            Debug.Log("No enemies found");
            yield break;
        }
        targets = e.Select(x => x.transform).ToArray();

        isMoving = true;
        initialPos = mainCamera.transform.position;
        // zoom int and out to targets one by one
        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 targetPos = targets[i].position - mainCamera.transform.forward * 5;
            while (Vector3.Distance(mainCamera.transform.position, targetPos) > .25f)
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, Time.deltaTime * zoomInOutSpeed.x);
                yield return null;
            }
            Debug.Log("Zoomed to target " + i);
            while (Vector3.Distance(mainCamera.transform.position, initialPos) > 0.1f)
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, initialPos, Time.deltaTime * zoomInOutSpeed.y);
                yield return null;
            }
            Debug.Log("Zoomed back to initial position");
            yield return new WaitForSeconds(0.25f);
        }
        ResetTransform();
        isMoving = false;
    }

    void ResetTransform()
    {
        mainCamera.transform.position = initialPos;
    }
}
