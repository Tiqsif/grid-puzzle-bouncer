using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DynamicDepthOfField : MonoBehaviour
{
    Transform player;
    public Vector2 minMaxDist = new Vector2(7.4f, 9.4f);
    Volume volume;
    private float targetDoF;
    DepthOfField dofLayer;
    private void Start()
    {
        volume = GetComponentInChildren<Volume>();
        volume.profile.TryGet(out dofLayer);
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

    }
    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        else
        {
            float distance = Vector3.Distance(new Vector3(0,player.position.y, player.position.z), new Vector3(0, transform.position.y, transform.position.z)); // set x to zero to ignore the x axis
            // map the distance from minMaxDist to 2-20
            targetDoF = Mathf.Lerp(2, 20, Mathf.InverseLerp(minMaxDist.x, minMaxDist.y, distance));
            float dof = Mathf.Lerp(dofLayer.focusDistance.value, targetDoF, Time.deltaTime);
            dofLayer.focusDistance.value = dof;
            //Debug.Log(dof);

        }
        
    }
}
