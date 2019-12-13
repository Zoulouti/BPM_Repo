using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BPMSystem : MonoBehaviour
{
    public BPM _BPM = new BPM();
    [Serializable]
    public class BPM
    {
        public int maxBPM = 300;
        public int criticalLvlOfBPM = 75;
    }

    float _currentBPM;

    [Space]
    public WeaponsLevel _weaponsLevel = new WeaponsLevel();
    [Serializable]
    public class WeaponsLevel
    {
        public int firstWeaponLevel = 150;
        public int secondWeaponLevel = 225;
    }

    enum WeaponState
    {
        Level0,
        Level1,
        Level2,
        Overdrenaline
    }
    WeaponState _currentWeaponState = WeaponState.Level0;

    [Space]
    public Electrarythmie _electrarythmie = new Electrarythmie();
    [Serializable]
    public class Electrarythmie
    {
        [Tooltip("When does the electrarythmie have to trigger ?")]
        public int electrarythmieBPMTrigger = 50;
        public int maxElectrarythmiePoints = 100;
    }

    float _currentElectrarythmiePoints;
    bool _hasElectrarythmie;

    [Space]
    public Overdrenaline _overdrenaline = new Overdrenaline();
    [Serializable]
    public class Overdrenaline
    {
        [Tooltip("In seconds")]
        public float overdrenalineCooldown = 60f;
    }

    bool _hasOverdrenaline;

    private void Start()
    {
        _currentBPM = _BPM.maxBPM;
        _currentElectrarythmiePoints = _electrarythmie.maxElectrarythmiePoints;
    }





    #region BPM Gain and Loss
    public void LoseBPM(float BPMLoss)
    {
        if (_currentBPM - BPMLoss > _electrarythmie.electrarythmieBPMTrigger)  //Check if the damage reaches the trigger
        {
            _currentBPM -= BPMLoss;
        }
        else if (_hasElectrarythmie)                                           //Check if the player has the electrarythmie activated
        {
            _currentBPM = _electrarythmie.electrarythmieBPMTrigger;

            Debug.Log("Electrarythmie Time");

            _hasElectrarythmie = false;

        }
        else                                                                   //If not it's death
        {
            Debug.Log("Je suis mort");
        }
    }

    public void GainBPM(float BPMGain)
    {
        if(_currentBPM + BPMGain < _BPM.maxBPM)
        {
            _currentBPM += BPMGain;
        }
        else
        {
            _currentBPM = _BPM.maxBPM;

            _hasOverdrenaline = true;

        }
    }
    #endregion


    void ActivateWeaponLevel(float currentBPM)
    {
        if (currentBPM >= _weaponsLevel.firstWeaponLevel)
        {
            if(currentBPM >= _weaponsLevel.secondWeaponLevel)
            {
                _currentWeaponState = WeaponState.Level2;
            }
            else
            {
                _currentWeaponState = WeaponState.Level1;
            }
        }
    }

    void DeactivateWeaponLevel(float currentBPM)
    {
        if (currentBPM < _weaponsLevel.secondWeaponLevel)
        {
            if (currentBPM < _weaponsLevel.firstWeaponLevel)
            {
                _currentWeaponState = WeaponState.Level2;
            }
            else
            {
                _currentWeaponState = WeaponState.Level1;
            }
        }
    }
}
