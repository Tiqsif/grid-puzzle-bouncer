using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelect : MonoBehaviour
{
    public RectTransform unitPanel; // panel
    public RectTransform sizePanel; // select grid size panel
    public AllUnitsSO allUnitsSO; // all units in scriptable object
    public GameObject unitButtonPrefab;
    public GameObject emptyUnitButtonPrefab;
    //public Transform unitParent;
    public Button playStopButton;
    private GameObject[] unitPrefabs; // all units
    private List<Button> buttons = new List<Button>();

    private int selectedUnitIndex = -1;

    private Vector2Int currentCellPos;
    private UnitPlacer currentUnitPlacer;
    public delegate void OnUnitSelected(UnitPlacer selectedPlacer, int index);
    public static event OnUnitSelected onUnitSelected;

    public delegate void OnEditorPlayStopClicked();
    public static event OnEditorPlayStopClicked onEditorPlayStopClicked;

    public delegate void OnEditorSaveClicked();
    public static event OnEditorSaveClicked onEditorSaveClicked;

    private void OnEnable()
    {
        UnitPlacer.onUnitPlacerSelected += OnPlaceSelected;
        //Player.onPlayerDeath += OnDeath;
        EditorPlatform.onEditorIsPlayingUpdated += OnEditorIsPlayingUpdated;
    }

    private void OnDisable()
    {
        UnitPlacer.onUnitPlacerSelected -= OnPlaceSelected;
        //Player.onPlayerDeath -= OnDeath;
        EditorPlatform.onEditorIsPlayingUpdated -= OnEditorIsPlayingUpdated;
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
        GameObject emptyButton = Instantiate(emptyUnitButtonPrefab, unitPanel);
        emptyButton.GetComponent<Button>().onClick.AddListener(() => SelectUnit(-1));

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
        if (unitPanel.gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                unitPanel.gameObject.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.E) && selectedUnitIndex != -1)
            {
                if (currentUnitPlacer.isClicked)
                {
                    currentUnitPlacer.rotationAngle = (currentUnitPlacer.rotationAngle + 45) % 360;
                    currentUnitPlacer.rotationIndicator.localEulerAngles = new Vector3(0, currentUnitPlacer.rotationAngle - 90, 0);
                    SelectUnit(selectedUnitIndex);
                }
            }
            if (Input.GetKeyDown(KeyCode.X) && currentUnitPlacer.isClicked)
            {
                SelectUnit(-1);
            }
        }
    }
    private void SelectUnit(int index)
    {
        // set the selected unit to the unit prefab
        selectedUnitIndex = index;
        //Debug.Log(index);
        //unitPanel.gameObject.SetActive(false);
        onUnitSelected?.Invoke(currentUnitPlacer, selectedUnitIndex);
        AudioManager.Instance.PlayClick();

        //Debug.Log("Selected unit: " + unitPrefabs[selectedUnitIndex].name);
    }

    private void OnPlaceSelected(UnitPlacer selectedPlacer)
    {
        currentCellPos = selectedPlacer.cellPosition;
        currentUnitPlacer = selectedPlacer;
        unitPanel.gameObject.SetActive(true);
        FloatingTextManager.Instance.CreateFloatingText("E to rotate, X to delete", 0.5f, 1.5f, 1f);
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
            Texture2D texture = AssetPreview.GetAssetPreview(unitPrefabs[i]) ?? null;
            while (texture == null)
            {
                yield return new WaitForSeconds(0.05f);
                texture = AssetPreview.GetAssetPreview(unitPrefabs[i]) ?? null;
                yield return null;
            }
            buttons[i].image.sprite = TextureToSprite(texture);
        }
    }

    // ----- play/stop and save buttons -----
    public void EditorPlayStop()
    {
        Debug.Log("UnitSelect: Play/Stop");
        AudioManager.Instance.PlayClick();
        //TextMeshProUGUI text = playStopButton.GetComponentInChildren<TextMeshProUGUI>();
        //text.text = text.text == "Play" ? "Stop" : "Play";
        onEditorPlayStopClicked?.Invoke();
    }

    


    public void EditorSave()
    {
        Debug.Log("UnitSelect: Save");
        AudioManager.Instance.PlayClick();
        onEditorSaveClicked?.Invoke();
    }

    void OnEditorIsPlayingUpdated(bool isPlaying)
    {
        TextMeshProUGUI text = playStopButton.GetComponentInChildren<TextMeshProUGUI>();
        text.text = isPlaying ? "Stop" : "Play";
        sizePanel.gameObject.SetActive(!isPlaying);
    }
}
