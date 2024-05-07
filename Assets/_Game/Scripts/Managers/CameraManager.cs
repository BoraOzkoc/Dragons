using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private List<CinemachineVirtualCamera> _cameraList = new List<CinemachineVirtualCamera>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
        else Instance = this;
    }

    private void OnEnable()
    {
        GameManager.LevelCompletedEvent += ActivateEndCamera;
    }

    private void OnDisable()
    {
        GameManager.LevelCompletedEvent -= ActivateEndCamera;
    }


    public void ActivateGameCamera(CinemachineVirtualCamera activeCamera)
    {
        activeCamera.Priority = 0;
        int index = 0;
        if (index > _cameraList.Count - 1) index = _cameraList.Count - 1;
        for (int i = 0; i < _cameraList.Count; i++)
        {
            if (i == index) _cameraList[index].Priority = 50;
            _cameraList[i].Priority = 0;
        }
    }

    public void ActivateEndCamera()
    {
        int index = _cameraList.Count - 1;
        if (index > _cameraList.Count - 1) index = _cameraList.Count - 1;
        for (int i = 0; i < _cameraList.Count; i++)
        {
            if (i == index) _cameraList[index].Priority = 50;
            else _cameraList[i].Priority = 0;
        }
    }

    public void ActivateCamera(CinemachineVirtualCamera cam)
    {
        cam.Priority = 50;
        for (int i = 0; i < _cameraList.Count; i++)
        {
            _cameraList[i].Priority = 0;
        }
    }
}