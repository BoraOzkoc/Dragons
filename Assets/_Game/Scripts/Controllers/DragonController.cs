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
        SetAttackDamage();
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

    public void PullRightNode()
    {
        transform.DOLocalMoveX(-0.5f, 0).SetRelative(true);
        if (_rightNode) _rightNode.PullRightNode();
    }

    public void PullLeftNode()
    {
        transform.DOLocalMoveX(0.5f, 0).SetRelative(true);
        if (_leftNode) _leftNode.PullLeftNode();
    }

    public void AddRightNode(DragonController dragonController)
    {
        if (_rightNode)
        {
            _rightNode.PushRightNode();
            _rightNode._leftNode = dragonController;
            dragonController._rightNode = _rightNode;
        }

        _rightNode = dragonController;
        dragonController._leftNode = this;
    }

    public void AddLeftNode(DragonController dragonController)
    {
        if (_leftNode)
        {
            _leftNode.PushLeftNode();
            _leftNode._rightNode = dragonController;
            dragonController._leftNode = _leftNode;
        }

        _leftNode = dragonController;
        dragonController._rightNode = this;
    }

    private void CheckTouchPosition(DragonController dragonController)
    {
        Vector3 targetPos = transform.localPosition;
        if (dragonController.transform.position.x > transform.position.x) //Goes Right
        {
            targetPos.x += 1;
            dragonController.MoveToTarget(targetPos);
            AddRightNode(dragonController);
        }
        else //Goes Left
        {
            targetPos.x -= 1;
            dragonController.MoveToTarget(targetPos);

            AddLeftNode(dragonController);
        }

        //dragonController.CheckNeighbourNodes();
    }

    private void CheckNeighbourNodes()
    {
        if (_leftNode && _leftNode.GetNumber() == GetNumber())
        {
            _leftNode.GetMerged();
            GetDestroyed();
        }
        else if (_rightNode && _rightNode.GetNumber() == GetNumber())
        {
            _rightNode.GetMerged();
            GetDestroyed();
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

    public bool InGroup()
    {
        return _groupJoined;
    }

    private void EmptyNodes()
    {
        if (_leftNode)
        {
            _leftNode.EmptyRightNode();
            _leftNode.PullLeftNode();
        }

        if (_rightNode)
        {
            _rightNode.EmptyLeftNode();
            _rightNode.PullRightNode();
        }

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
        SetAttackDamage();
    }

    private void SetAttackDamage()
    {
        _attackController.SetDamage(_number);
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

    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        if (_leftNode) Gizmos.DrawCube(_leftNode.transform.position + (Vector3.up * 2), new Vector3(1, 1, 1));
        if (_rightNode) Gizmos.DrawCube(_rightNode.transform.position + (Vector3.up * 2), new Vector3(1, 1, 1));
        Gizmos.color = new Color(1, 0, 1, 0.5f);
        Gizmos.DrawCube(transform.position + (Vector3.up * 2), new Vector3(1, 1, 1));
    }
}