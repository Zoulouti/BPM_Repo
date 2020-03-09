using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EnemyStateEnum;

public class EnemyCara : MonoBehaviour
{
    public DebugOuvreSurtoutPas _debug = new DebugOuvreSurtoutPas();
    [Serializable] public class DebugOuvreSurtoutPas
    {
        public GameObject[] weakSpots;
        public GameObject[] armorSpots;
        public GameObject[] noSpot;
    }
    [Space]
    public EnemyArchetype enemyArchetype;
    [Space]
    public EnemyCaractéristique _enemyCaractéristique = new EnemyCaractéristique();
    [Serializable] public class EnemyCaractéristique
    {
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
            public int damageMultiplicatorOnWeakSpot = 1;
            public int damageMultiplicatorOnNoSpot = 1;
        }
    }
    EnemyController controller;
    float _currentLife;
    int _currentDamage;

    public void Awake()
    {
        controller = GetComponent<EnemyController>();
        enemyArchetype.PopulateArray();

        #region Activate Archetype
        for (int i = 0, l= enemyArchetype.e_TypeOfSpot.Length; i < l; ++i)
        {
            if(enemyArchetype.e_TypeOfSpot[i] == EnemyArchetype.TypeOfSpot.WeakSpot)
            {
                _debug.weakSpots[i].SetActive(true);
                _debug.armorSpots[i].SetActive(false);
                _debug.noSpot[i].SetActive(false);
            }
            else if(enemyArchetype.e_TypeOfSpot[i] == EnemyArchetype.TypeOfSpot.NoSpot)
            {
                _debug.weakSpots[i].SetActive(false);
                _debug.armorSpots[i].SetActive(false);
                _debug.noSpot[i].SetActive(true);
            }
        }
        #endregion

        InitializeEnemyStats();
    }

    public void TakeDamage(float damage, int i)
    {
        switch (i)
        {
            case 0:

                _currentLife -= damage * _enemyCaractéristique._health.damageMultiplicatorOnNoSpot;

                break;
            case 1:

                _currentLife -= damage * _enemyCaractéristique._health.damageMultiplicatorOnWeakSpot;

                break;
            default:
                break;
        }

        if(_currentLife <= 0)
        {
            controller.m_sM.ChangeState((int)EnemyState.Enemy_DieState);
        }

    }
    void InitializeEnemyStats()
    {
        _currentLife = _enemyCaractéristique._health.maxHealth;
        _currentDamage = _enemyCaractéristique._attack.damage;
    }
}
