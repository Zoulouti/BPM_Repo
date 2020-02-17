using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponPlayerBehaviour : WeaponBehaviour
{

    float _currentBPMGain;
    float _currentBPMCost;

    GameObject _currentProjectil;

    BPMSystem _BPMSystem;

    public Camera playerCamera;
    public LayerMask rayCastCollision;

    public enum TypeOfFire
    {
        OnClick,
        Rafale,
        Auto
    
    }

    public SMG _SMG = new SMG();
    [Serializable]
    public class SMG
    {
        public Weapon weaponStats;
        [Space]
        public TypeOfFire typeOfFire;
        public GameObject firePoint;
        public GameObject fireAudio;
        [Space]
        public AnimationCurve recoilCurve;
        public float timeToRecoverFromRecoil;
        public float recoilHeight;
    }

    //public ShotGun _ShotGun = new ShotGun();
    //[Serializable]
    //public class ShotGun
    //{
    //    public TypeOfFire typeOfFire;
    //    public GameObject firePoint;
    //}

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

                if (Input.GetKeyDown(KeyCode.Mouse0) && canShoot)
                {
                    StartCoroutine(OnShoot(_currentnbrOfShoot, _currentAttackSpeed, _currentTimeBetweenEachBurst));
                }

                break;
            case TypeOfFire.Auto:

                if (Input.GetKey(KeyCode.Mouse0) && canShoot)
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

                InitiateWeaponVar(_SMG.weaponStats._weaponLevel0.damage, _SMG.weaponStats._weaponLevel0.attackCooldown, _SMG.weaponStats._weaponLevel0.BPMGainOnHit, _SMG.weaponStats._weaponLevel0.BPMCost, _SMG.weaponStats._weaponLevel0.bullet, _SMG.weaponStats._weaponLevel0.bulletSpeed);

                //if (_SMG.typeOfFire == TypeOfFire.Rafale)
                //{
                //    _currentTimeBetweenEachBurst = _SMG.weaponStats._weaponLevel0.timeBetweenBurst;
                //    _currentnbrOfShoot = _SMG.weaponStats._weaponLevel0.nbrOfShoot;
                //}

                break;
            case BPMSystem.WeaponState.Level1:

                InitiateWeaponVar(_SMG.weaponStats._weaponLevel1.damage, _SMG.weaponStats._weaponLevel1.attackCooldown, _SMG.weaponStats._weaponLevel1.BPMGainOnHit, _SMG.weaponStats._weaponLevel1.BPMCost, _SMG.weaponStats._weaponLevel0.bullet, _SMG.weaponStats._weaponLevel1.bulletSpeed);

                //if (_SMG.typeOfFire == TypeOfFire.Rafale)
                //{
                //    _currentTimeBetweenEachBurst = _SMG.weaponStats._weaponLevel1.timeBetweenBurst;
                //    _currentnbrOfShoot = _SMG.weaponStats._weaponLevel1.nbrOfShoot;
                //}

                break;
            case BPMSystem.WeaponState.Level2:

                InitiateWeaponVar(_SMG.weaponStats._weaponLevel2.damage, _SMG.weaponStats._weaponLevel2.attackCooldown, _SMG.weaponStats._weaponLevel2.BPMGainOnHit, _SMG.weaponStats._weaponLevel2.BPMCost, _SMG.weaponStats._weaponLevel2.newBullet, _SMG.weaponStats._weaponLevel2.bulletSpeed);

                //if (_SMG.typeOfFire == TypeOfFire.Rafale)
                //{
                //    _currentTimeBetweenEachBurst = _SMG.weaponStats._weaponLevel2.timeBetweenBurst;
                //    _currentnbrOfShoot = _SMG.weaponStats._weaponLevel2.nbrOfShoot;
                //}

                break;
        }
    }

    void InitiateWeaponVar(int damage, float attackSpeed, float BPMGain, float BPMCost, GameObject projectileObject, float projectileSpeed)
    {
        _currentDamage = damage;

        _currentAttackSpeed = attackSpeed;
        _currentBPMGain = BPMGain;
        _currentBPMCost = BPMCost;

        _currentProjectil = projectileObject;
        _currentProjectilSpeed = projectileSpeed;
    }

    #region ShootingMethods
    public override IEnumerator OnShoot(int nbrOfShoot, float timeEachShoot, float recoilTimeEachBurst)
    {
        canShoot = false;

        for (int i = 0; i < nbrOfShoot; ++i)
        {
            StartCoroutine(RecoilCurve());

            _BPMSystem.LoseBPM(_currentBPMCost);

            InitiateRayCast(InstatiateProj());
            
            yield return new WaitForSeconds(timeEachShoot);

        }
        yield return new WaitForSeconds(recoilTimeEachBurst);

        canShoot = true;
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

    //public override void InitiateProjectileVar(GameObject gameObject, Vector3 posToLookAt)
    //{
    //    NewProjectilWithFX projScript = gameObject.GetComponentInChildren<NewProjectilWithFX>();

    //    projScript.BPMSystem = _BPMSystem;
    //    projScript.Damage = _currentDamage;
    //    projScript.BPMGain = _currentBPMGain;
    //    Enum m_projectilState = projScript.ProjectileType1 = NewProjectilWithFX.ProjectileType.Player;

    //    gameObject.transform.LookAt(posToLookAt);
    //}


    //public override Vector3 GetProjectilLookAt()
    //{
    //    return Camera.main.transform.position + Camera.main.transform.forward * defaultDistance;            // Dirige le projectil dans la bonne direction (par rapport au reticule)
    //}
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
                projVar.DistanceToReach = _hit.point;
                projVar.Col = _hit.collider;
                projVar.BPMSystem = _BPMSystem;
                projVar.RayCastCollision = rayCastCollision;
                projVar.TransfoPos = playerCamera.transform.position;
                projVar.TransfoDir = playerCamera.transform.forward;
                projVar.CurrentBPMGain = _currentBPMGain;
                projVar.CurrentDamage = _currentDamage;
            }
            #endregion

        }
    }


    #endregion

}
