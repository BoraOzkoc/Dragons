using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    [SerializeField] private List<Color> colorList = new List<Color>();
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private float _lifeTime, _speed;
    [SerializeField] private int _damage;
    [SerializeField] private ParticleSystem _fireEffect,_obstacleHitEffect,_towerHitEffect;
    private PoolingManager _poolingManager;
    private BossController _owner;
    private int _fireLevel;

    public void Init(PoolingManager poolingManager)
    {
        _poolingManager = poolingManager;
    }

    public void SetStats(float lifeTime, float speed, int damage, int level)
    {
        _lifeTime = lifeTime;
        _speed = speed;
        _damage = damage;
        _fireLevel = level;
        ChangeColor();
    }

    public void ChangeColor()
    {
        if (_fireLevel > colorList.Count - 1) _fireLevel = colorList.Count - 1;
        _mesh.material.DOColor(colorList[_fireLevel], 0);
        _fireEffect.startColor = colorList[_fireLevel];
    }

    public void ActivateDefault(Vector3 spawnPos, Vector3 direction)
    {
        TeleportObject(spawnPos);
        FaceDirection(direction);
        StartLifeTimer();
    }

    public void ActivateFakeFireBall(Vector3 spawnPos, TowerController towerController)
    {
        TeleportObject(spawnPos);

        var position = towerController.transform.position;
        transform.LookAt(position);
        transform.DOMove(position, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            towerController.GetHit();
            _poolingManager.PushToPool(this);
        });
    }

    private void SetOwner(BossController owner)
    {
        _owner = owner;
    }

    private void FaceDirection(Vector3 direction)
    {
        transform.LookAt(transform.position + direction);
    }

    public void PlayObstacleHitEffect()
    {
        IEnumerator ReturnCoroutine()
        {
        _obstacleHitEffect.Play();
        _obstacleHitEffect.transform.SetParent(transform.parent);
        yield return  new WaitForSeconds(0.5f);
        _obstacleHitEffect.transform.SetParent(transform);
        _obstacleHitEffect.transform.localPosition = Vector3.zero;
        }

        StartCoroutine(ReturnCoroutine());
    }
    private void StartLifeTimer()
    {
        StartCoroutine(LifeTimeCoroutine());
    }

    IEnumerator LifeTimeCoroutine()
    {
        yield return new WaitForSeconds(_lifeTime);
        _fireEffect.Stop();

        _poolingManager.PushToPool(this);
    }

    private void Deactivate()
    {
        _fireEffect.Stop();
        _poolingManager.PushToPool(this);
        ResetColor();
    }

    private void Update()
    {
        GoForward();
    }

    private void GoForward()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void ResetColor()
    {
        _fireLevel = 0;
        ChangeColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ObstacleController obstacleController))
        {
            obstacleController.DecreaseNumber(_damage);

            Deactivate();
        }

        if (!_owner) return;
        if (!other.TryGetComponent(out TowerController towerController)) return;


        towerController.GetHit();
    }

    private void TeleportObject(Vector3 pos)
    {
        transform.position = pos;
        _fireEffect.Play();

    }

    private void ToggleObject(bool state)
    {
        gameObject.SetActive(state);
    }
}