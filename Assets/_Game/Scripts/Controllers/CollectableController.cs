using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour, ICollectable
{
    public Type collectableType;
    [SerializeField] private int amount = 1;
    [SerializeField] private Animator _animator;
    private bool _isCollected;
    private string _hatchTrigger = "HatchTrigger";

    public enum Type
    {
        GoldenEgg
    }

    public void GetCollected(Vector3 pos)
    {
        if (IsCollected()) return;
        _isCollected = true;
        EggCounter.Instance.IncreaseCount(amount);
        _animator.SetTrigger(_hatchTrigger);
        Destroy(gameObject, 2);
    }

    public bool IsCollected()
    {
        return _isCollected;
    }
    private Type GetCollectableType()
    {
        return collectableType;
    }

    public void JoinGroup()
    {
    }
}