using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelect : MonoBehaviour
{
    public RectTransform unitPanel; // panel
    public AllUnitsSO allUnitsSO; // all units in scriptable object
    public GameObject unitButtonPrefab;
    public Transform unitParent;
    
    private GameObject[] unitPrefabs; // all units
    private List<Button> buttons = new List<Button>();

    private int selectedUnitIndex = -1;

    private Vector2Int currentCellPos;
    public delegate void OnUnitSelected(Vector2Int cellPos, int index);
    public static event OnUnitSelected onUnitSelected;

    private void OnEnable()
    {
        UnitPlacer.onUnitPlacerSelected += OnPlaceSelected;
    }

    private void OnDisable()
    {
        UnitPlacer.onUnitPlacerSelected -= OnPlaceSelected;
    }
    private void Awake()
    {
        unitPanel.gameObject.SetActive(false);
        unitPrefabs = allUnitsSO.allUnits.ToArray();

        for (int i = 0; i < unitPrefabs.Length; i++)
        {
            int index = i;
            GameObject button = Instantiate(unitButtonPrefab, unitPanel);
            button.GetComponent<Button>().onClick.AddListener(() => SelectUnit(index));
            buttons.Add(button.GetComponent<Button>());

        }

        StartCoroutine(AssignSprites());
    }

    private void Update()
    {
        /*
        if (selectedUnitIndex != -1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 10);
                RaycastHit hit;
                Debug.Log("Raycast");
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Hit");
                    Instantiate(unitPrefabs[selectedUnitIndex], hit.point, Quaternion.identity, unitParent);
                    
                }
            }
        }
        */
    }
    private void SelectUnit(int index)
    {
        // set the selected unit to the unit prefab
        selectedUnitIndex = index;
        Debug.Log(index);
        unitPanel.gameObject.SetActive(false);
        onUnitSelected?.Invoke(currentCellPos, selectedUnitIndex);

        //Debug.Log("Selected unit: " + unitPrefabs[selectedUnitIndex].name);
    }

    private void OnPlaceSelected(Vector2Int cellPosition)
    {
        currentCellPos = cellPosition;
        unitPanel.gameObject.SetActive(true);
    }

    public Sprite TextureToSprite(Texture2D texture)
    {
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f); // Center pivot
        Sprite sprite = Sprite.Create(texture, rect, pivot);
        return sprite;
    }
    IEnumerator AssignSprites()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(unitPrefabs[i]);
            while (texture == null)
            {
                yield return new WaitForSeconds(0.05f);
                texture = AssetPreview.GetAssetPreview(unitPrefabs[i]);
                yield return null;
            }
            buttons[i].image.sprite = TextureToSprite(texture);
        }
    }
}
