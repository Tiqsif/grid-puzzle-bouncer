using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacer : MonoBehaviour
{
    public Material hoverOverMaterial;
    public Material clickedMaterial;
    private Material originalMaterial;

    public Vector2Int cellPosition;
    public Transform rotationIndicator;
    public float rotationAngle = 0;
    private MeshRenderer meshRenderer;
    public bool isClicked = false;
    public delegate void OnUnitPlacerSelected(UnitPlacer selectedPlacer);
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

    private void OnPlacerSelected(UnitPlacer selectedPlacer)
    {
        if (selectedPlacer == this)
        {
            isClicked = true;
            meshRenderer.material = clickedMaterial;
            rotationIndicator.gameObject.SetActive(true);
            rotationAngle = 0;
            rotationIndicator.localEulerAngles = new Vector3(0, rotationAngle - 90, 0);

        }
        else
        {
            isClicked = false;
            meshRenderer.material = originalMaterial;
            rotationIndicator.gameObject.SetActive(false);
        
        }

    }

    private void OnMouseDown()
    {
        //Debug.Log("Mouse Down at" + cellPosition);
        AudioManager.Instance.PlayClick();
        onUnitPlacerSelected?.Invoke(this);
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
            rotationIndicator.gameObject.SetActive(false);
            if (meshRenderer != null)
            {
                meshRenderer.material = originalMaterial;
            }
        }
        /*
        if (isClicked && Input.GetKeyDown(KeyCode.E))
        {
            rotationAngle = (rotationAngle + 90) % 360;
            rotationIndicator.localEulerAngles = new Vector3(0, rotationAngle -90, 0);
        }
        */
    }
}
