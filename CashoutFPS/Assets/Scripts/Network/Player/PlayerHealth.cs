using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using TMPro;



public class PlayerHealth : NetworkBehaviour, IDamageable, IGameStateListener
{
    public static event Action<PlayerStats.Team, string, string> OnKillfeedReady;
    public static event Action<bool, float> OnDamageEffectNeeded;
   
    [SerializeField] private OperatorProperties _operatorProperties;
    
    [Networked(OnChanged = nameof(NetworkedHealthChanged))] private float _currentHealth { get; set; }
  
    [Networked(OnChanged = nameof(NetworkedPlayerStateChange))] private NetworkBool _isPlayerDead { get; set; }
    public GameManager.GameState CurrentGameState { get ; set; }

    [SerializeField] private Camera _fpsCamera;
    private PlayerRef _playerRef;
    private bool _damageTaken;
    private float _lastDamageValue;
    private float _regenTimer = 5f;
    private float _lastHealth;
  
  
    public override void Spawned()
    {
         _currentHealth = _operatorProperties.TotalHealth;
    }

   

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ReduceHealth(float damageValue, PlayerRef player)
    {
        _playerRef = player;
        _lastDamageValue = damageValue;
        _currentHealth -= _lastDamageValue;
       

        if (_currentHealth > 0)
             _damageTaken = true;
        else
        {
            transform.GetComponent<PlayerController>().IsPlayerDead = true;
            _isPlayerDead = true;
        }
          

    }

    public override void FixedUpdateNetwork()
    {
        EventLibrary.DebugMessage?.Invoke("RegenTimer " + _regenTimer);
        if (_damageTaken)
        {
            RegenTimer();
        }
        else
        {
            IncreasePlayerHealth();
        }

     }
   

    private static void NetworkedHealthChanged(Changed<PlayerHealth> changed)
    {
         changed.Behaviour.UpdatePlayerUI();
        if (changed.Behaviour._currentHealth > 0)
             changed.Behaviour._lastHealth = changed.Behaviour._currentHealth;
    }


    private static void NetworkedPlayerStateChange(Changed<PlayerHealth> change)
    {
        if (change.Behaviour._isPlayerDead)
            change.Behaviour.KillPlayer();
    }

    private void RegenTimer()
    {
        if(_currentHealth == _lastHealth && _currentHealth > 0)
        {
            if (_regenTimer >= 0)
               _regenTimer -= Runner.DeltaTime;
            else
              _damageTaken = false;
        }
        else
        {
           _regenTimer = 5f;
        }
       
    }

    private void UpdatePlayerUI()
    {
       
        var isLocalPlayer = Runner.LocalPlayer == Object.HasInputAuthority;
        
        if (isLocalPlayer)
        {
            EventLibrary.OnPlayerGetHit?.Invoke(_currentHealth);

            if (_currentHealth + 2f > _lastHealth)
            {
               OnDamageEffectNeeded?.Invoke(false, _lastDamageValue);
            }
            else
            {
                OnDamageEffectNeeded?.Invoke(true, _lastDamageValue);
            }
        }


    }

 
    private void IncreasePlayerHealth()
    {

        if (_currentHealth < _operatorProperties.TotalHealth)
        {
           _currentHealth = Mathf.Ceil((Mathf.Lerp(_currentHealth, _operatorProperties.TotalHealth, Runner.DeltaTime * 0.01f)));
        }
        else
        {
            _regenTimer = 5f;
        }
    }


   

    private void KillPlayer()
    {
        var stats = transform.GetComponent<PlayerStats>();
        if (Runner.TryGetPlayerObject(_playerRef, out var playerNetworkObject))
        {
            playerNetworkObject.transform.GetComponent<PlayerStats>().RPC_UpdateKillCount();
            var enemyStats = playerNetworkObject.transform.GetComponent<PlayerStats>();
            var enemyName = enemyStats.PlayerInfo.PlayerName;
            var playerName = stats.PlayerInfo.PlayerName;
            var playerTeam = stats.PlayerInfo.PlayerTeam;
            OnKillfeedReady?.Invoke(playerTeam, playerName.ToString(), enemyName.ToString());
            
        }

        if (Object.HasInputAuthority)
        {
            
            transform.GetComponent<PlayerStats>().RPC_UpdateDieCount();
           
            _fpsCamera.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            EventLibrary.OnPlayerDiedLocal?.Invoke(true);
            
            // if(CurrentGameState == GameManager.GameState.Match)
            EventLibrary.OnPlayerDiedNetwork?.Invoke();
            EventLibrary.OnMoneyDropNeeded?.Invoke();
        }
    }

    public void UpdateGameState(GameManager.GameState currentGameState)
    {
        CurrentGameState = currentGameState;
    }
}
