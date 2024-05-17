using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    [SerializeField] private GameObject _winGroup, _failGroup;
    [SerializeField] private CanvasGroup _levelStateCanvasGroup, _levelCanvasGroup;
    [SerializeField] private TextMeshProUGUI _levelText;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    private void OnEnable()
    {
        GameManager.GameStartedEvent += GameStartedEvent;
        GameManager.LevelCompletedEvent += GameCompletedEvent;
        GameManager.LevelFailedEvent += GameFailedEvent;
    }

    private void OnDisable()
    {
        GameManager.GameStartedEvent -= GameStartedEvent;
        GameManager.LevelCompletedEvent -= GameCompletedEvent;
        GameManager.LevelFailedEvent -= GameFailedEvent;
    }

    private void Start()
    {
    }

    private void GameStartedEvent()
    {
    }

    public void Reset()
    {
        _winGroup.SetActive(false);
        _failGroup.SetActive(false);
        _levelStateCanvasGroup.DOFade(0, 0);

    }

    public void SetLevelText()
    {
        int level = GameManager.Instance.GetLevel();
        _levelText.text = "Level " + level;
    }
    private void GameCompletedEvent()
    {
        StartCoroutine(LevelCompletedEvent());
    }

    IEnumerator LevelCompletedEvent()
    {
        yield return new WaitForSeconds(3);
        ToggleGroups(true);
        _levelStateCanvasGroup.DOFade(1, 1).OnComplete(() => ToggleCanvas(true));
    }

    private void ToggleCanvas(bool state)
    {
        _levelStateCanvasGroup.interactable = state;
        _levelStateCanvasGroup.blocksRaycasts = state;
    }

    private void ToggleGroups(bool levelWon)
    {
        _winGroup.SetActive(levelWon);
        _failGroup.SetActive(!levelWon);
    }

    private void GameFailedEvent()
    {
        ToggleGroups(false);
        _levelStateCanvasGroup.DOFade(1, 1).OnComplete(() => ToggleCanvas(true));
    }
}