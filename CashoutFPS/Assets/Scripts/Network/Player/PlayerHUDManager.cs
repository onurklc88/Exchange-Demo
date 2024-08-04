
using UnityEngine;
using Fusion;
using TMPro;
using System;
public class PlayerHUDManager : NetworkBehaviour, IReadInput
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _debugTest;
    [SerializeField] private TextMeshProUGUI _collectedMoneyText;
    [SerializeField] private GameObject _ATMBar;
    [SerializeField] private TextMeshProUGUI _atmText;
    [SerializeField] private string[] _atmString;
    [SerializeField] private GameObject _scoreboard;
    public static event Action<bool> Test;

    public NetworkButtons PreviousInput { get; set; }

    private void OnEnable()
    {
        EventLibrary.OnPlayerGetHit.AddListener(ShowHealth);
        EventLibrary.DebugMessage.AddListener(PrintDebug);
        EventLibrary.OnPlayerCollectMoney.AddListener(UpdateCollectedMoneyUI);
        EventLibrary.OnATMDisable.AddListener(DisableATMContext);
        PlayerInteraction.OnPlayerBetweenATM += ShowATMText;
    }

    private void OnDisable()
    {
        EventLibrary.OnPlayerGetHit.RemoveListener(ShowHealth);
        EventLibrary.DebugMessage.RemoveListener(PrintDebug);
        EventLibrary.OnPlayerCollectMoney.RemoveListener(UpdateCollectedMoneyUI);
        PlayerInteraction.OnPlayerBetweenATM -= ShowATMText;
        EventLibrary.OnATMDisable.RemoveListener(DisableATMContext);

    }

    private void ShowHealth(float damage)
    {
         _healthText.text = damage.ToString();
    }

    private void PrintDebug(string debug)
    {
        _debugTest.text = debug;
    }

    private void UpdateCollectedMoneyUI(float moneyAmount)
    {
        var value = Mathf.Ceil(moneyAmount);
        _collectedMoneyText.text = value.ToString();
    }

   

    private void ShowATMText(bool action ,bool condition)
    {
        if (action)
        {

            if (condition)
            {
                _atmText.text = _atmString[0];
            }
            else
            {
                _atmText.text = _atmString[1];
            }
        }
        else
        {
            _atmText.text = " ";
        }
       
        
    }


    private void DisableATMContext(bool activeState)
    {
        if (!activeState)
            _atmText.text = " ";
        
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerInputData>(Object.InputAuthority, out var input))
        {
            ReadInputs(input);
        }
    }


    private void ShowScoreboard(bool condition)
    {
        if(Object.HasInputAuthority)
            Test?.Invoke(condition);
    }

    public void ReadInputs(PlayerInputData input)
    {
        var pressedButton = input.NetworkButtons.IsSet(LocalInputPoller.PlayerInputButtons.Tab);
        ShowScoreboard(pressedButton);
    }
}
