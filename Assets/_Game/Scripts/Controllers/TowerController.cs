using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] private List<GameObject> meshList = new List<GameObject>();
    [SerializeField] private int _maxHealth, _currentHealth;
    [SerializeField] private TextMeshPro _healthText;
    [SerializeField, ReadOnly] private int _storedDamage, _totalFloorCount;

    private void Start()
    {
        SetCurrentHealth(_maxHealth);
        UpdateText();
        SetTotalFloorCount();
    }

    private void SetTotalFloorCount()
    {
        _totalFloorCount = meshList.Count;
    }
    private void Save()
    {
    }

    private void Load()
    {
    }

    private void SetCurrentHealth(int amount)
    {
        _currentHealth = amount;
        UpdateText();
    }

    [Button]
    public void GetHit()
    {
        _currentHealth -= 2;
        _storedDamage += 2;
        CheckStoredDamage();
        UpdateText();
        CheckHealth();
    }

    private void CheckStoredDamage()
    {
        int targetAmount = _maxHealth / _totalFloorCount;
        if (_storedDamage >= targetAmount)
        {
            _storedDamage = _storedDamage - targetAmount;
            LowerTower();            
        }
    }

    private void CheckHealth()
    {
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            UpdateText();
            GameManager.Instance.LevelCompleted();
        }
    }

    private void LowerTower()
    {
        IEnumerator MoveCoroutine()
        {
            for (int i = 0; i < meshList.Count; i++)
            {
                meshList[i].transform.DOLocalMoveY(-GetMeshLength_y(), 0.05f).SetRelative(true);
                yield return new WaitForSeconds(0.02f);
            }
        }

        StartCoroutine(MoveCoroutine());
    }

    private void OnValidate()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        _healthText.text = _currentHealth.ToString();
    }

    private float GetMeshLength_y()
    {
        float yLength = meshList[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.y;

        return yLength;
    }
}