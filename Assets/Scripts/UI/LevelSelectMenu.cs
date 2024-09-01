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
    public int firstLevelIndex = 4;
    private void Start()
    {
        foreach (Transform child in frame)
        {
            Destroy(child.gameObject);
        }
        int sceneCount = SceneManager.sceneCountInBuildSettings-1;
        Debug.Log("sceneCount: " + sceneCount);
        for (int i = firstLevelIndex; i < sceneCount; i++)
        {
            GameObject button = Instantiate(levelButtonPrefab, frame);
            button.GetComponent<LevelButton>().SetLevelIndex(i);
            button.GetComponentInChildren<TMP_Text>().text ="" + (i - firstLevelIndex + 1);
        }
    }
   
}