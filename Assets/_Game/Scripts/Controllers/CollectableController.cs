using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour, ICollectable
{
    public Type collectableType;
    [SerializeField] private int amount = 1;
    public enum Type
    {
        GoldenEgg
    }

    public void GetCollected(Vector3 pos)
    {
        EggCounter.Instance.IncreaseCount(amount);
        Destroy(gameObject);
    }

    private Type GetCollectableType()
    {
        return collectableType;
    }

    public void JoinGroup()
    {
    }
}