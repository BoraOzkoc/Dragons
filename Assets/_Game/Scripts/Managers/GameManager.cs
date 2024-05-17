using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action GameStartedEvent, LevelFailedEvent, LevelCompletedEvent;
    [SerializeField] private List<LevelController> levelList = new List<LevelController>();
    private LevelController _activeLevel;
    private bool _levelStarted, _levelFailed, _LevelCompleted;
    private string _level = "CurrentLevel";

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        PrepareNextLevel();
        SetFps();
    }

    private void DisableDebugger()
    {
        DebugManager.instance.enableRuntimeUI = false;
    }
    private void SetFps()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }

    public string GetLevelSaveName()
    {
        return _level;
    }

    public void StartGame()
    {
        if (!_levelStarted)
        {
            _levelStarted = true;
            GameStartedEvent?.Invoke();
        }
    }

    private void Update()
    {
        if (_levelStarted) return;
        if (Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    public void RepeatLevel()
    {
        int level = PlayerPrefs.GetInt(_level, 0);

        DOTween.KillAll();
        _activeLevel.GetDestroyed();
        PlayerPrefs.SetInt(_level, level);
        SpawnLevel(level);
    }

    public void NextLevel()
    {
        PrepareNextLevel();
    }


    private void PrepareNextLevel()
    {
        int level = PlayerPrefs.GetInt(_level, 0);

        if (_activeLevel)
        {
            DOTween.KillAll();
            _activeLevel.GetDestroyed();
            level++;
            PlayerPrefs.SetInt(_level, level);
        }

        SpawnLevel(level);
    }

    public int GetLevel()
    {
        int level = PlayerPrefs.GetInt(_level, 0);

        return level;
    }
    private void SpawnLevel(int level)
    {
        ResetStatus();
        if (level >= levelList.Count) level = Random.Range(3, levelList.Count);

        _activeLevel = Instantiate(levelList[level]);
    }

    public bool GameHasStarted()
    {
        return _levelStarted;
    }

    public void LevelFailed()
    {
        if (_LevelCompleted) return;
        _levelFailed = true;
        LevelFailedEvent?.Invoke();
    }

    public void LevelCompleted()
    {
        if (_levelFailed) return;
        _LevelCompleted = true;
        LevelCompletedEvent?.Invoke();
    }

    private void ResetStatus()
    {
        _levelStarted = false;
        _LevelCompleted = false;
        _levelFailed = false;
    }
}