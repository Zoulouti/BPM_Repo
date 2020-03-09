﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TypeOfFireEnum;

[Serializable]
public class SMG
{
    
    public TypeOfFire typeOfFire;
    public GameObject firePoint;
    public GameObject fireAudio;
    [Space]
    public AnimationCurve recoilCurve;
    public float timeToRecoverFromRecoil;
    public float recoilHeight;
}
