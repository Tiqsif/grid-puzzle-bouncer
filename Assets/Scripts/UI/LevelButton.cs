using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;

    public void SetLevelIndex(int i)
    {
        levelIndex = i;
    }
    public void LoadLevel()
    {
        Debug.Log("Loading level " + levelIndex);
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelIndex);
    }
}
