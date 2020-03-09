﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public SpawnerController[] spawners;
    [Space]
    public float timeBetweenEachSpawn;
    int _nbrOfEnemy;
    int _nbrOfDeadEnemy;
    int _nbrOfWave;

    #region Get Set
    public int NbrOfEnemy { get => _nbrOfEnemy; set => _nbrOfEnemy = value; }
    public int NbrOfDeadEnemy { get => _nbrOfDeadEnemy; set => _nbrOfDeadEnemy = value; }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateAllSpawner(_nbrOfWave);
        }
    }

    public void CheckLivingEnemies()
    {
        if (NbrOfDeadEnemy != 0 && NbrOfDeadEnemy == NbrOfEnemy)
        {
            _nbrOfWave++;
            if(_nbrOfWave < spawners[0]._nbrOfWaves.Length)
            {
                ActivateAllSpawner(_nbrOfWave);
            }
            //End of Wave
        }
    }

    void ActivateAllSpawner(int waveNbr)
    {
        for (int i = 0, l = spawners.Length; i < l; ++i)
        {
            StartCoroutine(spawners[i].WaveSpawner(waveNbr, this));
        }
    }
}
