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
    float m_speed = 25;
    [Space]
    [Header("FX")]
    public GameObject m_dieFX;
    [Space]
    bool m_dieWhenHit = true;

    public float m_maxLifeTime = 5;

    WeaponBehaviour _WeaponBehaviour;
    BPMSystem m_BPMSystem;

    Vector3 m_awakeDistance;
    Vector3 m_currentDistance;
    Collider m_col;
    Vector3 m_distanceToReach;
    float m_BPMGain;
    float deltaLength;
    float newLength;
    LayerMask m_rayCastCollision;
    Vector3 m_transfoPos;
    Vector3 m_transfoDir;
    float _currentBPMGain;
    int _currentDamage;
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

    bool startCalculateDistance;
    public void Start()
    {
        startCalculateDistance = true;
        m_awakeDistance = transform.localPosition;
        deltaLength = Vector3.Distance(m_distanceToReach, m_awakeDistance);
        StartCoroutine(CalculateDistance());
    }
    public void Update()
    {
        //Debug.Log(startCalculateDistance);

        if (startCalculateDistance)
        {
            //Debug.Log("DELATLENGTH " + deltaLength);
            //Debug.Log("NewLength "+ newLength);
            //Debug.Log("Distance "+ Vector3.Distance(m_currentDistance, m_awakeDistance));
            //Debug.Log("startCalculateDistance = " + startCalculateDistance);
            if (newLength <= 0)
            {
                //Debug.Log("startCalculateDistance = " + startCalculateDistance);
                startCalculateDistance = false;
            }
        }
    }

    IEnumerator CalculateDistance()
    {
        while(newLength > 0)
        {
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);
            m_currentDistance = transform.localPosition;
            newLength = deltaLength - Vector3.Distance(m_currentDistance, m_awakeDistance);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        DestroyProjectile();
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

    RaycastHit _hit;
    void DestroyProjectile()
    {
        bool ray = Physics.Raycast(transform.position, transform.forward, out _hit, Mathf.Infinity, RayCastCollision, QueryTriggerInteraction.Collide);
        if (ray)
        {
            string tag = _hit.collider.tag;
            if(Col == _hit.collider)
            {
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
                    /*// Le tir du player touche un ArmorSpot
                    case "ArmorSpot":

                        BPMGain = BPMSystem._BPM.BPMGain_OnArmor * CurrentBPMGain;

                        _hit.collider.GetComponent<ReferenceScipt>().cara.TakeDamage(CurrentDamage, 2);

                        break;*/
                }
                //Debug.Log(BPMGain);
                BPMSystem.GainBPM(BPMGain);
                if (m_dieFX != null)
                {
                    Level.AddFX(m_dieFX, transform.position, Quaternion.identity);    //Impact FX
                }
                Destroy(gameObject);
            }
            else
            {
                Debug.DrawLine(transform.position, _hit.point, Color.blue, 5f);
                Debug.DrawRay(transform.localPosition, transform.forward, Color.red, 5f);

                m_awakeDistance = m_currentDistance = transform.localPosition;
                m_distanceToReach = _hit.point;

                deltaLength = newLength = Vector3.Distance(m_distanceToReach, m_awakeDistance);

                StartCoroutine(CalculateDistance());

                Col = _hit.collider;

            }
        }
    }

    /*public void SetTargetPos(Vector3 targetPos)
    {
        Vector3 projectileToMouse = targetPos - transform.position;
        projectileToMouse.y = 0f;
        Quaternion newRotation = Quaternion.LookRotation(projectileToMouse);
        transform.rotation = newRotation;
    }*/

    /*public void SetTargetPosWithGamepad(Vector3 targetPos)
    {
        Vector3 projectileToMouse = targetPos - transform.position;
        projectileToMouse.y = 0f;
        Quaternion newRotation = Quaternion.LookRotation(projectileToMouse);
        newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z);
        transform.rotation = newRotation;
    }*/

}
