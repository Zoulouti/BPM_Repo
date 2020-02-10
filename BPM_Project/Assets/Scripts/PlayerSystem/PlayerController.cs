using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class PlayerController : MonoBehaviour
{

    public static PlayerController s_instance;

    CameraController m_cameraControls;

	[SerializeField] Transform m_cameraPivot;

#region [SerializeField] Variables
    [SerializeField] StateMachine m_sM = new StateMachine();
	public bool m_useGravity = true;

	[Header("Sprint")]
	public float m_dashDistance = 5;
	public float m_timeToDash = 0.5f;

#endregion

#region Private Variables

    //References to attached components;
	Transform m_trans;
	Mover m_mover;

	//Names of input axes used for horizontal and vertical input;
	string m_horizontalInputAxis = "Horizontal";
	string m_verticalInputAxis = "Vertical";

	//Whether or not to use raw input values;
	[SerializeField] bool m_useRawInput = false;

    //crouch key variables
    bool m_isCrouching;

    [Header("Movement speed variables")]
    //Movement speed;
    float m_currentSpeed;
    // public float walkingSpeed = 7f;
    [SerializeField] float m_sprintingSpeed = 17f;
    [Space]
    [Header("Sprint variables")]
    [SerializeField] float m_fovWhenWalking = 90f;
    [SerializeField] float m_fovWhenSprinting = 110f;
    float m_currentFOV;
    [Space]
    [Header("Crouch varibales")]
    [SerializeField] float m_crouchingSpeed = 5f;
    [SerializeField] float m_crouchColliderHeight;
    [SerializeField] LayerMask m_crouchLayer;
    float m_initialColliderHeight;

    [Header("Jump variables")]
	//'Aircontrol' determines to what degree the player is able to move while in the air;
	[Range(0f, 1f)]
	[SerializeField] float m_airControl = 0.4f;

	//Jump speed;
	[SerializeField] float m_jumpSpeed = 10f;

	//Jump duration variables;
	public float m_jumpDuration = 0.2f;
	
	float m_currentJumpStartTime = 0f;

	//'AirFriction' determines how fast the controller loses its momentum while in the air;
	//'GroundFriction' is used instead, if the controller is grounded;
	[SerializeField] float m_airFriction = 0.5f;
	[SerializeField] float m_groundFriction = 100f;

	//Current momentum;
	Vector3 m_momentum = Vector3.zero;

	//Saved velocity from last frame;
	Vector3 m_savedVelocity = Vector3.zero;

	//Saved horizontal movement velocity from last frame;
	Vector3 m_savedMovementVelocity = Vector3.zero;

	//Amount of downward gravitation;
	[SerializeField] float m_gravity = 30f;
	//Amount of downward gravitation when sliding down a slope;
	[SerializeField] float m_slideGravity = 30f;

    bool m_isSliding = false;

	//Acceptable slope angle limit;
	[SerializeField] float m_slopeLimit = 80f;

    //Enum describing in what state the PC is
    protected enum ActionState
    {
        // Walk,
        Sprint,
        Crouch,
        Slide,
    }
    // protected ActionState _currentActionState = ActionState.Walk;
    ActionState m_currentActionState = ActionState.Sprint;

    Vector3 m_slidingFwrd;
    Vector3 m_slidingRight;

	bool m_hasJump = false;
	bool m_hasDoubleJump = false;

	Vector3 m_playerMoveInputsDirection;

#endregion

#region Event Functions
	void Awake()
    {
        SetupSingleton();
		SetupStateMachine();

        m_mover = GetComponent<Mover>();
		m_trans = GetComponent<Transform>();

        //Search for camera controller reference in this gameobjects' children;
		m_cameraControls = GetComponentInChildren<CameraController>();

        // _currentSpeed = walkingSpeed;
        m_currentSpeed = m_sprintingSpeed;
        m_initialColliderHeight = m_mover.colliderHeight;
	}

	void OnEnable()
	{

	}

	void Start()
    {
		
    }

	void FixedUpdate()
	{
		m_sM.FixedUpdate();
	}

	void Update()
	{
		UpdateInputs();
		m_sM.Update();

        // HandleSprintKeyInput();
        // HandleCrouchKeyInput();
	}

	void LateUpdate()
	{
		m_sM.LateUpdate();
	}

	void OnDrawGizmos()
	{
		
	}
#endregion

#region Private Functions
	void SetupSingleton()
	{
		if(s_instance == null){
			s_instance = this;
		}else{
			Debug.LogError("Two instance of PlayerController");
		}
	}

	void SetupStateMachine()
	{
		m_sM.AddStates(new List<IState> {
			new PlayerIdleState(this),				// 0 = Idle
			new PlayerRunState(this),				// 1 = Run
			new PlayerFallState(this),				// 2 = Fall
			new PlayerJumpState(this),				// 3 = Jump
			new PlayerDashState(this),				// 4 = Dash
		});

        string[] playerStateNames = System.Enum.GetNames(typeof(PlayerState));
		if(m_sM.States.Count != playerStateNames.Length){
			Debug.LogError("You need to have the same number of State in PlayerController and PlayerStateEnum");
		}

        ChangeState(PlayerState.Idle);
	}

	void UpdateInputs()
	{
		
	}

	// Utiliser ce changement de FOV lorsqu'on slide !
    void HandleSprintKeyInput()
    {
        // bool _newSprintKeyPressedState = IsSprintActive();
        // bool _forwardKeyIsPressed = IsForwarding();
        // if (_newSprintKeyPressedState && _forwardKeyIsPressed && !isCrouching)   // verify when the sprint needs to be canceled
        // {
        //     _currentSpeed = sprintingSpeed;

        //     #region FOV
        //     Camera.main.fieldOfView = Mathf.Lerp(currentFOV, fovWhenSprinting, Time.deltaTime * 4f);

        //     currentFOV = Camera.main.fieldOfView;

        //     if(currentFOV > fovWhenSprinting - 0.5f)
        //     {
        //         currentFOV = fovWhenSprinting;
        //     }
        //     #endregion

        //     _currentActionState = ActionState.Sprint;
        // }
        // else if(currentControllerState == ControllerState.Grounded && _currentActionState != ActionState.Slide)
        // {
        //     _currentSpeed = crouchingSpeed;

        //     #region FOV
        //     Camera.main.fieldOfView = Mathf.Lerp(currentFOV, fovWhenWalking, Time.deltaTime * 4f);
        //     currentFOV = Camera.main.fieldOfView;
        //     if (currentFOV < fovWhenWalking + 0.5f)
        //     {
        //         currentFOV = fovWhenWalking;
        //     }
        //     #endregion

        //     isSprinting = false;
        //     if (!isCrouching)
        //     {
        //         // _currentSpeed = walkingSpeed;
        //         _currentSpeed = sprintingSpeed;
        //         // _currentActionState = ActionState.Walk;
        //         _currentActionState = ActionState.Sprint;
        //     }

        // }
    }

    //Handle crouch booleans
    void HandleCrouchKeyInput()
    {
        bool _crouchKeyIsPressed = IsCrouching();

        if (_crouchKeyIsPressed)
        {
            CrouchingSwitch();
        }
    }

    void CrouchingSwitch()
    {
        switch (m_isCrouching)
        {
            case true:

                m_mover.colliderHeight = m_initialColliderHeight;
                // _currentActionState = ActionState.Walk;
                m_currentActionState = ActionState.Sprint;
                m_isCrouching = false;

                break;
            case false:

                m_mover.colliderHeight = m_crouchColliderHeight;
                m_isCrouching = true;

                // if (_currentActionState != ActionState.Sprint)
                // {
                    m_currentActionState = ActionState.Crouch;
                // }
                // /*else*/ if(_currentActionState != ActionState.Slide)
                // {
                //     _currentActionState = ActionState.Slide;
                //     slidingFwrd = Camera.main.transform.forward;
                //     StartCoroutine(_slidingCoroutine(_useCustomCurve));
                // }

                break;
            default:
                break;
        }
    }

    //Calculate and return movement direction based on player input;
	//This function can be overridden by inheriting scripts to implement different player controls;
	Vector3 CalculateMovementDirection()
	{
		float _horizontalInput;
		float _verticalInput;

		//Get input;
		if(m_useRawInput){
			_horizontalInput = Input.GetAxisRaw(m_horizontalInputAxis);
			_verticalInput = Input.GetAxisRaw(m_verticalInputAxis);
		} else {
			_horizontalInput = Input.GetAxis(m_horizontalInputAxis);
			_verticalInput = Input.GetAxis(m_verticalInputAxis);
		}

		Vector3 _direction = Vector3.zero;

		//Use camera axes to construct movement direction;
		// _direction += cameraControls.GetFacingDirection() * _verticalInput;
		// _direction += cameraControls.GetStrafeDirection() * _horizontalInput;
        if(m_currentActionState != ActionState.Slide)
        {
		    _direction += m_cameraPivot.forward * _verticalInput;
		    _direction += m_cameraPivot.right * _horizontalInput;
        }
        else
        {
            if (_verticalInput > 0)
            {
                _direction += m_slidingFwrd * _verticalInput;
            }
            else
            {
                // _currentActionState = ActionState.Walk;
                m_currentActionState = ActionState.Sprint;
            }
            _direction += m_slidingRight * _horizontalInput;
        }

        //Clamp movement vector to magnitude of '1f';
        if (_direction.magnitude > 1f)
			_direction.Normalize();

		m_playerMoveInputsDirection = _direction;
		return _direction;
	}

	//Calculate and return movement velocity based on player input, controller state, ground normal [...];
	Vector3 CalculateMovementVelocity()
	{
		//Calculate (normalized) movement direction;
		Vector3 _velocity = CalculateMovementDirection();

		//Save movement direction for later;
		Vector3 _velocityDirection = _velocity;

		//Multiply (normalized) velocity with movement speed;
		_velocity *= m_currentSpeed;

		//If controller is in the air, multiply movement velocity with 'airControl';
		if(!PlayerIsGrounded())
			_velocity = _velocityDirection * m_currentSpeed * m_airControl;

		//If controller is standing (or walking) on a slope, decrease player velocity based on the slope's angle;
		if(m_isSliding)
		{
			float _factor = Mathf.InverseLerp(90f, 0f, Vector3.Angle(m_trans.up, m_mover.GetGroundNormal()));
			_velocity *= _factor;  
		}

		return _velocity;
	}

	//Returns 'true' if the player presses the jump key;
	public bool IsJumpKeyPressed()
	{
        return (Input.GetButtonDown("Jump"));
	}
	public bool CanJump()
	{
		return !m_hasJump || !m_hasDoubleJump;
	}

	public bool IsDashKeyPressed()
	{
        return (Input.GetButtonDown("Dash"));
	}
	public bool CanDash()
	{
		if (CalculateMovementDirection() != Vector3.zero)
			return true;
		
		return false;
	}
	
    //Returns 'true' if the player presses the forward key;
    bool IsForwarding()
    {
        // return Input.GetKey(_controlKeyBinding.forward);
		if (Input.GetAxis(m_verticalInputAxis) > 0.1)
		{
			return true;
		}
		return false;
    }

    bool IsCrouching()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            return true;
        }
        return false;
    }
	bool IsSliding()
	{
		if (Input.GetButtonDown("Slide"))
		{
			return true;
		}
		return false;
	}

	//Apply friction to both vertical and horizontal momentum based on 'friction' and 'gravity';
	//Handle sliding down steep slopes;
	void HandleMomentum()
	{
		Vector3 _verticalMomentum = Vector3.zero;
		Vector3 _horizontalMomentum = Vector3.zero;

		//Split momentum into vertical and horizontal components;
		if(m_momentum != Vector3.zero)
		{
			_verticalMomentum = VectorMath.ExtractDotVector(m_momentum, m_trans.up);
			_horizontalMomentum = m_momentum - _verticalMomentum;
		}

        //Add gravity to vertical momentum;
		if (m_useGravity)
		{
			if(m_isSliding)
				_verticalMomentum -= m_trans.up * m_slideGravity * Time.deltaTime;
			else
				_verticalMomentum -= m_trans.up * m_gravity * Time.deltaTime;
			if(PlayerIsGrounded() && !m_isSliding)
				_verticalMomentum = Vector3.zero;
		}

		//Apply friction to horizontal momentum based on whether the controller is grounded;
		if(PlayerIsGrounded())
			_horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, m_groundFriction, Time.deltaTime, 0f);
		else
			_horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, m_airFriction, Time.deltaTime, 0f); 

		//Add horizontal and vertical momentum back together;
		m_momentum = _horizontalMomentum + _verticalMomentum;

        //Project the current momentum onto the current ground normal if the controller is sliding down a slope;
		if(m_isSliding)
		{
			m_momentum = Vector3.ProjectOnPlane(m_momentum, m_mover.GetGroundNormal());
		}

		//If controller is jumping, override vertical velocity with jumpSpeed;
		// if(m_currentControllerState == ControllerState.Jumping)
		// {
		// 	m_momentum = VectorMath.RemoveDotVector(m_momentum, m_trans.up);
		// 	m_momentum += m_trans.up * m_jumpSpeed;
		// }

        if(CurrentState(PlayerState.Jump))
        {
            m_momentum = VectorMath.RemoveDotVector(m_momentum, m_trans.up);
			m_momentum += m_trans.up * m_jumpSpeed;
        }
	}

	//Events;

	//This function is called when the player has initiated a jump;
	public void On_JumpStart()
	{
		//Add jump force to momentum;
		m_momentum += m_trans.up * m_jumpSpeed;

		//Set jump start time;
		m_currentJumpStartTime = Time.time;

		//Call event;
		if(OnJump != null)
			OnJump(m_momentum);
	}

	//This function is called when the player has lost ground contact, i.e. is either falling or rising, or generally in the air;
	public void On_GroundContactLost()
	{
		//Calculate current velocity;
		//If velocity would exceed the controller's movement speed, decrease movement velocity appropriately;
		//This prevents unwanted accumulation of velocity;
		float _horizontalMomentumSpeed = VectorMath.RemoveDotVector(GetMomentum(), m_trans.up).magnitude;
		Vector3 _currentVelocity = GetMomentum() + Vector3.ClampMagnitude(m_savedMovementVelocity, Mathf.Clamp(m_currentSpeed - _horizontalMomentumSpeed, 0f, m_currentSpeed));

		//Calculate length and direction from '_currentVelocity';
		float _length = _currentVelocity.magnitude;
		
		//Calculate velocity direction;
		Vector3 _velocityDirection = Vector3.zero;
		if(_length != 0f)
			_velocityDirection = _currentVelocity/_length;

		//Subtract from '_length', based on 'movementSpeed' and 'airControl', check for overshooting;
		if(_length >= m_currentSpeed * m_airControl)
			_length -= m_currentSpeed * m_airControl;
		else
			_length = 0f;

		m_momentum = _velocityDirection * _length;
	}

	//This function is called when the player has landed on a surface after being in the air;
	public void On_GroundContactRegained()
	{
		On_PlayerHasJump(false);
		On_PlayerHasDoubleJump(false);

		//Call 'OnLand' event;
		if(OnLand != null)
			OnLand(m_momentum);
	}

	//Helper functions;

	//Returns 'true' if vertical momentum is above a small threshold;
	bool IsFalling()
	{
		//Calculate current vertical momentum;
		Vector3 _verticalMomentum = VectorMath.ExtractDotVector(m_momentum, m_trans.up);

		//Setup threshold to check against;
		//For most applications, a value of '0.001f' is recommended;
		float _limit = 0.001f;

		//Return true if vertical momentum is above '_limit';
		return(_verticalMomentum.magnitude > _limit);
	}

#endregion

#region Public Functions
	public void ChangeState(PlayerState newPlayerState){
		m_sM.ChangeState((int)newPlayerState);
	}
    public bool CurrentState(PlayerState playerState)
    {
        return m_sM.CurrentStateIndex == (int)playerState;
    }

    public void CheckForGround()
	{
		m_mover.CheckForGround();
	}
    public bool PlayerIsGrounded()
	{
		return m_mover.IsGrounded();
	}

    public bool PlayerIsFalling()
	{
		return IsFalling() && (VectorMath.GetDotProduct(GetMomentum(), m_trans.up) > 0f);
	}

    public bool PlayerIsSliding()
	{
		return m_isSliding = PlayerIsGrounded() && (Vector3.Angle(m_mover.GetGroundNormal(), m_trans.up) > m_slopeLimit);
	}

    public bool PlayerInputIsMoving()
	{
		if(CalculateMovementDirection() != Vector3.zero){
			return true;
		}else{
			return false;
		}
	}

    public void Move()
	{
		//Apply friction and gravity to 'momentum';
		HandleMomentum();

		//Calculate movement velocity;
		Vector3 velocity = CalculateMovementVelocity();

		//Add current momentum to velocity;
		velocity += m_momentum;
		
		//If player is grounded or sliding on a slope, extend mover's sensor range;
		//This enables the player to walk up/down stairs and slopes without losing ground contact;
		m_mover.SetExtendSensorRange(PlayerIsGrounded());

		SetPlayerVelocity(velocity);
	}

    public void SetPlayerVelocity(Vector3 velocity)
	{
		//Set mover velocity;		
		m_mover.SetVelocity(velocity);

		//Store velocity for next frame;
        m_savedVelocity = velocity;
        m_savedMovementVelocity = velocity - m_momentum;
	}
	public void ResetPlayerVelocity()
	{
		SetPlayerVelocity(Vector3.zero);
	}

	public void On_PlayerHasJump(bool hasJump)
	{
		m_hasJump = hasJump;
	}
	public void On_PlayerHasDoubleJump(bool hasDoubleJump)
	{
		m_hasDoubleJump = hasDoubleJump;
	}

    //Get last frame's velocity;
	public Vector3 GetVelocity ()
	{
		return m_savedVelocity;
	}

	//Get last frame's movement velocity (momentum is ignored);
	public Vector3 GetMovementVelocity()
	{
		return m_savedMovementVelocity;
	}

	//Get current momentum;
	public Vector3 GetMomentum()
	{
		return m_momentum;
	}

	//Add momentum to controller;
	public void AddMomentum (Vector3 _momentum)
	{
		m_momentum += _momentum;	
	}

	public Vector3 GetPlayerMoveInputsDirection()
	{
		return m_playerMoveInputsDirection;
	}

	//Events;
	public delegate void VectorEvent(Vector3 v);
	public event VectorEvent OnJump;
	public event VectorEvent OnLand;

#endregion
    
}