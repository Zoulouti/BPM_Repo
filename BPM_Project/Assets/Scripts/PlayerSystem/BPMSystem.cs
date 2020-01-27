using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BPMSystem : MonoBehaviour
{
    public BPM _BPM = new BPM();
    [Serializable]
    public class BPM
    {
        public int maxBPM = 300;
        public int startingBPM = 100;
        public int criticalLvlOfBPM = 75;
        [Space]
        public int BPMGain_OnNoSpot;
        public int BPMGain_OnWeak;
        public int BPMGain_OnArmor;
        public int BPMGain_OnDestructableEnvironment;
        [Space]
        public Image BPM_Gauge;
        public Image Electra_Gauge;
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

    public enum WeaponState
    {
        Level0,
        Level1,
        Level2
    }
    WeaponState _currentWeaponState = WeaponState.Level0;
    public WeaponState CurrentWeaponState { get => _currentWeaponState; set => _currentWeaponState = value; }

    [Space]
    public Electrarythmie _electrarythmie = new Electrarythmie();
    [Serializable]
    public class Electrarythmie
    {
        [Tooltip("When does the electrarythmie have to trigger ?")]
        public int electrarythmieBPMTrigger = 50;
        public int startingElectrarythmiePoints;
        public int maxElectrarythmiePoints = 100;
        [Space]
        public float timeOfElectrarythmie = 10f;
        [Space]
        public int _electrarythmieGain_OnDestructableEnvironment;
        public Image _electraFeedBack;
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
        public float timeOfOverAdrenaline = 15f;
        [Space]
        public Image _overadrenalineCoolDownGauge;
        public Image _overdrenalineFeedBack;
        public Image _overdrenalineButton;

    }
    float _currentOverdrenalineCooldown;
    bool _overdrenalineCooldownOver = true;
    bool _hasOverdrenaline;
    bool _overdrenalineHasBeenUsed;

    bool _currentCanMove;

    private void Start()
    {
        _currentBPM = _BPM.startingBPM;
        _currentElectrarythmiePoints = _electrarythmie.startingElectrarythmiePoints;
        _currentOverdrenalineCooldown = _overdrenaline.overdrenalineCooldown;

        _BPM.Electra_Gauge.fillAmount = Mathf.InverseLerp(0, _electrarythmie.maxElectrarythmiePoints, _currentElectrarythmiePoints);
        _BPM.BPM_Gauge.fillAmount = Mathf.InverseLerp(0, _BPM.maxBPM, _currentBPM);

        _currentCanMove = gameObject.GetComponent<BasicWalkerController>().CanMove = true;

    }




    private void Update()
    {
        if(_hasOverdrenaline && Input.GetKey(KeyCode.A) && _overdrenalineCooldownOver)
        {
            _hasOverdrenaline = false;
            _overdrenalineHasBeenUsed = true;
            _overdrenalineCooldownOver = false;
            _currentOverdrenalineCooldown = _overdrenaline.overdrenalineCooldown;
            _overdrenaline._overdrenalineButton.gameObject.SetActive(false);
            StartCoroutine(OnOverADActivate());
        }

        if (!_hasOverdrenaline && !_overdrenalineCooldownOver)
        {
            //Debug.Log(_currentOverdrenalineCooldown);
            _currentOverdrenalineCooldown -= Time.deltaTime;
            _overdrenaline._overadrenalineCoolDownGauge.fillAmount = Mathf.InverseLerp(_overdrenaline.overdrenalineCooldown, 0, _currentOverdrenalineCooldown);
            if (_currentOverdrenalineCooldown <= 0)
            {
                _currentOverdrenalineCooldown = 0;
                _overdrenalineCooldownOver = true;
            }
        }
    }

    #region BPM Gain and Loss
    public void LoseBPM(float BPMLoss)
    {
        if (!_overdrenalineHasBeenUsed && _currentCanMove)
        {
            if (_currentBPM - BPMLoss >= _electrarythmie.electrarythmieBPMTrigger)  //Check if the damage reaches the trigger
            {
                //Debug.Log(BPMLoss);
                if (_currentBPM - BPMLoss <= _BPM.criticalLvlOfBPM)
                {
                    if (_hasElectrarythmie)
                    {
                        //Debug.Log("Critical with electra");
                    }
                    else
                    {
                        //Debug.Log("Critical without electra");
                    }
                }
                _currentBPM -= BPMLoss;
                DeactivateWeaponLevel(_currentBPM);

            }
            else if (_hasElectrarythmie)                                           //Check if the player has the electrarythmie activated
            {
                _currentBPM = _electrarythmie.electrarythmieBPMTrigger;

                //Debug.Log("Electrarythmie Time");
                ActivateElectrarythmie();

            }
            else                                                                   //If not, it's death
            {
                //Debug.Log("Je suis mort");
            }
        }

       //Debug.Log("Level of BPM (lose): " + _currentBPM);
       FeedBackBPM();

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
            if (_overdrenalineCooldownOver)
            {
                _overdrenaline._overdrenalineButton.gameObject.SetActive(true);
                _hasOverdrenaline = true;
            }

        }
        //Debug.Log("Level of BPM (gain): " + _currentBPM);
        FeedBackBPM();
    }

    void FeedBackBPM()
    {
        _BPM.BPM_Gauge.fillAmount = Mathf.InverseLerp(0, _BPM.maxBPM, _currentBPM);
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

            ChangeWeaponLevel();

        }
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

            ChangeWeaponLevel();

        }
    }

    void ChangeWeaponLevel()
    {
        GetComponent<WeaponBehaviour>().ChangeWeaponStats();
    }

    // script contien les armes ?
    // les armes regarde la state ?
    // enum en privé comment l'atteindre ?

    #endregion


    #region Electrarythmie Handeler
    void ActivateElectrarythmie()
    {
        _currentElectrarythmiePoints = 0;
        _BPM.Electra_Gauge.fillAmount = Mathf.InverseLerp(0, _electrarythmie.maxElectrarythmiePoints, _currentElectrarythmiePoints);
        StartCoroutine(OnElectrarythmieActivate());
        _hasElectrarythmie = false;

    }

    IEnumerator OnElectrarythmieActivate()
    {
        _currentCanMove = gameObject.GetComponent<BasicWalkerController>().CanMove = false;
        _electrarythmie._electraFeedBack.gameObject.SetActive(true);
        yield return new WaitForSeconds(_electrarythmie.timeOfElectrarythmie);
        _currentCanMove = gameObject.GetComponent<BasicWalkerController>().CanMove = true;
        _electrarythmie._electraFeedBack.gameObject.SetActive(false);
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
        //Debug.Log("ElectraPoints : " + _currentElectrarythmiePoints);
        _BPM.Electra_Gauge.fillAmount = Mathf.InverseLerp(0, _electrarythmie.maxElectrarythmiePoints, _currentElectrarythmiePoints);
    }
    #endregion

    #region Overadrenaline

    IEnumerator OnOverADActivate()
    {
        _overdrenaline._overdrenalineFeedBack.gameObject.SetActive(true);
        yield return new WaitForSeconds(_overdrenaline.timeOfOverAdrenaline);
        _overdrenaline._overdrenalineFeedBack.gameObject.SetActive(false);
        _overdrenalineHasBeenUsed = false;
    }
    #endregion
}
