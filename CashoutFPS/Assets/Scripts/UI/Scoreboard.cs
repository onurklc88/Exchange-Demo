using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Scoreboard : NetworkBehaviour
{
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private GameObject _playerStatsEntry;
    [SerializeField] private Transform[] _content;
    private List<GameObject> _entryList = new List<GameObject>();
    [SerializeField] private Color[] _teamColors;
  

    private void OnEnable()
    {
        GetPlayerList();
    }

    private void OnDisable()
    {
        ClearScoreboard();
    }


    
    private void GetPlayerList()
    {
        for (int i = 0; i < _playerSpawner.ActivePlayerList.Count; i++)
        {
            if (Runner.TryGetPlayerObject(_playerSpawner.ActivePlayerList[i], out var playerNetworkObject))
            {
                var playerTeam = playerNetworkObject.gameObject.GetComponent<PlayerStats>().PlayerInfo.PlayerTeam;
                var contentIndex = 0;
                
                if (playerTeam == PlayerStats.Team.RedPanters)
                     contentIndex = 0;
                else
                    contentIndex = 1;
                
                GameObject entry = GameObject.Instantiate(_playerStatsEntry, _content[contentIndex]);
                entry.gameObject.GetComponent<Image>().color = _teamColors[contentIndex];
               
                
                for (int j = 0; j < entry.GetComponentsInChildren<TextMeshProUGUI>().Length; j++)
                {
                   switch (j)
                    {
                        case 0:
                            entry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerNetworkObject.gameObject.GetComponent<PlayerStats>().PlayerInfo.PlayerName.ToString();
                            break;
                        case 1:
                            entry.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerNetworkObject.gameObject.GetComponent<PlayerStats>().PlayerInfo.PlayerKillCount.ToString();
                            break;
                        case 2:
                            entry.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = playerNetworkObject.gameObject.GetComponent<PlayerStats>().PlayerInfo.PlayerDieCount.ToString();
                            break;
                        case 3:
                             entry.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = playerNetworkObject.gameObject.GetComponent<PlayerStats>().PlayerInfo.CollectedTotalMoney.ToString();
                            break;

                   }
                }

                _entryList.Add(entry);
            }
        }
        SortEntriesByScores();
    }

    private void SortEntriesByScores()
    {
       
        _entryList = _entryList.OrderByDescending(entry => {
            string killCountString = entry.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
            float killCount = float.Parse(killCountString);
            return killCount;
        }).ToList();

        for (int i = 0; i < _entryList.Count; i++)
             _entryList[i].transform.SetSiblingIndex(i);
        
    }

    private void ClearScoreboard()
    {
       for(int i = 0; i < _entryList.Count; i++)
        {
           Destroy(_entryList[i].transform.gameObject);
       }
        _entryList.Clear();
    }
    
}
