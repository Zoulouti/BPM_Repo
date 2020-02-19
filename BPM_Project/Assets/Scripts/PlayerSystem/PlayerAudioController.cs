﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GroundType;

public class PlayerAudioController : AudioController
{

#region SerializeField Functions
    [Header("Steps")]
    [SerializeField] Steps m_steps;
    [System.Serializable] class Steps
    {
        public float m_footStepDistance = 0.25f;
        public float m_footStepDistanceRandomizer = 0.025f;
        public Sounds m_stoneStep;
        public Sounds m_metalStep;
        public Sounds m_woodStep;
    }

    [Header("Jump")]
    [SerializeField] Sounds m_jump;

    [Header("Double Jump")]
    [SerializeField] Sounds m_doubleJump;

    [Header("Land")]
    [SerializeField] Land m_land;
    [System.Serializable] class Land
    {
        public Sounds m_stone;
        public Sounds m_metal;
        public Sounds m_wood;
    }

    [Header("Dash")]
    [SerializeField] Sounds m_dash;

    [Header("Steps & Land Raycast")]
    [SerializeField] Transform m_raycastTrans;
    [SerializeField] float m_raycastMaxDistance = 0.25f;
    [SerializeField] LayerMask m_groundMask;

    [Header("Gizmos")]
    [SerializeField] bool m_showGizmos = false;
    [SerializeField] Color m_gizmosColor = Color.magenta;
#endregion

#region Private Variables
    bool m_isRunning = false;
    float m_targetedFootStepDistance;
	float m_currentFootStepDistance = 0f;
#endregion

#region Event Functions
    void Start()
    {
        SetTargetedFootStepDistance();
    }
	void Update () {
        if (m_isRunning)
            CheckFootSteps();
	}
    void OnDrawGizmos()
    {
        if (!m_showGizmos)
            return;
        
        Gizmos.color = m_gizmosColor;
        Gizmos.DrawLine(m_raycastTrans.position, m_raycastTrans.position + m_raycastTrans.forward * m_raycastMaxDistance);
    }
#endregion
	
#region Private Functions
    void CheckFootSteps()
    {
        m_currentFootStepDistance += Time.deltaTime;
        if (m_currentFootStepDistance > m_targetedFootStepDistance)
        {
            PlayFootStepSound();
            m_currentFootStepDistance = 0;
            SetTargetedFootStepDistance();
        }
    }
    void PlayFootStepSound()
	{
        switch (CheckPlayerGround())
        {
            case GroundTypeEnum.Stone:
                StartSoundFromArray(m_steps.m_stoneStep.m_audioSource, m_steps.m_stoneStep.m_sounds, m_steps.m_stoneStep.m_volume, m_steps.m_stoneStep.m_volumeRandomizer, m_steps.m_stoneStep.m_pitch, m_steps.m_stoneStep.m_pitchRandomizer);
            break;
            case GroundTypeEnum.Metal:
                StartSoundFromArray(m_steps.m_metalStep.m_audioSource, m_steps.m_metalStep.m_sounds, m_steps.m_metalStep.m_volume, m_steps.m_metalStep.m_volumeRandomizer, m_steps.m_metalStep.m_pitch, m_steps.m_metalStep.m_pitchRandomizer);
            break;
            case GroundTypeEnum.Wood:
                StartSoundFromArray(m_steps.m_woodStep.m_audioSource, m_steps.m_woodStep.m_sounds, m_steps.m_woodStep.m_volume, m_steps.m_woodStep.m_volumeRandomizer, m_steps.m_woodStep.m_pitch, m_steps.m_woodStep.m_pitchRandomizer);
            break;
        }
    }
    GroundTypeEnum CheckPlayerGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_raycastTrans.position, m_raycastTrans.forward, out hit, m_raycastMaxDistance, m_groundMask))
        {
            Ground hitGround = hit.collider.GetComponent<Ground>();
            if (hitGround != null)
                return hitGround.GroundType;
        }
        return GroundTypeEnum.Metal;
    }
    void SetTargetedFootStepDistance()
    {
        m_targetedFootStepDistance = GetRandomValue(m_steps.m_footStepDistance, m_steps.m_footStepDistanceRandomizer);
    }
#endregion

#region Public Functions
    public void On_Run(bool isRunning)
    {
        m_isRunning = isRunning;
        if (m_isRunning)
        {
            m_currentFootStepDistance = 0;
            SetTargetedFootStepDistance();
        }
    }
    public void On_Jump()
    {
        StartSoundFromArray(m_jump.m_audioSource, m_jump.m_sounds, m_jump.m_volume, m_jump.m_volumeRandomizer, m_jump.m_pitch, m_jump.m_pitchRandomizer);
    }
    public void On_DoubleJump()
    {
        StartSoundFromArray(m_doubleJump.m_audioSource, m_doubleJump.m_sounds, m_doubleJump.m_volume, m_doubleJump.m_volumeRandomizer, m_doubleJump.m_pitch, m_doubleJump.m_pitchRandomizer);
    }
    public void On_Land()
    {
        switch (CheckPlayerGround())
        {
            case GroundTypeEnum.Stone:
                StartSoundFromArray(m_land.m_stone.m_audioSource, m_land.m_stone.m_sounds, m_land.m_stone.m_volume, m_land.m_stone.m_volumeRandomizer, m_land.m_stone.m_pitch, m_land.m_stone.m_pitchRandomizer);
            break;
            case GroundTypeEnum.Metal:
                StartSoundFromArray(m_land.m_metal.m_audioSource, m_land.m_metal.m_sounds, m_land.m_metal.m_volume, m_land.m_metal.m_volumeRandomizer, m_land.m_metal.m_pitch, m_land.m_metal.m_pitchRandomizer);
            break;
            case GroundTypeEnum.Wood:
                StartSoundFromArray(m_land.m_wood.m_audioSource, m_land.m_wood.m_sounds, m_land.m_wood.m_volume, m_land.m_wood.m_volumeRandomizer, m_land.m_wood.m_pitch, m_land.m_wood.m_pitchRandomizer);
            break;
        }
    }
    public void On_Dash()
    {
        StartSoundFromArray(m_dash.m_audioSource, m_dash.m_sounds, m_dash.m_volume, m_dash.m_volumeRandomizer, m_dash.m_pitch, m_dash.m_pitchRandomizer);
    }
#endregion

}