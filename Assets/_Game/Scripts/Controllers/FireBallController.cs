using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    [SerializeField] private float _lifeTime, _speed;
    [SerializeField] private int _damage;
    private PoolingManager _poolingManager;
    private BossController _owner;

    public void Init(PoolingManager poolingManager)
    {
        _poolingManager = poolingManager;
    }

    public void SetStats(float lifeTime, float speed, int damage)
    {
        _lifeTime = lifeTime;
        _speed = speed;
        _damage = damage;
    }

    public void ChangeColor()
    {
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

        transform.DOMove(towerController.transform.position, 1).OnComplete(() =>
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

    private void StartLifeTimer()
    {
        StartCoroutine(LifeTimeCoroutine());
    }

    IEnumerator LifeTimeCoroutine()
    {
        yield return new WaitForSeconds(_lifeTime);
        _poolingManager.PushToPool(this);
    }

    private void Deactivate()
    {
        _poolingManager.PushToPool(this);
    }

    private void Update()
    {
        GoForward();
    }

    private void GoForward()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
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
    }

    private void ToggleObject(bool state)
    {
        gameObject.SetActive(state);
    }
}