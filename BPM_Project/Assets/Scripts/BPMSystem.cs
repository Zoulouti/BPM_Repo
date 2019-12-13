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
        Level2
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
    float _currentOverdrenalineCooldown;
    bool _overdrenalineCooldownOver;
    bool _hasOverdrenaline;

    private void Start()
    {
        _currentBPM = _BPM.maxBPM;
        _currentElectrarythmiePoints = _electrarythmie.maxElectrarythmiePoints;
        _currentOverdrenalineCooldown = _overdrenaline.overdrenalineCooldown;
    }

    RaycastHit _hit;
    public Transform origin;
    public GameObject camera;

    private void Update()
    {
        if(_hasOverdrenaline && Input.GetKey(KeyCode.A) && _overdrenalineCooldownOver)
        {
            _hasOverdrenaline = false;
            _currentOverdrenalineCooldown = _overdrenaline.overdrenalineCooldown;

        }

        if (!_hasOverdrenaline && !_overdrenalineCooldownOver)
        {
            _currentOverdrenalineCooldown -= Time.deltaTime;
            if(_currentOverdrenalineCooldown <= 0)
            {
                _overdrenalineCooldownOver = true;
            }
        }


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(Physics.Raycast(origin.position, camera.transform.forward, out _hit, Mathf.Infinity))
            {

                Debug.DrawRay(origin.position, camera.transform.forward, Color.blue, 10f);

                if (_hit.collider.CompareTag("NoSpot"))
                {
                    GainBPM(10f);
                }
                else if (_hit.collider.CompareTag("WeakSpot"))
                {
                    GainBPM(20f);
                }
                else if (_hit.collider.CompareTag("ArmorSpot"))
                {
                    GainBPM(5f);
                }
                else if (_hit.collider.CompareTag("DestroyableObject"))
                {
                    GainBPM(5f);
                    GainElectrarythmiePoints(5);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            LoseBPM(10f);
        }
    }

    #region BPM Gain and Loss
    public void LoseBPM(float BPMLoss)
    {
        if (_currentBPM - BPMLoss > _electrarythmie.electrarythmieBPMTrigger)  //Check if the damage reaches the trigger
        {
            if (_currentBPM - BPMLoss <= _BPM.criticalLvlOfBPM)
            {
                if (_hasElectrarythmie)
                {
                    Debug.Log("Critical with electra");
                }
                else
                {
                    Debug.Log("Critical without electra");
                }
            }
            _currentBPM -= BPMLoss;
            DeactivateWeaponLevel(_currentBPM);

        }
        else if (_hasElectrarythmie)                                           //Check if the player has the electrarythmie activated
        {
            _currentBPM = _electrarythmie.electrarythmieBPMTrigger;

            Debug.Log("Electrarythmie Time");
            ActivateElectrarythmie();

        }
        else                                                                   //If not it's death
        {
            Debug.Log("Je suis mort");
        }

        Debug.Log("Level of BPM : " + _currentBPM);

    }

    public void GainBPM(float BPMGain)
    {
        if(_currentBPM + BPMGain < _BPM.maxBPM)
        {
            _currentBPM += BPMGain;
            ActivateWeaponLevel(_currentBPM);
        }
        else
        {
            _currentBPM = _BPM.maxBPM;

            _hasOverdrenaline = true;

        }
        Debug.Log("Level of BPM : " + _currentBPM);
    }
    #endregion


    #region Activate and Deactivate Weapon
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
        Debug.Log("WeaponState : "+_currentWeaponState);
    }

    void DeactivateWeaponLevel(float currentBPM)
    {
        if (currentBPM < _weaponsLevel.secondWeaponLevel)
        {
            if (currentBPM < _weaponsLevel.firstWeaponLevel)
            {
                _currentWeaponState = WeaponState.Level0;
            }
            else
            {
                _currentWeaponState = WeaponState.Level1;
            }
        }
        Debug.Log("WeaponState : " + _currentWeaponState);
    }

    // script contien les armes ?
    // les armes regarde la state ?
    // enum en privé comment l'atteindre ?

    #endregion


    #region Electrarythmie Handeler
    void ActivateElectrarythmie()
    {
        _currentElectrarythmiePoints = 0;
        _hasElectrarythmie = false;
    }

    public void GainElectrarythmiePoints(int points)
    {
        if(_currentElectrarythmiePoints + points < _electrarythmie.maxElectrarythmiePoints)
        {
            _currentElectrarythmiePoints += points;
        }
        else
        {
            _currentElectrarythmiePoints = _electrarythmie.maxElectrarythmiePoints;
            _hasElectrarythmie = true;
        }
        Debug.Log("ElectraPoints : " + _currentElectrarythmiePoints);
    }
    #endregion
}
