using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class NetworkRunnerController : MonoBehaviour, INetworkRunnerCallbacks
{
    public static List<SessionInfo> GameSessions = new List<SessionInfo>();
    [SerializeField] private NetworkRunner _networkRunner;
    private NetworkRunner _networkRunnerInstance;
   
   
    private void OnEnable()
    {
        EventLibrary.OnPlayerCreateNickname.AddListener(ConnectLobby);
        EventLibrary.OnJoinButtonClicked.AddListener(JoinGame);
        CreateGamePanel.OnPlayerCreateGame += CreateGame;
    }

    private void OnDisable()
    {
        EventLibrary.OnPlayerCreateNickname.RemoveListener(ConnectLobby);
        EventLibrary.OnJoinButtonClicked.RemoveListener(JoinGame);
        CreateGamePanel.OnPlayerCreateGame -= CreateGame;
    }

    
   
    private void Start()
    {
        if (PlayerPrefs.HasKey("Nickname"))
        {
            EventLibrary.OnPlayerJoinedServer?.Invoke(true);
            ConnectLobby();
        }
    }

    public async void ConnectLobby()
    {
        if (_networkRunnerInstance == null)
        {
            _networkRunnerInstance = gameObject.AddComponent<NetworkRunner>();
            //_networkRunnerInstance.gameObject.AddComponent<ObjectPoolManager>();
        }
           
       
        var result = await _networkRunnerInstance.JoinSessionLobby(SessionLobby.Shared);
       
        if (result.Ok)
          EventLibrary.OnPlayerJoinedServer?.Invoke(false);
    }

    private async void CreateGame(GameMode mode, string roomName, string joinCode, string password)
    {
        Dictionary<string, SessionProperty> sessionProperties = new Dictionary<string, SessionProperty>();
        sessionProperties.Add("SessionPassword", password);
        sessionProperties.Add("JoinCode", joinCode);
        _networkRunnerInstance.AddCallbacks(this);
        _networkRunnerInstance.ProvideInput = true;
        var startGameArgs = new StartGameArgs()
        {
            GameMode = mode, 
            SessionName = roomName,
            SessionProperties = sessionProperties,
            PlayerCount = 10,
            SceneManager = _networkRunnerInstance.GetComponent<INetworkSceneManager>(),
            //ObjectPool = _networkRunnerInstance.GetComponent<ObjectPoolManager>()
        };
        var result = await _networkRunnerInstance.StartGame(startGameArgs);
        if (result.Ok)
        {
            const string SCENE_NAME = "TestScene";
            _networkRunnerInstance.SetActiveScene(SCENE_NAME);
            EventLibrary.OnPlayerJoinedServer?.Invoke(false);
        }
    }
    private async void JoinGame(string joinCode)
    {
        Dictionary<string, SessionProperty> sessionProperties = new Dictionary<string, SessionProperty>();
        sessionProperties.Add("JoinCode", joinCode);
        _networkRunnerInstance.AddCallbacks(this);
        _networkRunnerInstance.ProvideInput = true;
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionProperties = sessionProperties,
            PlayerCount = 10,
            SceneManager = _networkRunnerInstance.GetComponent<INetworkSceneManager>(),
            //ObjectPool = _networkRunnerInstance.GetComponent<ObjectPoolManager>()
        };

        var result = await _networkRunnerInstance.StartGame(startGameArgs);
        if (result.Ok)
        {
            const string SCENE_NAME = "TestScene";
            _networkRunnerInstance.SetActiveScene(SCENE_NAME);
        }
        else
        {
            Debug.LogError("PLayer Cannot Joined");
        }
    }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Session List Updated");
        GameSessions.Clear();
        GameSessions = sessionList;
    }


    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
    {
       
    }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
       
    }

    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
    {
        
    }

    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
       
    }

    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
      
    }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
     
    }

    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
    {
       
    }

    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
    {
      
    }

    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
       
    }
}
