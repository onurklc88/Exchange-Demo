using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;



public class Ak74m : Weapon, IReadInput
{
    //public static event Action<Vector3, string> OnBulletHit;
    [Networked(OnChanged = nameof(OnADSActived))] private bool _isADSActive { get; set; }
    [Networked] public NetworkButtons PreviousInput { get; set; }
    [Networked] private NetworkButtons _previousADSInput { get; set; }
  
    [Networked(OnChanged = nameof(ExecuteShootingEvents))] private int _currentBulletCount { get; set; }

    [SerializeField] private GameObject _debugObject;
 
    private float _ADSTimer = 0f;
    private float _incraseRate = 0.5f;
    private bool _isRunKeyActive = false;
    private float _currentRecoilXPosition;
    private float _currentRecoilYPosition;
    private float _maxRecoilTime = 4;
    private float _timePressed;
    private bool _reloading = false;
   
   
    protected override void InitWeapon()
    {
        base.WeaponType = this;
        base.WeaponProperties.MagazineCapacity = 30;
        base.WeaponProperties.WeaponRangeDistance = 40f;
        base.WeaponProperties.WeaponHeadDamageValue = 100f;
        base.WeaponProperties.WeaponBodyDamageValue = 30f;
        base.WeaponProperties.WeaponLegDamageValue = 20f;
        TimeBetweenShots = 0.15f;
        TargetFOV = 21f;
       
    }


    private void OnEnable()
    {
       if(Object != null)
       {
            if (!Object.HasInputAuthority) return;
               EventLibrary.OnMagazineUpdated?.Invoke(_currentBulletCount);

            if (_reloading)
            {
                if(Object.HasInputAuthority)
                    EventLibrary.OnPlayerReloading?.Invoke();
                ReloadWeapon();
            }
       }
       
    }


    private void Start()
    {
        _currentBulletCount = WeaponProperties.MagazineCapacity;
    }


    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerInputData>(Object.InputAuthority, out var input))
        {
            ReadInputs(input);
            CheckADSInput(input);
        }
    }

   

    public void ReadInputs(PlayerInputData input)
    {
        var pressedButton = input.NetworkButtons.GetPressed(PreviousInput);
        _isRunKeyActive = input.NetworkButtons.IsSet(LocalInputPoller.PlayerInputButtons.Sprint);

       

        if (pressedButton.WasReleased(PreviousInput, LocalInputPoller.PlayerInputButtons.Mouse1))
        {
            
            if(!_isRunKeyActive)
              IncreaseCameraFOV();
        }
        


        if (pressedButton.WasReleased(PreviousInput, LocalInputPoller.PlayerInputButtons.Mouse0) && ShootCooldown.ExpiredOrNotRunning(Runner))
        {
            FireWeapon();
        }
      

         if(pressedButton.WasPressed(PreviousInput, LocalInputPoller.PlayerInputButtons.Reload))
         {
            ReloadWeapon();
         }

       

        PreviousInput = input.NetworkButtons;
    }

    private void CheckADSInput(PlayerInputData input)
    {
        var pressedButton = input.NetworkButtons.GetPressed(_previousADSInput);
        var isPlayerHoldingMouse1Button = pressedButton.IsSet(LocalInputPoller.PlayerInputButtons.Mouse1);
        if (isPlayerHoldingMouse1Button)
            _isADSActive = true;
        else
            _isADSActive = false;
         
    }

    private static void OnADSActived(Changed<Ak74m> changed)
    {
        var currentState = changed.Behaviour._isADSActive;
        changed.LoadOld();
        var oldState = changed.Behaviour._isADSActive;
        if (oldState != currentState)
        {
            changed.Behaviour.AimSightDown(currentState);
        }
    }

    private void AimSightDown(bool active)
    {
        if (!Object.HasInputAuthority) return;

        if (active)
        {
            WeaponSwayController.swayClamp = 0.001f;
            EventLibrary.OnPlayerActivateADS?.Invoke(true);
        }
        else
        {
            WeaponSwayController.swayClamp = 0.05f;
            _ADSTimer = 0f;
            StartCoroutine(ChangeCameraFOV(16f));
            EventLibrary.OnPlayerWalking?.Invoke(false);
            EventLibrary.OnPlayerActivateADS?.Invoke(false);
        }
    }

    protected override void IncreaseCameraFOV()
    {
        if (FPSCamera.focalLength >= TargetFOV) return;
       
        if(_ADSTimer < 2f)
        {
            _ADSTimer += Runner.DeltaTime;
            FPSCamera.focalLength = Mathf.Lerp(FPSCamera.focalLength, TargetFOV, _ADSTimer / 2f);
        }
      
    }

    private IEnumerator ChangeCameraFOV(float targetValue)
    {
        float timeElapsed = 0;
        while(timeElapsed < 1f)
        {
            FPSCamera.focalLength = Mathf.Lerp(FPSCamera.focalLength, targetValue, timeElapsed / 1f);
            timeElapsed += Runner.DeltaTime;
            yield return null;
        }
    }

   
    protected override void FireWeapon()
    {
        if (_currentBulletCount > 0)
        {
            if (_isADSActive)
                FirePoint.transform.localRotation = Quaternion.Euler(88.639f, -90f, -89.985f);
            else
                FirePoint.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            ShootCooldown = TickTimer.CreateFromSeconds(Runner, TimeBetweenShots);
           
           
            Runner.LagCompensation.Raycast(FirePoint.transform.position, FirePoint.transform.forward, 150f, player: Object.InputAuthority, out var hit, Colliders, HitOptions.IncludePhysX);
            
            if (hit.Hitbox != null)
            {
                if (Object.HasInputAuthority)
                {
                    var collidedObject = hit.Hitbox.GetComponentInParent<NetworkObject>();
                    var damage = CalculateGivenDamage(hit.Distance, hit.Hitbox.HitboxIndex);
                    if (hit.Hitbox.GetComponentInParent<PlayerController>() != null)
                    {
                        var didHitOwnHitbox = collidedObject.GetComponentInParent<PlayerController>().Object.InputAuthority.PlayerId == Object.InputAuthority.PlayerId;
                        
                        if (!didHitOwnHitbox)
                            collidedObject.GetComponent<IDamageable>().RPC_ReduceHealth(damage, Runner.LocalPlayer);
                    }
                }
            }
            _currentBulletCount -= 1;
            if (hit.GameObject != null)
            {
                BulletHitPoint = hit.Point;
                CollidedObjectTag = hit.GameObject.transform.tag;
            }
           
        
        }
        else
            ReloadWeapon();
    }

    private static void ExecuteShootingEvents(Changed<Ak74m> changed)
    {
        changed.Behaviour.UpdateBulletCount();
        if (changed.Behaviour._currentBulletCount == changed.Behaviour.WeaponProperties.MagazineCapacity) return;
       
            changed.Behaviour.ApplyRecoil();
            changed.Behaviour.UpdateFPSAnimator();
            changed.Behaviour.UpdateGunVFX();
       
       

    }

    private void UpdateBulletCount()
    {
        if (!Object.HasInputAuthority) return;
        EventLibrary.OnMagazineUpdated?.Invoke(_currentBulletCount);
    }
    protected override void UpdateFPSAnimator()
    {
      if (!Object.HasInputAuthority) return;
       
        if (_isADSActive)
        {
           EventLibrary.OnPlayerADSShoot?.Invoke();
        }
        else if (_currentBulletCount > 0)
        {
            EventLibrary.OnPlayerFiring?.Invoke();
        }
       
    }

    protected override void ReloadWeapon()
    {
        if (_currentBulletCount == WeaponProperties.MagazineCapacity) return;
       
        if (Object.HasInputAuthority && !_reloading)
        {
            EventLibrary.OnPlayerReloading?.Invoke();
        }
        _reloading = true;
        StartCoroutine(DelayReload());
    }

    protected override void UpdateGunVFX()
    {
      
        if (!Object.HasInputAuthority) return;
        var isLocalPlayer = Runner.LocalPlayer == Object.HasInputAuthority;

        if (isLocalPlayer)
        {
            EventLibrary.OnGunFired?.Invoke();
            //OnBulletHit?.Invoke(BulletHitPoint, CollidedObjectTag);
            base.RaiseHitEvent();
        }
    }


    private IEnumerator DelayReload()
    {
        var tempCount = _currentBulletCount;
        yield return new WaitForSeconds(3f);
        _reloading = false;
        if (tempCount == _currentBulletCount)
            _currentBulletCount = WeaponProperties.MagazineCapacity;
    }



    protected override void ApplyRecoil()
    {
        _currentRecoilXPosition = ((UnityEngine.Random.value - .5f) / 2) * RecoilAmountX;
        _currentRecoilYPosition = ((UnityEngine.Random.value - .5f) / 2) * (_timePressed >= 2 ? RecoilAmountY / 4 : RecoilAmountY);
        PlayerController.VerticalCamRotation -= Mathf.Abs(_currentRecoilYPosition);
        PlayerController.MouseXPosition -= _currentRecoilXPosition;
    }

   
    private void OnDrawGizmos()
    {
        Vector3 forward = FirePoint.transform.TransformDirection(Vector3.forward) * 150f;
        Debug.DrawRay(FirePoint.transform.position, forward, Color.green);
    }
}
