using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacer : MonoBehaviour
{
    public Material hoverOverMaterial;
    private Material originalMaterial;

    public Vector2Int cellPosition;
    private MeshRenderer meshRenderer;

    public delegate void OnUnitPlacerSelected(Vector2Int cellPos);
    public static event OnUnitPlacerSelected onUnitPlacerSelected;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        Physics.queriesHitTriggers = true;
    }

    private void OnMouseDown()
    {
        Debug.Log("Mouse Down at" + cellPosition);
        onUnitPlacerSelected?.Invoke(cellPosition);
    }
    private void OnMouseOver()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material = hoverOverMaterial;

        }
        
    }
    private void OnMouseExit()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material = originalMaterial;
        }
    }
}
