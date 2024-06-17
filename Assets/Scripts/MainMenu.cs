using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        // button pressed from main menu
        //scenemanager current+1
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
