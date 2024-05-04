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

    public void CheckLocations()
    {
        int leftCount = 0, rightCount = 0;

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
            for (int j = 0; j < times; j++)
            {
                _dragonList[i].MoveRight();
            }
        }
    }

    private void MoveAllLeft(int times)
    {
        for (int i = 0; i < _dragonList.Count; i++)
        {
            for (int j = 0; j < times; j++)
            {
                _dragonList[i].MoveLeft();
            }
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
        CheckListNumber();
    }

    public void StartEndingProtocol(EndingFightController endingFightController)
    {
        StartCoroutine(EndingCoroutine(endingFightController));
    }

    IEnumerator EndingCoroutine(EndingFightController endingFightController)
    {
        _movementController.StopMovement();

        TriggerDragons(endingFightController);
        yield return new WaitForSeconds(1);
    }

    private void TriggerDragons(EndingFightController endingFightController)
    {
        StartCoroutine(DelayCoroutine(endingFightController));
    }

    IEnumerator DelayCoroutine(EndingFightController endingFightController)
    {
        for (int i = 0; i < _dragonList.Count; i++)
        {
            _dragonList[i].StartEndingProtocol(endingFightController);
            yield return new WaitForSeconds(0.3f);
        }
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

    private void ExtractFromList()
    {
    }

    public void RepositionList(List<DragonController> list)
    {
        List<DragonController> tempList = new List<DragonController>(_dragonList);
        DragonController closestDragon = FindClosest(tempList);

        //do something then remove
        tempList.Remove(closestDragon);
    }

    private void CheckListNumber()
    {
        if (_dragonList.Count <= 0 && _movementController.CanMove())
        {
            Debug.Log("fail");
            _movementController.StopMovement();
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
            StartCoroutine(MergeProtocol(dragon_1, dragon_2, 0.1f));
            return true;
        }
        else
        {
            dragon_2.JoinGroup();
            return false;
        }
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

        if (dragon_1_distance < dragon_2_distance)
        {
            yield return new WaitForSeconds(delay);
            dragon_2.GetDestroyed();
            dragon_1.Reposition();
            dragon_1.GetMerged();
        }
        else
        {
            yield return new WaitForSeconds(delay);
            dragon_1.GetDestroyed();
            dragon_2.Reposition();
            dragon_2.GetMerged();
        }
    }
}