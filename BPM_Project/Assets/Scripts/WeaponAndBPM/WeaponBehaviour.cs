using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponBehaviour : MonoBehaviour
{
    public Weapon[] allWeapons;
    [Space]
    public GameObject projectils;

    BPMSystem _BPMSystem;
    int activatedWeapon = 0;

    int defaultDistance = 500;

    

    private void Awake()
    {
        _BPMSystem = GetComponent<BPMSystem>();
    }

    // ---------- A virer -----------
    RaycastHit _hit;
    public GameObject firePoint;
    // ---------- A virer -----------

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnPlayerShoot();
        }
    }

    void OnPlayerShoot()
    {
        Vector3 posToLookAt;

        _BPMSystem.LoseBPM(allWeapons[activatedWeapon]._weaponLevel0.BPMCost);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, Mathf.Infinity))
        {
            posToLookAt = _hit.point;
        }
        else
        {
            posToLookAt = GetProjectilLookAt();
        }

        GameObject go = Instantiate(allWeapons[activatedWeapon]._weaponLevel0.bullet, firePoint.transform.position, firePoint.transform.rotation);
        Projectile projScript = go.GetComponent<Projectile>();

        projScript.BPMSystem = _BPMSystem;
        Enum m_projectilState = projScript.ProjectileType1 = Projectile.ProjectileType.Player;

        go.transform.LookAt(posToLookAt);

        OnShoot(m_projectilState);
    }

    public void OnShoot(Enum m_projectileType)
    {
        //Debug.Log(m_projectileType);
    }

    Vector3 GetProjectilLookAt()
    {
        return Camera.main.transform.position + Camera.main.transform.forward * defaultDistance;            // Dirige le projectil dans la bonne direction (par rapport au reticule)
    }
}
