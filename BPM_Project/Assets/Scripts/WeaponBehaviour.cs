using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponBehaviour : MonoBehaviour
{
    public Weapon[] allWeapons;

    public GameObject projectils;

    BPMSystem _BPMSystem;
    int activatedWeapon = 0;

    private void Awake()
    {
        _BPMSystem = GetComponent<BPMSystem>();
    }

    // ---------- A virer -----------
    RaycastHit _hit;
    // ---------- A virer -----------

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _BPMSystem.LoseBPM(allWeapons[activatedWeapon]._weaponLevel0.BPMCost);

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, Mathf.Infinity))
            {

                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.blue, 10f);

                if (_hit.collider.CompareTag("NoSpot"))
                {
                    _BPMSystem.GainBPM(10f);
                }
                else if (_hit.collider.CompareTag("WeakSpot"))
                {
                    _BPMSystem.GainBPM(20f);
                }
                else if (_hit.collider.CompareTag("ArmorSpot"))
                {
                    _BPMSystem.GainBPM(5f);
                }
                else if (_hit.collider.CompareTag("DestroyableObject"))
                {
                    _BPMSystem.GainBPM(5f);
                    _BPMSystem.GainElectrarythmiePoints(5);
                }
            }
        }
    }
}
