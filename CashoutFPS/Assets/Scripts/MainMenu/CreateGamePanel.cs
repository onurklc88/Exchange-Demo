using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Fusion;

public class CreateGamePanel : MonoBehaviour
{
    public static event Action<GameMode, string, string, string> OnPlayerCreateGame;
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] private TMP_InputField _joincodeInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private Button _startGameButton;
   
    private void OnEnable()
    {
       _joincodeInput.text = RandomStringGenerator(7);
    }

    private void Awake()
    {
        _startGameButton.onClick.AddListener(HostGame);
    }
    private void Start()
    {
        _roomNameInput.text = PlayerPrefs.GetString("Nickname") + " 's Lobby";
    }

    string RandomStringGenerator(int lenght)
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        string generated_string = "";

        for (int i = 0; i < lenght; i++)
            generated_string += characters[UnityEngine.Random.Range(0, 36)];

        return generated_string;
    }

    private void HostGame()
    {
       EventLibrary.OnPlayerJoinedServer?.Invoke(true);
       OnPlayerCreateGame?.Invoke(GameMode.Host, _roomNameInput.text, _joincodeInput.text, _passwordInput.text);
    }



}
