using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance;

    [SerializeField] private FireBallController objectPrefab;
    [SerializeField] private int poolSize;
    private Queue<FireBallController> pooledObjectQueue;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        SpawnObjects();

    }

    private void Start()
    {
    }

    private void SpawnObjects()
    {
        pooledObjectQueue = new Queue<FireBallController>();

        for (int i = 0; i < poolSize; i++)
        {
            FireBallController spawnedObject = Instantiate(objectPrefab, transform);
            spawnedObject.Init(this);
            spawnedObject.gameObject.SetActive(false);

            pooledObjectQueue.Enqueue(spawnedObject);
        }
    }

    public FireBallController PullFromPool()
    {
        FireBallController spawnedObject = pooledObjectQueue.Dequeue();
        spawnedObject.gameObject.SetActive(true);
        if (pooledObjectQueue.Count <= 1)
        {
            SpawnObjects();
        }


        return spawnedObject;
    }

    public void PushToPool(FireBallController obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.position = transform.position;
        pooledObjectQueue.Enqueue(obj);
    }
}