using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DragonController : MonoBehaviour, ICollectable
{
    [SerializeField] private int _number;
    [SerializeField] private bool _isCaged;
    [SerializeField] private bool _isCollected;
    [SerializeField] private TextMeshPro _numberText;
    private DragonManager _dragonManager;

    private void Start()
    {
    }

    public void Init(DragonManager dragonManager)
    {
        _dragonManager = dragonManager;
    }

    private void OnValidate()
    {
        UpdateText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCaged() || !_isCollected) return;
        if (!other.TryGetComponent(out DragonController dragonController)) return;

        if(dragonController.IsCaged()) return;
        if (_isCollected)
        {
            if (dragonController._isCollected) return;

            if (dragonController.GetNumber() == GetNumber())
            {
                dragonController.GetDestroyed();
                GetMerged();
            }
            else
            {
                dragonController.GetCollected();

                _dragonManager.AddToList(dragonController);
                CheckTouchPosition(dragonController);
            }
        }
    }

    public bool IsCaged()
    {
        return _isCaged;
    }
    private void CheckTouchPosition(DragonController dragonController)
    {
        Vector3 targetPos = transform.localPosition;
        if (dragonController.transform.position.x > transform.position.x)
        {
            targetPos.x += 1;
            dragonController.MoveToTarget(targetPos);

            _dragonManager.MoveListToLeft();
        }
        else
        {
            targetPos.x -= 1;
            dragonController.MoveToTarget(targetPos);

            _dragonManager.MoveListToRight();
        }
    }

    public void GetDestroyed()
    {
        if (_dragonManager) _dragonManager.RemoveFromList(this);
        Destroy(gameObject);
    }

    public int GetNumber()
    {
        return _number;
    }

    public void GetMerged()
    {
        SetNumber(_number * 2);
    }

    private void SetNumber(int amount)
    {
        _number = amount;
        UpdateText();
    }

    private void UpdateText()
    {
        _numberText.text = _number.ToString();
    }

    public void GetCaged()
    {
        _isCaged = true;
    }

    public void GetFreed()
    {
        _isCaged = false;
    }

    public void MoveToTarget(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    public void GetCollected()
    {
        _isCollected = true;
    }
}