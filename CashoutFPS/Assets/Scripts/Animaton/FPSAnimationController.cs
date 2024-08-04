using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FPSAnimationController : MonoBehaviour
{
     private Animator _weaponAnimator;
    private float _velocity;
    private void OnEnable()
    {
        EventLibrary.OnPlayerWalking.AddListener(PlayIdleAnimaton);
        EventLibrary.OnPlayerFiring.AddListener(PlayFireAnimation);
        EventLibrary.OnPlayerRun.AddListener(PlayRunAnimation);
        EventLibrary.OnPlayerActivateADS.AddListener(PlayADSAnimation);
        EventLibrary.OnPlayerADSShoot.AddListener(PlayADSShootAnimation);
        EventLibrary.OnPlayerReloading.AddListener(PlayReloadAnimation);
    }

    private void OnDisable()
    {
        EventLibrary.OnPlayerWalking.RemoveListener(PlayIdleAnimaton);
        EventLibrary.OnPlayerFiring.RemoveListener(PlayFireAnimation);
        EventLibrary.OnPlayerRun.RemoveListener(PlayRunAnimation);
        EventLibrary.OnPlayerADSShoot.RemoveListener(PlayADSShootAnimation);
        EventLibrary.OnPlayerReloading.RemoveListener(PlayReloadAnimation);
    }


    private void Start()
    {
        _weaponAnimator = GetComponent<Animator>();
    }
    private void PlayIdleAnimaton(bool action)
    {
       
        _weaponAnimator.SetBool("IsWalking", action);
    }
    
    private void PlayFireAnimation()
    {
      
        if (_weaponAnimator == null) return;
        
        _weaponAnimator.SetTrigger("Fire");

    }

    private void PlayRunAnimation(bool action)
    {
        if(_weaponAnimator == null) return;
        _weaponAnimator.SetBool("IsRunning", action);
       
    }

    private void PlayADSAnimation(bool action)
    {
        if (_weaponAnimator == null) return;
        if (action)
        {
            _weaponAnimator.SetBool("IsADSActive", false);
            _weaponAnimator.Play("Rig_AK_ADS");
        }
        else
        {
           _weaponAnimator.SetBool("IsADSActive", true);
        }
      
    }

    private void PlayADSShootAnimation()
    {
        if (_weaponAnimator == null) return;
        /*
        var local = Object.HasInputAuthority;
        if (local)
        {
            this._weaponAnimator.SetTrigger("ADSFire");
        }
        */
        _weaponAnimator.SetTrigger("ADSFire");
    }

    private void PlayReloadAnimation()
    {
        if (_weaponAnimator == null)
        {
            _weaponAnimator = GetComponent<Animator>();
        }
        _weaponAnimator.SetTrigger("Reload");
    }

   
}
