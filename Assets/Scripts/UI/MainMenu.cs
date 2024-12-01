using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //public int levelEditorMenuIndex = 1;
    //public int levelEditorSceneIndex = 2;
    public int levelMenuSceneIndex = 1;
    public RectTransform mainMenu;
    public RectTransform levelSelect;
    private bool isMainMenuOpen = true;
    private bool isLevelSelectOpen = false;

    private void Start()
    {
        mainMenu.gameObject.SetActive(true);
        levelSelect.gameObject.SetActive(false);
    }
    public void Play()
    {
        AudioManager.Instance.PlayClick();
        // button pressed from main menu
        //scenemanager current+1
        //SceneManager.LoadScene(levelMenuSceneIndex);
        OpenLevelSelect();

    }
    public void LevelEditor() // scrapped for now
    {
        AudioManager.Instance.PlayClick();
        //SceneManager.LoadScene(levelEditorMenuIndex);
    }
    public void Quit()
    {
        AudioManager.Instance.PlayClick();
        Invoke("ExecuteQuit", 0.1f);
    }

    private void ExecuteQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Update()
    {
        if (isLevelSelectOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseLevelSelect();
        }
    }

    private void OpenLevelSelect()
    {
        levelSelect.gameObject.SetActive(true);
        isLevelSelectOpen = true;

        mainMenu.gameObject.SetActive(false);
        isMainMenuOpen = false;
    }
    private void CloseLevelSelect()
    {
        levelSelect.gameObject.SetActive(false);
        isLevelSelectOpen = false;

        mainMenu.gameObject.SetActive(true);
        isMainMenuOpen = true;
    }
}
