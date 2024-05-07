using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickListener : MonoBehaviour
{
    [SerializeField] private BossController _bossController;
    private void Update()
    {
        if (_bossController.CanFight())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Clicked");
            }        }
    }
}
