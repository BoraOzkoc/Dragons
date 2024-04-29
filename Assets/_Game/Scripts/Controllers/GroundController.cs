using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class GroundController : MonoBehaviour
{
    public static GroundController Instance;
    [ReadOnly] public float Mesh_z_lengt, Mesh_x_length;
    [SerializeField] private int _length;
    [SerializeField] private GameObject _floorPrefab;

    [SerializeField, ReadOnly] private Transform _floorParent, _endPos;

    private List<GameObject> _floorList = new List<GameObject>();
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

    public void Init(LevelController levelController)
    {
        _levelController = levelController;
    }

    private void Start()
    {
        _floorParent.SetParent(LevelController.Instance.transform);
    }

    [Button]
    private void CreateGround()
    {
        DeleteExistingGround();

        GetMeshLength_x();
        GetMeshLength_z();

        _floorParent = new GameObject("FloorParent").transform;
        for (int i = 0; i < _length; i++)
        {
            Vector3 pos = transform.position + (i * Mesh_z_lengt * Vector3.forward);
            pos.x = -Mesh_x_length;
            GameObject obj = Instantiate(_floorPrefab, pos, quaternion.identity, _floorParent.transform);
            _floorList.Add(obj);
            if (i == _length - 1)
            {
                Transform ending = new GameObject("end").transform;


                Vector3 targetPos = pos;
                targetPos.x += -Mesh_x_length;
                targetPos.z += Mesh_z_lengt;

                ending.position = targetPos;
                _endPos = ending;
                _endPos.SetParent(_floorParent);
            }
        }
    }

    public Transform GetEndingPos()
    {
        return _endPos;
    }

    private void DeleteExistingGround()
    {
        if (_floorList.Count <= 0) return;
        // for (int i = 0; i < _floorList.Count; i++)
        // {
        //     DestroyImmediate(_floorList[i]);
        // }

        if (_floorParent)
        {
            DestroyImmediate(_floorParent.gameObject);
            _floorParent = null;
        }

        _floorList.Clear();
    }

    private void GetMeshLength_x()
    {
        float xLength = _floorPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;

        Mesh_x_length = xLength;
    }

    private void GetMeshLength_z()
    {
        float zLength = _floorPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.z;

        Mesh_z_lengt = zLength;
    }
}