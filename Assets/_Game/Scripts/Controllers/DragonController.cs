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
    [SerializeField] private SkinnedMeshRenderer _mesh;
    [SerializeField] private List<Material> materialList = new List<Material>();
    [SerializeField] private bool _isCaged;
    [SerializeField] private bool _groupJoined;
    [SerializeField] private TextMeshPro _numberText;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private AttackController _attackController; 
    [SerializeField] private List<ParticleSystem> particleList = new List<ParticleSystem>();

    [SerializeField, ReadOnly] private DragonController _leftNode, _rightNode;

    private DragonManager _dragonManager;
    private bool _isBlocked, _canGroundCheck = true;
    private int _level = 0;

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

    private void ChangeParticleColor(Color color)
    {
        for (int i = 0; i < particleList.Count; i++)
        {
            particleList[i].startColor = color;
        }
    }
    private void Update()
    {
        if (_canGroundCheck) CheckGround();
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckCollisions(other);
    }

    private bool IsBlocked()
    {
        return _isBlocked;
    }

    private void CheckCollisions(Collider other)
    {
        if (IsCaged() || !_groupJoined) return;
        if (!other.TryGetComponent(out DragonController dragonController)) return;

        if (dragonController.IsCaged() || dragonController.IsBlocked()) return;
        if (_groupJoined)
        {
            if (dragonController._groupJoined) return;

            CompareNumbers(dragonController, 0.1f);
        }
    }

    public void ToggleBlock(bool state)
    {
        _isBlocked = state;
    }

    private void CompareNumbers(DragonController dragonController, float delay = 0)
    {
        if (IsBlocked() || dragonController.IsBlocked()) return;

        bool isSame = _dragonManager.CheckIsSame(this, dragonController);

        if (!isSame)
        {
            CheckTouchPosition(dragonController);
        }
    }

    public void Reposition()
    {
        float leftNodeDistance = 999;
        float rightNodeDistance = 999;

        if (_leftNode)
            leftNodeDistance = Vector3.Distance(_leftNode.transform.position, _dragonManager.transform.position);
        if (_rightNode)
            rightNodeDistance = Vector3.Distance(_rightNode.transform.position, _dragonManager.transform.position);

        if (leftNodeDistance < rightNodeDistance)
        {
            MoveNextToLeftNode();
        }
        else
        {
            MoveNextToRightNode();
        }
    }

    public void MoveNextToLeftNode()
    {
        if (!_leftNode) return;

        Vector3 targetPos = _leftNode.transform.localPosition;
        targetPos.x += 1;
        transform.DOLocalMove(targetPos, 0.1f);
        if (_rightNode) _rightNode.MoveNextToLeftNode();
    }

    public void MoveNextToRightNode()
    {
        if (!_rightNode) return;
        Vector3 targetPos = _rightNode.transform.localPosition;
        targetPos.x -= 1;
        transform.DOLocalMove(targetPos, 0.1f);
        if (_leftNode) _leftNode.MoveNextToRightNode();
    }

    private void StopGroundCheck()
    {
        _canGroundCheck = false;
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

    public void MoveMiddle()
    {
        transform.DOLocalMoveX(0, 0.1f);
    }

    public void Move(float x_moveAmount)
    {
        transform.DOLocalMoveX(x_moveAmount, 0.3f).SetRelative(true);
    }

    public int GetLevel()
    {
        return _level;
    }
    public void StartEndingProtocol(EndingFightController endingFightController)
    {
        StopGroundCheck();
        Vector3 targetPos = endingFightController.GetAllyBossPos();
        targetPos.y = transform.position.y;
        transform.DOMove(targetPos, 1).SetEase(Ease.Linear).OnComplete(() =>
        {
            endingFightController.UpgradeAllyBoss(GetNumber());
            GetDestroyed();
        });
    }

    public void MoveLeft(int times)
    {
        transform.DOLocalMoveX(-1 * times, 0.1f).SetRelative(true);
    }

    public void MoveRight(int times)
    {
        transform.DOLocalMoveX(1 * times, 0.1f).SetRelative(true);
    }

    public void Attack()
    {
        _attackController.Attack();
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
        transform.SetParent(null);
    }

    public bool IsCaged()
    {
        return _isCaged;
    }

    public void PushRightNode()
    {
        transform.DOLocalMoveX(1, 0.1f).SetRelative(true);
        if (_rightNode) _rightNode.PushRightNode();
    }

    public void PushLeftNode()
    {
        transform.DOLocalMoveX(-1, 0.1f).SetRelative(true);
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
        dragonController.JoinGroup();

        if (_rightNode)
        {
            _rightNode.PushRightNode();
            _rightNode.SetLeftNode(dragonController);
            dragonController._rightNode = _rightNode;
        }

        _rightNode = dragonController;
        dragonController._leftNode = this;
    }

    public void AddLeftNode(DragonController dragonController)
    {
        dragonController.JoinGroup();
        if (_leftNode)
        {
            _leftNode.PushLeftNode();
            _leftNode.SetRightNode(dragonController);
            dragonController._leftNode = _leftNode;
        }

        _leftNode = dragonController;
        dragonController._rightNode = this;
    }

    public void SetRightNode(DragonController newNode)
    {
        _rightNode = newNode;
    }

    public void SetLeftNode(DragonController newNode)
    {
        _leftNode = newNode;
    }


    private void CheckTouchPosition(DragonController dragonController)
    {
        Vector3 targetPos = transform.localPosition;
        if (dragonController.transform.position.x > transform.position.x) //Goes Right
        {
            targetPos.x += 1;
            AddRightNode(dragonController);
        }
        else //Goes Left
        {
            targetPos.x -= 1;
            AddLeftNode(dragonController);
        }

        _dragonManager.AddToList(dragonController);

        dragonController.MoveToPosition(targetPos);
    }

    private void CheckNeighbourNodes()
    {
        bool sameFound = false;
        if (_leftNode)
        {
            sameFound = _dragonManager.CheckIsSame(this, _leftNode);
        }

        if (_rightNode && !sameFound)
        {
            _dragonManager.CheckIsSame(this, _rightNode);
        }
    }

    public void GetDestroyed(float seconds = 0)
    {
        transform.DOKill();
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
        _dragonManager.ConnectNodes(_leftNode, _rightNode);

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
        ToggleBlock(false);
        CheckNeighbourNodes();
        SetMeshMaterial();
        
    }

    private void SetMeshMaterial()
    {
        _level += 1;
        if (_level == materialList.Count - 1) _level = materialList.Count - 1;
        _mesh.material = materialList[_level];
        ChangeParticleColor(_mesh.material.color);
        particleList[0].Play();
    }

    public float GetAnimationTime()
    {
        return _attackController.GetAnimTime();
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

    public void MoveToPosition(Vector3 pos)
    {
        transform.DOLocalMove(pos, 0.1f).OnComplete(CheckNeighbourNodes);
    }

    public void JoinGroup()
    {
        if (_groupJoined) return;
        _groupJoined = true;
    }

    public AttackController GetAttackController()
    {
        return _attackController;
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