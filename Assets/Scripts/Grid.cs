using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    // x-axis, z-axis, and cell size
    int width;
    int height;
    float cellSize;

    int[,] gridArray;

    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new int[width, height];
    }


    // --- Get World Position ---
    public Vector3 GetWorldPosition(int x, int y) // NOT SECURED (no check if x and y are in the grid)
    {
        return new Vector3(x, 0, y) * cellSize;
    }

    /// <summary>
    /// Get the world position of a cell. NOT SECURED (no check if x and y are in the grid)
    /// </summary>
    /// <param name="xy"></param>
    /// <returns></returns>
    public Vector3 GetWorldPosition(Vector2Int xy)
    {
        return GetWorldPosition(xy.x, xy.y);
    }

    //--------------------------------

    
    public Vector2Int GetXY(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x / cellSize), Mathf.FloorToInt(worldPosition.z / cellSize));
    }

    //--------------------------------

    // ------ Set Value ------
    public void SetValue(int x, int y, int value) // platform.celltype as value
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
        }
    }
    public void SetValue(Vector2Int xy, int value)
    {
        SetValue(xy.x, xy.y, value);
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        Vector2Int xy = GetXY(worldPosition);
        SetValue(xy.x, xy.y, value);
    }

    // ------ Get Value ------
    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return -1;
        }
    }
    public int GetValue(Vector2Int xy)
    {
        return GetValue(xy.x, xy.y);
    }
    public int GetValue(Vector3 worldPosition)
    {
        Vector2Int xy = GetXY(worldPosition);
        return GetValue(xy.x, xy.y);
    }

    // --------------------------------


    public void DrawGrid()
    {
        for (int x = 0; x < width+1; x++) // draw vertical lines
        {
            Vector3 start = GetWorldPosition(x, 0) + new Vector3(-cellSize/2, 0, -cellSize/2);
            Vector3 end = GetWorldPosition(x, height) + new Vector3(-cellSize / 2, 0, -cellSize/2);
            Debug.DrawLine(start, end, Color.black);
        }

        for (int z = 0; z < height+1; z++) // draw horizontal lines
        {
            Vector3 start = GetWorldPosition(0, z) + new Vector3(-cellSize/2, 0, -cellSize / 2);
            Vector3 end = GetWorldPosition(width, z) + new Vector3(-cellSize/2, 0, -cellSize / 2);
            Debug.DrawLine(start, end, Color.black);
        }

    }

    public Vector3 GetClosestCellWorldPosition(Vector3 worldPosition)
    {
        Vector3 closest = Vector3.positiveInfinity;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 cellWorldPosition = GetWorldPosition(x, z);
                if (Vector3.Distance(worldPosition, cellWorldPosition) < Vector3.Distance(worldPosition, closest))
                {
                    closest = cellWorldPosition;
                }
            }
        }
        return closest;
    }

    public void Clear()
    {
        gridArray = new int[width, height];
    }
}
