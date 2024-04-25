using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DragonController : MonoBehaviour, ICollectable
{
    [SerializeField] private Transform targetLocation;
    [SerializeField] private int _number;
    [SerializeField] private bool _isCaged;
    [SerializeField] private bool _isCollected;
    [SerializeField] private TextMeshPro _numberText;
    
    public void Init()
    {
    }

    private void OnValidate()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        _numberText.text = _number.ToString();
    }

    public void GetCaged()
    {
        _isCaged = true;
    }

    public void GetFreed()
    {
        _isCaged = false;
    }

    public void MoveToTarget()
    {
        transform.position = targetLocation.position;
    }

    public void GetCollected()
    {
    }
}