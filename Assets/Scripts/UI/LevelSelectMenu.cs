using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelSelectMenu : MonoBehaviour
{
    public RectTransform frame;
    public GameObject levelButtonPrefab;
    public LevelData levelData;
    public int gameSceneIndex = 1;
    private void Start()
    {
        foreach (Transform child in frame)
        {
            Destroy(child.gameObject);
        }
        int sceneCount = levelData.allLevels.Count;
        Debug.Log("sceneCount: " + sceneCount);
        for (int i = 0; i < sceneCount; i++)
        {
            //Debug.Log("i: " + i);
            GameObject button = Instantiate(levelButtonPrefab, frame);
            button.TryGetComponent(out LevelButton buttonComponent);
            buttonComponent.SetButton(i, levelData, gameSceneIndex);
            button.GetComponentInChildren<TMP_Text>().text ="" + (i + 1);
        }
    }
   
}