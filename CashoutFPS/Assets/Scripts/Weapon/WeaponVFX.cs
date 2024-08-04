using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class WeaponVFX : NetworkBehaviour
{
  
    [SerializeField] private ParticleSystem _bulletChasing;
    [SerializeField] private ParticleSystem _muzzleFlash;
   
    private void OnEnable()
    {
        EventLibrary.OnGunFired.AddListener(PlayGunVFX);
    }

    private void OnDisable()
    {
        EventLibrary.OnGunFired.RemoveListener(PlayGunVFX);
     }


    
    private void PlayGunVFX()
    {
        if (!Object.HasInputAuthority) return;
        
            _muzzleFlash.Play();
            _bulletChasing.Play();
        
      
    }

  
}
