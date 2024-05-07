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
    [SerializeField] private Animator _animator;
    [SerializeField] private BossController _bossController;
    private bool _canAttack;
    private PoolingManager _poolingManager;
    private Coroutine _attackCoroutine;
    private string _fireTrigger = "Base Layer.SFly Attack FireBall";

    private void Start()
    {
        _poolingManager = PoolingManager.Instance;
    }

    public void ToggleAttack(bool state)
    {
        _canAttack = state;
        if (state) _attackCoroutine ??= StartCoroutine(AttackCoroutine());
    }

    public bool GetAttackState()
    {
        return _canAttack;
    }

    public void SetDamage(int amount)
    {
        _damage = amount;
    }

    IEnumerator AttackCoroutine()
    {
        while (_canAttack)
        {
            _animator.Play(_fireTrigger, 0, 0f);

            yield return new WaitForSeconds(_waitBetweenAttacks);
        }
    }

    public void Fire()
    {
        FireBallController fireBallController = _poolingManager.PullFromPool();
        fireBallController.SetStats(_projectileLifeTime, _projectileSpeed, _damage);
        fireBallController.ActivateDefault(_firePoint.position, transform.forward);
    }
}