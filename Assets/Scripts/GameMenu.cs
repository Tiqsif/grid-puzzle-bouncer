using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public RectTransform deathMenu;
    public RectTransform pauseMenu;

    public TextMeshProUGUI levelNum;
    private void Awake()
    {
        Platform.onPlayerDeath += OnPlayerDeath;
        levelNum.text = (SceneManager.GetActiveScene().buildIndex ).ToString();
    }

    private void OnDestroy()
    {
        Platform.onPlayerDeath -= OnPlayerDeath;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.gameObject.activeSelf) // if the pause menu is active
            {
                SetPauseMenu(false);
            }
            else // if the pause menu is not active
            {
                SetPauseMenu(true);
            }
        }
    }

    void OnPlayerDeath()
    {
        SetDeathMenu(true);
    }

    public void RestartGame()
    {
        SetDeathMenu(false);
        LevelManager.Instance.ReloadLevel();
    }

    void SetDeathMenu(bool activate)
    {
        deathMenu.gameObject.SetActive(activate);
    }

    void SetPauseMenu(bool activate)
    {
        pauseMenu.gameObject.SetActive(activate);
        Debug.Log("Pause Menu: " + activate);
    }
}
