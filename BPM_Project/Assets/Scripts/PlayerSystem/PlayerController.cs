using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class PlayerController : MonoBehaviour
{
    public static PlayerController s_instance;

#region Public Variables
	[Header("Debug")]
    [SerializeField] StateMachine m_sM = new StateMachine();
	// public bool m_useGravity = true;

	[Header("References")]
	[SerializeField] References m_references;
	[Serializable] class References{
		public Transform m_cameraPivot;
		public PlayerAudioController m_playerAudio;
	}

	[Header("Movements")]
	[SerializeField] Movements m_movements;
	[Serializable] class Movements{
		public bool m_useRawInput = true;
		public float m_movementSpeed = 10f;
	}

	[Header("Physics")]
	[SerializeField] Physics m_physics;
	[Serializable] class Physics{
		//'AirFriction' determines how fast the controller loses its momentum while in the air;
		public float m_airFriction = 3f;

		//'GroundFriction' is used instead, if the controller is grounded;
		public float m_groundFriction = 100f;

		//Amount of downward gravitation;
		public float m_gravity = 35f;
	}

	[Header("Jump variables")]
	public Jump m_jump;
	[Serializable] public class Jump{
		//'Aircontrol' determines to what degree the player is able to move while in the air;
		[Range(0f, 1f)] public float m_airControl = 0.4f;
		public float m_speed = 10f;
		public float m_duration = 0.2f;
	}

	[Header("Dash")]
	public Dash m_dash;
	[Serializable] public class Dash{
		public float m_distance = 10;
		public float m_timeToDash = 0.25f;
	}

	[Header("Field Of View")]
	public FieldOfView m_fov;
	[Serializable] public class FieldOfView
	{
		[Header("Values")]
		public float m_normalFov = 90f;
		public float m_dashFov = 110f;

		[Header("Anims")]
		public FovChanger m_startDash;
		public FovChanger m_endDash;

		[Serializable] public class FovChanger
		{
			public float m_timeToChangeFov;
			public AnimationCurve m_changeFovCurve;
		}
	}

#endregion

#region Private Variables
    //References to attached components;
	Transform m_trans;
	Mover m_mover;

	//Names of input axes used for horizontal and vertical input;
	string m_horizontalInputAxis = "Horizontal";
	string m_verticalInputAxis = "Vertical";

    //Movement speed;
    float m_currentSpeed;
	
	float m_currentJumpStartTime = 0f;

	//Current momentum;
	Vector3 m_momentum = Vector3.zero;

	//Saved velocity from last frame;
	Vector3 m_savedVelocity = Vector3.zero;

	//Saved horizontal movement velocity from last frame;
	Vector3 m_savedMovementVelocity = Vector3.zero;

	bool m_hasJump = false;
	bool m_hasDoubleJump = false;
	bool m_hasDash = false;

	Vector3 m_playerMoveInputsDirection;

    CameraController m_cameraController;

#endregion

#region Event Functions
	void Awake()
    {
        SetupSingleton();
		SetupStateMachine();

        m_mover = GetComponent<Mover>();
		m_trans = GetComponent<Transform>();

		m_cameraController = GetComponentInChildren<CameraController>();

        m_currentSpeed = m_movements.m_movementSpeed;
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
		m_sM.Update();
	}

	void LateUpdate()
	{
		m_sM.LateUpdate();
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

    //Calculate and return movement direction based on player input;
	//This function can be overridden by inheriting scripts to implement different player controls;
	Vector3 CalculateMovementDirection()
	{
		float _horizontalInput;
		float _verticalInput;

		//Get input;
		if(m_movements.m_useRawInput){
			_horizontalInput = Input.GetAxisRaw(m_horizontalInputAxis);
			_verticalInput = Input.GetAxisRaw(m_verticalInputAxis);
		} else {
			_horizontalInput = Input.GetAxis(m_horizontalInputAxis);
			_verticalInput = Input.GetAxis(m_verticalInputAxis);
		}

		Vector3 _direction = Vector3.zero;

		//Use camera axes to construct movement direction;
		_direction += m_references.m_cameraPivot.forward * _verticalInput;
		_direction += m_references.m_cameraPivot.right * _horizontalInput;

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
			_velocity = _velocityDirection * m_currentSpeed * m_jump.m_airControl;

		return _velocity;
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
		// if (m_useGravity)
		// {
			_verticalMomentum -= m_trans.up * m_physics.m_gravity * Time.deltaTime;
			if(PlayerIsGrounded())
				_verticalMomentum = Vector3.zero;
		// }

		//Apply friction to horizontal momentum based on whether the controller is grounded;
		if(PlayerIsGrounded())
			_horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, m_physics.m_groundFriction, Time.deltaTime, 0f);
		else
			_horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, m_physics.m_airFriction, Time.deltaTime, 0f); 

		//Add horizontal and vertical momentum back together;
		m_momentum = _horizontalMomentum + _verticalMomentum;

        if(CurrentState(PlayerState.Jump))
        {
            m_momentum = VectorMath.RemoveDotVector(m_momentum, m_trans.up);
			m_momentum += m_trans.up * m_jump.m_speed;
        }
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
		if (CalculateMovementDirection() != Vector3.zero && !m_hasDash)
			return true;

		return false;
	}

	public void On_PlayerIsRunning(bool isRunning)
	{
		m_references.m_playerAudio.On_Run(isRunning);
	}
	public void On_PlayerHasJump(bool hasJump)
	{
		m_hasJump = hasJump;
		if (hasJump)
			m_references.m_playerAudio.On_Jump();
	}
	public void On_PlayerHasDoubleJump(bool hasDoubleJump)
	{
		m_hasDoubleJump = hasDoubleJump;
		if (hasDoubleJump)
			m_references.m_playerAudio.On_DoubleJump();
	}
	public void On_PlayerHasDash(bool hasDash)
	{
		m_hasDash = hasDash;
		if (hasDash)
			m_references.m_playerAudio.On_Dash();
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
	//This function is called when the player has initiated a jump;
	public void On_JumpStart()
	{
		//Add jump force to momentum;
		m_momentum += m_trans.up * m_jump.m_speed;

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
		if(_length >= m_currentSpeed * m_jump.m_airControl)
			_length -= m_currentSpeed * m_jump.m_airControl;
		else
			_length = 0f;

		m_momentum = _velocityDirection * _length;
	}

	//This function is called when the player has landed on a surface after being in the air;
	public void On_GroundContactRegained()
	{
		On_PlayerHasJump(false);
		On_PlayerHasDoubleJump(false);
		On_PlayerHasDash(false);

		//Call 'OnLand' event;
		if(OnLand != null)
			OnLand(m_momentum);

		m_references.m_playerAudio.On_Land();
	}

	//Events;
	public delegate void VectorEvent(Vector3 v);
	public event VectorEvent OnJump;
	public event VectorEvent OnLand;

	public void ChangeCameraFov(float newFov, float timeToChangeFov, AnimationCurve changeFovCurve)
	{
		m_cameraController.ChangeCameraFov(newFov, timeToChangeFov, changeFovCurve);
	}

#endregion
}