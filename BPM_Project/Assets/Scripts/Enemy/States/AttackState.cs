using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class AttackState : IState
{

    EnemyController m_enemyController;
    WeaponEnemyBehaviour m_weaponEnemyBehaviour;

    public AttackState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }


    public void Enter()
    {
        m_weaponEnemyBehaviour = m_enemyController.GetComponent<WeaponEnemyBehaviour>();
        m_weaponEnemyBehaviour.StartCoroutine(m_weaponEnemyBehaviour.OnShoot(m_weaponEnemyBehaviour.nbrOfShootOnRafale, m_weaponEnemyBehaviour._SMG.weaponStats._weaponLevel0.attackCooldown, m_weaponEnemyBehaviour.timeForEachBurst));

    }

    public void Exit()
    {
        m_weaponEnemyBehaviour.StopCoroutine(m_weaponEnemyBehaviour.OnShoot(m_weaponEnemyBehaviour.nbrOfShootOnRafale, m_weaponEnemyBehaviour._SMG.weaponStats._weaponLevel0.attackCooldown, m_weaponEnemyBehaviour.timeForEachBurst));
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void Update()
    {
        if (m_enemyController.DistanceToTarget > m_enemyController.Agent.stoppingDistance)
        {
            m_enemyController.ChangeState((int)EnemyState.Enemy_ChaseState);
        }
    }

}
