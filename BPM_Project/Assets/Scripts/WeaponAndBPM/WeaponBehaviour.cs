using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponBehaviour : MonoBehaviour
{
    public Weapon[] allWeapons;
    [Space]
    public Transform projectilRoot;

    /*[Flags]
    public enum TypeOfFire
    {
        None = 0,
        OnClick = 1 << 0,
        Rafale = 1 << 1,
        Auto = 1 << 2, 
        Everything = ~None
    }*/
    public enum TypeOfFire
    {
        OnClick,
        Rafale,
        Auto
    }
    public SMG _SMG = new SMG();
    [Serializable] public class SMG
    {
        public TypeOfFire typeOfFire;
        /*public Burst _burst = new Burst();
        [Serializable]
        public class Burst
        {
            public int nbrOfShoot = 3;
            public float timeBetweenShoots;
            public float timeBetweenEachBurst;
        }*/

        /*public Auto _auto = new Auto();
        [Serializable]
        public class Auto
        {
            public float timeBetweenShoots;
        }*/
        public GameObject firePoint;
    }

    BPMSystem _BPMSystem;

    int activatedWeapon = 0;
    int defaultDistance = 500;
    int _currentDamage;

    float _currentAttackSpeed;
    float _currentTimeBetweenEachBurst;
    int _currentnbrOfShoot;

    bool canShoot = true;

    private void Awake()
    {
        _BPMSystem = GetComponent<BPMSystem>();
    }
    // ---------- A virer -----------
    RaycastHit _hit;
    // ---------- A virer -----------

    private void Update()
    {
        switch (_SMG.typeOfFire)
        {
            case TypeOfFire.OnClick:

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    StartCoroutine(OnPlayerShoot(1, 0, 0));
                }

                break;
            case TypeOfFire.Rafale:

                if (Input.GetKeyDown(KeyCode.Mouse0) && canShoot)
                {
                    StartCoroutine(OnPlayerShoot(_currentnbrOfShoot, _currentAttackSpeed, _currentTimeBetweenEachBurst));
                }

                break;
            case TypeOfFire.Auto:

                if (Input.GetKey(KeyCode.Mouse0) && canShoot)
                {
                    StartCoroutine(OnPlayerShoot(1, _currentAttackSpeed, 0));
                }

                break;
        }
    }

    public void ChangeWeaponStats()
    {
        switch (_BPMSystem.CurrentWeaponState)
        {
            case BPMSystem.WeaponState.Level0:

                _currentDamage = allWeapons[activatedWeapon]._weaponLevel0.damage;
                _currentAttackSpeed = allWeapons[activatedWeapon]._weaponLevel0.attackCooldown;

                if (_SMG.typeOfFire == TypeOfFire.Rafale)
                {
                    _currentTimeBetweenEachBurst = allWeapons[activatedWeapon]._weaponLevel0.timeBetweenBurst;
                    _currentnbrOfShoot = allWeapons[activatedWeapon]._weaponLevel0.nbrOfShoot;
                }

                break;
            case BPMSystem.WeaponState.Level1:

                _currentDamage = allWeapons[activatedWeapon]._weaponLevel1.damage;
                _currentAttackSpeed = allWeapons[activatedWeapon]._weaponLevel1.attackCooldown;

                if (_SMG.typeOfFire == TypeOfFire.Rafale)
                {
                    _currentTimeBetweenEachBurst = allWeapons[activatedWeapon]._weaponLevel1.timeBetweenBurst;
                    _currentnbrOfShoot = allWeapons[activatedWeapon]._weaponLevel1.nbrOfShoot;
                }

                break;
            case BPMSystem.WeaponState.Level2:

                _currentDamage = allWeapons[activatedWeapon]._weaponLevel2.damage;
                _currentAttackSpeed = allWeapons[activatedWeapon]._weaponLevel2.attackCooldown;

                if (_SMG.typeOfFire == TypeOfFire.Rafale)
                {
                    _currentTimeBetweenEachBurst = allWeapons[activatedWeapon]._weaponLevel2.timeBetweenBurst;
                    _currentnbrOfShoot = allWeapons[activatedWeapon]._weaponLevel2.nbrOfShoot;
                }

                break;
        }
    }

    IEnumerator OnPlayerShoot(int nbrOfShoot, float timeEachShoot, float recoilTimeEachBurst)
    {
        canShoot = false;

        _BPMSystem.LoseBPM(allWeapons[activatedWeapon]._weaponLevel0.BPMCost);

        for (int i = 0; i < nbrOfShoot; ++i)
        {

            InitiateProjectileVar(InstatiateProj(_BPMSystem.CurrentWeaponState), OnSearchForLookAt());

            yield return new WaitForSeconds(timeEachShoot);

        }
        yield return new WaitForSeconds(recoilTimeEachBurst);

        canShoot = true;
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

    GameObject InstatiateProj(Enum weaponState)
    {
        switch (weaponState)
        {
            case BPMSystem.WeaponState.Level0:

                return Instantiate(allWeapons[activatedWeapon]._weaponLevel0.bullet, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation, projectilRoot);

            case BPMSystem.WeaponState.Level1:

                return Instantiate(allWeapons[activatedWeapon]._weaponLevel0.bullet, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation, projectilRoot);

            case BPMSystem.WeaponState.Level2:

                return Instantiate(allWeapons[activatedWeapon]._weaponLevel2.newBullet, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation, projectilRoot);

            default:
                break;
        }
        return null;
    }

    void InitiateProjectileVar(GameObject gameObject, Vector3 posToLookAt)
    {

        
        NewProjectilWithFX projScript = gameObject.GetComponentInChildren<NewProjectilWithFX>();

        projScript.BPMSystem = _BPMSystem;
        projScript.Damage = _currentDamage;
        Enum m_projectilState = projScript.ProjectileType1 = NewProjectilWithFX.ProjectileType.Player;

        gameObject.transform.LookAt(posToLookAt);
    }

    Vector3 GetProjectilLookAt()
    {
        return Camera.main.transform.position + Camera.main.transform.forward * defaultDistance;            // Dirige le projectil dans la bonne direction (par rapport au reticule)
    }
}