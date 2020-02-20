using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnemyBehaviour : WeaponBehaviour
{
    [Space]
    public int nbrOfShootOnRafale;
    public float timeForEachBurst;
    public float enemyAttackDispersement;
    [Space]
    public GameObject enemyProjectil;
    [Space]
    [Tooltip("Pour que l'ennemies ne tir pas dans les pieds du player")]
    public float YOffset = 1f;
    EnemyController enemyController;

    public override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyController>();
    }


    #region ShootingMethods


    public override IEnumerator OnShoot(int nbrOfShoot, float timeEachShoot, float recoilTimeEachBurst)
    {
        for (int i = 0; i < nbrOfShoot; ++i)
        {
            StartCoroutine(RecoilCurve());

            InstatiateProj();

            yield return new WaitForSeconds(timeEachShoot);

        }
        yield return new WaitForSeconds(recoilTimeEachBurst);
    }

    public override IEnumerator RecoilCurve()
    {
        _currentTimeToRecoverFromRecoil = 0;
        while (_currentTimeToRecoverFromRecoil / _SMG.timeToRecoverFromRecoil <= 1)
        {
            _currentTimeToRecoverFromRecoil += Time.deltaTime;

            float recoil = _SMG.recoilCurve.Evaluate(_currentTimeToRecoverFromRecoil / _SMG.timeToRecoverFromRecoil);

            Vector3 rotationTemp = weaponObj.transform.localRotation.eulerAngles;

            float rotationX = _originalWeaponXRotation - _SMG.recoilHeight * recoil;
            rotationTemp.x = rotationX;

            weaponObj.transform.localEulerAngles = rotationTemp;
            yield return null;
        }
        _currentTimeToRecoverFromRecoil = 0;
    }
    #endregion


    #region FeedBack Projectile Methods
    public override GameObject InstatiateProj()
    {
        _SMG.firePoint.transform.LookAt(OnSearchForLookAt());
        GameObject go = Instantiate(enemyProjectil, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation, projectilRoot);
        InitiateProjVar(go.GetComponent<Projectile>());
        return go;
    }


    void InitiateProjVar(Projectile proj)
    {
        proj.m_colType = Projectile.TypeOfCollision.Rigibody;
        proj.ProjectileType1 = Projectile.ProjectileType.Enemy;
        proj.Speed = _SMG.weaponStats._weaponLevel0.bulletSpeed;
        proj.CurrentDamage = _SMG.weaponStats._weaponLevel0.damage;
    }

    public override Vector3 OnSearchForLookAt()
    {
        Vector2 dispersion = Random.insideUnitCircle * enemyAttackDispersement;
        return new Vector3(enemyController.Target.position.x + dispersion.x, (enemyController.Target.position.y + YOffset) + dispersion.y, enemyController.Target.position.z ) ;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_SMG.firePoint.transform.position, enemyAttackDispersement);
    }
    #endregion
}
