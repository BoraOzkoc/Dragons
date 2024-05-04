using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingFightController : MonoBehaviour
{
    [SerializeField] private BossController _enemyBoss;
    [SerializeField] private BossController _allyBoss;


    public void ActivateFight()
    {
        if (!_allyBoss || !_enemyBoss) return;
        _enemyBoss.StartAttacking();
        _allyBoss.StartAttacking();
    }

    public BossController GetAllyBoss()
    {
        return _allyBoss;
    }

    public void UpgradeAllyBoss(float amount)
    {
        _allyBoss.Upgrade(amount);
    }

    public Vector3 GetAllyBossPos()
    {
        Vector3 pos = _allyBoss.transform.position;
        pos.y = 0;
        return pos;
    }
}