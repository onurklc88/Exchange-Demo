using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;




public class Weapon : NetworkBehaviour
{
    public static event Action<Vector3, string> OnBulletHit;
    [Networked] protected TickTimer ShootCooldown { get; set; }
    [Range(0, 7f)] [SerializeField] protected float RecoilAmountY;
    [Range(0, 3f)] [SerializeField] protected float RecoilAmountX;
    [SerializeField] protected Animator WeaponAnimation;
    [SerializeField] protected Camera FPSCamera;
    [SerializeField] protected Transform FirePoint;
    [SerializeField] protected LayerMask Colliders;
    protected WeaponProperties WeaponProperties;
    protected WeaponSway WeaponSwayController;
    protected PlayerController PlayerController;
    protected int MagazineCapacity;
    protected int CurrentBulletCount;
    protected float TimeBetweenShots;
    protected float TargetFOV;
    protected bool IsReloading = false;
    protected Weapon WeaponType;
    protected Vector3 BulletHitPoint;
    protected string CollidedObjectTag;
    private void Awake()
    {
        WeaponSwayController = GetComponent<WeaponSway>();
        PlayerController = GetComponentInParent<PlayerController>();
        InitWeapon();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

    protected void RaiseHitEvent()
    {
        OnBulletHit?.Invoke(BulletHitPoint, CollidedObjectTag);
    }
    protected virtual void InitWeapon() { }

    protected virtual void IncreaseCameraFOV() { }
    protected virtual void FireWeapon() { }
    protected virtual void ApplyRecoil() { }
    protected virtual void UpdateFPSAnimator() { }
    protected virtual void ReloadWeapon() { }
    protected virtual void UpdateGunVFX() { }
    protected virtual float CalculateGivenDamage(float distance, int hitboxIndex)
    {
        float givenDamage = 0;

        switch (hitboxIndex)
        {
            case 0:
                givenDamage = WeaponProperties.WeaponLegDamageValue;
                break;
            case 1:
                givenDamage = WeaponProperties.WeaponLegDamageValue;
                break;
            case 2:
                givenDamage = WeaponProperties.WeaponBodyDamageValue;
                break;
            case 3:
                givenDamage = WeaponProperties.WeaponHeadDamageValue;
                break;
        }

        if (distance > WeaponProperties.WeaponRangeDistance)
            return givenDamage * 0.25f;
        else
            return givenDamage;

    }

   
}
