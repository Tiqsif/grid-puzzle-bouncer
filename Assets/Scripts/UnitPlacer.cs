using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacer : MonoBehaviour
{
    public Material hoverOverMaterial;
    public Material clickedMaterial;
    private Material originalMaterial;

    public Vector2Int cellPosition;
    private MeshRenderer meshRenderer;
    private bool isClicked = false;
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
        //Debug.Log("Mouse Down at" + cellPosition);
        isClicked = true;
        if (meshRenderer != null)
        {
            meshRenderer.material = clickedMaterial;
        }
        onUnitPlacerSelected?.Invoke(cellPosition);
    }
    private void OnMouseOver()
    {
        if (!isClicked && meshRenderer != null)
        {
            meshRenderer.material = hoverOverMaterial;

        }
        
    }
    private void OnMouseExit()
    {
        if (!isClicked && meshRenderer != null)
        {
            meshRenderer.material = originalMaterial;
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            isClicked = false;
            if (meshRenderer != null)
            {
                meshRenderer.material = originalMaterial;
            }
        }
    }
}
