using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class PlayerJumpState : IState
{

    float m_timer = 0;
    bool m_haseJump = false;

    PlayerController m_playerController;

    // Constructor (CTOR)
    public PlayerJumpState(PlayerController playerController)
    {
        m_playerController = playerController;
    }

    public void Enter()
    {
        m_timer = 0;
        m_haseJump = false;

        m_playerController.On_GroundContactLost();
        m_playerController.On_JumpStart();
    }
    public void FixedUpdate()
    {
        m_timer += Time.deltaTime;
        if(m_timer > m_playerController.m_jumpDuration && !m_haseJump)
        {
            m_haseJump = true;
            m_playerController.ChangeState(PlayerState.Fall);
        }

        if(!m_haseJump)
        {
            m_playerController.Move();
        }
        else
        {
            m_playerController.CheckForGround();
            m_playerController.Move();
        }
    }
    public void Update()
    {
        if(m_playerController.IsJumpKeyPressed() && m_playerController.CanJump())
        {
            m_playerController.On_PlayerHasDoubleJump(true);
            m_playerController.ChangeState(PlayerState.Jump);
        }

        if(m_playerController.IsDashKeyPressed() && m_playerController.CanDash())
        {
            m_playerController.ChangeState(PlayerState.Dash);
        }
    }
    public void LateUpdate()
    {

    }
    public void Exit()
    {

    }

}
