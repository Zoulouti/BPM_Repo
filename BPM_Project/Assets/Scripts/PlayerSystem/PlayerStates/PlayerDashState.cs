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
        m_dashDirection = m_playerController.GetPlayerMoveInputsDirection();

        m_dashTimer = 0;
        m_haseDash = false;

        m_playerController.ResetPlayerVelocity();

        m_dashSpeed = m_playerController.m_dashDistance / m_playerController.m_timeToDash;
        // Debug.Log("m_dashSpeed = " + m_dashSpeed);
    }
    public void FixedUpdate()
    {
        m_dashTimer += Time.deltaTime;
        if(m_dashTimer > m_playerController.m_timeToDash && !m_haseDash)
        {
            m_haseDash = true;
            m_playerController.ChangeState(PlayerState.Run);
        }

        m_playerController.SetPlayerVelocity(m_dashDirection * m_playerController.m_dashDistance);
    }
    public void Update()
    {

    }
    public void LateUpdate()
    {

    }
    public void Exit()
    {

    }

}
