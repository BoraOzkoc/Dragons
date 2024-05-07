using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Type BossType;
    [SerializeField] private TowerController _target;
    [SerializeField] private int _health, _damage_;
    [SerializeField] private float _moveSpeed, range;
    [SerializeField] private TextMeshPro _healthText;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private bool _isDead;
    [SerializeField] private Animator _animator;
    private bool _canFight, _isAttacking;
    private PoolingManager _poolingManager;
    private string _fireTrigger = "Base Layer.SFly Attack FireBall";

    public enum Type
    {
        Ally
    }

    private void Start()
    {
        _poolingManager = PoolingManager.Instance;
    }

    private void OnEnable()
    {
        GameManager.LevelCompletedEvent += StopEveryThing;
        GameManager.LevelFailedEvent += StopEveryThing;
    }

    private void OnDisable()
    {
        GameManager.LevelCompletedEvent -= StopEveryThing;
        GameManager.LevelFailedEvent -= StopEveryThing;
    }

    private void StopEveryThing()
    {
        _canFight = false;
        ToggleAttack(false);
    }

    public void StartAttacking()
    {
        _canFight = true;
    }

    public void Attack()
    {
        _animator.Play(_fireTrigger, 0, 0f);
    }

    public void Fire()
    {
        FireBallController fireBallController = _poolingManager.PullFromPool();
        fireBallController.ActivateFakeFireBall(_firePoint.position, _target);
    }

    public bool CanFight()
    {
        return _canFight;
    }

    public void AttackEndedEvent()
    {
        ResetAttack();
    }

    private void ResetAttack()
    {
        _isAttacking = false;
    }

    public void Upgrade(int amount)
    {
        if (!_mesh.activeSelf) _mesh.SetActive(true);
        _health += amount;
        UpdateText();
        _mesh.transform.localScale += Vector3.one * amount / 30;
    }

    public bool IsDead()
    {
        return _isDead;
    }

    private void ToggleAttack(bool state)
    {
    }

    private void UpdateText()
    {
        _healthText.text = _health.ToString();
    }

    private void OnValidate()
    {
        UpdateText();
    }
}