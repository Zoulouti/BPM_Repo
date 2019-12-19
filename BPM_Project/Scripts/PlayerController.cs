using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;

    public PlayerControl m_playerControl = new PlayerControl();
    [Serializable] public class PlayerControl
    {
        [Header("Mouse Variables")]
        public float mouseSensitivity = 100f;
        [Space]
        [Header("Speed Variables")]
        public float base_speed = 12f;
        public float Sprint_speed = 18f;
        [Range(0.01f,1f)]
        [Tooltip("Controle le ralentissement imposé par une marche arrière (Plus le chiffre est petit, plus le ralentissement est grand)")]
        public float backwardSpeedDivison = 0.5f;
        [Range(0.01f, 1f)]
        [Tooltip("Controle le ralentissement imposé par le crouch (Plus le chiffre est petit, plus le ralentissement est grand)")]
        public float crouchSpeedDivison = 0.5f;
        [Space]
        [Header("Crouch Variables")]
        [Tooltip("Il faut rester appuyé sur le crouch pour rester crouch ou pas")]
        public bool isCrouchStayOnInput;
        [Space]
        [Header("Jump Variables")]
        public float gravity = -9.81f;
        public float jumpHeight = 3f;
        [Space]
        [Header("Slide Variables")]
        public float slideDistance = 3f;
    }

    public PlayerDebug m_debug = new PlayerDebug();
    [Serializable] public class PlayerDebug
    {
        public GameObject MainCamera;
        public Transform groundCheck;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;
    }

    bool isGrounded;
    bool isCrouching;

    float controllerHeight;
    float currentSpeed;

    Vector3 velocity;
    Vector3 controllerCenter;
    Vector3 move;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controllerHeight = controller.height;
        controllerCenter = controller.center;
        currentSpeed = m_playerControl.base_speed;
    }
    void Update()
    {
        #region isGrounded Check
        isGrounded = Physics.CheckSphere(m_debug.groundCheck.position, m_debug.groundDistance, m_debug.groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        #endregion

        #region Axis Movement

        MoveSpeedControl();

        #endregion

        #region Jump Velocity
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(m_playerControl.jumpHeight * -2f * m_playerControl.gravity);
        }

        velocity.y += m_playerControl.gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        #endregion

        #region Crouch Methods
        if (m_playerControl.isCrouchStayOnInput)
        {
            if (Input.GetButtonDown("Crouch") && isGrounded)
            {
                OnCrouch(true, controllerHeight / 2, new Vector3(controller.center.x, -0.5f, controller.center.z), new Vector3(m_debug.MainCamera.transform.localPosition.x, 0f, m_debug.MainCamera.transform.localPosition.z));
            }
            if(Input.GetButtonUp("Crouch") && isGrounded)
            {
                OnCrouch(false, controllerHeight, controllerCenter, new Vector3(m_debug.MainCamera.transform.localPosition.x, 0.75f, m_debug.MainCamera.transform.localPosition.z));
            }
        }
        else
        {
            if(Input.GetButtonDown("Crouch") && isGrounded && !isCrouching)
            {
                OnCrouch(true, controllerHeight / 2, new Vector3(controller.center.x, -0.5f, controller.center.z), new Vector3(m_debug.MainCamera.transform.localPosition.x, 0f, m_debug.MainCamera.transform.localPosition.z));
            }
            else if(Input.GetButtonDown("Crouch") && isGrounded && isCrouching)
            {
                Debug.Log("ouch");
                OnCrouch(false, controllerHeight, controllerCenter, new Vector3(m_debug.MainCamera.transform.localPosition.x, 0.75f, m_debug.MainCamera.transform.localPosition.z));
            }
        }
        #endregion

        #region SpeedController

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !isCrouching)                                                // va falloir changer ça pour passer en manette
        {
            OnChangeSpeed(m_playerControl.Sprint_speed);
        }
        else if(!(Input.GetKey(KeyCode.LeftShift)) && isGrounded && !isCrouching)
        {
            OnChangeSpeed(m_playerControl.base_speed);
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isCrouching)
        {
            ResetDoOnce();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ResetDoOnce();
            if (isGrounded)
            { 
                currentSpeed = m_playerControl.base_speed;
            }
        }

        #endregion

        #region Slide Controller

        //nop

        #endregion

    }

    void OnCrouch(bool O_isCrouching, float height, Vector3 center, Vector3 cameraPos)
    {
        isCrouching = O_isCrouching;
        controller.height = height;
        controller.center = center;
        m_debug.MainCamera.transform.localPosition = cameraPos;
    }

    bool DoOnce = true;
    void OnChangeSpeed(float speed)
    {
        if (DoOnce)
        {
            currentSpeed = speed;
            DoOnce = false;
        }
    }

    void ResetDoOnce()
    {
        DoOnce = true;
    }


    void MoveSpeedControl()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (!isCrouching)
        {
            if (z >= 0)
            {
                MoveSpeedCalcul(x, z, 1f);
            }
            else
            {
                MoveSpeedCalcul(x, z, m_playerControl.backwardSpeedDivison);
            }
        }
        else
        {
            MoveSpeedCalcul(x, z, m_playerControl.crouchSpeedDivison);
        }
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    void MoveSpeedCalcul(float x , float z,float divison)
    {
        move = (transform.right * x + transform.forward * z) * divison;
    }
}
