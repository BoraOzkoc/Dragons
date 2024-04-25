using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;
    [SerializeField] private GroundController _groundController;
    [SerializeField] private EndingController _endingController;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
        else Instance = this;
    }

    public EndingController GetEndingController()
    {
        return _endingController;
    }
    public void Init()
    {
        _groundController.Init(this);
        _endingController.Init(this);
    }

    public void GetDestroyed()
    {
        //Do something then destroy
        Destroy(gameObject);
    }
}