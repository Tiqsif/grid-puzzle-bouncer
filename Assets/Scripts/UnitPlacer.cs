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

    private void OnEnable()
    {
        UnitPlacer.onUnitPlacerSelected += OnPlacerSelected;
    }

    private void OnDisable()
    {
        UnitPlacer.onUnitPlacerSelected -= OnPlacerSelected;
    }
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        Physics.queriesHitTriggers = true;
    }

    private void OnPlacerSelected(Vector2Int cellPos)
    {
        if (cellPos == cellPosition)
        {
            isClicked = true;
            meshRenderer.material = clickedMaterial;
        }
        else
        {
            isClicked = false;
            meshRenderer.material = originalMaterial;
        
        }

    }

    private void OnMouseDown()
    {
        //Debug.Log("Mouse Down at" + cellPosition);
        AudioManager.Instance.PlayClick();
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
            if(isClicked) AudioManager.Instance.PlayClick();
            isClicked = false;
            if (meshRenderer != null)
            {
                meshRenderer.material = originalMaterial;
            }
        }
    }
}
