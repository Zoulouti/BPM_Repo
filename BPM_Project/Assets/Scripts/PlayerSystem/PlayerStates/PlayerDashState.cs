using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class PlayerDashState : IState
{
    
    float m_dashTimer = 0;
    bool m_haseDash = false;

    Vector3 m_dashDirection;
    float m_dashSpeed;

    PlayerController m_playerController;

    // Constructor (CTOR)
    public PlayerDashState(PlayerController playerController)
    {
        m_playerController = playerController;
    }

    public void Enter()
    {
        m_playerController.On_PlayerHasDash(true);
        m_playerController.On_PlayerStartDash(true);
        m_playerController.GetComponent<WeaponPlayerBehaviour>().CanShoot = false;
        m_dashDirection = m_playerController.GetPlayerMoveInputsDirection();

        m_dashTimer = 0;
        m_haseDash = false;

        m_playerController.ResetPlayerVelocity();

        m_dashSpeed = m_playerController.m_dash.m_distance / m_playerController.m_dash.m_timeToDash;

        m_playerController.ChangeCameraFov(m_playerController.m_fov.m_dashFov, m_playerController.m_fov.m_startDash.m_timeToChangeFov, m_playerController.m_fov.m_startDash.m_changeFovCurve);
    }
    public void FixedUpdate()
    {
        m_dashTimer += Time.deltaTime;
        if(m_dashTimer > m_playerController.m_dash.m_timeToDash && !m_haseDash)
        {
            m_haseDash = true;
            m_playerController.ChangeState(PlayerState.Run);
            // Check si on touche le sol ou pas pour passer en RunState, FallState ou IdleState si on bouge pas
        }

        m_playerController.SetPlayerVelocity(m_dashDirection * m_dashSpeed);
    }
    public void Update()
    {

    }
    public void LateUpdate()
    {

    }
    public void Exit()
    {
        m_playerController.GetComponent<WeaponPlayerBehaviour>().CanShoot = true;

        m_playerController.ChangeCameraFov(m_playerController.m_fov.m_normalFov, m_playerController.m_fov.m_endDash.m_timeToChangeFov, m_playerController.m_fov.m_endDash.m_changeFovCurve);

        m_playerController.On_PlayerStartDash(false);

        if (m_playerController.PlayerIsGrounded())
        {
		    // m_playerController.On_PlayerHasDash(false);
            m_playerController.StartDashCooldown();
        }
    }

}
