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
        if(GetTargetDistance(m_enemyController.Target) < m_enemyController.Agent.stoppingDistance)
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

    public float GetTargetDistance(Transform target)
    {
        return Vector3.Distance(m_enemyController.Target.position, m_enemyController.transform.position);
    }
}
