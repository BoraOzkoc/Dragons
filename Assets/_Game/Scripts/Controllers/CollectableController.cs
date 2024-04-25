using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour, ICollectable
{
    public Type collectableType;

    public enum Type
    {
        Cube
    }

    public void GetCollected(Vector3 pos)
    {
        Destroy(gameObject);
    }

    private Type GetCollectableType()
    {
        return collectableType;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}