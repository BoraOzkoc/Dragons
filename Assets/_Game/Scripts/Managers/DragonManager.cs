using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lofelt.NiceVibrations;
using NaughtyAttributes;
using UnityEngine;

public class DragonManager : MonoBehaviour
{
    [SerializeField] private List<DragonController> _dragonList = new List<DragonController>();
    [SerializeField] private MovementController _movementController;

    [SerializeField] private float _attackSpeed;
    private bool _attackState;
    private Coroutine _attackCoroutine, _centerCoroutine;

    private void Start()
    {
        for (int i = 0; i < _dragonList.Count; i++)
        {
            _dragonList[i].Init(this);
            _dragonList[i].JoinGroup();
        }
    }

    private void OnEnable()
    {
        GameManager.GameStartedEvent += GameStartedEvent;
        GameManager.LevelCompletedEvent += StopAttack;
        GameManager.LevelFailedEvent += StopAttack;
    }

    private void OnDisable()
    {
        GameManager.GameStartedEvent -= GameStartedEvent;
        GameManager.LevelCompletedEvent -= StopAttack;
        GameManager.LevelFailedEvent -= StopAttack;
    }

    private void StopAttack()
    {
        _attackState = false;
        StopCoroutine(_attackCoroutine);
    }

    private void GameStartedEvent()
    {
        _attackState = true;
        _attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        while (_attackState)
        {
            for (int i = 0; i < _dragonList.Count; i++)
            {
                _dragonList[i].Attack();
            }

            AudioManager.Instance.PlayFire();
            yield return new WaitForSeconds(1);
        }
    }

    public void CheckLocations()
    {
        if (_dragonList.Count == 0) return;
        int leftCount = 0, rightCount = 0;
        if (_dragonList.Count == 1) _dragonList[0].MoveMiddle();

        for (int i = 0; i < _dragonList.Count; i++)
        {
            if (_dragonList[i].transform.localPosition.x > 0) rightCount++;
            else leftCount++;
        }

        if (leftCount < rightCount)
        {
            MoveAllRight(leftCount - rightCount);
        }
        else if (leftCount == rightCount)
        {
            //do nothing
        }
        else
        {
            MoveAllLeft(rightCount - leftCount);
        }
    }

    private void MoveAllRight(int times)
    {
        for (int i = 0; i < _dragonList.Count; i++)
        {
            _dragonList[i].MoveRight(times);
        }
    }

    private void MoveAllLeft(int times)
    {
        for (int i = 0; i < _dragonList.Count; i++)
        {
            _dragonList[i].MoveLeft(times);
        }
    }

    public void AddToList(DragonController dragonController)
    {
        _dragonList.Add(dragonController);
        dragonController.Init(this);
        dragonController.transform.SetParent(transform);
    }

    public void RemoveFromList(DragonController dragonController)
    {
        if (!_dragonList.Contains(dragonController)) return;
        _dragonList.Remove(dragonController);
        //CenterDragons();
        CheckListNumber();
    }

    public void StartEndingProtocol(EndingFightController endingFightController)
    {
        StartCoroutine(EndingCoroutine(endingFightController));
    }

    IEnumerator EndingCoroutine(EndingFightController endingFightController)
    {
        _movementController.StopMovement();
        StopAttack();
        TriggerDragons(endingFightController);
        yield return new WaitForSeconds(_attackSpeed);
    }

    private void TriggerDragons(EndingFightController endingFightController)
    {
        StartCoroutine(DelayCoroutine(endingFightController));
    }

    IEnumerator DelayCoroutine(EndingFightController endingFightController)
    {
        int totalDragonNum = _dragonList.Count;
        for (int i = 0; i < _dragonList.Count; i++)
        {
            _dragonList[i].StartEndingProtocol(endingFightController);
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.5f * (totalDragonNum));

        endingFightController.ActivateFight();
    }

    private void CenterDragons()
    {
        IEnumerator MoveDelay()
        {
            yield return new WaitForSeconds(0.5f);
            Vector3 center = CalculateCenter();

            float moveAmount = -center.x;
            MoveList(moveAmount);
        }

        if (_centerCoroutine != null) StopCoroutine(_centerCoroutine);
        _centerCoroutine = StartCoroutine(MoveDelay());
    }

    private void MoveList(float x_amount)
    {
        for (int i = 0; i < _dragonList.Count; i++)
        {
            _dragonList[i].Move(x_amount);
        }
    }

    private Vector3 CalculateCenter()
    {
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < _dragonList.Count; i++)
        {
            pos += _dragonList[i].transform.localPosition;
        }

        return pos / _dragonList.Count;
    }

    private DragonController FindClosest(List<DragonController> tempList)
    {
        int times = tempList.Count;
        float minDistance = 999;
        DragonController minDistancedDragon = null;
        for (int i = 0; i < times; i++)
        {
            Vector3 pos = _dragonList[i].transform.position;

            float distance = Vector3.Distance(pos, transform.position);
            if (distance <= minDistance)
            {
                minDistance = distance;
                minDistancedDragon = _dragonList[i];
            }
        }

        return minDistancedDragon;
    }

    private void CheckListNumber()
    {
        if (_dragonList.Count <= 0 && _movementController.CanMove())
        {
            _movementController.StopMovement();
            GameManager.Instance.LevelFailed();
        }
    }

    public void ConnectNodes(DragonController dragon_left, DragonController dragon_right)
    {
        if (dragon_left) dragon_left.SetRightNode(dragon_right);
        if (dragon_right) dragon_right.SetLeftNode(dragon_left);
    }

    public bool CheckIsSame(DragonController dragon_1, DragonController dragon_2)
    {
        if (dragon_1.GetNumber() == dragon_2.GetNumber())
        {
            StartCoroutine(MergeProtocol(dragon_1, dragon_2, 0.3f));
            return true;
        }
        else
        {
            dragon_2.Init(this);
            dragon_2.JoinGroup();
            CenterDragons();
            return false;
        }
    }

    public float GetFirstAnimTime()
    {
        return _dragonList[0].GetAnimationTime();
    }

    IEnumerator MergeProtocol(DragonController dragon_1, DragonController dragon_2, float delay)
    {
        dragon_1.ToggleBlock(true);
        dragon_2.ToggleBlock(true);

        Vector3 dragon_1_local_pos, dragon_2_local_pos;
        dragon_1_local_pos = dragon_1.transform.localPosition;
        dragon_2_local_pos = dragon_2.transform.localPosition;

        float dragon_1_distance, dragon_2_distance;
        dragon_1_distance = Vector3.Distance(dragon_1_local_pos, Vector3.zero);
        dragon_2_distance = Vector3.Distance(dragon_2_local_pos, Vector3.zero);

        if (!dragon_2.InGroup())
        {
            dragon_1.GetAttackController().SetAnimTime(GetFirstAnimTime());
            dragon_2.GetDestroyed();
            dragon_1.Reposition();
            dragon_1.GetMerged();
        }
        else if (dragon_1_distance < dragon_2_distance)
        {
            dragon_2.transform.DOLocalMove(dragon_1.transform.localPosition, delay - 0.1f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(delay);

            dragon_1.GetAttackController().SetAnimTime(GetFirstAnimTime());

            dragon_2.GetDestroyed();
            dragon_1.Reposition();
            dragon_1.GetMerged();
        }
        else if (dragon_1_distance >= dragon_2_distance)
        {
            dragon_1.transform.DOLocalMove(dragon_2.transform.localPosition, delay - 0.1f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(delay);
            dragon_1.GetAttackController().SetAnimTime(GetFirstAnimTime());

            dragon_1.GetDestroyed();
            dragon_2.Reposition();
            dragon_2.GetMerged();
        }
        CenterDragons();

    }
}