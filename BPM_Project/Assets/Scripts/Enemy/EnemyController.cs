using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyStateEnum;
using System;
using UnityEngine.UI;
using PoolTypes;

public class EnemyController : MonoBehaviour
{
    public DEBUG _debug = new DEBUG();
    [Serializable] public class DEBUG
    {
        public bool useGizmos;
        public Text m_stateText;
        public Text m_lifeText;
    }

    #region State Machine

    public StateMachine m_sM = new StateMachine();

    public virtual void OnEnable()
    {
        
        DistanceToTarget = GetTargetDistance(Target);
        EnemyCantShoot = false;
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

    EnemyCara cara;
    WeaponEnemyBehaviour weaponBehavior;
    NavMeshAgent agent;
    Transform target;

    float distanceToTarget;
    bool _enemyCanShoot;

    #region Get Set
    public NavMeshAgent Agent { get => agent; set => agent = value; }
    public Transform  Target { get => target; set => target = value; }

    public float DistanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }
    public EnemyCara Cara { get => cara; set => cara = value; }
    public bool EnemyCantShoot { get => _enemyCanShoot; set => _enemyCanShoot = value; }
    #endregion


    public void Awake()
    {
        SetupStateMachine();
        agent = GetComponent<NavMeshAgent>();
        cara = GetComponent<EnemyCara>();
        weaponBehavior = GetComponent<WeaponEnemyBehaviour>();
        Target = PlayerController.s_instance.gameObject.transform;
    }

    private void Start()
    {
        m_sM.Start();
    }

    private void Update()
    {
        m_sM.Update();
        if (_debug.useGizmos)
        {
            _debug.m_stateText.text = string.Format("{0}", m_sM.m_currentStateString);
            _debug.m_lifeText.text = string.Format("{0}", cara.CurrentLife);
        }
        _debug.m_stateText.gameObject.SetActive(_debug.useGizmos);
        _debug.m_lifeText.gameObject.SetActive(_debug.useGizmos);
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

    public IEnumerator IsStun()
    {
        EnemyCantShoot = true;
        yield return new WaitForSeconds(Cara.CurrentTimeForElectricalStun);
        EnemyCantShoot = false;
        ChangeState((int)EnemyState.Enemy_ChaseState);
    }

    public void KillNPC(float time)
    {
        EnemyCantShoot = Cara.IsDead;
        StartCoroutine(OnWaitForAnimToEnd(time));
    }


    IEnumerator OnWaitForAnimToEnd(float time)
    {
        cara.enabled = false;
        agent.enabled = false;
        yield return new WaitForSeconds(time);    //Animation time
        Spawned_Tracker spawnTracker = GetComponent<Spawned_Tracker>();
        if (spawnTracker != null)
        {
            spawnTracker.CallDead();
            Destroy(spawnTracker);
        }
        PoolTracker poolTracker = GetComponent<PoolTracker>();
        if (poolTracker != null)
        {
            Destroy(poolTracker);
        }
        ObjectPooler.Instance.ReturnEnemyToPool(EnemyType.EnemyBase, gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (_debug.useGizmos)
        { 
            if(agent != null && weaponBehavior != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(new Vector3(weaponBehavior._SMG.firePoint.transform.position.x, weaponBehavior._SMG.firePoint.transform.position.y, weaponBehavior._SMG.firePoint.transform.position.z + (agent.stoppingDistance - weaponBehavior._attack.enemyAttackDispersement*2)* weaponBehavior._attack._debugGizmos), weaponBehavior._attack.enemyAttackDispersement);
            }
            else
            {
                agent = GetComponent<NavMeshAgent>();
                weaponBehavior = GetComponent<WeaponEnemyBehaviour>();
            }
        }
    }
}
