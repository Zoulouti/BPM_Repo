using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyStateEnum;

public class EnemyController : MonoBehaviour
{
    #region State Machine

    public StateMachine m_sM = new StateMachine();

    public virtual void OnEnable()
    {
        ChangeState((int)EnemyState.Enemy_ChaseState);
    }

    public void ChangeState(int i)
    {
        m_sM.ChangeState(i);
    }

    public int GetLastStateIndex()
    {
        return m_sM.LastStateIndex;
    }

    #endregion

}
