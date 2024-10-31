using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandererParticle : MonoBehaviour
{
    public Transform parent;
    private Bounds bounds;
    private Vector3 target;
    private void Awake()
    {
        parent = transform.parent;

    }
    private void Start()
    {
        if (parent && parent.TryGetComponent(out BoxCollider parentCollider))
        {
            bounds = parentCollider.bounds;
        }
        RandomizeTarget();
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, target) < 0.1f)
        {
            RandomizeTarget();
        }
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime);
        
    }

    private void RandomizeTarget()
    {
        target = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
    }
}
