using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PropSpawner : MonoBehaviour
{
    public GameObject platformMesh; // not the prefab but the in scene object
    public Transform propParent;
    public bool debug = true;
    public float sidePropMaxOffset = 0.25f;
    [Range(0,100)] public int sidePropSpawnChance = 30;
    [Range(0,100)] public int topPropSpawnChance = 20;
    public float topPropMaxOffset = 0.15f;
    public List<GameObject> sidePropPrefabs;
    public List<GameObject> topPropPrefabs;
    
    
    private Camera mainCamera;
    private Vector3 origin = new Vector3(0, 0, 0);
    private List<Vector3> leftPropPositions = new List<Vector3>();
    private List<Vector3> rightPropPositions = new List<Vector3>();
    private List<Vector3> frontPropPositions = new List<Vector3>();
    private List<Vector3> backPropPositions = new List<Vector3>();

    private List<Vector3> topPropPositions = new List<Vector3>();

    private int propHeight = 2;
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (debug)
        {
            for (int i = 0; i < backPropPositions.Count; i++)
            {
                Debug.DrawLine(backPropPositions[i], backPropPositions[i] + Vector3.back, Color.red);
            }
            for (int i = 0; i < frontPropPositions.Count; i++)
            {
                Debug.DrawLine(frontPropPositions[i], frontPropPositions[i] + Vector3.forward, Color.red);
            }
            for (int i = 0; i < leftPropPositions.Count; i++)
            {
                Debug.DrawLine(leftPropPositions[i], leftPropPositions[i] + Vector3.left, Color.red);
            }
            for (int i = 0; i < rightPropPositions.Count; i++)
            {
                Debug.DrawLine(rightPropPositions[i], rightPropPositions[i] + Vector3.right, Color.red);
            }

            for (int i = 0; i < topPropPositions.Count; i++)
            {
                if (debug) Debug.DrawLine(topPropPositions[i], topPropPositions[i] + Vector3.up, Color.magenta);
            }

            
        }
    }
    public void CreatePlatform(LevelSO levelSO)
    {
        ClearProps();

        Vector2Int gridSize = levelSO.gridSize;
        float cellSize = levelSO.cellSize;
        // reshape the platform mesh
        platformMesh.transform.localScale = new Vector3(gridSize.x / 10f * cellSize, platformMesh.transform.localScale.y, gridSize.y / 10f * cellSize) ;

        // calculate the prop positions
        // top props
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 pos = origin + new Vector3(x * cellSize, 0, y * cellSize);
                topPropPositions.Add(pos);
            }
        }

        // side props
        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < propHeight; j++)
            {
                leftPropPositions.Add(origin + new Vector3(-cellSize / 2f,                                   -cellSize / 2f - (j * cellSize),      i * cellSize));
                rightPropPositions.Add(origin + new Vector3((gridSize.x * cellSize) - (cellSize / 2f),       -cellSize / 2f - (j * cellSize),      i * cellSize));
                
            }
        }

        // front and back props
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < propHeight; j++)
            {
                frontPropPositions.Add(origin + new Vector3(i * cellSize,          -cellSize / 2f - (j * cellSize),    (gridSize.y * cellSize) - (cellSize / 2f)));
                backPropPositions.Add(origin + new Vector3(i * cellSize,       -cellSize / 2f - (j * cellSize),    -cellSize / 2f));
            }
        }
        
        

        // spawn the props

        Random.InitState(levelSO.name.GetHashCode());
        // top props ------------------------------
        for (int i = 0; i < topPropPositions.Count; i++)
        {
            if (Random.Range(0, 100) >= topPropSpawnChance)
            {
                continue;
            }
            float offset = Random.Range(-topPropMaxOffset, topPropMaxOffset);
            Vector3 pos = topPropPositions[i] + new Vector3(offset, 0, offset);
            Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
            GameObject prop = Instantiate(topPropPrefabs[Random.Range(0, topPropPrefabs.Count)], pos, rot, propParent);
            prop.transform.localScale *= 0.1f;
        }

        // side props ------------------------------
        // left
        for (int i = 0; i < leftPropPositions.Count; i++)
        {
            if (Random.Range(0, 100) >= sidePropSpawnChance)
            {
                continue;
            }
            float offset = Random.Range(-sidePropMaxOffset, sidePropMaxOffset);
            Vector3 pos = leftPropPositions[i] + new Vector3(0, offset, offset);
            Quaternion rot = Quaternion.Euler(0, 0, 60);
            GameObject prop = Instantiate(sidePropPrefabs[Random.Range(0, sidePropPrefabs.Count)], pos, rot, propParent);
            prop.transform.localScale *= 0.2f;
        }

        // right
        for (int i = 0; i < rightPropPositions.Count; i++)
        {
            if (Random.Range(0, 100) >= sidePropSpawnChance)
            {
                continue;
            }
            float offset = Random.Range(-sidePropMaxOffset, sidePropMaxOffset);
            Vector3 pos = rightPropPositions[i] + new Vector3(0, offset, offset);
            Quaternion rot = Quaternion.Euler(0, 0, -60);
            GameObject prop = Instantiate(sidePropPrefabs[Random.Range(0, sidePropPrefabs.Count)], pos, rot, propParent);
            prop.transform.localScale *= 0.2f;
        }

        // front
        for (int i = 0; i < frontPropPositions.Count; i++)
        {
            if (Random.Range(0, 100) >= sidePropSpawnChance)
            {
                continue;
            }
            float offset = Random.Range(-sidePropMaxOffset, sidePropMaxOffset);
            Vector3 pos = frontPropPositions[i] + new Vector3(offset, offset, 0);
            Quaternion rot = Quaternion.Euler(60, 0, 0);
            GameObject prop = Instantiate(sidePropPrefabs[Random.Range(0, sidePropPrefabs.Count)], pos, rot, propParent);
            prop.transform.localScale *= 0.2f;
        }

        // back
        for (int i = 0; i < backPropPositions.Count; i++)
        {
            if (Random.Range(0, 100) >= sidePropSpawnChance)
            {
                continue;
            }
            float offset = Random.Range(-sidePropMaxOffset, sidePropMaxOffset);
            Vector3 pos = backPropPositions[i] + new Vector3(offset, offset, 0);
            Quaternion rot = Quaternion.Euler(-60, 0, 0);
            GameObject prop = Instantiate(sidePropPrefabs[Random.Range(0, sidePropPrefabs.Count)], pos, rot, propParent);
            prop.transform.localScale *= 0.2f;
        }

        mainCamera.transform.position = new Vector3((gridSize.x - 1) / 2f, (gridSize.y + 9f) / 2f, mainCamera.transform.position.z);
        mainCamera.TryGetComponent(out CameraSpin cameraSpin);
        // sping the camera with the middle of the platform as pivot point
        Vector3 pivot = new Vector3((gridSize.x - 1) / 2f, 0, (gridSize.y - 1) / 2f);
        //Debug.Log(pivot);
        cameraSpin.Spin(pivot);
    }

    public void ClearProps()
    {
        leftPropPositions = new List<Vector3>();
        rightPropPositions = new List<Vector3>();
        frontPropPositions = new List<Vector3>();
        backPropPositions = new List<Vector3>();

        topPropPositions = new List<Vector3>();
        foreach (Transform child in propParent)
        {
            Destroy(child.gameObject);
        }
    }
}
