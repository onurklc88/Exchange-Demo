using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.UI;
using TMPro;

public class FindGamePanel : MonoBehaviour
{
    [SerializeField] private Transform _sessionListContent;
    [SerializeField] private GameObject _sessionEntryPrefab;
    [SerializeField] private Button _refreshButton;
    [SerializeField] private TMP_InputField _joincode;
    [SerializeField] private Button _joinByCodeButton;
   
    private void Awake()
    {
        _refreshButton.onClick.AddListener(RefreshSessionList);
        _joincode.onValueChanged.AddListener(OnInputValueChange);
        _joinByCodeButton.onClick.AddListener(JoinByArgRoom);
    }

    private void RefreshSessionListContent()
    {
        foreach (Transform child in _sessionListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (SessionInfo session in NetworkRunnerController.GameSessions)
        {
            if (session.IsVisible)
            {
                GameObject entry = GameObject.Instantiate(_sessionEntryPrefab, _sessionListContent);
                SessionEntry script = entry.GetComponent<SessionEntry>();
                script.RoomNameText.text = session.Name;
                script.PlayerCountText.text = session.PlayerCount + "/" + session.MaxPlayers;
                script.SessionInfo = session;
                if (session.IsOpen == false || session.PlayerCount >= session.MaxPlayers)
                    script.JoinButton.interactable = false;
                else
                    script.JoinButton.interactable = true;
            }
        }
     }

    private void OnInputValueChange(string arg0)
    {
        _joinByCodeButton.interactable = arg0.Length >= 5;
    }

    private void JoinByArgRoom()
    {
        EventLibrary.OnJoinButtonClicked?.Invoke(_joincode.text);
    }

   
    private void RefreshSessionList()
    {
        StartCoroutine(RefreshWait());
    }
    private IEnumerator RefreshWait()
    {
        _refreshButton.interactable = false;
        RefreshSessionListContent();
        yield return new WaitForSeconds(3f);
        _refreshButton.interactable = true;

    }


   
}
