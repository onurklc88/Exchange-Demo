using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System;
public class PlayerSpawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft, IGameStateListener
{
    [Networked, Capacity(10)]
    public NetworkLinkedList<PlayerRef> ActivePlayerList { get; } = new NetworkLinkedList<PlayerRef>();
    //public GameManager.GameState CurrentGameState { get; set; }


    [Networked(OnChanged = nameof(OnGameStateChange))] public GameManager.GameState CurrentGameState { get; set; }

    [SerializeField] private NetworkPrefabRef _playerNetworkPrefab = NetworkPrefabRef.Empty;

    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject[] _redTeamSpawnPositions;
    [SerializeField] private Transform[] _blueTeamSpawnPositions;
    [SerializeField] private Transform[] _randomSpawnPositions;




    private void OnEnable()
    {
        EventLibrary.OnPlayerRespawn.AddListener(RPC_RespawnPlayer);
        UIManager.OnPlayerSelectionsEnded += RPC_SpawnPlayer;
        EventLibrary.OnGameStateChange.AddListener(UpdateGameState);
    }

    private void OnDisable()
    {
        EventLibrary.OnPlayerRespawn.RemoveListener(RPC_RespawnPlayer);
        UIManager.OnPlayerSelectionsEnded -= RPC_SpawnPlayer;
        EventLibrary.OnGameStateChange.RemoveListener(UpdateGameState);
    }

    public void PlayerJoined(PlayerRef player)
    {
       
    }
    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }

  


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SpawnPlayer(PlayerRef playerRef, PlayerInfo playerInfo)
    {
       
        if (Runner.IsServer)
        {
            var spawnpoint = _redTeamSpawnPositions[0].transform.position; 
            var playerObject = Runner.Spawn(_playerNetworkPrefab, spawnpoint, Quaternion.identity, playerRef);
            playerObject.gameObject.GetComponent<PlayerStats>().RPC_SetPlayerInfo(playerInfo);
            ActivePlayerList.Add(playerRef);
            Runner.SetPlayerObject(playerRef, playerObject);
            CheckGameState(ActivePlayerList.Count);
        }
    }

    private void DespawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            var isPlayerAlreadySpawned = Runner.GetPlayerObject(playerRef);
            if (isPlayerAlreadySpawned)
            {
               
                if(Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
                {
                    ActivePlayerList.Remove(playerRef);
                    Runner.Despawn(playerNetworkObject);
                }
            }
        }

    }

    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_RespawnPlayer(PlayerRef playerRef)
    {
            var isPlayerAlreadySpawned = Runner.GetPlayerObject(playerRef);
            if (isPlayerAlreadySpawned)
            {

                if (Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
                {
                    var playerStats = playerNetworkObject.transform.GetComponent<PlayerStats>().PlayerInfo;
                    Runner.Despawn(playerNetworkObject);
                   Vector3 spawnPoint = Vector3.zero;
                   if(CurrentGameState == GameManager.GameState.Warmup)
                   {
                     spawnPoint = _randomSpawnPositions[UnityEngine.Random.Range(0, 10)].transform.position;
                   }
                   else
                   {
                     if(playerStats.PlayerTeam == PlayerStats.Team.RedPanters)
                        spawnPoint = _redTeamSpawnPositions[UnityEngine.Random.Range(0, 10)].transform.position;
                     else
                        spawnPoint = _redTeamSpawnPositions[UnityEngine.Random.Range(0, 10)].transform.position;
                   }
                  
                    var playerObject = Runner.Spawn(_playerNetworkPrefab, spawnPoint, Quaternion.identity, playerRef);
                    playerObject.transform.GetComponent<PlayerStats>().RPC_SetPlayerInfo(playerStats);
                    EventLibrary.OnPlayerDiedLocal?.Invoke(false);
                    Runner.SetPlayerObject(playerRef, playerObject);
                }

            }
    }



    private void CheckGameState(int playerCount)
    {
        if(playerCount == Runner.SessionInfo.MaxPlayers)
        {
            EventLibrary.OnGameStateChange?.Invoke(GameManager.GameState.Match);
        }
       

    }

    public void UpdateGameState(GameManager.GameState currentGameState)
    {
        CurrentGameState = currentGameState;
        if (currentGameState == GameManager.GameState.Match)
            SetupPlayerPositions();
    }

    private static void OnGameStateChange(Changed<PlayerSpawner> changed)
    {
        if(changed.Behaviour.CurrentGameState == GameManager.GameState.Match)
        {
           changed.Behaviour.SetupPlayerPositions();
        }
    }

  

    private void SetupPlayerPositions()
    {
        int currentRedTeamPlayerCount = 0;
        int currentBlueTeamPlayerCount = 0;

        for (int i = 0; i < ActivePlayerList.Count; i++)
        {
            
            if (Runner.TryGetPlayerObject(ActivePlayerList[i], out var playerNetworkObject))
            {
                var playerTeam = playerNetworkObject.GetComponent<PlayerStats>().PlayerInfo.PlayerTeam;
                if(playerTeam  == PlayerStats.Team.RedPanters)
                {
                    var spawnPoint = _redTeamSpawnPositions[currentRedTeamPlayerCount].transform.position;
                    playerNetworkObject.transform.position = spawnPoint;
                    currentRedTeamPlayerCount++;
                }
                else
                {
                    var spawnPoint = _blueTeamSpawnPositions[currentRedTeamPlayerCount].transform.position;
                    playerNetworkObject.transform.position = _blueTeamSpawnPositions[currentBlueTeamPlayerCount].transform.position;
                    currentBlueTeamPlayerCount++;
                }

            }
        }
    }
}
