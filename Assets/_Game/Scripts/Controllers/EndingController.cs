using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    public static EndingController Instance;
    [SerializeField] private GroundController _groundController;
    [SerializeField] private EndingFightController _endingFightController;
    [SerializeField] private TowerController _towerController;
    private LevelController _levelController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
        else Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out DragonManager dragonManager))
        {
            dragonManager.StartEndingProtocol(_endingFightController);
            _towerController.TriggerEndingProtocol();
            
            CameraManager.Instance.ActivateEndCamera();
        }
    }

    [Button]
    public void SetPosition()
    {
        MoveToLocation(_groundController.GetEndingPos());
    }

    public void Init(LevelController levelController)
    {
        _levelController = levelController;
    }

    public void MoveToLocation(Transform location)
    {
        transform.position = location.position;
    }
}