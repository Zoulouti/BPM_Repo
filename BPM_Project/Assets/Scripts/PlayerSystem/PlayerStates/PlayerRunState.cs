using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class PlayerRunState : IState
{

    PlayerController m_playerController;

    // Constructor (CTOR)
    public PlayerRunState(PlayerController playerController)
    {
        m_playerController = playerController;
    }

    public void Enter()
    {

    }
    public void FixedUpdate()
    {
        m_playerController.CheckForGround();

        if(!m_playerController.PlayerIsGrounded() || m_playerController.PlayerIsFalling())
        {
            m_playerController.ChangeState(PlayerState.Fall);
        }

        m_playerController.PlayerIsSliding();

        m_playerController.Move();
    }
    public void Update()
    {
        if(!m_playerController.PlayerInputIsMoving())
        {
            m_playerController.ChangeState(PlayerState.Idle);
        }

        if(m_playerController.IsJumpKeyPressed() && m_playerController.CanJump())
        {
            m_playerController.On_PlayerHasJump(true);
            m_playerController.ChangeState(PlayerState.Jump);
        }
    }
    public void LateUpdate()
    {

    }
    public void Exit()
    {

    }

}
