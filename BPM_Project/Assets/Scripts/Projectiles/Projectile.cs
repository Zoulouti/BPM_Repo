using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Projectile Type
    ProjectileType m_projectileType = ProjectileType.Player;
    public enum ProjectileType
    {
        Player,
        Enemy
    }
    #endregion Projectile Type

    [Space]
    [Header("FX")]
    public GameObject m_dieFX;
    [Space]
    public float m_maxLifeTime = 5;
    [Header("DEBUG")]
    [Space]
    public bool useRigibody;
    public float forceBuffer = 50f;
    Rigidbody rb;
    SphereCollider col;



    RaycastHit _hit;
    bool m_dieWhenHit = true;

    float deltaLength;
    float newLength;
    Vector3 m_awakeDistance;
    Vector3 m_currentDistance;


    WeaponBehaviour _WeaponBehaviour;
    BPMSystem m_BPMSystem;
    LayerMask m_rayCastCollision;

    Collider m_col;

    Vector3 m_distanceToReach;
    Vector3 m_transfoPos;
    Vector3 m_transfoDir;

    float m_BPMGain;
    float _currentBPMGain;
    float m_speed = 25;
    int _currentDamage;
    int damage;
    

    #region Get Set
    public WeaponBehaviour WeaponBehaviour { get => _WeaponBehaviour; set => _WeaponBehaviour = value; }
    public LayerMask RayCastCollision { get => m_rayCastCollision; set => m_rayCastCollision = value; }
    public BPMSystem BPMSystem { get => m_BPMSystem; set => m_BPMSystem = value; }

    public int CurrentDamage { get => _currentDamage; set => _currentDamage = value; }
    public float CurrentBPMGain { get => _currentBPMGain; set => _currentBPMGain = value; }
    public int Damage { get => damage; set => damage = value; }
    public float BPMGain { get => m_BPMGain; set => m_BPMGain = value; }
    public float Speed { get => m_speed; set => m_speed = value; }

    public Vector3 TransfoPos { get => m_transfoPos; set => m_transfoPos = value; }
    public Vector3 TransfoDir { get => m_transfoDir; set => m_transfoDir = value; }
    public Collider Col { get => m_col; set => m_col = value; }
    public Vector3 DistanceToReach { get => m_distanceToReach; set => m_distanceToReach = value; }

    public ProjectileType ProjectileType1 { get => m_projectileType; set => m_projectileType = value; }
    #endregion

    public void Start()
    {
        if (useRigibody)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            col = gameObject.AddComponent<SphereCollider>();

            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.interpolation = RigidbodyInterpolation.Extrapolate;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.useGravity = false;

            col.isTrigger = true;
            col.radius = 0.05f;
        }
        else
        {
            #region Set Starting Length
            m_awakeDistance = transform.localPosition;
            deltaLength = Vector3.Distance(m_distanceToReach, m_awakeDistance);
            newLength = deltaLength;
            #endregion

            StartCoroutine(CalculateDistance());
        }

        StartCoroutine(DestroyAnyway());

    }

    #region When using RayCast

    IEnumerator CalculateDistance()
    {
        while(newLength > 0)
        {
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);
            m_currentDistance = transform.localPosition;
            newLength = deltaLength - Vector3.Distance(m_currentDistance, m_awakeDistance);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        OnProjectilReachingMaxDistance();
    }

    void OnProjectilReachingMaxDistance()
    {
        bool ray = Physics.Raycast(TransfoPos, TransfoDir, out _hit, Mathf.Infinity, RayCastCollision, QueryTriggerInteraction.Collide);
        if (ray)
        {
            string tag = _hit.collider.tag;
            if(Col == _hit.collider)
            {
                StopCoroutine(CalculateDistance());

                #region Switch For WeakSpots
                switch (tag)
                {
                    // Le tir du player touche un NoSpot
                    case "NoSpot":

                        BPMGain = BPMSystem._BPM.BPMGain_OnNoSpot * CurrentBPMGain;

                        _hit.collider.GetComponent<ReferenceScipt>().cara.TakeDamage(CurrentDamage, 0);

                        break;

                    // Le tir du player touche un WeakSpot
                    case "WeakSpot":

                        BPMGain = BPMSystem._BPM.BPMGain_OnWeak * CurrentBPMGain;

                        _hit.collider.GetComponent<ReferenceScipt>().cara.TakeDamage(CurrentDamage, 1);

                        break;
                }
                #endregion

                BPMSystem.GainBPM(BPMGain);

                if (m_dieFX != null)
                {
                    Level.AddFX(m_dieFX, transform.position, Quaternion.identity);    //Impact FX
                    if(_hit.collider.GetComponent<Rigidbody>() != null)
                    {
                        Rigidbody _rb = _hit.collider.GetComponent<Rigidbody>();
                        _rb.AddForceAtPosition(-(_hit.normal * forceBuffer), _hit.point);
                    }
                }

                DestroyProj();
            }
            else //Si la cible a bougé avant que le projectile ait atteind sa cible
            {
                #region Set New Length
                m_awakeDistance = m_currentDistance = transform.localPosition;
                m_distanceToReach = _hit.point;

                deltaLength = newLength = Vector3.Distance(m_distanceToReach, m_awakeDistance);
                TransfoPos = m_awakeDistance;
                TransfoDir = transform.forward;
                Col = _hit.collider;
                #endregion

                StartCoroutine(CalculateDistance());
            }
        }
    }

    #endregion

    #region When Using Rigibody

    private void FixedUpdate()
    {
        if (useRigibody)
        {
            rb.velocity = transform.forward * Speed;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(col != null)
        {
            string tag = other.tag;
            #region Switch For WeakSpots

            switch (tag)
            {
                // Le tir du player touche un NoSpot
                case "NoSpot":

                    BPMGain = BPMSystem._BPM.BPMGain_OnNoSpot * CurrentBPMGain;

                    other.GetComponent<ReferenceScipt>().cara.TakeDamage(CurrentDamage, 0);

                    break;

                // Le tir du player touche un WeakSpot
                case "WeakSpot":

                    BPMGain = BPMSystem._BPM.BPMGain_OnWeak * CurrentBPMGain;

                    other.GetComponent<ReferenceScipt>().cara.TakeDamage(CurrentDamage, 1);

                    break;
            }

            #endregion

            BPMSystem.GainBPM(BPMGain);

            if (m_dieFX != null)
            {
                Level.AddFX(m_dieFX, transform.position, Quaternion.identity);    ///Impact FX
            }

            DestroyProj();
        }
    }
    #endregion

    IEnumerator DestroyAnyway()
    {
        yield return new WaitForSeconds(m_maxLifeTime);
        DestroyProj();
    }

    void DestroyProj()
    {
        Destroy(gameObject);
    }
}
