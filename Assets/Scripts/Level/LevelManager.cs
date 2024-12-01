using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int gameSceneIndex = 1;
    public LevelData levelData;
    public LevelSpawner levelSpawner;
    [HideInInspector] public int currentLevelIndex = 0;
    static LevelManager _instance;

    public delegate void OnLevelChange();
    public static event OnLevelChange onLevelChange;
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
        levelSpawner = GetComponent<LevelSpawner>();
    }

    private void Start()
    {
        LoadLevel(levelData.allLevels.IndexOf(levelData.currentLevel));
        
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

    // same (game) scene level change methods ------------------------------------------------
    public void LoadLevel(int level)
    {
        Debug.Log("Loading level: " + level);
        if (level < 0)
        {
            level = 0;
        }
        if (level > levelData.allLevels.Count - 1)
        {
            level = levelData.allLevels.Count - 1;
        }
        currentLevelIndex = level;
        levelData.currentLevel = levelData.allLevels[currentLevelIndex];
        levelSpawner.SpawnLevel();
        onLevelChange?.Invoke();
    }

    public void LoadNextLevel()
    {
        LoadLevel(currentLevelIndex + 1);
    }

    public void LoadPreviousLevel()
    {
        LoadLevel(currentLevelIndex - 1);
    }

    public void ReloadLevel()
    {
        LoadLevel(currentLevelIndex);
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

    // build scene change methods ------------------------------------------------
    public void LoadLevelSelect()
    {
        //SceneManager.LoadScene(1);
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }
}
