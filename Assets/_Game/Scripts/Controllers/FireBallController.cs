using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    [SerializeField] private float _lifeTime, _speed;
    [SerializeField] private int _damage;
    private PoolingManager _poolingManager;

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

    public void Activate(Vector3 pos)
    {
        TeleportObject(pos);
        StartLifeTimer();
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