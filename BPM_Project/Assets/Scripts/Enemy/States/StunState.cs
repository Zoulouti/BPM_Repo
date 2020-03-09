using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class StunState : IState
{

    EnemyController m_enemyController;

    public StunState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        m_enemyController.StartCoroutine(m_enemyController.IsStun());
    }

    public void Exit()
    {
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void Update()
    {
    }

    

}
