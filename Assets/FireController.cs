using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [SerializeField] private DragonController _dragonController;
    private bool _canAttack;

    private void Update()
    {
        if (_canAttack) Attack();
    }

    private void Attack()
    {
    }
}