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
        if (!_bossController.CanFight()) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (!_tutorialClosed)
            {
                TutorialController.Instance.StopTapAnim();
                _tutorialClosed = true;
            }

            _bossController.Attack();
        }
    }
}