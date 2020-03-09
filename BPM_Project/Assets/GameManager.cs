﻿using System.Collections;
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

    public float enemyAttackDispersement;
    public float timeMeshShowingUp;

    [Header("Differents Spots Multiplicator Tweaking")]
    public float noSpotDamageMultiplicateur;
    public float weakSpotDamageMultiplicateur;


    /*public void Start()
    {
        StartCoroutine(TestSpawnDispersion());
    }

    IEnumerator TestSpawnDispersion()
    {
        Vector2 dispersion = UnityEngine.Random.insideUnitCircle * enemyAttackDispersement;
        Vector3 pos = transform.position;
        pos.x = dispersion.x;
        pos.y = dispersion.y;
        transform.position = pos;
        yield return new WaitForSeconds(timeMeshShowingUp);
        StartCoroutine(TestSpawnDispersion());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, enemyAttackDispersement);
    }*/
}
