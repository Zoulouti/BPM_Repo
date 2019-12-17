using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Projectile Type
    public ProjectileType m_projectileType = ProjectileType.Player;
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
    #endregion

    public void Start()
    {
        RBody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        RBody.velocity = transform.forward * m_speed;
    }

    void OnTriggerEnter(Collider col)
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
        else if (m_projectileType == ProjectileType.Enemy)
        {
            if (col.CompareTag("Player"))
            {
                if (m_dieWhenHit)
                {
                    DestroyProjectile();
                }
            }
        }
        else if (m_projectileType == ProjectileType.Player)
        {
            switch (tag)
            {
                // Le tir du player touche un NoSpot
                case "NoSpot":

                    Debug.Log(tag);

                    break;

                // Le tir du player touche un WeakSpot
                case "WeakSpot":

                    Debug.Log(tag);

                    break;
                // Le tir du player touche un ArmorSpot
                case "ArmorSpot":

                    Debug.Log(tag);

                    break;
                // Le tir du player touche un DestroyableObject
                case "DestroyableObject":

                    Debug.Log(tag);

                    break;
                default:
                    break;
            }
        }

        DestroyProjectile();
    }

    void OnProjectilHits()
    {

    }


    void DestroyProjectile()
    {
        Destroy(gameObject);

        if (m_dieFX != null)
        {
            Level.AddFX(m_dieFX, transform.position, transform.rotation);    //Impact FX
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
