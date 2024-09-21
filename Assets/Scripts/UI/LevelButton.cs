using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;
    LevelData levelData;
    int gameSceneIndex;
    /// <summary>
    /// Level index i starts from 0
    /// </summary>
    /// <param name="i"></param>
    public void SetButton(int levelIndex, LevelData levelData, int gameSceneIndex)
    {
        this.levelIndex = levelIndex;
        this.levelData = levelData;
        this.gameSceneIndex = gameSceneIndex;
    }
    
    public void LoadLevel()
    {
        levelData.currentLevel = levelData.allLevels[levelIndex];
        SceneManager.LoadScene(gameSceneIndex);
    }
}
