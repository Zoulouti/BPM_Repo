using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponBehaviour : MonoBehaviour
{
    public Weapon[] allWeapons;
    [Space]
    public GameObject projectils;
    public Transform projectilRoot;
    [Space]
    public TypeOfFire typeOfFire;
    public enum TypeOfFire
    {
        OnClick,
        Rafale,
        Auto
    }

    /*[Flags]
    public enum TypeOfFire
    {
        None = 0,
        OnClick = 1 << 0,
        Rafale = 1 << 1,
        Auto = 1 << 2, 
        Everything = ~None
    }*/

    public Burst _burst = new Burst();
    [Serializable]
    public class Burst
    {
        public int nbrOfShoot = 3;
        public float timeBetweenShoots;
        public float timeBetweenEachBurst;
    }

    public Auto _auto = new Auto();
    [Serializable]
    public class Auto
    {
        public float timeBetweenShoots;
    }

    BPMSystem _BPMSystem;

    int activatedWeapon = 0;
    int defaultDistance = 500;
    int _currentDamage;

    bool canShoot = true;

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

        switch (typeOfFire)
        {
            case TypeOfFire.OnClick:

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    StartCoroutine(OnPlayerShoot(1,0,0));
                }

                break;
            case TypeOfFire.Rafale:

                if (Input.GetKeyDown(KeyCode.Mouse0) && canShoot)
                {
                    StartCoroutine(OnPlayerShoot(_burst.nbrOfShoot, _burst.timeBetweenShoots, _burst.timeBetweenEachBurst));
                }

                break;
            case TypeOfFire.Auto:

                if (Input.GetKey(KeyCode.Mouse0) && canShoot)
                {
                    StartCoroutine(OnPlayerShoot(1, _auto.timeBetweenShoots, 0));
                }

                break;
        }

        switch (_BPMSystem.CurrentWeaponState)
        {
            case BPMSystem.WeaponState.Level0:

                _currentDamage = allWeapons[activatedWeapon]._weaponLevel0.damage;

                break;
            case BPMSystem.WeaponState.Level1:

                _currentDamage = allWeapons[activatedWeapon]._weaponLevel1.damage;

                break;
            case BPMSystem.WeaponState.Level2:

                _currentDamage = allWeapons[activatedWeapon]._weaponLevel2.damage;

                break;
        }

    }
    IEnumerator OnPlayerShoot(int nbrOfShoot, float timeEachShoot, float recoilTimeEachBurst)
    {
        canShoot = false;

        for (int i = 0; i < nbrOfShoot; ++i)
        {

            _BPMSystem.LoseBPM(allWeapons[activatedWeapon]._weaponLevel0.BPMCost);

            InitiateProjectileVar(InstatiateProj(), OnSearchForLookAt());

            yield return new WaitForSeconds(timeEachShoot);

        }
        yield return new WaitForSeconds(recoilTimeEachBurst);

        canShoot = true;
    }

    public void OnShoot(Enum m_projectileType)
    {
        //Debug.Log(m_projectileType);
    }

    Vector3 OnSearchForLookAt()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, Mathf.Infinity))
        {
            return _hit.point;
        }
        else
        {
            return GetProjectilLookAt();
        }
    }

    GameObject InstatiateProj()
    {
        return Instantiate(allWeapons[activatedWeapon]._weaponLevel0.bullet, firePoint.transform.position, firePoint.transform.rotation, projectilRoot);
    }

    void InitiateProjectileVar(GameObject gameobject, Vector3 posToLookAt)
    {
        NewProjectilWithFX projScript = gameobject.GetComponentInChildren<NewProjectilWithFX>();

        projScript.BPMSystem = _BPMSystem;
        projScript.Damage = _currentDamage;
        Enum m_projectilState = projScript.ProjectileType1 = NewProjectilWithFX.ProjectileType.Player;

        gameobject.transform.LookAt(posToLookAt);
    }

    Vector3 GetProjectilLookAt()
    {
        return Camera.main.transform.position + Camera.main.transform.forward * defaultDistance;            // Dirige le projectil dans la bonne direction (par rapport au reticule)
    }
}