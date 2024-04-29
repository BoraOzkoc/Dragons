using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class EggCounter : MonoBehaviour
{
    public static EggCounter Instance;
    [SerializeField, ReadOnly] private int _count;
    [SerializeField] private TextMeshProUGUI _countText;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        UpdateText();
    }

    public void IncreaseCount(int amount = 1)
    {
        _count += amount;
        UpdateText();
    }

    public void DecreaseCount(int amount)
    {
        if (amount <= _count)
        {
            _count -= amount;
        }
        else
        {
            Debug.LogError("not enough Egg");
        }

        UpdateText();
    }

    private void UpdateText()
    {
        _countText.text = _count.ToString();
    }

    // public bool CanPurchase(int currentAmount)
    // {
    //     return currentAmount >= _count;
    // }
}