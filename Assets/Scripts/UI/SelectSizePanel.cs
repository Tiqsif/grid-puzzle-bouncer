using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSizePanel : MonoBehaviour
{
    public EditorPlatform editorPlatform;
    public TMPro.TextMeshProUGUI gridXText;
    public TMPro.TextMeshProUGUI gridYText;
    private void Awake()
    {
        editorPlatform = FindObjectOfType<EditorPlatform>();
        LevelManager.onLevelChange += OnLevelChanged;
    }

    public void IncreaseX()
    {
        if(editorPlatform.levelSpawner.levelData.currentLevel.gridSize.x >= 9)
        {
            return;
        }
        editorPlatform.levelSpawner.levelData.currentLevel.gridSize.x++;
        LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevelIndex);
    }

    public void DecreaseX()
    {
        if (editorPlatform.levelSpawner.levelData.currentLevel.gridSize.x <= 3)
        {
            return;
        }
        editorPlatform.levelSpawner.levelData.currentLevel.gridSize.x--;
        LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevelIndex);
    }

    public void IncreaseY()
    {
        if (editorPlatform.levelSpawner.levelData.currentLevel.gridSize.y >= 9)
        {
            return;
        }
        editorPlatform.levelSpawner.levelData.currentLevel.gridSize.y++;
        LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevelIndex);
    }

    public void DecreaseY()
    {
        if (editorPlatform.levelSpawner.levelData.currentLevel.gridSize.y <= 3)
        {
            return;
        }
        editorPlatform.levelSpawner.levelData.currentLevel.gridSize.y--;
        LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevelIndex);
    }

    private void OnLevelChanged()
    {
        gridXText.text = editorPlatform.levelSpawner.levelData.currentLevel.gridSize.x.ToString();
        gridYText.text = editorPlatform.levelSpawner.levelData.currentLevel.gridSize.y.ToString();

    }
}
