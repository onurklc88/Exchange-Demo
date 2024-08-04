using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateListener
{
    public GameManager.GameState CurrentGameState { get; set; }

    public void UpdateGameState(GameManager.GameState currentGameState);
}
