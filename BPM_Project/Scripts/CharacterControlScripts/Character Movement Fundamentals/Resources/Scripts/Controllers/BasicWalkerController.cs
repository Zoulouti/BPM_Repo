﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Basic controller script;
//This controller is used as a basis for more advanced controller types ('SidescrollerController', 'CameraWalkerController');
//Custom movement input can be implemented by creating a new script that inherits 'BasicWalkerController' and overrides the 'CalculateMovementDirection' function;
public class BasicWalkerController : MonoBehaviour {

	//References to attached components;
	protected Transform trans;
	protected Mover mover;

	//Names of input axes used for horizontal and vertical input;
	public string horizontalInputAxis = "Horizontal";
	public string verticalInputAxis = "Vertical";

	//Whether or not to use raw input values;
	public bool useRawInput = false;
    public ControlKeyBinding _controlKeyBinding = new ControlKeyBinding();
    [Serializable] public class ControlKeyBinding
    {
        [Header("Movement KeyBind")]
        public KeyCode forward = KeyCode.Z;
        public KeyCode back = KeyCode.S;
        public KeyCode left= KeyCode.D;
        public KeyCode right= KeyCode.Q;
        [Header("Crouch KeyBind")]
        public KeyCode crouch = KeyCode.LeftControl;
    }
    [Space]
    //Keycode used for jumping;
    public KeyCode jumpKey = KeyCode.Space;

	//Jump key variables;
	bool jumpKeyWasPressed = false;
	bool jumpKeyWasLetGo = false;
	bool jumpKeyIsPressed = false;

    //Keycode used for Sprinting;
    public KeyCode sprintKey = KeyCode.LeftShift;

    //crouch key variables
    bool isCrouching;

    //sprint key variables;
    bool sprintKeyWasPressed = false;
    bool sprintKeyWasLetGo = false;
    bool sprintKeyIsPressed = false;
    bool isSprinting;
    [Space]
    [Header("Movement speed variables")]
    //Movement speed;
    float _currentSpeed;
    public float walkingSpeed = 7f;
    public float sprintingSpeed = 17f;
    [Space]
    [Header("Sprint variables")]
    public float fovWhenWalking = 90f;
    public float fovWhenSprinting = 110f;
    float currentFOV;
    [Space]
    [Header("Crouch varibales")]
    public float crouchColliderHeight;
    public LayerMask crouchLayer;
    float initialColliderHeight;
    [Space]
    [Header("Slide varibales")]
    public float slideMaxSpeed = 20f;
    public float slideTime = 1f;
    float _currentSlidingTime;
    float detlaSpeed;
    AnimationCurve _curve;

    float _animationcurveFirstKey;

    [Tooltip("True if you want to use your custom curve")]
    public bool _useCustomCurve;
    public AnimationCurve customCurve;
    [Space]
    [Header("Jump variables")]
	//'Aircontrol' determines to what degree the player is able to move while in the air;
	[Range(0f, 1f)]
	public float airControl = 0.4f;

	//Jump speed;
	public float jumpSpeed = 10f;

	//Jump duration variables;
	public float jumpDuration = 0.2f;
	float currentJumpStartTime = 0f;

	//'AirFriction' determines how fast the controller loses its momentum while in the air;
	//'GroundFriction' is used instead, if the controller is grounded;
	public float airFriction = 0.5f;
	public float groundFriction = 100f;

	//Current momentum;
	Vector3 momentum = Vector3.zero;

	//Saved velocity from last frame;
	Vector3 savedVelocity = Vector3.zero;

	//Saved horizontal movement velocity from last frame;
	Vector3 savedMovementVelocity = Vector3.zero;

	//Amount of downward gravitation;
	public float gravity = 30f;
	//Amount of downward gravitation when sliding down a slope;
	public float slideGravity = 30f;

	//Acceptable slope angle limit;
	public float slopeLimit = 80f;

	//Enum describing basic controller states; 
	public enum ControllerState
	{
		Grounded,
		Sliding,
		Falling,
		Rising,
		Jumping
	}
	ControllerState currentControllerState = ControllerState.Falling;

    //Enum describing in what state the PC is
    protected enum ActionState
    {
        Walk,
        Sprint,
        Crouch,
        Slide,
    }
    protected ActionState _currentActionState = ActionState.Walk;
	 
	//Get references to all necessary components;
	void Awake () {
        
		mover = GetComponent<Mover>();
		trans = GetComponent<Transform>();

		Setup();
        _currentSpeed = walkingSpeed;
        initialColliderHeight = mover.colliderHeight;


        #region Animation Curve Initialisation

        detlaSpeed = slideMaxSpeed - sprintingSpeed;
        //_animationcurveFirstKey = slideMaxSpeed / sprintingSpeed;
        _curve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));

        #endregion

    }

    //This function is called right after Awake(); It can be overridden by inheriting scripts;
    protected virtual void Setup()
	{

	}

	void Update()
	{
		HandleJumpKeyInput();
        HandleSprintKeyInput();
        HandleCrouchKeyInput();

    }

	//Handle jump booleans for later use in FixedUpdate;
	void HandleJumpKeyInput()
	{
		bool _newJumpKeyPressedState = IsJumpKeyPressed();

		if(jumpKeyIsPressed == false && _newJumpKeyPressedState == true)
        {

            if (_currentActionState == ActionState.Slide || _currentActionState == ActionState.Sprint)
            {
                _currentActionState = ActionState.Sprint;
                StopCoroutine(_slidingCoroutine(_useCustomCurve));
                isCrouching = true;
            }
            else if(_currentActionState == ActionState.Crouch || _currentActionState == ActionState.Walk)
            {
                _currentActionState = ActionState.Walk;
            }

            RaycastHit _hit;
            if (!Physics.Raycast(new Ray(transform.position, transform.up), out _hit, Mathf.Infinity, crouchLayer))
            {
                jumpKeyWasPressed = true;
                if (isCrouching)
                {
                    CrouchingSwitch();
                }
            }
            Debug.DrawLine(transform.position, transform.up * 100, Color.blue, 0.1f);
        }


		if(jumpKeyIsPressed == false && _newJumpKeyPressedState == false)
			jumpKeyWasLetGo = true;

		jumpKeyIsPressed = _newJumpKeyPressedState;
	}

    //Handle sprint booleans
    void HandleSprintKeyInput()
    {
        bool _newSprintKeyPressedState = IsSprintActive();
        bool _forwardKeyIsPressed = IsForwarding();
        if (_newSprintKeyPressedState && _forwardKeyIsPressed && !isCrouching)   // verify when the sprint needs to be canceled
        {
            _currentSpeed = sprintingSpeed;

            #region FOV
            Camera.main.fieldOfView = Mathf.Lerp(currentFOV, fovWhenSprinting, Time.deltaTime * 4f);

            currentFOV = Camera.main.fieldOfView;

            if(currentFOV > fovWhenSprinting - 0.5f)
            {
                currentFOV = fovWhenSprinting;
            }
            #endregion

            _currentActionState = ActionState.Sprint;
        }
        else if(currentControllerState == ControllerState.Grounded && _currentActionState != ActionState.Slide)
        {
            _currentSpeed = walkingSpeed;

            #region FOV
            Camera.main.fieldOfView = Mathf.Lerp(currentFOV, fovWhenWalking, Time.deltaTime * 4f);
            currentFOV = Camera.main.fieldOfView;
            if (currentFOV < fovWhenWalking + 0.5f)
            {
                currentFOV = fovWhenWalking;
            }
            #endregion

            isSprinting = false;
            if (!isCrouching)
            {
                _currentActionState = ActionState.Walk;
            }

        }
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


    protected Vector3 slidingFwrd;
    protected Vector3 slidingRight;

    void CrouchingSwitch()
    {
        switch (isCrouching)
        {
            case true:

                mover.colliderHeight = initialColliderHeight;
                _currentActionState = ActionState.Walk;
                isCrouching = false;

                break;
            case false:

                mover.colliderHeight = crouchColliderHeight;
                isCrouching = true;

                if (_currentActionState != ActionState.Sprint)
                {
                    _currentActionState = ActionState.Crouch;
                }
                else if(_currentActionState != ActionState.Slide)
                {
                    _currentActionState = ActionState.Slide;
                    slidingFwrd = Camera.main.transform.forward;
                    StartCoroutine(_slidingCoroutine(_useCustomCurve));
                }

                break;
            default:
                break;
        }
    }

    IEnumerator _slidingCoroutine(bool CustomCurve)
    {
        _currentSlidingTime = 0;
        while (_currentSlidingTime / slideTime <= 1 && _currentActionState == ActionState.Slide)
        {
            yield return new WaitForSeconds(0.02f);
            _currentSlidingTime += Time.deltaTime;
            if (!CustomCurve)
            {
                float speed = _curve.Evaluate(_currentSlidingTime / slideTime);
                _currentSpeed = sprintingSpeed + (detlaSpeed * speed);
            }
            else
            {
                float speed = customCurve.Evaluate(_currentSlidingTime / slideTime);
                _currentSpeed = sprintingSpeed + (detlaSpeed * speed);
            }
        }

        if(_currentActionState != ActionState.Sprint)
        {
            isCrouching = false;
            CrouchingSwitch();
        }
        _currentSlidingTime = 0;
        StopCoroutine(_slidingCoroutine(_useCustomCurve));
    }

    //FixedUpdate;
    void FixedUpdate()
	{
		//Check if mover is grounded;
		mover.CheckForGround();

		//Determine controller state;
		HandleState();

		//Apply friction and gravity to 'momentum';
		HandleMomentum();

		//Check if the player has initiated a jump;
		HandleJumping();

		//Calculate movement velocity;
		Vector3 _velocity = CalculateMovementVelocity();

		//Add current momentum to velocity;
		_velocity += momentum;
		
		//If player is grounded or sliding on a slope, extend mover's sensor range;
		//This enables the player to walk up/down stairs and slopes without losing ground contact;
		mover.SetExtendSensorRange(IsGrounded());

		//Set mover velocity;		
		mover.SetVelocity(_velocity);
		// Debug.Log("Forward Velocity= " + _velocity.z);
		// Debug.Log("Right Velocity= " + _velocity.x);

		//Store velocity for next frame;
		savedVelocity = _velocity;
		savedMovementVelocity = _velocity - momentum;

		//Reset jump key booleans;
		jumpKeyWasLetGo = false;
		jumpKeyWasPressed = false;
	}

	//Calculate and return movement direction based on player input;
	//This function can be overridden by inheriting scripts to implement different player controls;
	protected virtual Vector3 CalculateMovementDirection()
	{
		Vector3 _velocity = Vector3.zero;

		float _horizontalInput;
		float _verticalInput;

		//Get input;
		if(useRawInput){
			_horizontalInput = Input.GetAxisRaw(horizontalInputAxis);
			_verticalInput = Input.GetAxisRaw(verticalInputAxis);
		} else {
			_horizontalInput = Input.GetAxis(horizontalInputAxis);
			_verticalInput = Input.GetAxis(verticalInputAxis);
		}

		_velocity += trans.right * _horizontalInput;
		_velocity += trans.forward * _verticalInput;

		//Clamp movement vector to magnitude of 1f;
		if(_velocity.magnitude > 1f)
			_velocity.Normalize();

		return _velocity;
	}

	//Calculate and return movement velocity based on player input, controller state, ground normal [...];
	protected Vector3 CalculateMovementVelocity()
	{
		//Calculate (normalized) movement direction;
		Vector3 _velocity = CalculateMovementDirection();

		//Save movement direction for later;
		Vector3 _velocityDirection = _velocity;

		//Multiply (normalized) velocity with movement speed;
		_velocity *= _currentSpeed;

		//If controller is in the air, multiply movement velocity with 'airControl';
		if(!IsGrounded())
			_velocity = _velocityDirection * _currentSpeed * airControl;

		//If controller is standing (or walking) on a slope, decrease player velocity based on the slope's angle;
		if(currentControllerState == ControllerState.Sliding)
		{
			float _factor = Mathf.InverseLerp(90f, 0f, Vector3.Angle(trans.up, mover.GetGroundNormal()));
			_velocity *= _factor;  
		}

		return _velocity;
	}

	//Returns 'true' if the player presses the jump key;
	protected virtual bool IsJumpKeyPressed()
	{
		 return (Input.GetKeyDown(jumpKey));
	}
    //Returns 'true' if the player presses the forward key;
    protected virtual bool IsForwarding()
    {
        return Input.GetKey(_controlKeyBinding.forward);
    }
    //Returns 'true' if the player had pressed the sprint key;
    protected virtual bool IsSprintActive()
    {
        if(Input.GetKeyDown(sprintKey) || isSprinting)
        {
            isSprinting = true;
            return true;
        }
        return false;
    }
    //Returns 'true' if the player had pressed the crouch key;
    protected virtual bool IsCrouching()
    {
        if (Input.GetKeyDown(_controlKeyBinding.crouch))
        {
            return true;
        }
        return false;
    }

    //Handle state transitions;
    //Determine current controller state based on current momentum and whether the controller is grounded (or not);
    void HandleState()
	{
		//Check if vertical momentum is pointing upwards;
		bool _isRising = IsRisingOrFalling() && (VectorMath.GetDotProduct(GetMomentum(), trans.up) > 0f);
		//Check if controller is sliding;
		bool _isSliding = mover.IsGrounded() && (Vector3.Angle(mover.GetGroundNormal(), trans.up) > slopeLimit);

		switch(currentControllerState)
		{
		case ControllerState.Grounded:

			if(_isRising){
				currentControllerState = ControllerState.Rising;
				OnGroundContactLost();
				break;
			}
			if(!mover.IsGrounded()){
				currentControllerState = ControllerState.Falling;
				OnGroundContactLost();
				break;
			}
			if(_isSliding){
				currentControllerState = ControllerState.Sliding;
				break;
			}
			break;

		case ControllerState.Falling:

			if(_isRising){
				currentControllerState = ControllerState.Rising;
				break;
			}
			if(mover.IsGrounded() && !_isSliding){
				currentControllerState = ControllerState.Grounded;
				OnGroundContactRegained(momentum);
				break;
			}
			if(_isSliding){
				currentControllerState = ControllerState.Sliding;
				OnGroundContactRegained(momentum);
				break;
			}
			break;

		case ControllerState.Sliding:

			if(_isRising){
				currentControllerState = ControllerState.Rising;
				OnGroundContactLost();
				break;
			}
			if(!mover.IsGrounded()){
				currentControllerState = ControllerState.Falling;
				break;
			}
			if(mover.IsGrounded() && !_isSliding){
				OnGroundContactRegained(momentum);
				currentControllerState = ControllerState.Grounded;
				break;
			}
			break;

		case ControllerState.Rising:

			if(!_isRising){
				if(mover.IsGrounded() && !_isSliding){
					currentControllerState = ControllerState.Grounded;
					OnGroundContactRegained(momentum);
					break;
				}
				if(_isSliding){
					currentControllerState = ControllerState.Sliding;
					break;
				}
				if(!mover.IsGrounded()){
					currentControllerState = ControllerState.Falling;
					break;
				}
			}
			break;

		case ControllerState.Jumping:

			//Check for jump timeout;
			if((Time.time - currentJumpStartTime) > jumpDuration)
				currentControllerState = ControllerState.Rising;

			//Check if jump key was let go;
			if(jumpKeyWasLetGo)
				currentControllerState = ControllerState.Rising;

			break;
		}
	}

	//Check if player has initiated a jump;
	void HandleJumping()
	{
		if(currentControllerState == ControllerState.Grounded)
		{
			if(jumpKeyIsPressed == true || jumpKeyWasPressed)
			{
				//Call events;
				OnGroundContactLost();
				OnJumpStart();

				currentControllerState = ControllerState.Jumping;
			}
		}
	}

	//Apply friction to both vertical and horizontal momentum based on 'friction' and 'gravity';
	//Handle sliding down steep slopes;
	void HandleMomentum()
	{
		Vector3 _verticalMomentum = Vector3.zero;
		Vector3 _horizontalMomentum = Vector3.zero;

		//Split momentum into vertical and horizontal components;
		if(momentum != Vector3.zero)
		{
			_verticalMomentum = VectorMath.ExtractDotVector(momentum, trans.up);
			_horizontalMomentum = momentum - _verticalMomentum;
		}

		//Add gravity to vertical momentum;
		if(currentControllerState == ControllerState.Sliding)
			_verticalMomentum -= trans.up * slideGravity * Time.deltaTime;
		else
			_verticalMomentum -= trans.up * gravity * Time.deltaTime;

		//Remove any downward force if the controller is grounded;
		if(currentControllerState == ControllerState.Grounded)
			_verticalMomentum = Vector3.zero;

		//Apply friction to horizontal momentum based on whether the controller is grounded;
		if(IsGrounded())
			_horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, groundFriction, Time.deltaTime, 0f);
		else
			_horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, airFriction, Time.deltaTime, 0f); 

		//Add horizontal and vertical momentum back together;
		momentum = _horizontalMomentum + _verticalMomentum;

		//Project the current momentum onto the current ground normal if the controller is sliding down a slope;
		if(currentControllerState == ControllerState.Sliding)
		{
			momentum = Vector3.ProjectOnPlane(momentum, mover.GetGroundNormal());
		}

		//If controller is jumping, override vertical velocity with jumpSpeed;
		if(currentControllerState == ControllerState.Jumping)
		{
			momentum = VectorMath.RemoveDotVector(momentum, trans.up);
			momentum += trans.up * jumpSpeed;
		}
	}

	//Events;

	//This function is called when the player has initiated a jump;
	void OnJumpStart()
	{
		//Add jump force to momentum;
		momentum += trans.up * jumpSpeed;

		//Set jump start time;
		currentJumpStartTime = Time.time;

		//Call event;
		if(OnJump != null)
			OnJump(momentum);
	}

	//This function is called when the player has lost ground contact, i.e. is either falling or rising, or generally in the air;
	void OnGroundContactLost()
	{
		//Calculate current velocity;
		//If velocity would exceed the controller's movement speed, decrease movement velocity appropriately;
		//This prevents unwanted accumulation of velocity;
		float _horizontalMomentumSpeed = VectorMath.RemoveDotVector(GetMomentum(), trans.up).magnitude;
		Vector3 _currentVelocity = GetMomentum() + Vector3.ClampMagnitude(savedMovementVelocity, Mathf.Clamp(_currentSpeed - _horizontalMomentumSpeed, 0f, _currentSpeed));

		//Calculate length and direction from '_currentVelocity';
		float _length = _currentVelocity.magnitude;
		
		//Calculate velocity direction;
		Vector3 _velocityDirection = Vector3.zero;
		if(_length != 0f)
			_velocityDirection = _currentVelocity/_length;

		//Subtract from '_length', based on 'movementSpeed' and 'airControl', check for overshooting;
		if(_length >= _currentSpeed * airControl)
			_length -= _currentSpeed * airControl;
		else
			_length = 0f;

		momentum = _velocityDirection * _length;
	}

	//This function is called when the player has landed on a surface after being in the air;
	void OnGroundContactRegained(Vector3 _collisionVelocity)
	{
		//Call 'OnLand' event;
		if(OnLand != null)
			OnLand(_collisionVelocity);
	}

	//Helper functions;

	//Returns 'true' if vertical momentum is above a small threshold;
	private bool IsRisingOrFalling()
	{
		//Calculate current vertical momentum;
		Vector3 _verticalMomentum = VectorMath.ExtractDotVector(momentum, trans.up);

		//Setup threshold to check against;
		//For most applications, a value of '0.001f' is recommended;
		float _limit = 0.001f;

		//Return true if vertical momentum is above '_limit';
		return(_verticalMomentum.magnitude > _limit);
	}

	//Getters;

	//Get last frame's velocity;
	public Vector3 GetVelocity ()
	{
		return savedVelocity;
	}

	//Get last frame's movement velocity (momentum is ignored);
	public Vector3 GetMovementVelocity()
	{
		return savedMovementVelocity;
	}

	//Get current momentum;
	public Vector3 GetMomentum()
	{
		return momentum;
	}

	//Returns 'true' if controller is grounded (or sliding down a slope);
	public bool IsGrounded()
	{
		return(currentControllerState == ControllerState.Grounded || currentControllerState == ControllerState.Sliding);
	}

	//Add momentum to controller;
	public void AddMomentum (Vector3 _momentum)
	{
		momentum += _momentum;	
	}

	//Events;
	public delegate void VectorEvent(Vector3 v);
	public event VectorEvent OnJump;
	public event VectorEvent OnLand;

}
