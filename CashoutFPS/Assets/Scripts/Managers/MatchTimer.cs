using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using System;

public class MatchTimer : NetworkBehaviour, IGameStateListener
{

   
    [Networked] private TickTimer _matchTimer { get; set; }
    public GameManager.GameState CurrentGameState { get; set; }

    private const float WARMUP_MATCH_TIME = 60f;
    private const float MATCH_TIME_AMOUNT = 600f;

    private float _currentTimeAmount;
    private string _gameStateText;
    private float _matchStartCountdown = 4f;
    [SerializeField] private TextMeshProUGUI _timerText;
   
    private void OnEnable()
    {
        EventLibrary.OnGameStateChange.AddListener(UpdateGameState);
    }

    private void OnDisable()
    {
        EventLibrary.OnGameStateChange.RemoveListener(UpdateGameState);
    }


   
    public override void FixedUpdateNetwork()
    {
        if(_matchTimer.Expired(Runner) == false && _matchTimer.RemainingTime(Runner).HasValue && CurrentGameState != GameManager.GameState.MatchStart)
        {
            var timeSpan = TimeSpan.FromSeconds(_matchTimer.RemainingTime(Runner).Value);
            var output = $"{_gameStateText}{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            _timerText.text = output;
        }
        else if (_matchTimer.Expired(Runner))
        {
            _matchTimer = TickTimer.None;
            if(CurrentGameState == GameManager.GameState.Warmup)
            {
                CurrentGameState = GameManager.GameState.MatchStart;
                EventLibrary.OnGameStateChange?.Invoke(CurrentGameState);
                StartCoroutine(DelayEvents());
            }
        }
    }

    public void UpdateGameState(GameManager.GameState currentGameState)
    {
        CurrentGameState = currentGameState;
        switch (CurrentGameState)
        {
            case GameManager.GameState.Warmup:
                _currentTimeAmount = WARMUP_MATCH_TIME;
                _gameStateText = " warm up  ";
                break;
            case GameManager.GameState.MatchStart:
                _gameStateText = "MATCH WILL BE START IN ";
                StartCoroutine(StartMatchCountdown());
                break;
            case GameManager.GameState.Match:
                _currentTimeAmount = MATCH_TIME_AMOUNT;
                _gameStateText = " countdown   ";
                break;
           
        }

        _matchTimer = TickTimer.CreateFromSeconds(Runner, _currentTimeAmount);
    }

  
    private IEnumerator DelayEvents()
    {
        
        yield return new WaitForSeconds(5f);
         CurrentGameState = GameManager.GameState.Match;
        EventLibrary.OnGameStateChange?.Invoke(CurrentGameState);
        _currentTimeAmount = MATCH_TIME_AMOUNT;
    }


    private IEnumerator StartMatchCountdown()
    {

        while (_matchStartCountdown > 0)
        {
            yield return new WaitForSeconds(1f); 

            _matchStartCountdown -= 1f; 
           _timerText.text = _gameStateText + _matchStartCountdown.ToString();
           
        }
    }

    private static void OnGameStateTextChange(Changed<MatchTimer> changed)
    {

    }

}
