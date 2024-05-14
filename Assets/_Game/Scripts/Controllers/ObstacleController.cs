using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lofelt.NiceVibrations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleController : MonoBehaviour
{
    public Type ObstacleType;
    [SerializeField] private DragonController _dragonController;
    [SerializeField] private TextMeshPro _numberText;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private int _obstacleNumber;
    [SerializeField] private bool isDestroyed;

    public enum Type
    {
        Cage,
        Egg
    }

    private void Start()
    {
        _dragonController.GetCaged();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDestroyed) return;
        if (other.TryGetComponent(out DragonController dragonController))
        {
            if (!dragonController.IsCaged())
            {
                dragonController.GetDestroyed(0.1f);
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
            }
        }
    }

    public void DecreaseNumber(int amount)
    {
        if (ObstacleType == Type.Cage) AudioManager.Instance.PlayMetalHit();
        else if (ObstacleType == Type.Egg) AudioManager.Instance.PlayEggHit();
        transform.DOShakeScale(0.15f, 1, 5);
        if (isDestroyed) return;
        _obstacleNumber -= amount;
        if (_obstacleNumber <= 0)
        {
            _obstacleNumber = 0;
            isDestroyed = true;
            DestroyAnim();
            FreeDragon();
        }

        UpdateText();
    }

    private void DestroyAnim()
    {
        _mesh.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBounce).OnComplete(DestroyCage);
    }

    private void DestroyCage()
    {
        _mesh.SetActive(false);
    }

    private void RandomizeDragon()
    {
        _dragonController.Randomize();
    }

    public bool IsDestroyed()
    {
        return isDestroyed;
    }

    private void FreeDragon()
    {
        if (ObstacleType == Type.Egg) RandomizeDragon();
        _dragonController.GetFreed();
        _dragonController.transform.DOScale(Vector3.one, 0.3f);
        _dragonController = null;
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