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
    [SerializeField] private PrincessController _princessController;
    [SerializeField] private Transform _towerTip;
    [SerializeField] private ParticleSystem _confettiEffect;
    [SerializeField, ReadOnly] private int _storedDamage, _totalFloorCount;
    private string _towerHealthSaveName = "TowerHealth", _storedDamageSaveName = "StoredDamage";
    private bool _isDestroyed;

    private void Start()
    {
        SetTotalFloorCount();
        Load();
    }

    private void SetTotalFloorCount()
    {
        _totalFloorCount = meshList.Count;
    }

    private void Save()
    {
        PlayerPrefs.SetInt(_towerHealthSaveName, _currentHealth);
        PlayerPrefs.SetInt(_storedDamageSaveName, _storedDamage);
    }

    private void Load()
    {
        int health = PlayerPrefs.GetInt(_towerHealthSaveName, _maxHealth);
        int storedDamage = PlayerPrefs.GetInt(_storedDamageSaveName, 0);

        if (health <= 0)
        {
            ResetTower();
            health = _maxHealth;
            storedDamage = 0;
        }

        _storedDamage = storedDamage;
        SetCurrentHealth(health);
        UpdateText();
        if (health != _maxHealth) LowerTower(Mathf.Abs(health - _maxHealth) / (_maxHealth / _totalFloorCount));
    }

    private void ResetTower()
    {
        PlayerPrefs.SetInt(_towerHealthSaveName, _maxHealth);
        PlayerPrefs.SetInt(_storedDamageSaveName, 0);
    }

    private void SetCurrentHealth(int amount)
    {
        _currentHealth = amount;
        UpdateText();
    }

    public bool IsDestroyed()
    {
        return _isDestroyed;
    }

    [Button]
    public void GetHit()
    {
        _currentHealth -= 2;
        _storedDamage += 2;
        CheckStoredDamage();
        UpdateText();
        CheckHealth();
        Save();
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
            _princessController.PlayCheerAnim();
            UpdateText();
            _confettiEffect.Play();
            GameManager.Instance.LevelCompleted();
        }
    }

    private void LowerTower(int times = 1)
    {
        IEnumerator MoveCoroutine()
        {
            for (int i = 0; i < meshList.Count; i++)
            {
                meshList[i].transform.DOLocalMoveY(-GetMeshLength_y() * times, 0.05f).SetRelative(true).OnComplete(() =>
                {
                    _princessController.MoveTo(_towerTip.position);
                });
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