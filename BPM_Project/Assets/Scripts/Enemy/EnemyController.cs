using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyStateEnum;
using System;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public DEBUG _debug = new DEBUG();
    [Serializable] public class DEBUG
    {
        public bool useGizmos;
        public Text m_text;
    }

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

    NavMeshAgent agent;
    Transform target;

    float distanceToTarget;

    #region Get Set
    public NavMeshAgent Agent { get => agent; set => agent = value; }
    public Transform  Target { get => target; set => target = value; }

    public float DistanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }
    #endregion

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        SetupStateMachine();
    }

    private void Start()
    {
        m_sM.Start();
        Target = PlayerController.s_instance.gameObject.transform;
        DistanceToTarget = GetTargetDistance(Target);

    }

    private void Update()
    {
        m_sM.Update();
        if (_debug.useGizmos)
        {
            _debug.m_text.text = string.Format("{0}", m_sM.m_currentStateString);
        }
        _debug.m_text.gameObject.SetActive(_debug.useGizmos);
        DistanceToTarget = GetTargetDistance(Target);
    }

    void FixedUpdate()
    {
        m_sM.FixedUpdate();
    }

    void LateUpdate()
    {
        m_sM.LateUpdate();
    }


    void SetupStateMachine()
    {
        m_sM.AddStates(new List<IState> { 
            new ChaseState(this),				// 0 = Chase
			new AttackState(this),				// 2 = Attack
			new StunState(this),				// 6 = Stun
			new DieState(this),				    // 7 = Die
		});

        string[] playerStateNames = System.Enum.GetNames(typeof(EnemyState));
        if (m_sM.States.Count != playerStateNames.Length)
        {
            Debug.LogError("You need to have the same number of State in PlayerController and PlayerStateEnum");
        }

        ChangeState((int)EnemyState.Enemy_ChaseState);
    }


    public float GetTargetDistance(Transform target)
    {
        return Vector3.Distance(Target.position, transform.position);
    }






    private void OnDrawGizmosSelected()
    {
        if (_debug.useGizmos)
        { 
            if(agent != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
            }
            else
            {
                agent = GetComponent<NavMeshAgent>();
            }
        }
    }
}
