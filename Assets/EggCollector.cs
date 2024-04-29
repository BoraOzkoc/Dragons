using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggCollector : MonoBehaviour
{
    private DragonController _dragonController;
    private void Start()
    {
        _dragonController = GetComponent<DragonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_dragonController.InGroup())
        {
            if (other.TryGetComponent(out CollectableController collectable))
            {
                collectable.GetCollected(transform.position);
            }
        }
    }
}
