using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Two instance of GameManager");
        }
    }
    #endregion

    [Space]
    [Header("Enemy Stats Tweaking")]
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
        public float recoilTime;
    }
    public Health _health = new Health();
    [Serializable]
    public class Health
    {
        public float maxHealth;
    }
    [Header("Differents Spots Multiplicator Tweaking")]
    public float noSpotDamageMultiplicateur;
    public float weakSpotDamageMultiplicateur;
    public float armorSpotDamageMultiplicateur;
}
