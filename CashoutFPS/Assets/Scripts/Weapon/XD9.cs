using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;


public class XD9 : Weapon, IReadInput
{
   
    [Networked] public NetworkButtons PreviousInput { get; set; }
    [Networked(OnChanged = nameof(ExecuteShootingEvents))] private int _currentBulletCount { get; set; }
   
    [SerializeField] private GameObject _debugObject;
  
   
    private float _currentRecoilXPosition;
    private float _currentRecoilYPosition;
    private float _timePressed;
    private bool _reloading = false;
    protected override void InitWeapon()
    {
        base.WeaponType = this;
        base.WeaponProperties.WeaponHeadDamageValue = 50f;
        base.WeaponProperties.WeaponBodyDamageValue = 15f;
        base.WeaponProperties.WeaponLegDamageValue = 10f;
        base.WeaponProperties.MagazineCapacity = 16;
        base.WeaponProperties.WeaponRangeDistance = 15f;
        TimeBetweenShots = 0.1f;
    }


    private void OnEnable()
    {
        if (Object != null)
        {
            if (!Object.HasInputAuthority) return;
            EventLibrary.OnMagazineUpdated?.Invoke(_currentBulletCount);

            if (_reloading)
            {
                if (Object.HasInputAuthority)
                    EventLibrary.OnPlayerReloading?.Invoke();

                ReloadWeapon();
            }
        }

    }

    private void Start()
    {
       _currentBulletCount = WeaponProperties.MagazineCapacity;
       if (Object.HasStateAuthority) return;
            EventLibrary.OnMagazineUpdated?.Invoke(_currentBulletCount);
    }


  
    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerInputData>(Object.InputAuthority, out var input))
        {
            ReadInputs(input);
        }
    }
    public void ReadInputs(PlayerInputData input)
    {
        var pressedButton = input.NetworkButtons.GetPressed(PreviousInput);


        if(pressedButton.WasPressed(PreviousInput, LocalInputPoller.PlayerInputButtons.Mouse0) && ShootCooldown.ExpiredOrNotRunning(Runner) && !pressedButton.WasReleased(PreviousInput, LocalInputPoller.PlayerInputButtons.Mouse1))
        {  
           
            TimeBetweenShots = 0.12f;
            FireWeapon();
        }
        else
        {
             TimeBetweenShots = 0.15f;
        }

        if (pressedButton.WasReleased(PreviousInput, LocalInputPoller.PlayerInputButtons.Mouse1) && ShootCooldown.ExpiredOrNotRunning(Runner) && !pressedButton.IsSet(LocalInputPoller.PlayerInputButtons.Mouse0))
        {
            FireWeapon();
        }

        if (pressedButton.WasPressed(PreviousInput, LocalInputPoller.PlayerInputButtons.Reload))
        {
            if (_currentBulletCount == 16) return;

            ReloadWeapon();
        }

       PreviousInput = input.NetworkButtons;
    }
  

    protected override void FireWeapon()
    {
        if (_currentBulletCount > 0)
        {

            ShootCooldown = TickTimer.CreateFromSeconds(Runner, TimeBetweenShots);
            

            Runner.LagCompensation.Raycast(FirePoint.transform.position, FirePoint.transform.forward, 150f, Object.InputAuthority, out var hit, Colliders, HitOptions.IncludePhysX);
           
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

    private static void ExecuteShootingEvents(Changed<XD9> changed)
    {
        changed.Behaviour.UpdateBulletCount();
        if (changed.Behaviour._currentBulletCount == 16) return;
        changed.Behaviour.ApplyRecoil();
        changed.Behaviour.UpdateFPSAnimator();
        changed.Behaviour.UpdateGunVFX();
    }

    protected override void ApplyRecoil()
    {
        _currentRecoilXPosition = ((UnityEngine.Random.value - .5f) / 2) * RecoilAmountX;
        _currentRecoilYPosition = ((UnityEngine.Random.value - .5f) / 2) * (_timePressed >= 2 ? RecoilAmountY / 4 : RecoilAmountY);
        PlayerController.VerticalCamRotation -= Mathf.Abs(_currentRecoilYPosition);
        PlayerController.MouseXPosition -= _currentRecoilXPosition;
    }

    private void UpdateBulletCount()
    {
        if (!Object.HasInputAuthority) return;
        EventLibrary.OnMagazineUpdated?.Invoke(_currentBulletCount);
    }

    protected override void UpdateFPSAnimator()
    {
        if (!Object.HasInputAuthority) return;
        EventLibrary.OnPlayerFiring?.Invoke();
    }

    protected override void UpdateGunVFX()
    {
        if (!Object.HasInputAuthority) return;
        var isLocalPlayer = Runner.LocalPlayer == Object.HasInputAuthority;

        if (isLocalPlayer)
        {
            EventLibrary.OnGunFired?.Invoke();
            base.RaiseHitEvent();
        }
    }

    private IEnumerator DelayReload()
    {
        var tempCount = _currentBulletCount;
        yield return new WaitForSeconds(1.2f);
        _reloading = false;
        if (tempCount == _currentBulletCount)
            _currentBulletCount = WeaponProperties.MagazineCapacity;
    }

    private void OnDrawGizmos()
    {
        Vector3 forward = FirePoint.transform.TransformDirection(Vector3.forward) * 150f;
        Debug.DrawRay(FirePoint.transform.position, forward, Color.green);
    }

}
