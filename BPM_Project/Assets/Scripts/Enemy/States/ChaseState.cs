using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class ChaseState : IState
{
    EnemyController m_enemyController;

    public ChaseState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }


    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void FixedUpdate()
    {
        m_enemyController.Agent.SetDestination(m_enemyController.Target.position);
        if(m_enemyController.DistanceToTarget <= m_enemyController.Agent.stoppingDistance)
        {

            m_enemyController.ChangeState((int)EnemyState.Enemy_AttackState);
        }
    }

    public void LateUpdate()
    {
    }

    public void Update()
    {
    }

    
}
