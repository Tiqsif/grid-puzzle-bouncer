using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //public int levelEditorMenuIndex = 1;
    //public int levelEditorSceneIndex = 2;
    public int levelMenuSceneIndex = 1;
    public void Play()
    {
        // button pressed from main menu
        //scenemanager current+1
        SceneManager.LoadScene(levelMenuSceneIndex);
    }
    public void LevelEditor() // scrapped for now
    {
        //SceneManager.LoadScene(levelEditorMenuIndex);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
