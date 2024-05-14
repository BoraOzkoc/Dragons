using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField] private Transform _firePoint,_ground;
    [SerializeField] private bool _isDead;
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _upgradingEffect;
    private bool _canFight, _isAttacking;
    private PoolingManager _poolingManager;
    private string _fireTrigger = "Base Layer.SFly Attack FireBall", _sadTrigger = "Base Layer.Sad",_IdleTrigger  = "Base Layer.Idle";

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
        ToggleHealthText(false);
    }

    private void ToggleHealthText(bool state)
    {
        _healthText.gameObject.SetActive(state);
    }
    public void TriggerHappyAnim()
    {
        _animator.Play(_fireTrigger, 0, 0f);
    }

    public void StartAttacking()
    {
        _canFight = true;
    }

    public void Attack()
    {
        if (HasHealthToFire() && !_target.IsDestroyed()) _animator.Play(_fireTrigger, 0, 0f);
    }

    public void Fire()
    {
        TakeDamage();
        FireBallController fireBallController = _poolingManager.PullFromPool();
        fireBallController.ActivateFakeFireBall(_firePoint.position, _target);
    }

    private bool HasHealthToFire()
    {
        return _health >= 2;
    }

    public bool CanFight()
    {
        return _canFight;
    }

    private void TakeDamage()
    {
        _health -= 2;
        CheckHealth();
        UpdateText();
    }

    private void CheckHealth()
    {
        if (_health <= 0)
        {
            _health = 0;
            GameManager.Instance.LevelCompleted();
            transform.DOMove(_ground.position, 0.5f).OnComplete(() =>
            {
                TriggerSadAnim();
            });
        }
    }

    private void TriggerSadAnim()
    {
        _animator.Play(_sadTrigger, 0, 0f);        
    }

    public void Upgrade(int amount)
    {
        _upgradingEffect.Play();
        if (!_mesh.activeSelf)
        {
            _mesh.SetActive(true);
            Vector3 startScale = transform.localScale;
            transform.localScale = Vector3.zero;
            transform.DOScale(startScale, 0.25f).SetEase(Ease.OutBounce);
        }
        _health += amount;
        UpdateText();
        _mesh.transform.localScale += Vector3.one * amount / 30;
    }

    public bool IsDead()
    {
        return _isDead;
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