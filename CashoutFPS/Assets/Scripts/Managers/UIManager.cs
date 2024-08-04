using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;
using System;
using DG.Tweening;

public class UIManager : NetworkBehaviour,IGameStateListener
{
    public static event Action<PlayerRef, PlayerInfo> OnPlayerSelectionsEnded;
   
    [Networked(OnChanged = nameof(OnTeamCollectedMoneyChange))] private float _redTeamCollectedMoney { get; set; }
    [Networked(OnChanged = nameof(OnTeamCollectedMoneyChange))] private float _blueTeamCollectedMoney { get; set; }
    public GameManager.GameState CurrentGameState { get; set; }

    private PlayerStats.Team _playerTeam;
    private float _tempAmount;
    private Slider[] _teamSliderBar;

    [SerializeField] private GameObject _scoreBoard;
    [SerializeField] private Text _magazineUI;
    
    [SerializeField] private GameObject _respawnPanel;
    [SerializeField] private Button _respawnButton;
    [Header("TeamSelectPanel")]
    #region TeamSelectPanel
    [SerializeField] private Button _assultOperatorButton;
    [SerializeField] private Button _redPantersButton;
    [SerializeField] private Button _seaHorseSquadButton;
    [SerializeField] private GameObject _selectTeamPanel;
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private Transform _redButton;
    [SerializeField] private GameObject _playerNamePrefab;
    [SerializeField] private Slider[] _teamBar;
    #endregion
  
    [Header("Killfeed Variables")]
    [SerializeField] private Transform _killfeedContext;
    [SerializeField] private Color[] _teamColors;
    [SerializeField] private GameObject _killfeed;
    private GameObject _spawnedKillFeed;


    [SerializeField] private TextMeshProUGUI _matchStartCounter;
    private PlayerInfo _playerInfo;
    private float _matchStartCountAmount = 4f;
   

    private void OnEnable()
    {
        EventLibrary.OnGameStateChange.AddListener(UpdateGameState);
        EventLibrary.OnMagazineUpdated.AddListener(UpdateMagazineUI);
        EventLibrary.OnPlayerDiedLocal.AddListener(ShowOrHideRespawnPanel);
        PlayerInteraction.OnPlayerSendMoney += RPC_UpdateTeamCollectedMoney;
        PlayerHealth.OnKillfeedReady += RPC_ShowKillfeed;
        PlayerHUDManager.Test += ShowScoreBoard;

       
    }
    private void OnDisable()
    {
        EventLibrary.OnGameStateChange.AddListener(UpdateGameState);
        EventLibrary.OnMagazineUpdated.RemoveListener(UpdateMagazineUI);
        EventLibrary.OnPlayerDiedLocal.RemoveListener(ShowOrHideRespawnPanel);
        PlayerInteraction.OnPlayerSendMoney -= RPC_UpdateTeamCollectedMoney;
        PlayerHealth.OnKillfeedReady -= RPC_ShowKillfeed;
        PlayerHUDManager.Test -= ShowScoreBoard;

    }

    private void Awake()
    {
        _respawnButton.onClick.AddListener(OnRespawnButtonClicked);
        _redPantersButton.onClick.AddListener(() => UpdateSelectTeam(PlayerStats.Team.RedPanters));
        _seaHorseSquadButton.onClick.AddListener(() => UpdateSelectTeam(PlayerStats.Team.Squadrons));
        _assultOperatorButton.onClick.AddListener(() => UpdateOperator(OperatorProperties.OperatorType.Assult));
        

    }


    public override void Spawned()
    {
         _respawnPanel.SetActive(false);
        _selectTeamPanel.SetActive(true);
    }

    private void UpdateMagazineUI(int currentBulletCount)
    {
         //_magazineUI.text = currentBulletCount.ToString();
    }

    private void ShowOrHideRespawnPanel(bool active)
    {
       _respawnPanel.SetActive(active);
        
    }

    private void UpdateSelectTeam(PlayerStats.Team selectedTeam)
    {
        _playerInfo.PlayerTeam = selectedTeam;
        EventLibrary.OnTeamSelected?.Invoke(true);
       
    }

    private void UpdateOperator(OperatorProperties.OperatorType selectedOperator)
    {
       _playerInfo.PlayerOperator = selectedOperator;
       _playerInfo.PlayerName = PlayerPrefs.GetString("Nickname");
       OnPlayerSelectionsEnded.Invoke(Runner.LocalPlayer, _playerInfo);
       EventLibrary.OnTeamSelected?.Invoke(false);
       _selectTeamPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnRespawnButtonClicked()
    {
        ShowOrHideRespawnPanel(false);
        Cursor.lockState = CursorLockMode.Locked;
        EventLibrary.OnPlayerRespawn?.Invoke(Runner.LocalPlayer);
    }

    public void UpdateGameState(GameManager.GameState currentGameState)
    {
        switch (currentGameState)
        {
            case GameManager.GameState.Warmup:
                HideOrEnableTeamBar(false);
                break;
            case GameManager.GameState.MatchStart:
                //HideOrEnableTeamBar(true);
                break;
            case GameManager.GameState.Match:
                HideOrEnableTeamBar(true);
                break;
             
        } 
       
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpdateTeamCollectedMoney(float amount, PlayerStats.Team team)
    {
       _playerTeam = team;

       if(team == PlayerStats.Team.RedPanters)
          _redTeamCollectedMoney += amount;
       else
        _blueTeamCollectedMoney += amount;
        
        EventLibrary.DebugMessage?.Invoke("amount: " + _redTeamCollectedMoney + " Team: " + team);
        StartCoroutine(Delay());
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ShowKillfeed(PlayerStats.Team playerTeam, string killerPlayer, string deadPlayer)
    {
       _spawnedKillFeed = GameObject.Instantiate(_killfeed, _killfeedContext);
       TextMeshProUGUI[] playerNames = _spawnedKillFeed.GetComponentsInChildren<TextMeshProUGUI>();
       playerNames[0].text = killerPlayer;
       playerNames[1].text = deadPlayer;
       if(playerTeam == PlayerStats.Team.RedPanters)
       {
            playerNames[0].color = _teamColors[1];
            playerNames[1].color = _teamColors[0];
       }
       else
       {
            playerNames[0].color = _teamColors[0];
            playerNames[1].color = _teamColors[1];
            
       }

        StartCoroutine(HideKillfeed());
            
    }

    private static void OnTeamCollectedMoneyChange(Changed<UIManager> changed)
    {

       changed.Behaviour.ChangeTeamsSliderBar();
       
    }


    private void ChangeTeamsSliderBar()
    {
      
        var amount = 0f;
        var barIndex = 0;

        if (_playerTeam == PlayerStats.Team.RedPanters)
        {
           
            barIndex = 0;
            amount = _redTeamCollectedMoney;
        }
        else
        {
            barIndex = 1;
            amount = _blueTeamCollectedMoney;
        }

        DOTween.To(() => _teamBar[barIndex].value, x => _teamBar[barIndex].value = x, amount, 1f);

    }

    private void HideOrEnableTeamBar(bool condition)
    {
        for (int i = 0; i < _teamBar.Length; i++)
            _teamBar[i].gameObject.SetActive(condition);
    }

    private void ShowScoreBoard(bool condition)
    {
        _scoreBoard.SetActive(condition);
    }

   private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);
        EventLibrary.DebugMessage?.Invoke(" ");
   }

    private IEnumerator HideKillfeed()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(_spawnedKillFeed);
        _spawnedKillFeed = null;

    }

}
