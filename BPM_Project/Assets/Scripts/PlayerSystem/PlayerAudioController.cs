
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : AudioController
{

#region SerializeField Functions
    [Header("Steps")]
	[SerializeField] float m_footStepDistance = 0.2f;
    [SerializeField] Sounds m_woodStep;
    [SerializeField] Sounds m_metalStep;

    [Header("Jump")]
    [SerializeField] Sounds m_jump;

    [Header("Double Jump")]
    [SerializeField] Sounds m_doubleJump;

    [Header("Land")]
    [SerializeField] Sounds m_land;

    [Header("Dash")]
    [SerializeField] Sounds m_dash;
    
    [System.Serializable] class Sounds
    {
        [Header("References")]
        public AudioSource m_audioSource;

        [Header("Sounds")]
        public AudioClip[] m_sounds;

        [Header("Volume")]
        [Range(0, 1)] public float m_volume = 0.75f;
        [Range(0, 0.5f)] public float m_volumeRandomizer = 0.2f;

        [Header("Pitch")]
        [Range(-3, 3)] public float m_pitch = 1;
        [Range(0, 1.5f)] public float m_pitchRandomizer = 0.1f;
    }
#endregion

#region Private Variables
    bool m_isRunning = false;
    float m_currentFootstepDistance = 0f;
	float m_currentFootStepValue = 0f;
    float m_footSpeedThreshold = 0.05f;
#endregion

#region Event Functions
	void Update () {
        if (m_isRunning)
            CheckFootSteps();
	}
#endregion
	
#region Private Functions
    void CheckFootSteps()
    {

        PlayFootStepSound();
    }
    void PlayFootStepSound()
	{
		// int _footStepClipIndex = Random.Range(0, m_footStepSounds.Length);
		// m_audioSource.PlayOneShot(m_footStepSounds[_footStepClipIndex], m_audioClipVolume + m_audioClipVolume * Random.Range(-m_relativeRandomizedVolumeRange, m_relativeRandomizedVolumeRange));

        // WOOD
        // StartSoundFromArray(m_woodStep.m_audioSource, m_woodStep.m_sounds, m_woodStep.m_volume, m_woodStep.m_volumeRandomizer, m_woodStep.m_pitch, m_woodStep.m_pitchRandomizer);
        // Metal
        // StartSoundFromArray(m_metalStep.m_audioSource, m_metalStep.m_sounds, m_metalStep.m_volume, m_metalStep.m_volumeRandomizer, m_metalStep.m_pitch, m_metalStep.m_pitchRandomizer);
    }
    void CheckPlayerGround()
    {

    }

    void FootStepUpdate(float _movementSpeed)
	{
		// float _speedThreshold = 0.05f;

        // m_currentFootstepDistance += Time.deltaTime * _movementSpeed;

        // //Play foot step audio clip if a certain distance has been traveled;
        // if(m_currentFootstepDistance > m_footstepDistance)
        // {
        //     //Only play footstep sound if mover is grounded and movement speed is above the threshold;
        //     if(m_mover.IsGrounded() && _movementSpeed > _speedThreshold)
        //         PlayFootstepSound();
        //     m_currentFootstepDistance = 0f;
        // }
	}
#endregion

#region Public Functions
    public void On_Run(bool isRunning)
    {
        m_isRunning = isRunning;
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
        StartSoundFromArray(m_land.m_audioSource, m_land.m_sounds, m_land.m_volume, m_land.m_volumeRandomizer, m_land.m_pitch, m_land.m_pitchRandomizer);
    }
    public void On_Dash()
    {
        StartSoundFromArray(m_dash.m_audioSource, m_dash.m_sounds, m_dash.m_volume, m_dash.m_volumeRandomizer, m_dash.m_pitch, m_dash.m_pitchRandomizer);
    }
#endregion

}