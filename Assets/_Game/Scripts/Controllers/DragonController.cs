using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DragonController : MonoBehaviour, ICollectable
{
    [SerializeField] private int _number;
    [SerializeField] private bool _isCaged;
    [SerializeField] private bool _isCollected;
    [SerializeField] private TextMeshPro _numberText;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField, ReadOnly] private DragonController _leftNode, _rightNode;

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

    private void Update()
    {
        CheckGround();
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckCollisions(other);
    }

    private void CheckCollisions(Collider other)
    {
        if (IsCaged() || !_isCollected) return;
        if (!other.TryGetComponent(out DragonController dragonController)) return;

        if (dragonController.IsCaged()) return;
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

    private void CheckGround()
    {
        if (!_isCollected) return;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5, _groundLayer))
        {
            Vector3 groundPoint = hit.point;
        }
        else
        {
            _dragonManager.RemoveFromList(this);
            StartFalling();
        }
    }

    private void StartFalling()
    {
        GetDestroyed(1);
        transform.SetParent(null);
        _isCollected = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = false;
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public bool IsCaged()
    {
        return _isCaged;
    }

    public void SetRightNode(DragonController dragonController)
    {
        _rightNode = dragonController;
    }

    public void SetLeftNode(DragonController dragonController)
    {
        _leftNode = dragonController;
    }

    private void CheckTouchPosition(DragonController dragonController)
    {
        Vector3 targetPos = transform.localPosition;
        if (dragonController.transform.position.x > transform.position.x) //Goes Right
        {
            targetPos.x += 1;
            dragonController.MoveToTarget(targetPos);
            SetRightNode(dragonController);
            dragonController.SetLeftNode(this);
            //_dragonManager.MoveListToLeft();
        }
        else //Goes Left
        {
            targetPos.x -= 1;
            dragonController.MoveToTarget(targetPos);

            SetLeftNode(dragonController);
            dragonController.SetRightNode(this);
            //_dragonManager.MoveListToRight();
        }
    }

    public void GetDestroyed(float seconds = 0)
    {
        if (_isCollected)
        {
            EmptyNodes();
            _dragonManager.RemoveFromList(this);
        }

        Destroy(gameObject, seconds);
    }

    public DragonController GetRightNode()
    {
        return _rightNode;
    }

    public DragonController GetLeftNode()
    {
        return _leftNode;
    }

    private void EmptyNodes()
    {
        if (_leftNode) _leftNode.SetRightNode(_rightNode);
        if (_rightNode) _rightNode.SetLeftNode(_leftNode);
        SetRightNode(null);
        SetLeftNode(null);
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