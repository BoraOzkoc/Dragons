using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickListener : MonoBehaviour
{
    [SerializeField] private BossController _bossController;
    private bool _tutorialClosed;
    private void Update()
    {
        if (!_tutorialClosed)
        {
            TutorialController.Instance.StopTapAnim();
            _tutorialClosed = true;
        } 
        if (_bossController.CanFight())
        {
            if (Input.GetMouseButtonDown(0))
            {
                _bossController.Attack();
            }
        }
    }
}