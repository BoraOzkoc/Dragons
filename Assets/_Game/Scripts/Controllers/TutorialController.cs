using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private Transform _start, _end;
    [SerializeField] private Image _handImage;

    private void Start()
    {
        StartAnim();
    }

    private void OnEnable()
    {
        GameManager.GameStartedEvent += StopAnim;
    }

    private void OnDisable()
    {
        GameManager.GameStartedEvent -= StopAnim;
    }

    private void StopAnim()
    {
        _handImage.DOFade(0, 0.25f).SetDelay(0.25f);
        _handImage.transform.DOKill();
    }

    private void MoveToEnd()
    {
        _handImage.transform.DOMove(_end.position, 1).OnComplete(MoveToStart).SetEase(Ease.InQuad);
    }

    private void MoveToStart()
    {
        _handImage.transform.DOMove(_start.position, 1).OnComplete(MoveToEnd).SetEase(Ease.InQuad);
    }

    private void StartAnim()
    {
        _handImage.transform.DOMove(_start.position, 0);
        _handImage.DOFade(1, 0.1f);
        MoveToEnd();
    }
}