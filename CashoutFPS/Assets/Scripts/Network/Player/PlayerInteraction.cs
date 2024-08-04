using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
public class PlayerInteraction : NetworkBehaviour, IReadInput
{
    public static Action<float, PlayerStats.Team> OnPlayerSendMoney;
    public static Action<bool, bool> OnPlayerBetweenATM;
    public NetworkButtons PreviousInput { get; set; }
    private NetworkObject _collidedObject;
    private PlayerStats _stats;
    public override void Spawned()
    {
        _stats = transform.GetComponent<PlayerStats>();
    }



    public void ReadInputs(PlayerInputData input)
    {
        var pressedButton = input.NetworkButtons.GetPressed(PreviousInput);
        if (_collidedObject == null) return;
         
           
            if (pressedButton.WasPressed(PreviousInput, LocalInputPoller.PlayerInputButtons.Interact))
            {
                if(_collidedObject.transform.GetComponent<MoneyCase>() != null)
                {
                     CollectMoney();
                }
                else if(_collidedObject.transform.GetComponent<ATM>() != null)
                {
                     DepositMoney();
                }
                else if(_collidedObject.transform.GetComponent<MoneyBag>() != null)
                {
                    RPC_LootMoney();
                }
                    
              
            }
       
            
        PreviousInput = input.NetworkButtons;
    }
    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerInputData>(Object.InputAuthority, out var input))
        {
            ReadInputs(input);
        }
    }


   
    private void OnTriggerEnter(Collider other)
    {
     
        if (other.transform.gameObject.layer == 8)
        {
            if (!Object.HasInputAuthority) return;
            _collidedObject = other.transform.GetComponent<NetworkObject>();
            var moneyCase = other.transform.gameObject.GetComponent<MoneyCase>();
            moneyCase.HighlightObject(true);
            moneyCase.ShowContext(CheckIfMoneyIsAvaible(moneyCase));
        }
        else if(other.transform.gameObject.layer == 9)
        {
            if (!Object.HasInputAuthority) return;
            var ATM = other.transform.gameObject.GetComponent<ATM>();
            ATM.HighlightObject(true);
            OnPlayerBetweenATM?.Invoke(true, ATM.IsATMActive);
            _collidedObject = other.transform.GetComponent<NetworkObject>();
        }
        else if(other.transform.gameObject.layer == 11)
        {
            if (!Object.HasInputAuthority) return;
            _collidedObject = other.transform.GetComponent<NetworkObject>();
            var moneyBag = other.transform.gameObject.GetComponent<MoneyBag>();
            moneyBag.HighlightObject(true);
        }
        

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.layer == 8)
        {
            if (!Object.HasInputAuthority) return;
            other.transform.gameObject.GetComponent<MoneyCase>().HighlightObject(false);
        }
  
        if (other.transform.gameObject.layer == 9)
        {
            if (!Object.HasInputAuthority) return;
            other.transform.gameObject.GetComponent<ATM>().HighlightObject(false);
            OnPlayerBetweenATM?.Invoke(false, other.transform.gameObject.GetComponent<ATM>().IsATMActive);
        }
        
        if (other.transform.gameObject.layer == 11)
        {
            if (!Object.HasInputAuthority) return;
           other.GetComponent<MoneyBag>().HighlightObject(false);
        }
        
        _collidedObject = null;
    }

    private void DepositMoney()
    {
         StartCoroutine(Delay());
       
        var collectedMoney = _stats.PlayerCollectedMoney;
        if (_stats.PlayerCollectedMoney < 0 || !_collidedObject.transform.GetComponent<ATM>().IsATMActive) return;
        OnPlayerSendMoney?.Invoke(collectedMoney, _stats.PlayerInfo.PlayerTeam);
        _stats.RPC_UpdateDepositAmount(collectedMoney);
        _stats.RPC_UpdatePlayerCollectedMoney(-collectedMoney);
       
        EventLibrary.OnPlayerReachATM?.Invoke(false);
        if (!Object.HasInputAuthority) return;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);
        EventLibrary.DebugMessage?.Invoke(" ");

    }

    private void CollectMoney()
    {
        var moneyCase = _collidedObject.GetComponent<MoneyCase>();
        moneyCase.RPC_MoveToPlayer();
        transform.GetComponent<PlayerStats>().RPC_UpdatePlayerCollectedMoney(moneyCase.MoneyAmount);
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_LootMoney()
    {
       var moneyBag = _collidedObject.GetComponent<MoneyBag>();
       _stats.RPC_UpdatePlayerCollectedMoney(moneyBag.LootMoneyAmount);
       moneyBag.GetComponent<MoneyBag>().RPC_MoveMoneyBag(transform.position);

    }

    private bool CheckIfMoneyIsAvaible(MoneyCase obj)
    {
        if (transform.GetComponent<PlayerStats>().PlayerInfo.PlayerTeam == obj.SpecialTeam || obj.SpecialTeam == PlayerStats.Team.None)
            return true;
        else
            return false;
    }
    

}
