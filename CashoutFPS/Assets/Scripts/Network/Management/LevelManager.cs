using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
public class LevelManager : NetworkBehaviour, IGameStateListener
{
    public GameManager.GameState CurrentGameState { get; set; }
    private Dictionary<ATM, List<NetworkObject>> _side = new Dictionary<ATM, List<NetworkObject>>();
    [SerializeField] private List<NetworkObject> _aSideMoneyCase;
    [SerializeField] private List<NetworkObject> _bSideMoneyCase;
    [SerializeField] private List<ATM> _atm;
    private int _currentATMindex;
    private const float MAX_MONEY_AMOUNT_FOR_SIDE_MONEY = 15000f;
    private const float MIN_MONEY_AMOUNT_FOR_SIDE_MONEY = 5000f;
    private const float TOTAL_TIME_COUNT = 10f;
    private float _currentTime;

    public override void Spawned()
    {
        _side.Add(_atm[0], _aSideMoneyCase);
        _side.Add(_atm[1], _bSideMoneyCase);
    }


    private void OnEnable()
    {
        EventLibrary.OnGameStateChange.AddListener(UpdateGameState);
    }

    private void OnDisable()
    {
        EventLibrary.OnGameStateChange.RemoveListener(UpdateGameState);
    }

    public void UpdateGameState(GameManager.GameState currentGameState)
    {
        CurrentGameState = currentGameState;
        switch (currentGameState)
        {
            case GameManager.GameState.Match:
                ActivateRandomATM();
                break;
        }
    }



    public override void FixedUpdateNetwork()
    {
        if (CurrentGameState == GameManager.GameState.Match)
        {
            if (_currentTime > 0)
            {
                _currentTime -= Runner.DeltaTime;
               
            }
            else
            {
                ActivateRandomATM();
            }
        }
    }

    private void ActivateRandomATM()
    {
        _currentTime = TOTAL_TIME_COUNT;
        _currentATMindex = UnityEngine.Random.Range(0, 2);
        _atm[_currentATMindex].UpdateATMState(true);
        _atm[1 - _currentATMindex].UpdateATMState(false);


        foreach (NetworkObject item in _side[_atm[_currentATMindex]])
        {
            item.GetComponent<MoneyCase>().MoneyAmount = MIN_MONEY_AMOUNT_FOR_SIDE_MONEY;
        }

        foreach (NetworkObject item in _side[_atm[1 - _currentATMindex]])
        {
            item.GetComponent<MoneyCase>().MoneyAmount = MAX_MONEY_AMOUNT_FOR_SIDE_MONEY;
        }
    }

   
  
}
