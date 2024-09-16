using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
    public GameObject platformMesh; // not the prefab but the in scene object
    public List<GameObject> propPrefabs;
    public bool debug = true;
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
    public void CreatePlatform(Vector2Int gridSize, float cellSize)
    {
        platformMesh.transform.localScale = new Vector3(gridSize.x / 10f * cellSize, platformMesh.transform.localScale.y, gridSize.y / 10f * cellSize) ;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 pos = origin + new Vector3(x * cellSize, 0, y * cellSize);
                if(debug) Debug.DrawLine(pos, pos + Vector3.up, Color.magenta, 1000f);
            }
        }

        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < propHeight; j++)
            {
                leftPropPositions.Add(origin + new Vector3(-cellSize / 2f,                                   -cellSize / 2f - (j * cellSize),      i * cellSize));
                rightPropPositions.Add(origin + new Vector3((gridSize.x * cellSize) - (cellSize / 2f),       -cellSize / 2f - (j * cellSize),      i * cellSize));
                
            }
        }

        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < propHeight; j++)
            {
                frontPropPositions.Add(origin + new Vector3(i * cellSize,          -cellSize / 2f - (j * cellSize),    (gridSize.y * cellSize) - (cellSize / 2f)));
                backPropPositions.Add(origin + new Vector3(i * cellSize,       -cellSize / 2f - (j * cellSize),    -cellSize / 2f));
            }
        }
        
        if (debug)
        {
            for(int i = 0; i < backPropPositions.Count; i++)
            {
                Debug.DrawLine(backPropPositions[i], backPropPositions[i] + Vector3.back, Color.red, 1000f);
            }
            for (int i = 0; i < frontPropPositions.Count; i++)
            {
                Debug.DrawLine(frontPropPositions[i], frontPropPositions[i] + Vector3.forward, Color.red, 1000f);
            }
            for (int i = 0; i < leftPropPositions.Count; i++)
            {
                Debug.DrawLine(leftPropPositions[i], leftPropPositions[i] + Vector3.left, Color.red, 1000f);
            }
            for (int i = 0; i < rightPropPositions.Count; i++)
            {
                Debug.DrawLine(rightPropPositions[i], rightPropPositions[i] + Vector3.right, Color.red, 1000f);
            }
        }
        mainCamera.transform.position = new Vector3((gridSize.x - 1) / 2f, (gridSize.y + 5f) / 2f, mainCamera.transform.position.z);
    }
}
