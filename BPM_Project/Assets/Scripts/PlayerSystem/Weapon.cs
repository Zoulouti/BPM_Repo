using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName ="New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public WeaponLevel0 _weaponLevel0 = new WeaponLevel0();
    [Serializable] public class WeaponLevel0
    {
        public int damage;
        [Space]
        public float attackCooldown;
        [Tooltip("Only if you use Rafale")]
        public float timeBetweenBurst;
        [Tooltip("Only if you use Rafale")]
        public int nbrOfShoot;
        [Space]
        public float BPMGainOnHit;
        [Space]
        public int BPMCost;
        [Space]
        public GameObject bullet;

    }
    public WeaponLevel1 _weaponLevel1 = new WeaponLevel1();
    [Serializable]
    public class WeaponLevel1
    {
        public int damage;
        [Space]
        public float attackCooldown;
        [Tooltip("Only if you use Rafale")]
        public float timeBetweenBurst;
        [Tooltip("Only if you use Rafale")]
        public int nbrOfShoot;
        [Space]
        public float BPMGainOnHit;
        [Space]
        public int BPMCost;
    }
    public WeaponLevel2 _weaponLevel2 = new WeaponLevel2();
    [Serializable]
    public class WeaponLevel2
    {
        public int damage;
        [Space]
        public float attackCooldown;
        [Tooltip("Only if you use Rafale")]
        public float timeBetweenBurst;
        [Tooltip("Only if you use Rafale")]
        public int nbrOfShoot;
        [Space]
        public float BPMGainOnHit;
        [Space]
        public int BPMCost;
        [Space]
        public GameObject newBullet;
    }

}
