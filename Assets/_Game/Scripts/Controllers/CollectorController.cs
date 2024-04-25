using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorController : MonoBehaviour
{
    [SerializeField] private List<CollectableController.Type> collectedList = new List<CollectableController.Type>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ICollectable collectable))
        {
            collectable.GetCollected();
            
        }
    }
}