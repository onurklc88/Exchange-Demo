using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour,IGameStateListener
{
    
    [Networked] private NetworkButtons _prevButton { get; set; }
    [Networked] private TickTimer _crouchCooldown { get; set; }
    [Networked(OnChanged = nameof(OnPlayerHeightChanged)) ] private NetworkBool _isPlayerCrouching { get; set; }
    [Networked(OnChanged = nameof(OnPlayerMove)) ] private NetworkBool _isPlayerWalk { get; set; }
    public GameManager.GameState CurrentGameState { get; set; }
    public NetworkBool IsPlayerDead;
   
    
    [SerializeField] private OperatorProperties _operatorProperties;
    [SerializeField] private Camera _playerCamera;
  
    private NetworkCharacterControllerPrototype _characterController;
    private float _mouseSensitivity = 5f;
    private float _upDownRange = 80.0f;
    private float _elapsedTime = 0;
    private float _crouchHeight = 1.2f;
    private const int _layerFpsArm = 7;

    [HideInInspector] public float MouseXPosition;
    [HideInInspector] public float VerticalCamRotation;
    private float rotationYVelocity, cameraXVelocity;
    private float _currentYPosition;
    private float _curretXposition;
    private float _yRotationSpeed;
    private float _xCameraSpeed;
   
    private bool _isPlayerRunning = false;
   
    private void OnEnable()
    {
       EventLibrary.OnGameStateChange.AddListener(UpdateGameState);
    }
    private void OnDisable()
    {
        EventLibrary.OnGameStateChange.RemoveListener(UpdateGameState);
    }


    public override void Spawned()
    {
        _characterController = GetComponent<NetworkCharacterControllerPrototype>();
    }

 
    public override void FixedUpdateNetwork()
    {
        if (IsPlayerDead) return;

        if (Runner.TryGetInputForPlayer<PlayerInputData>(Object.InputAuthority, out var input))
        {
            ReadInputs(input);
            _characterController.Move(GetInputDirection(input));
            if(GetInputDirection(input) == Vector3.zero)
            {
                _isPlayerWalk = false;
            }
            else
            {
                _isPlayerWalk = true;
            }
            _prevButton = input.NetworkButtons;
            HandleRotation(input);
        }
    }

    private void ReadInputs(PlayerInputData input)
    {
       
        var pressedButton = input.NetworkButtons.GetPressed(_prevButton);
        var shootingButton = input.NetworkButtons.IsSet(LocalInputPoller.PlayerInputButtons.Mouse0) || input.NetworkButtons.IsSet(LocalInputPoller.PlayerInputButtons.Mouse1);

       
        
        if (shootingButton && !Object.HasInputAuthority)
        {
            _characterController.maxSpeed = 5f;
            EventLibrary.OnPlayerRun?.Invoke(false);
        }
      


            if (pressedButton.WasPressed(_prevButton, LocalInputPoller.PlayerInputButtons.Jump))
            {
               JumpPlayer();
            }
            else if (pressedButton.WasPressed(_prevButton, LocalInputPoller.PlayerInputButtons.Interact))
            {

            }
            else if (pressedButton.WasPressed(_prevButton, LocalInputPoller.PlayerInputButtons.Crouch))
            {
                CrouchPlayer();
            }
            else if (pressedButton.WasReleased(_prevButton, LocalInputPoller.PlayerInputButtons.Sprint) && !shootingButton && !_isPlayerCrouching && (input.VerticalInput != 0 || input.HorizontalInput != 0)) 
            {
             //IncreasePlayerSpeed();
              _characterController.maxSpeed = _operatorProperties.RunSpeed;
            if (_isPlayerRunning) return;
                _isPlayerRunning = true;
             
                UpdateRunAnimation();
            }
            else
            {
                if (!_isPlayerRunning) return;
                _isPlayerRunning = false;
                _characterController.maxSpeed = 5f;
                UpdateRunAnimation();
            }

    }

    private Vector3 GetInputDirection(PlayerInputData input)
    {
        float verticalSpeed = input.VerticalInput * _characterController.maxSpeed;
        float horizontalSpeed = input.HorizontalInput * _characterController.maxSpeed;
        Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;
        return horizontalMovement;
    }
    private void HandleRotation(PlayerInputData input)
    {
        MouseXPosition = input.MouseX * _mouseSensitivity;
        VerticalCamRotation -= input.MouseY * _mouseSensitivity;
        VerticalCamRotation = Mathf.Clamp(VerticalCamRotation, -_upDownRange, _upDownRange);
        _curretXposition = Mathf.SmoothDamp(_curretXposition, MouseXPosition, ref rotationYVelocity, _yRotationSpeed);
        _currentYPosition = Mathf.SmoothDamp(_currentYPosition, VerticalCamRotation, ref cameraXVelocity, _xCameraSpeed);
        transform.Rotate(0, _curretXposition, 0);
        _playerCamera.transform.localRotation = Quaternion.Euler(_currentYPosition, 0, 0);
    }

    private void IncreasePlayerSpeed()
    {
        if (_characterController.maxSpeed >= _operatorProperties.RunSpeed) return;
        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / 1f);
        _characterController.maxSpeed = Mathf.Lerp(_operatorProperties.WalkSpeed, _operatorProperties.RunSpeed, t);
        if (t >= 1f)
            _elapsedTime = 0;
    }

    private void CrouchPlayer()
    {
      
            if(_characterController.maxSpeed == _operatorProperties.WalkSpeed)
            {

                if (_characterController.Controller.height != _crouchHeight)
                {
                    _isPlayerCrouching = true;
                   
                }
                else
                {
                   _isPlayerCrouching = false;
                }
             }
             else if (_characterController.maxSpeed >= _operatorProperties.RunSpeed - 1f)
             {
                EventLibrary.OnPlayerCrouch?.Invoke();
                StartCoroutine(HandleSliding());
             }

    }
       
    private static void OnPlayerMove(Changed<PlayerController> changed)
    {
        var currentState = changed.Behaviour._isPlayerWalk;
        changed.LoadOld();
        var oldState = changed.Behaviour._isPlayerWalk;
        if (oldState != currentState)
        {
            changed.Behaviour.UpdateWalkAnimation(currentState);
        }
    }

    private static void OnPlayerHeightChanged(Changed<PlayerController> changed)
    {
        var currentState = changed.Behaviour._isPlayerCrouching;
        changed.LoadOld();
        var oldState = changed.Behaviour._isPlayerCrouching;
        if(oldState != currentState)
        {
            changed.Behaviour.ChangeHeight(currentState);
        }
     
    }

    private void UpdateWalkAnimation(bool state)
    {

        if (!Object.HasInputAuthority) return;
        EventLibrary.OnPlayerWalking?.Invoke(state);
    }

    private void UpdateRunAnimation()
    {
        if (!Object.HasInputAuthority) return;
        EventLibrary.OnPlayerRun?.Invoke(_isPlayerRunning);
    }

    private void ChangeHeight(bool crouch)
    {
        if (crouch)
        {
            _characterController.Controller.height = 1.2f;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.40f, transform.position.z);
            _characterController.Controller.height = 2f;
        }
    }


   
    private IEnumerator HandleSliding()
    {
        float timeElapsed = 0f;
        _isPlayerCrouching = true;
        while (timeElapsed < 1f)
        {
            if (Object.HasStateAuthority)
                _characterController.maxSpeed = 4f;
            else
                _characterController.maxSpeed = 5f;

           _characterController.Move(transform.forward);
           timeElapsed += Runner.DeltaTime;
           yield return null;
        }

        _isPlayerCrouching = false;
    }
   
    private void JumpPlayer()
    {
        if (!_characterController.IsGrounded) return;
         EventLibrary.OnPlayerJump?.Invoke();
        _characterController.Jump();
    }

    public void UpdateGameState(GameManager.GameState currentGameState)
    {
        switch (currentGameState)
        {
            case GameManager.GameState.MatchStart:
              
                break;
            case GameManager.GameState.Match:
               
                break;
                
        }
    }

  
}
