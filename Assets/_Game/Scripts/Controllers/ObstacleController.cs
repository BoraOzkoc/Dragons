using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] private DragonController _dragonController;
    [SerializeField] private TextMeshPro _numberText;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private int _obstacleNumber;
    [SerializeField] private bool isDestroyed;

    private void OnTriggerEnter(Collider other)
    {
        if (isDestroyed) return;
        if (other.TryGetComponent(out DragonController dragonController))
        {
        }
    }

    public void DecreaseNumber(int amount)
    {
        if (isDestroyed) return;
        _obstacleNumber -= amount;
        if (_obstacleNumber < 0)
        {
            _obstacleNumber = 0;
            isDestroyed = true;
        }

        UpdateText();
    }

    private void FreeDragon()
    {
        _dragonController.GetFreed();
    }

    private void UpdateText()
    {
        _numberText.text = _obstacleNumber.ToString();
        _numberText.gameObject.SetActive(_obstacleNumber > 0);
    }

    private void OnValidate()
    {
        UpdateText();
    }
}