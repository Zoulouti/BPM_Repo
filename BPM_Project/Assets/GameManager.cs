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

    [Header("Differents Spots Multiplicator Tweaking")]
    public float noSpotDamageMultiplicateur;
    public float weakSpotDamageMultiplicateur;
}
