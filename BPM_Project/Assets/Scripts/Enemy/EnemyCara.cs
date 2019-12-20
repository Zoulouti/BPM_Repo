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

        /*public Head _head = new Head();
        [Serializable]
        public class Head
        {
            public GameObject WeakSpot;
            public GameObject ArmoredSpot;

        }
        [Space]
        public RightShoulder _rightShoulder = new RightShoulder();
        [Serializable]
        public class RightShoulder
        {
            public GameObject WeakSpot;
            public GameObject ArmoredSpot;
        }
        [Space]
        public LeftShoulder _leftShoulder = new LeftShoulder();
        [Serializable]
        public class LeftShoulder
        {
            public GameObject WeakSpot;
            public GameObject ArmoredSpot;
        }
        [Space]
        public Torso _torso = new Torso();
        [Serializable]
        public class Torso
        {
            public GameObject WeakSpot;
            public GameObject ArmoredSpot;
        }
        [Space]
        public Backo _backo = new Backo();
        [Serializable]
        public class Backo
        {
            public GameObject WeakSpot;
            public GameObject ArmoredSpot;
        }
        [Space]
        public RightKnee _rightKnee = new RightKnee();
        [Serializable]
        public class RightKnee
        {
            public GameObject WeakSpot;
            public GameObject ArmoredSpot;
        }
        [Space]
        public LeftKnee _leftKnee = new LeftKnee();
        [Serializable]
        public class LeftKnee
        {
            public GameObject WeakSpot;
            public GameObject ArmoredSpot;
        }*/
    }
    [Space]
    public EnemyArchetype enemyArchetype;
    [Space]
    public EnemyCaractéristique _enemyCaractéristique = new EnemyCaractéristique();
    [Serializable] public class EnemyCaractéristique
    {
        public float maxLife;
        [Space]
        public float noSpotDamageMultiplicateur;
        public float weakSpotDamageMultiplicateur;
        public float armorSpotDamageMultiplicateur;
    }
    float _currentLife;


    public void Awake()
    {
        enemyArchetype.PopulateArray();
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

        _currentLife = _enemyCaractéristique.maxLife;
    }

    public void TakeDamage(float damage, int i)
    {
        switch (i)
        {
            case 0:

                _currentLife -= damage * _enemyCaractéristique.noSpotDamageMultiplicateur;

                break;
            case 1:

                _currentLife -= damage * _enemyCaractéristique.weakSpotDamageMultiplicateur;

                break;
            case 2:

                _currentLife -= damage * _enemyCaractéristique.armorSpotDamageMultiplicateur;

                break;
            default:
                break;
        }
        Debug.Log(_currentLife);
    }

}
