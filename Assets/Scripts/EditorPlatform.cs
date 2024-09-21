using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// scrapped for now, can be used in the future

//[ExecuteAlways]
public class EditorPlatform : Platform
{
    
    /*
    bool isPlaying = false;
    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            grid.DrawGrid();
        }

        foreach (Unit unit in units)
        {
            Color color = unit.color;
            Debug.DrawLine(GetWorldPosition(unit.cellPosition.x, unit.cellPosition.y), GetWorldPosition(unit.cellPosition.x, unit.cellPosition.y) + Vector3.up, color);
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void StartPlaying()
    {
        grid = new Grid(width, height, cellSize);
        units = new List<Unit>();
        if (Application.IsPlaying(this) || isPlaying) // if the game is running
        {
            foreach (Transform child in unitsHolder) // get all children of the object
            {
                units.Add(child.GetComponent<Unit>());
                if (child.TryGetComponent(out Player player))
                {
                    this.player = player;
                }
            }
            foreach (Unit unit in units)
            {
                CheckAndSnap(unit);
            }
        }
        SetGridElements();
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Update()
    {
        // if the game is NOT running (meaning its in the editor) -----------------------------------------------
        if (!Application.IsPlaying(this) || !isPlaying) // EDITOR
        {
            if (grid == null)
            {
                grid = new Grid(width, height, cellSize);
            }
            units = new List<Unit>();

            foreach (Transform child in unitsHolder) // get all children of the object
            {
                units.Add(child.GetComponent<Unit>());
                if (child.TryGetComponent(out Player player))
                {
                    this.player = player;
                }
            }

            // if an object is close to a cell in the grid of the platform, snap it to that cell
            foreach (Unit unit in units)
            {
                CheckAndSnap(unit);
            }
            SetGridElements();
        } // /EDITOR
        // -------------------------------------------------------------------------------------------------------------


        if (enemyCount == 0 && !player.isMoving && isPlaying) // if all enemies are cleared and player is not moving game is won
        {
            //Win();
        }


        grid.DrawGrid();
    }

   

    void Win()
    {
        if (isEnding || player.isDead || !isPlaying)
        {
            return;
        }
        AudioManager.Instance.PlaySFX(AudioManager.Instance.winClip);
        LevelManager.Instance.LoadNextLevel(2f);
        isEnding = true;
    }

    */
}
