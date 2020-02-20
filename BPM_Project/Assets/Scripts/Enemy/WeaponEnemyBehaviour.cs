using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnemyBehaviour : WeaponBehaviour
{
    [Space]
    public int nbrOfShootOnRafale;
    public float timeForEachBurst;
    [Space]
    public GameObject enemyProjectil;
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
        go.GetComponent<Projectile>().Speed = _SMG.weaponStats._weaponLevel0.bulletSpeed;
        return go;
    }

    public override Vector3 OnSearchForLookAt()
    {
        return enemyController.Target.position;
    }
    #endregion
}
