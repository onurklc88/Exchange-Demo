using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System;
public class TeamButton : NetworkBehaviour
{
    [SerializeField] private PlayerStats.Team _teamButton;
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private GameObject _playerNameEntry;
    [SerializeField] private Transform _content;
    private const int MAX_PLAYER_COUNT_FOR_TEAMS = 5;
    private int _currentPlayerCount;

    public override void Spawned()
    {
        GetPlayerList();
        _currentPlayerCount = 0;
    }

    
    private void GetPlayerList()
    {
        for(int i = 0; i < _playerSpawner.ActivePlayerList.Count; i++)
        {
                if (Runner.TryGetPlayerObject(_playerSpawner.ActivePlayerList[i], out var playerNetworkObject))
                {
                   if(_currentPlayerCount == MAX_PLAYER_COUNT_FOR_TEAMS){ GetComponent<Button>().interactable = false; return;}

                   if(playerNetworkObject.gameObject.GetComponent<PlayerStats>().PlayerInfo.PlayerTeam == _teamButton)
                   {
                        GameObject entry = GameObject.Instantiate(_playerNameEntry, _content);
                        var name = playerNetworkObject.gameObject.GetComponent<PlayerStats>().PlayerInfo.PlayerName;
                        entry.GetComponentInChildren<TextMeshProUGUI>().text = name.ToString();
                        _currentPlayerCount++;
                   }
                }
        }
    }
    

    

}
