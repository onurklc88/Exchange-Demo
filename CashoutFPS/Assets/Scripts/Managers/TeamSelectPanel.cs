using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;
public class TeamSelectPanel : NetworkBehaviour
{
    [SerializeField] private Button _redPantersButton;
    [SerializeField] private Button _seaHorseSquadButton;
    [SerializeField] private GameObject _selectTeamPanel;

    private void Awake()
    {
        _redPantersButton.onClick.AddListener(SelectRedPanters);
        _redPantersButton.onClick.AddListener(DisableTeamSelectPanel);
    }


    private void SelectRedPanters()
    {
        EventLibrary.OnPlayerSelectTeam?.Invoke(Runner.LocalPlayer);
    }
    private void Start()
    {
       
    }

    private void DisableTeamSelectPanel()
    {
        _selectTeamPanel.SetActive(false);
    }


}
