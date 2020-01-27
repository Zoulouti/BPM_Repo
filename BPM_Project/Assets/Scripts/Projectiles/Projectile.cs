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

    int damage;
    public float m_speed = 25;
    [Space]
    [Header("FX")]
    public GameObject m_dieFX;
    [Space]
    bool m_dieWhenHit = true;

    public float m_maxLifeTime = 5;

    Rigidbody m_rBody;
    WeaponBehaviour _WeaponBehaviour;
    BPMSystem m_BPMSystem;

    Vector3 m_awakeDistance;
    Vector3 m_currentDistance;
    Vector3 m_distanceToReach;

    #region Get Set
    public Rigidbody RBody
    {
        get
        {
            return m_rBody;
        }

        set
        {
            m_rBody = value;
        }
    }

    public int Damage { get => damage; set => damage = value; }
    public WeaponBehaviour WeaponBehaviour { get => _WeaponBehaviour; set => _WeaponBehaviour = value; }
    public ProjectileType ProjectileType1 { get => m_projectileType; set => m_projectileType = value; }
    public BPMSystem BPMSystem { get => m_BPMSystem; set => m_BPMSystem = value; }
    public Vector3 DistanceToReach { get => m_distanceToReach; set => m_distanceToReach = value; }
    #endregion

    public void Start()
    {
        RBody = GetComponent<Rigidbody>();
        m_awakeDistance = transform.position;
    }

    public void FixedUpdate()
    {
        if(RBody != null)
        {
            m_currentDistance = transform.position;
            RBody.velocity = transform.forward * m_speed;
            //Debug.Log("La m_distanceToReach est : " + m_distanceToReach.magnitude);

            //Debug.Log("La m_currentDistance est : " + m_currentDistance.sqrMagnitude);
            float deltaLength = Mathf.Abs(m_distanceToReach.magnitude - m_awakeDistance.magnitude);
            float newLength = deltaLength - Vector3.Distance(m_currentDistance, m_awakeDistance);
            //float deltaLength = Vector3.Distance(m_distanceToReach, m_currentDistance);

            //Debug.Log("La deltaLength est : " + deltaLength);
            //Debug.Log("La newLength est : " + deltaLength);

            if (newLength <= 0)
            {
                DestroyProjectile();
            }
        }
    }

    /*void OnTriggerEnter(Collider col)
    {
        string tag = col.tag;

        if (col.CompareTag("Untagged"))
        {
            if (m_dieWhenHit)
            {
                DestroyProjectile();
            }
        }
                // Le tir d'un enemy touche le player
        else if (ProjectileType1 == ProjectileType.Enemy)
        {
            if (col.CompareTag("Player"))
            {
                if (m_dieWhenHit)
                {
                    DestroyProjectile();
                }
            }
        }
        else if (ProjectileType1 == ProjectileType.Player)
        {
            switch (tag)
            {
                // Le tir du player touche un NoSpot
                case "NoSpot":

                    BPMSystem.GainBPM(BPMSystem._BPM.BPMGain_OnNoSpot);
                    col.GetComponent<ReferenceScipt>().cara.TakeDamage(Damage, 0);
                    Debug.Log(tag);

                    break;

                // Le tir du player touche un WeakSpot
                case "WeakSpot":

                    BPMSystem.GainBPM(BPMSystem._BPM.BPMGain_OnWeak);
                    col.GetComponent<ReferenceScipt>().cara.TakeDamage(Damage, 1);
                    Debug.Log(tag);

                    break;
                // Le tir du player touche un ArmorSpot
                case "ArmorSpot":

                    BPMSystem.GainBPM(BPMSystem._BPM.BPMGain_OnArmor);
                    col.GetComponent<ReferenceScipt>().cara.TakeDamage(Damage, 2);
                    Debug.Log(tag);

                    break;
                // Le tir du player touche un DestroyableObject
                case "DestroyableObject":

                    BPMSystem.GainBPM(BPMSystem._BPM.BPMGain_OnDestructableEnvironment);
                    Debug.Log(tag);

                    break;
                default:
                    break;
            }
        }

        DestroyProjectile();
    }*/

    void OnProjectilHits()
    {

    }


    void DestroyProjectile()
    {
        Destroy(gameObject);

        if (m_dieFX != null)
        {
            Level.AddFX(m_dieFX, transform.position, Quaternion.identity);    //Impact FX
        }
    }


    public void SetTargetPos(Vector3 targetPos)
    {
        Vector3 projectileToMouse = targetPos - transform.position;
        projectileToMouse.y = 0f;
        Quaternion newRotation = Quaternion.LookRotation(projectileToMouse);
        transform.rotation = newRotation;
    }

    /*public void SetTargetPosWithGamepad(Vector3 targetPos)
    {
        Vector3 projectileToMouse = targetPos - transform.position;
        projectileToMouse.y = 0f;
        Quaternion newRotation = Quaternion.LookRotation(projectileToMouse);
        newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z);
        transform.rotation = newRotation;
    }*/

}
