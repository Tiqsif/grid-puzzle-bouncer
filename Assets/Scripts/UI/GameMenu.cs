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
    public int firsLevelIndex = 2;

    private bool isDead = false;
    private void Awake()
    {
        Player.onPlayerDeath += OnPlayerDeath;
        levelNum.text = (SceneManager.GetActiveScene().buildIndex - firsLevelIndex + 1).ToString(); // +1 because in build settings scenes start from zero
    }

    private void OnDestroy()
    {
        Player.onPlayerDeath -= OnPlayerDeath;
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
        isDead = activate;
    }

    void SetPauseMenu(bool activate)
    {
        pauseMenu.gameObject.SetActive(activate);
        Debug.Log("Pause Menu: " + activate);
    }

    public void ResumeGame()
    {
        SetPauseMenu(false);
    }

    public void MainMenu()
    {
        LevelManager.Instance.LoadMainMenu();
    }
}
