using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject _winGroup, _failGroup;
    [SerializeField] private CanvasGroup _canvasGroup;

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
        _canvasGroup.DOFade(0, 0);

    }

    private void GameCompletedEvent()
    {
        StartCoroutine(LevelCompletedEvent());
    }

    IEnumerator LevelCompletedEvent()
    {
        yield return new WaitForSeconds(3);
        ToggleGroups(true);
        _canvasGroup.DOFade(1, 1).OnComplete(() => ToggleCanvas(true));
    }

    private void ToggleCanvas(bool state)
    {
        _canvasGroup.interactable = state;
        _canvasGroup.blocksRaycasts = state;
    }

    private void ToggleGroups(bool levelWon)
    {
        _winGroup.SetActive(levelWon);
        _failGroup.SetActive(!levelWon);
    }

    private void GameFailedEvent()
    {
        ToggleGroups(false);
        _canvasGroup.DOFade(1, 1).OnComplete(() => ToggleCanvas(true));
    }
}