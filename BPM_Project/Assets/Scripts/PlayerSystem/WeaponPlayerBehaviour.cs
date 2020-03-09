﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TypeOfFireEnum;

public class WeaponPlayerBehaviour : WeaponBehaviour
{
    [Space]
    public Weapon weaponStats;
    float _currentBPMGain;
    float _currentBPMCost;
    float _currentTimeOfElctricalStun;
    bool _currentHasToStun;

    GameObject _currentProjectil;

    BPMSystem _BPMSystem;

    public Camera playerCamera;
    public LayerMask rayCastCollision;



    int defaultDistance = 500;

    RaycastHit _hit;


    public override void Awake()
    {
        base.Awake();
        _BPMSystem = GetComponent<BPMSystem>();
        ChangeWeaponStats();
    }

    public override void Update()
    {
        switch (_SMG.typeOfFire)
        {
            case TypeOfFire.OnClick:

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    StartCoroutine(OnShoot(1, 0, 0));
                }

                break;
            case TypeOfFire.Rafale:

                if (Input.GetKeyDown(KeyCode.Mouse0) && CanShoot)
                {
                    StartCoroutine(OnShoot(_currentnbrOfShoot, _currentAttackSpeed, _currentTimeBetweenEachBurst));
                }

                break;
            case TypeOfFire.Auto:

                if (Input.GetKey(KeyCode.Mouse0) && CanShoot)
                {
                    StartCoroutine(OnShoot(1, _currentAttackSpeed, 0));
                }

                break;
        }
    }

    public override void ChangeWeaponStats()
    {
        switch (_BPMSystem.CurrentWeaponState)
        {
            case BPMSystem.WeaponState.Level0:

                InitiateWeaponVar(weaponStats._weaponLevel0.damage, weaponStats._weaponLevel0.attackCooldown, weaponStats._weaponLevel0.BPMGainOnHit, weaponStats._weaponLevel0.BPMCost, weaponStats._weaponLevel0.bullet, weaponStats._weaponLevel0.bulletSpeed, false,0);

                break;
            case BPMSystem.WeaponState.Level1:

                InitiateWeaponVar(weaponStats._weaponLevel1.damage, weaponStats._weaponLevel1.attackCooldown, weaponStats._weaponLevel1.BPMGainOnHit, weaponStats._weaponLevel1.BPMCost, weaponStats._weaponLevel0.bullet, weaponStats._weaponLevel1.bulletSpeed, false,0);

                break;
            case BPMSystem.WeaponState.Level2:

                InitiateWeaponVar(weaponStats._weaponLevel2.damage, weaponStats._weaponLevel2.attackCooldown, weaponStats._weaponLevel2.BPMGainOnHit, weaponStats._weaponLevel2.BPMCost, weaponStats._weaponLevel2.newBullet, weaponStats._weaponLevel2.bulletSpeed, true, weaponStats._weaponLevel2.timeOfElectricalStun);

                break;
        }
    }

    void InitiateWeaponVar(int damage, float attackSpeed, float BPMGain, float BPMCost, GameObject projectileObject, float projectileSpeed, bool hasToStun, float timeOfElctricalStun)
    {
        _currentDamage = damage;

        _currentAttackSpeed = attackSpeed;
        _currentBPMGain = BPMGain;
        _currentBPMCost = BPMCost;

        _currentProjectil = projectileObject;
        _currentProjectilSpeed = projectileSpeed;

        _currentHasToStun = hasToStun;
        _currentTimeOfElctricalStun = timeOfElctricalStun;
    }

    #region ShootingMethods
    public override IEnumerator OnShoot(int nbrOfShoot, float timeEachShoot, float recoilTimeEachBurst)
    {
        CanShoot = false;

        for (int i = 0; i < nbrOfShoot; ++i)
        {
            StartCoroutine(RecoilCurve());

            _BPMSystem.LoseBPM(_currentBPMCost);

            InitiateRayCast(InstatiateProj());
            
            yield return new WaitForSeconds(timeEachShoot);

        }
        yield return new WaitForSeconds(recoilTimeEachBurst);

        CanShoot = true;
    }

    public override IEnumerator RecoilCurve()
    {
        _currentTimeToRecoverFromRecoil = 0;
        while (_currentTimeToRecoverFromRecoil / _SMG.timeToRecoverFromRecoil <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeToRecoverFromRecoil += Time.deltaTime;

            float recoil = _SMG.recoilCurve.Evaluate(_currentTimeToRecoverFromRecoil / _SMG.timeToRecoverFromRecoil);

            Vector3 rotationTemp = weaponObj.transform.localRotation.eulerAngles;

            float rotationX = _originalWeaponXRotation - _SMG.recoilHeight * recoil;
            rotationTemp.x = rotationX;

            weaponObj.transform.localEulerAngles = rotationTemp;
        }
        _currentTimeToRecoverFromRecoil = 0;
    }
    #endregion

        
    #region FeedBack Projectile Methods
    public override GameObject InstatiateProj()
    {
        _SMG.firePoint.transform.LookAt(OnSearchForLookAt());
        GameObject go = Instantiate(_currentProjectil, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation, projectilRoot);
        go.GetComponent<Projectile>().Speed = _currentProjectilSpeed;
        return go;
    }

    public override Vector3 OnSearchForLookAt()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, Mathf.Infinity, rayCastCollision, QueryTriggerInteraction.Collide))
        {
            return _hit.point;
        }
        return playerCamera.transform.position + playerCamera.transform.forward * defaultDistance;
    }
    #endregion

    #region RayCast Methods

    void InitiateRayCast(GameObject projectileFeedback)
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, Mathf.Infinity, rayCastCollision, QueryTriggerInteraction.Collide))
        {
            string tag = _hit.collider.tag;

            #region Initiate Proj Var
            Projectile projVar = projectileFeedback.GetComponent<Projectile>();

            if(projVar != null)
            {
                projVar.ProjectileType1 = Projectile.ProjectileType.Player;
                projVar.DistanceToReach = _hit.point;
                projVar.Col = _hit.collider;
                projVar.BPMSystem = _BPMSystem;
                projVar.RayCastCollision = rayCastCollision;
                projVar.TransfoPos = playerCamera.transform.position;
                projVar.TransfoDir = playerCamera.transform.forward;
                projVar.CurrentBPMGain = _currentBPMGain;
                projVar.CurrentDamage = _currentDamage;
                projVar.HasToStun = _currentHasToStun;
                projVar.TimeForElectricalStun = _currentTimeOfElctricalStun;
            }
            #endregion

        }
    }


    #endregion

}
