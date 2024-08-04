using System;
using UnityEngine;
using Fusion;
public static class EventLibrary
{
    
    //Main Menu Events
    public static readonly GameEvent OnPlayerCreateNickname = new GameEvent();
    public static readonly GameEvent<bool> OnPlayerJoinedServer = new GameEvent<bool>();
    public static readonly GameEvent<string> OnJoinButtonClicked = new GameEvent<string>();
    public static readonly GameEvent OnPasswordPanelRequested = new GameEvent();
    public static readonly GameEvent<bool> OnTeamSelected = new GameEvent<bool>();
    public static readonly GameEvent<NetworkString<_16>> OnTeamsUpdate = new GameEvent<NetworkString<_16>>();

    //FPS && Character animation events 
    public static readonly GameEvent<bool> OnPlayerWalking = new GameEvent<bool>();
    public static readonly GameEvent OnPlayerFiring = new GameEvent(); 
    public static readonly GameEvent OnPlayerRunning = new GameEvent();
    public static readonly GameEvent<bool> OnPlayerActivateADS = new GameEvent<bool>();
    public static readonly GameEvent<bool> OnPlayerRun = new GameEvent<bool>();
    public static readonly GameEvent OnPlayerADSShoot = new GameEvent();
    public static readonly GameEvent OnPlayerReloading = new GameEvent();
    public static readonly GameEvent OnPlayerCrouch = new GameEvent();
    public static readonly GameEvent OnPlayerJump = new GameEvent();

    //Player
    public static readonly GameEvent<float> OnPlayerGetHit = new GameEvent<float>();
    public static readonly GameEvent<PlayerRef> OnPlayerRespawn = new GameEvent<PlayerRef>();
    public static readonly GameEvent<bool> OnPlayerDiedLocal = new GameEvent<bool>();
    public static readonly GameEvent OnHealthRegen = new GameEvent();
    public static readonly GameEvent<PlayerRef> OnPlayerSelectTeam = new GameEvent<PlayerRef>();
    public static readonly GameEvent<bool> OnATMDisable = new GameEvent<bool>();
    public static readonly GameEvent OnPlayerDiedNetwork = new GameEvent();
    public static readonly GameEvent OnMoneyDropNeeded = new GameEvent();

    //weaponVFX
    public static readonly GameEvent OnGunFired = new GameEvent();
    public static readonly GameEvent<Vector3> OnBulletHit = new GameEvent<Vector3>();

    //Economy Events
    public static readonly GameEvent<float> OnPlayerCollectMoney = new GameEvent<float>();
    public static readonly GameEvent<bool> OnPlayerReachATM = new GameEvent<bool>();

    //Weapon
    public static readonly GameEvent<int> OnPlayerSwitchWeapon = new GameEvent<int>();
    public static readonly GameEvent<int> OnMagazineUpdated = new GameEvent<int>();

    //Gameloop Events
    public static readonly GameEvent<GameManager.GameState> OnGameStateChange = new GameEvent<GameManager.GameState>();


    //debug
    public static readonly GameEvent<string> DebugMessage = new GameEvent<string>();

    //HealthVisuals
    public static readonly GameEvent<bool> OnPlayerHealthDecrease = new GameEvent<bool>();




}
