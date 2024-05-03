using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField, ReadOnly] private BossController _targetBoss;
    [SerializeField] private float _health, _moveSpeed, _damage_, range;
    [SerializeField] private TextMeshPro _healthText;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private bool _isDead;
    private bool _isMoving;


    public void StartAttacking()
    {
        StartCoroutine(MoveTowardsTarget());
    }

    private void AttackEnemy()
    {
        if (!_targetBoss) return;
        if (!_targetBoss.IsDead())
        {
            _targetBoss.TakeDamage(_damage_);
        }
    }

    public void Upgrade(float amount)
    {
        if (!_mesh.activeSelf) _mesh.SetActive(true);
        _health += amount;
        UpdateText();
        _mesh.transform.localScale += Vector3.one * amount / 2;
    }

    public bool IsDead()
    {
        return _isDead;
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
        _isMoving = true;

        while (_isMoving)
        {
            if (!_targetBoss)
            {
                Debug.LogWarning("Target is not set!");
                yield break;
            }

            // Calculate direction to target
            Vector3 direction = _targetBoss.transform.position - transform.position;
            direction.y = 0f; // Ensure movement is only along the XZ plane

            // If within stopping distance, stop moving
            if (direction.magnitude <= range)
            {
                _isMoving = false;
                AttackEnemy();
                yield break;
            }

            // Move towards the target
            transform.position += direction.normalized * _moveSpeed * Time.deltaTime;

            yield return null;
        }
    }

    public void TakeDamage(float amount)
    {
        _health -= amount;
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (_health <= 0)
        {
            //death
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