using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public static TutorialController Instance;
    [SerializeField] private Transform _start, _end;
    [SerializeField] private Image _swerveHand, _tapHand;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
        else Instance = this;
    }

    private void Start()
    {
        StartAnim();
        _tapHand.DOFade(0, 0);
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
        _swerveHand.DOFade(0, 0.25f).SetDelay(0.25f);
        _swerveHand.transform.DOKill();
    }

    public void StartTapAnim()
    {
        _tapHand.DOFade(1, 0.25f).SetDelay(0.25f);
        MoveBetween();
    }

    public void StopTapAnim()
    {
        _tapHand.DOFade(0, 0.25f).SetDelay(0.25f);
        _tapHand.transform.DOKill();
    }

    private void MoveToEnd()
    {
        _swerveHand.transform.DOMove(_end.position, 1).OnComplete(MoveToStart).SetEase(Ease.InQuad);
    }

    private void MoveToStart()
    {
        _swerveHand.transform.DOMove(_start.position, 1).OnComplete(MoveToEnd).SetEase(Ease.InQuad);
    }

    private void MoveBetween()
    {
        Vector3 between = (_end.position + _start.position + (Vector3.up * 10)) / 2;
        _tapHand.DOFade(1, 0.25f).SetDelay(0.25f);

        _tapHand.transform.DOMove(between, 0);
        _tapHand.transform.DOScale(_swerveHand.transform.localScale * 1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    private void StartAnim()
    {
        _swerveHand.transform.DOMove(_start.position, 0);
        _swerveHand.DOFade(1, 0.1f);
        MoveToEnd();
    }
}