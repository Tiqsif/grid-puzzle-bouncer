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
    private bool isDead = false;
    private void Awake()
    {
        Platform.onPlayerDeath += OnPlayerDeath;
        LevelManager.onLevelChange += OnLevelChange;
       
    }

    private void OnDestroy()
    {
        Platform.onPlayerDeath -= OnPlayerDeath;
        LevelManager.onLevelChange -= OnLevelChange;
    }

    private void Start()
    {
        levelNum.text = (LevelManager.Instance.currentLevelIndex + 1).ToString();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !isDead)
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

    void OnLevelChange()
    {
        levelNum.text = (LevelManager.Instance.currentLevelIndex + 1).ToString();
        SetDeathMenu(false);
        SetPauseMenu(false);
    }
    void OnPlayerDeath()
    {
        SetDeathMenu(true);
    }

    public void RestartGame()
    {
        AudioManager.Instance.PlayClick();
        SetDeathMenu(false);
        LevelManager.Instance.ReloadLevel();
    }

    void SetDeathMenu(bool activate)
    {
        deathMenu.gameObject.SetActive(activate);
        isDead = activate;
    }

    void SetPauseMenu(bool activate)
    {
        pauseMenu.gameObject.SetActive(activate);
        if (activate)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void ResumeGame()
    {
        AudioManager.Instance.PlayClick();
        SetPauseMenu(false);
    }

    public void MainMenu()
    {
        AudioManager.Instance.PlayClick();
        SetPauseMenu(false);
        LevelManager.Instance.LoadMainMenu();
    }
}
