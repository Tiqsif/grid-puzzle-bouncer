using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject("LevelManager");
                    _instance = singleton.AddComponent<LevelManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LoadPreviousLevel();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            LoadNextLevel();
        }
        

    }

    public void LoadLevel(int level)
    {
        if (level < 0)
        {
            level = 0;
        }
        if (level >= SceneManager.sceneCountInBuildSettings -1)
        {
            level = SceneManager.sceneCountInBuildSettings-1;
        }
        SceneManager.LoadScene(level);
    }

    public void LoadNextLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadPreviousLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ReloadLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void LoadNextLevel(float delay)
    {
        StartCoroutine(LoadNextLevelDelay(delay));
    }

    IEnumerator LoadNextLevelDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextLevel();
    }
}
