﻿using System.Collections;
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
        //public Attack _attack = new Attack();
        //[Serializable]
        //public class Attack
        //{
        //    public int damage;
        //    public float timeBetweenShots;
        //    public float reloadTime;
        //}
        public Health _health = new Health();
        [Serializable]
        public class Health
        {
            public float maxHealth;
            public int damageMultiplicatorOnWeakSpot = 1;
            public int damageMultiplicatorOnNoSpot = 1;
        }
        public StunResistance _stunResistance = new StunResistance();
        [Serializable]
        public class StunResistance
        {
            public float timeForStunResistance;
        }
    }
    EnemyController controller;
    float _currentLife;
    int _currentDamage;
    bool _isDead;
    float _currentTimeForElectricalStun;
    float _currentTimeForStunResistance;

    #region Get Set
    public float CurrentLife { get => _currentLife; set => _currentLife = value; }
    public bool IsDead { get => _isDead; set => _isDead = value; }
    public float CurrentTimeForElectricalStun { get => _currentTimeForElectricalStun; set => _currentTimeForElectricalStun = value; }
    #endregion

    public void OnEnable()
    {
        _isDead = false;
    }


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

    private void Update()
    {
        if(_currentTimeForStunResistance != 0)
        {
            _currentTimeForStunResistance -= Time.deltaTime;
            if(_currentTimeForStunResistance <= 0)
            {
                _currentTimeForStunResistance = 0;
            }
        }
    }

    public void TakeDamage(float damage, int i, bool hasToBeStun, float timeForElectricalStun)
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

        if (hasToBeStun && !controller.m_sM.CompareState((int)EnemyState.Enemy_StunState) && _currentTimeForStunResistance == 0f)
        {
            _currentTimeForElectricalStun = timeForElectricalStun;
            _currentTimeForStunResistance = _enemyCaractéristique._stunResistance.timeForStunResistance;
            controller.m_sM.ChangeState((int)EnemyState.Enemy_StunState);
        }

        CheckIfDead();

    }

    void CheckIfDead()
    {
        if (_currentLife <= 0)
        {
            _isDead = true;
            controller.m_sM.ChangeState((int)EnemyState.Enemy_DieState);
        }
    }

    void InitializeEnemyStats()
    {
        _currentLife = _enemyCaractéristique._health.maxHealth;
        //_currentDamage = _enemyCaractéristique._attack.damage;
    }
}
