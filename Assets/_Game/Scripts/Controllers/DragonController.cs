using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class DragonController : MonoBehaviour, ICollectable
{
    [SerializeField] private int _number;
    [SerializeField] private bool _isCaged;
    [SerializeField] private bool _groupJoined;
    [SerializeField] private TextMeshPro _numberText;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private AttackController _attackController;
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
        if (IsCaged() || !_groupJoined) return;
        if (!other.TryGetComponent(out DragonController dragonController)) return;

        if (dragonController.IsCaged()) return;
        if (_groupJoined)
        {
            if (dragonController._groupJoined) return;

            if (dragonController.GetNumber() == GetNumber())
            {
                dragonController.GetDestroyed();
                GetMerged();
            }
            else
            {
                dragonController.JoinGroup();

                _dragonManager.AddToList(dragonController);
                CheckTouchPosition(dragonController);
            }
        }
    }

    private void CheckGround()
    {
        if (!_groupJoined) return;
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
        _groupJoined = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        Collider collider = GetComponent<Collider>();
        Destroy(collider);
        rb.isKinematic = false;
        rb.useGravity = true;
        GetDestroyed(1);
        LeaveGroup();
        _attackController.ToggleAttack(false);
        transform.SetParent(null);
    }

    public bool IsCaged()
    {
        return _isCaged;
    }

    public void SetRightNode(DragonController dragonController)
    {
        if (_rightNode)
        {
            _rightNode.PushRightNode();
            dragonController.SetRightNode(_rightNode);
            _rightNode.SetLeftNodeOneTime(dragonController);
        }

        _rightNode = dragonController;
    }

    public void SetLeftNode(DragonController dragonController)
    {
        if (_leftNode)
        {
            _leftNode.PushLeftNode();
            dragonController.SetLeftNode(_leftNode);
            _leftNode.SetRightNodeOneTime(dragonController);
        }

        _leftNode = dragonController;
    }

    public void SetLeftNodeOneTime(DragonController dragonController)
    {
        _leftNode._rightNode = dragonController;
    }

    public void SetRightNodeOneTime(DragonController dragonController)
    {
        _rightNode._leftNode = dragonController;
    }

    public void PushRightNode()
    {
        transform.DOLocalMoveX(1, 0).SetRelative(true);
        if (_rightNode) _rightNode.PushRightNode();
    }

    public void PushLeftNode()
    {
        transform.DOLocalMoveX(-1, 0).SetRelative(true);
        if (_leftNode) _leftNode.PushLeftNode();
    }

    public void PullRightNode(int times)
    {
        transform.DOLocalMoveX(-0.5f, 0).SetRelative(true);
        if (_rightNode) _rightNode.PullRightNode(times);
    }

    
    public void PullLeftNode(int times)
    {
        transform.DOLocalMoveX(0.5f, 0).SetRelative(true);
        if (_leftNode) _leftNode.PullLeftNode(times);
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
        }
        else //Goes Left
        {
            targetPos.x -= 1;
            dragonController.MoveToTarget(targetPos);

            SetLeftNode(dragonController);
            dragonController.SetRightNode(this);
        }
    }

    public void GetDestroyed(float seconds = 0)
    {
        if (_groupJoined)
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
        //_leftNode.PullLeftNode();
        // _rightNode.PullRightNode();
        EmptyRightNode();
        EmptyLeftNode();
    }

    private void EmptyRightNode()
    {
        _rightNode = null;
    }

    private void EmptyLeftNode()
    {
        _leftNode = null;
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

    public void JoinGroup()
    {
        _groupJoined = true;
        _attackController.ToggleAttack(true);
    }

    private void LeaveGroup()
    {
        _groupJoined = false;
    }
}