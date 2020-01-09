using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyCara : MonoBehaviour
{
    public DebugOuvreSurtoutPas _debug = new DebugOuvreSurtoutPas();
    [Serializable] public class DebugOuvreSurtoutPas
    {
        public GameObject[] weakSpots;
        public GameObject[] armorSpots;
    }
    [Space]
    public EnemyArchetype enemyArchetype;
    [Space]
    public EnemyCaractéristique _enemyCaractéristique = new EnemyCaractéristique();
    [Serializable] public class EnemyCaractéristique
    {
        [Tooltip("Override the stats in the GameManager")]
        public bool useCustomTweaking;
        [Space]
        public Move _move = new Move();
        [Serializable]
        public class Move
        {
            public float moveSpeed;
        }
        public Attack _attack = new Attack();
        [Serializable]
        public class Attack
        {
            public int damage;
            public float timeBetweenShots;
            public float reloadTime;
        }
        public Health _health = new Health();
        [Serializable]
        public class Health
        {
            public float maxHealth;
        }
    }
    float _currentLife;
    int _currentDamage;
    GameManager manager;

    public void Awake()
    {

        #region Get Singleton
        manager = GameManager.Instance;
        #endregion

        enemyArchetype.PopulateArray();

        #region Activate Archetype
        for (int i = 0, l= enemyArchetype.e_TypeOfSpot.Length; i < l; ++i)
        {
            if(enemyArchetype.e_TypeOfSpot[i] == EnemyArchetype.TypeOfSpot.WeakSpot)
            {
                _debug.weakSpots[i].SetActive(true);
                _debug.armorSpots[i].SetActive(false);
            }
            else if(enemyArchetype.e_TypeOfSpot[i] == EnemyArchetype.TypeOfSpot.ArmorSpot)
            {
                _debug.weakSpots[i].SetActive(false);
                _debug.armorSpots[i].SetActive(true);
            }
            else if(enemyArchetype.e_TypeOfSpot[i] == EnemyArchetype.TypeOfSpot.NoSpot)
            {
                _debug.weakSpots[i].SetActive(false);
                _debug.armorSpots[i].SetActive(false);
            }
        }
        #endregion

        InitializeEnemyStats(_enemyCaractéristique.useCustomTweaking);
    }

    public void TakeDamage(float damage, int i)
    {
        switch (i)
        {
            case 0:

                _currentLife -= damage * manager.noSpotDamageMultiplicateur;

                break;
            case 1:

                _currentLife -= damage * manager.weakSpotDamageMultiplicateur;

                break;
            case 2:

                _currentLife -= damage * manager.armorSpotDamageMultiplicateur;

                break;
            default:
                break;
        }
    }


    void InitializeEnemyStats(bool overRide)
    {
        if (overRide)
        {
            _currentLife = _enemyCaractéristique._health.maxHealth;
            _currentDamage = _enemyCaractéristique._attack.damage;
        }
        else
        {
            _currentLife = manager._health.maxHealth;
            _currentLife = manager._attack.damage;
        }
    }


}
