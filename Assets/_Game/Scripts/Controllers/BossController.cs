using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Type BossType;
    [SerializeField] private BossController _targetBoss;
    [SerializeField] private int _health, _damage_;
    [SerializeField] private float _moveSpeed, range;
    [SerializeField] private TextMeshPro _healthText;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private bool _isDead;
    [SerializeField] private Animator _animator;
    [SerializeField] private AttackController _attackController;
    private bool _canFight, _isAttacking;

    public enum Type
    {
        Ally,
        Enemy
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

    private void Start()
    {
        UpdateDamage();
    }

    private void StopEveryThing()
    {
        _canFight = false;
        ToggleAttack(false);
    }

    public void StartAttacking()
    {
        StartCoroutine(MoveTowardsTarget());
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
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        _damage_ = _health;
        _attackController.SetDamage(_damage_);
    }

    public bool IsDead()
    {
        return _isDead;
    }

    private void ToggleAttack(bool state)
    {
        if (state != _attackController.GetAttackState())
        {
            Debug.Log("toggle attack called : " + state);
            _attackController.ToggleAttack(state);
        }
    }

    private bool EnemyInRange()
    {
        Vector3 position = transform.position;
        Vector3 enemyPosition = _targetBoss.transform.position;

        float distance = Vector3.Distance(position, enemyPosition);
        return distance < range;
    }

    IEnumerator MoveTowardsTarget()
    {
        _canFight = true;

        while (_canFight)
        {

            if (!_targetBoss)
            {
                Debug.LogWarning("Target is not set!");
                yield break;
            }

            // Calculate direction to target
            Vector3 direction = _targetBoss.transform.position - transform.position;
            direction.y = 0f;

            // If within stopping distance, stop moving
            if (direction.magnitude <= range)
            {
                if (_targetBoss.IsDead() || IsDead()) ToggleAttack(false);
                else
                {
                    ToggleAttack(true);
                }

                yield break;
            }
            else
            {
                Debug.Log("in else");

                ToggleAttack(false);

                transform.position += direction.normalized * _moveSpeed * Time.deltaTime;
            }


            yield return null;
        }
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (_health <= 0)
        {
            _health = 0;
            //death
            _isDead = true;
            if (BossType == Type.Enemy)
            {
                Debug.Log("win condition");
                GameManager.Instance.LevelCompleted();
            }
            else
            {
                Debug.Log("lose condition");
                GameManager.Instance.GameFailed();
            }

            PoolingManager.Instance.ClearAll();
        }

        UpdateText();
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