using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewProjectilWithFX : MonoBehaviour
{
    #region Projectile Type
    ProjectileType m_projectileType = ProjectileType.Player;
    public enum ProjectileType
    {
        Player,
        Enemy
    }
    #endregion Projectile Type

    public GameObject parent;

    int damage;
    Rigidbody m_rBody;
    WeaponBehaviour _WeaponBehaviour;
    BPMSystem m_BPMSystem;
    float _BPMGain;

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
    public float BPMGain { get => _BPMGain; set => _BPMGain = value; }
    #endregion


    void OnCollisionEnter(Collision col)
    {
        string tag = col.collider.tag;

        if (col.collider.CompareTag("Untagged"))
        {
            DestroyProjectile();
        }
        // Le tir d'un enemy touche le player
        else if (ProjectileType1 == ProjectileType.Enemy)
        {
            if (col.collider.CompareTag("Player"))
            {
                DestroyProjectile();
            }
        }
        else if (ProjectileType1 == ProjectileType.Player)
        {
            switch (tag)
            {
                // Le tir du player touche un NoSpot
                case "NoSpot":

                    BPMSystem.GainBPM(BPMSystem._BPM.BPMGain_OnNoSpot * BPMGain);
                    col.collider.GetComponent<ReferenceScipt>().cara.TakeDamage(Damage, 0);
                    //Debug.Log(tag);

                    break;

                // Le tir du player touche un WeakSpot
                case "WeakSpot":

                    BPMSystem.GainBPM(BPMSystem._BPM.BPMGain_OnWeak * BPMGain);
                    col.collider.GetComponent<ReferenceScipt>().cara.TakeDamage(Damage, 1);
                    //Debug.Log(tag);

                    break;
                // Le tir du player touche un ArmorSpot
                case "ArmorSpot":

                    BPMSystem.GainBPM(BPMSystem._BPM.BPMGain_OnArmor * BPMGain);
                    col.collider.GetComponent<ReferenceScipt>().cara.TakeDamage(Damage, 2);
                    //Debug.Log(tag);

                    break;
                // Le tir du player touche un DestroyableObject
                case "DestroyableObject":

                    BPMSystem.GainBPM(BPMSystem._BPM.BPMGain_OnDestructableEnvironment);
                    BPMSystem.GainElectrarythmiePoints(BPMSystem._electrarythmie._electrarythmieGain_OnDestructableEnvironment);
                    //Debug.Log(tag);

                    break;
                default:
                    break;
            }
        }

        DestroyProjectile();
    }


    void DestroyProjectile()
    {
        Destroy(parent);
    }


    public void SetTargetPos(Vector3 targetPos)
    {
        Vector3 projectileToMouse = targetPos - transform.position;
        projectileToMouse.y = 0f;
        Quaternion newRotation = Quaternion.LookRotation(projectileToMouse);
        transform.rotation = newRotation;
    }
}
