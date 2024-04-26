using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DragonManager : MonoBehaviour
{
    [SerializeField] private List<DragonController> _dragonList = new List<DragonController>();
    [SerializeField] private MovementController _movementController;

    private void Start()
    {
        for (int i = 0; i < _dragonList.Count; i++)
        {
            _dragonList[i].JoinGroup();
            _dragonList[i].Init(this);
        }
    }

    public void AddToList(DragonController dragonController)
    {
        _dragonList.Add(dragonController);
        dragonController.Init(this);
        dragonController.transform.SetParent(transform);
        RearrangePositions();
    }

    public void RemoveFromList(DragonController dragonController)
    {
        if (!_dragonList.Contains(dragonController)) return;
        _dragonList.Remove(dragonController);
        CheckListNumber();
        RearrangePositions();
    }

    public void MoveListToLeft()
    {
        for (int i = 0; i < _dragonList.Count; i++)
        {
            _dragonList[i].transform.DOLocalMoveX(-0.5f, 0.1f).SetRelative(true);
        }
    }

    public void MoveListToRight()
    {
        for (int i = 0; i < _dragonList.Count; i++)
        {
            _dragonList[i].transform.DOLocalMoveX(0.5f, 0.1f).SetRelative(true);
        }
    }

    private void RearrangePositions()
    {
    }

    private void CheckListNumber()
    {
        if (_dragonList.Count <= 0)
        {
            Debug.Log("fail");
            _movementController.StopMovement();
        }
    }
}