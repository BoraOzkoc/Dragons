using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackController : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _waitBetweenAttacks, _projectileSpeed;
    [SerializeField] private float _projectileLifeTime;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private DragonController _dragonController;
    private bool _canAttack;
    private PoolingManager _poolingManager;
    private Coroutine _attackCoroutine;

    private void Start()
    {
        _poolingManager = PoolingManager.Instance;
    }

    public void ToggleAttack(bool state)
    {
        _canAttack = state;
        if (state) _attackCoroutine ??= StartCoroutine(AttackCoroutine());
    }


    public void SetDamage(int amount)
    {
        _damage = amount;
    }

    IEnumerator AttackCoroutine()
    {
        while (_canAttack)
        {
            Fire();
            yield return new WaitForSeconds(_waitBetweenAttacks);
        }
    }

    private void Fire()
    {
        FireBallController fireBallController = _poolingManager.PullFromPool();
        fireBallController.SetStats(_projectileLifeTime, _projectileSpeed, _damage);
        fireBallController.Activate(_firePoint.position);
    }
}