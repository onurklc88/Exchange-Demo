using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterAnimationController : NetworkBehaviour, IReadInput
{
    [SerializeField] private Animator _characterAnimator;
    [SerializeField] private RuntimeAnimatorController[] _characterNetworkAnimations;

    [Networked] public NetworkButtons PreviousInput { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerMoveForward { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerMoveRightForward { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerMoveLeftForward { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerMoveBackwards { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerCrouch { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerCrouchWalk { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerCrouchWalkRight { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerCrouchWalkLeft { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerCrouchWalkBackwards { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerJump { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerSprint { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerSlide { get; set; }
    [Networked(OnChanged = nameof(OnAnimationStateChange))] private NetworkBool _isPlayerFiring { get; set; }
    [Networked(OnChanged = nameof(OnPlayerSwitchWeapon))] private int _runtimeAnimatorIndex { get; set; }
    
    [Networked] private TickTimer _shootCooldown { get; set; }

    //trash
    private float _timeBetweenShots = 0.15f;

    
    public override void Spawned()
    {
        _runtimeAnimatorIndex = 1;
    }


   

    private static void OnPlayerSwitchWeapon(Changed<CharacterAnimationController> changed)
    {
       
        var currentState = changed.Behaviour._runtimeAnimatorIndex;
        changed.LoadOld();
        var oldState = changed.Behaviour._runtimeAnimatorIndex;
        

        if (currentState != oldState)
        {
           changed.Behaviour._characterAnimator.runtimeAnimatorController = changed.Behaviour._characterNetworkAnimations[changed.Behaviour._runtimeAnimatorIndex];
        }
    }




    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerInputData>(Object.InputAuthority, out var input))
        {
            ReadInputs(input);
            UpdateCharacterAnimations(input);
           
        }
    }


    public void ReadInputs(PlayerInputData input)
    {
        if (!Object.HasStateAuthority) return;
        var pressedButton = input.NetworkButtons.GetPressed(PreviousInput);
        var checkSprintKey = input.NetworkButtons.IsSet(LocalInputPoller.PlayerInputButtons.Sprint);
       
        if (pressedButton.WasPressed(PreviousInput, LocalInputPoller.PlayerInputButtons.Crouch) && !checkSprintKey)
        {
            if (_isPlayerCrouch)
                _isPlayerCrouch = false;
            else
                _isPlayerCrouch = true;
           
        }
        else if(pressedButton.WasPressed(PreviousInput, LocalInputPoller.PlayerInputButtons.Crouch) && checkSprintKey)
        {
            _isPlayerSlide = true;
        }
        else
        {
            _isPlayerSlide = false;
        }

        if(pressedButton.WasPressed(PreviousInput, LocalInputPoller.PlayerInputButtons.Jump))
        {
           
            _isPlayerJump = true;
        }
        else
        {
            _isPlayerJump = false;
        }

        if (pressedButton.WasReleased(PreviousInput, LocalInputPoller.PlayerInputButtons.Mouse0) && _shootCooldown.ExpiredOrNotRunning(Runner))
        {
            _shootCooldown = TickTimer.CreateFromSeconds(Runner, _timeBetweenShots);
            _isPlayerFiring = true;


        }
        else
            _isPlayerFiring = false;

        if (pressedButton.WasReleased(PreviousInput, LocalInputPoller.PlayerInputButtons.Sprint) && !_isPlayerFiring && !_isPlayerCrouch && (input.VerticalInput != 0 || input.HorizontalInput != 0))
        {
            if (_isPlayerSprint) return;
            _isPlayerSprint = true;
        }
        else
            _isPlayerSprint = false;

        if (pressedButton.WasPressed(PreviousInput, LocalInputPoller.PlayerInputButtons.Hotkey1))
        {
            _runtimeAnimatorIndex = 1;
        }
        else if (pressedButton.WasReleased(PreviousInput, LocalInputPoller.PlayerInputButtons.Hotkey2))
        {
            _runtimeAnimatorIndex = 0;
        }


        //Debug.Log("jump: " + _isPlayerJump);
        PreviousInput = input.NetworkButtons;
    }

    private void UpdateCharacterAnimations(PlayerInputData input)
    {
        if (!Object.HasStateAuthority) return;

        if (!_isPlayerCrouch)
        {
            _isPlayerMoveForward = input.VerticalInput > 0 && input.HorizontalInput == 0;
            _isPlayerMoveBackwards = input.VerticalInput < 0;
            _isPlayerMoveRightForward = input.HorizontalInput > 0;
            _isPlayerMoveLeftForward = input.HorizontalInput < 0;
            
        }
        else
        {
            _isPlayerCrouchWalkBackwards = _isPlayerCrouch && input.VerticalInput < 0;
            _isPlayerCrouchWalk = _isPlayerCrouch && input.VerticalInput > 0;
            _isPlayerCrouchWalkRight = _isPlayerCrouch && input.HorizontalInput > 0;
            _isPlayerCrouchWalkLeft = _isPlayerCrouch && input.HorizontalInput < 0;
        }
       
       
        
    }

    private static void OnAnimationStateChange(Changed<CharacterAnimationController> changed)
    {

        if (!changed.Behaviour._isPlayerCrouch)
        {
            if (changed.Behaviour._isPlayerMoveForward)
                changed.Behaviour.PlayCharacterAnimations("isMoveForward", true);
            else
                changed.Behaviour.PlayCharacterAnimations("isMoveForward", false);

            if (changed.Behaviour._isPlayerMoveBackwards)
                changed.Behaviour.PlayCharacterAnimations("isMoveBackwards", true);
            else
                changed.Behaviour.PlayCharacterAnimations("isMoveBackwards", false);

            if (changed.Behaviour._isPlayerMoveLeftForward)
                changed.Behaviour.PlayCharacterAnimations("isMoveRunLeftForward", true);
            else
                changed.Behaviour.PlayCharacterAnimations("isMoveRunLeftForward", false);


            if (changed.Behaviour._isPlayerMoveRightForward)
                changed.Behaviour.PlayCharacterAnimations("isMoveRightForward", true);
            else
                changed.Behaviour.PlayCharacterAnimations("isMoveRightForward", false);
        }
        
        if (changed.Behaviour._isPlayerCrouch)
        {
            changed.Behaviour.PlayCharacterAnimations("isCrouch", true);
        }
        else 
        {
            changed.Behaviour.PlayCharacterAnimations("isCrouch", false);
        }

        if (changed.Behaviour._isPlayerCrouchWalk && changed.Behaviour._isPlayerCrouch)
            changed.Behaviour.PlayCharacterAnimations("isCrouchWalk", true);
        else
            changed.Behaviour.PlayCharacterAnimations("isCrouchWalk", false);

        if (changed.Behaviour._isPlayerCrouchWalkLeft && changed.Behaviour._isPlayerCrouch)
            changed.Behaviour.PlayCharacterAnimations("isCrouchWalkLeft", true);
        else
            changed.Behaviour.PlayCharacterAnimations("isCrouchWalkLeft", false);

        if (changed.Behaviour._isPlayerCrouchWalkRight && changed.Behaviour._isPlayerCrouch)
            changed.Behaviour.PlayCharacterAnimations("isCrouchWalkRight", true);
        else
            changed.Behaviour.PlayCharacterAnimations("isCrouchWalkRight", false);

        if (changed.Behaviour._isPlayerCrouchWalkBackwards && changed.Behaviour._isPlayerCrouch)
            changed.Behaviour.PlayCharacterAnimations("isCrouchWalkBacwards", true);
        else
            changed.Behaviour.PlayCharacterAnimations("isCrouchWalkBacwards", false);

        if(changed.Behaviour._isPlayerSprint)
            changed.Behaviour.PlayCharacterAnimations("isPlayerSprint", true); 
        else
            changed.Behaviour.PlayCharacterAnimations("isPlayerSprint", false);



        if (changed.Behaviour._isPlayerJump == true)
             changed.Behaviour.PlayTriggerAnimations("isJump");
        
           

        if (changed.Behaviour._isPlayerSlide == true)
            changed.Behaviour.PlayTriggerAnimations("isSliding");

        if(changed.Behaviour._isPlayerFiring == true)
            changed.Behaviour.PlayTriggerAnimations("isFiring");
    }

    private void PlayCharacterAnimations(string animState, bool condition)
    {
        _characterAnimator.SetBool(animState, condition);
    }

    private void PlayCharacterSprintAnimation(bool state)
    {
        _isPlayerSprint = state;
    }

    private void PlayTriggerAnimations(string value)
    {
        _characterAnimator.SetTrigger(value);
    }

  

  


  
}
