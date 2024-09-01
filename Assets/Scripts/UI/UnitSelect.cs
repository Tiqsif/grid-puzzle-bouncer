using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelect : MonoBehaviour
{
    public RectTransform unitBar;
    public GameObject[] unitPrefabs;
    public GameObject unitButtonPrefab;
    public Transform unitParent;
    
    private List<Button> buttons = new List<Button>();

    private int selectedUnitIndex = -1;
    private void Awake()
    {
        for (int i = 0; i < unitPrefabs.Length; i++)
        {
            int index = i;
            GameObject button = Instantiate(unitButtonPrefab, unitBar);
            button.GetComponent<Button>().onClick.AddListener(() => SelectUnit(index));
            buttons.Add(button.GetComponent<Button>());

        }

        StartCoroutine(AssignSprites());
    }

    private void Update()
    {
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
    }
    private void SelectUnit(int index)
    {
        // set the selected unit to the unit prefab
        selectedUnitIndex = index;
        Debug.Log(index);
        Debug.Log("Selected unit: " + unitPrefabs[selectedUnitIndex].name);
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
