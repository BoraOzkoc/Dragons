using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackController : MonoBehaviour
{
    [SerializeField] private DragonController _dragonController;
    [SerializeField] private int _damage;
    [SerializeField] private float _waitBetweenAttacks, _projectileSpeed;
    [SerializeField] private float _projectileLifeTime;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Animator _animator;
    [SerializeField] private BossController _bossController;
    private PoolingManager _poolingManager;
    private Coroutine _attackCoroutine;
    private string _fireTrigger = "Base Layer.SFly Attack FireBall";
    private float _animTime;

    private void Start()
    {
        _poolingManager = PoolingManager.Instance;
    }

    public void SetAnimTime(float newTime)
    {
        _animTime = newTime;
    }

    public float GetAnimTime()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void SetDamage(int amount)
    {
        _damage = amount;
    }

    public void Attack()
    {
        _animator.Play(_fireTrigger, 0, 0);
    }

    public void Fire()
    {
        FireBallController fireBallController = _poolingManager.PullFromPool();
        fireBallController.SetStats(_projectileLifeTime, _projectileSpeed, _damage,_dragonController.GetLevel());
        fireBallController.ActivateDefault(_firePoint.position, transform.forward);
    }
}