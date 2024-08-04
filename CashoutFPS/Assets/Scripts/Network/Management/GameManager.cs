using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour
{
    public enum GameState
    {
        None,
        Warmup,
        MatchStart,
        Match,
        MatchEnd
    }

    public GameState _currentGameState;
  
    public override void Spawned()
    {
       
        _currentGameState = GameState.Warmup;
        EventLibrary.OnGameStateChange?.Invoke(_currentGameState);
       
    }


  

}
