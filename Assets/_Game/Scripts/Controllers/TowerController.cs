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
    [SerializeField] private BossController _bossController;
    [SerializeField] private Transform _towerTip, _towerMesh, _targetMesh;
    [SerializeField] private ParticleSystem _confettiEffect, _towerDisappearEffect;
    [SerializeField, ReadOnly] private int _storedDamage, _totalFloorCount;
    private string _towerHealthSaveName = "TowerHealth", _storedDamageSaveName = "StoredDamage";
    private bool _isDestroyed;

    private void Start()
    {
        SetTotalFloorCount();
        Load();
        ToggleText(false);
    }

    private void SetTotalFloorCount()
    {
        _totalFloorCount = meshList.Count;
    }

    public void TriggerEndingProtocol()
    {
        ToggleText(true);
    }

    private void Save()
    {
        PlayerPrefs.SetInt(_towerHealthSaveName, _currentHealth);
        PlayerPrefs.SetInt(_storedDamageSaveName, _storedDamage);
    }

    private void SetTarget(Transform target)
    {
        _targetMesh = target;
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
        //else SetTarget(meshList[0].transform);
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
        AudioManager.Instance.PlayStoneHit();
        if (_currentHealth > 0)
        {
            _currentHealth -= 2;
            _storedDamage += 2;
            CheckStoredDamage();
            UpdateText();
            CheckHealth();
            Save();
        }
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
            _towerDisappearEffect.Play();
            _bossController.TriggerHappyAnim();
            ToggleMesh(false);
            GameManager.Instance.LevelCompleted();
        }
    }

    private void ToggleMesh(bool state)
    {
        _towerMesh.gameObject.SetActive(state);
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
                yield return new WaitForSeconds(0.01f);
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

    private void ToggleText(bool state)
    {
        _healthText.gameObject.SetActive(state);
    }

    private float GetMeshLength_y()
    {
        float yLength = meshList[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.y;

        return yLength;
    }
}