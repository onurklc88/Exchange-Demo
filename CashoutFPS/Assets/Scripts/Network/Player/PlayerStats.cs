using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerStats : NetworkBehaviour
{
    public enum Team
    {
        None,
        RedPanters,
        Squadrons
   }
    [Networked] public PlayerInfo PlayerInfo { get; set; }
    [Networked(OnChanged = nameof(OnPlayerInfoChange))] public bool _isPlayerSpawned { get; set; }
    [Networked(OnChanged = nameof(OnPlayerMoneyUpdated))] public float PlayerCollectedMoney { get; set; }
    [SerializeField] private Color[] _colors;
    [SerializeField] private GameObject[] _teamBadges;
    [SerializeField] private NetworkPrefabRef _moneyBag;
    [SerializeField] private Transform _bagPos;
    private PlayerInfo _updatedPlayerInfo;
    private float _totalDepositAmount;

    private void OnEnable()
    {
       //PlayerHealth.OnMoneyDrop += RPC_Test;
       EventLibrary.OnMoneyDropNeeded.AddListener(RPC_DropCollectedMoney);
    }

    private void OnDisable()
    {
       //PlayerHealth.OnMoneyDrop -= RPC_Test;
       EventLibrary.OnMoneyDropNeeded.RemoveListener(RPC_DropCollectedMoney);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetPlayerInfo(PlayerInfo playerInfo)
    {
        _updatedPlayerInfo = playerInfo;
        PlayerInfo = _updatedPlayerInfo;
        
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdateDieCount()
    {
        _updatedPlayerInfo.PlayerDieCount += 1;
        PlayerInfo = _updatedPlayerInfo;
    }

    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdateKillCount()
    {
        _updatedPlayerInfo.PlayerKillCount += 1;
        PlayerInfo = _updatedPlayerInfo;
        Check();
    }
    

    public override void Spawned()
    {
        _isPlayerSpawned = true;
    }



    private static void OnPlayerInfoChange(Changed<PlayerStats> changed)
    {
        changed.Behaviour.SendDebug();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_DropCollectedMoney()
    {
      
        if (PlayerCollectedMoney > 0)
        {
            var obj = Runner.Spawn(_moneyBag, _bagPos.transform.position, Quaternion.identity);
            obj.GetComponent<MoneyBag>().LootMoneyAmount = PlayerCollectedMoney;
            obj.GetComponent<NetworkRigidbody>().Rigidbody.AddForce(_bagPos.transform.forward * 50f, ForceMode.Impulse);
        }
            
        
      
    }

   
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdatePlayerCollectedMoney(float money)
    {
         PlayerCollectedMoney += money;

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdateDepositAmount(float money)
    {
        _totalDepositAmount += Mathf.Ceil(money);
        _updatedPlayerInfo.CollectedTotalMoney = _totalDepositAmount;
        PlayerInfo = _updatedPlayerInfo;
    }


    private static void OnPlayerMoneyUpdated(Changed<PlayerStats> changed)
    {
        changed.Behaviour.UpdateMoneyText();
    }


    private void UpdateMoneyText()
    {
      
        var isLocalPlayer = Runner.LocalPlayer == Object.HasInputAuthority;

        if (isLocalPlayer)
        {
            EventLibrary.OnPlayerCollectMoney?.Invoke(PlayerCollectedMoney);
        }
    }

    private void SendDebug()
    {
      
            for (int i = 0; i < _teamBadges.Length; i++)
            {
                if (PlayerInfo.PlayerTeam == Team.RedPanters)
                    _teamBadges[i].GetComponent<MeshRenderer>().materials[0].color = _colors[0];
                else
                    _teamBadges[i].GetComponent<MeshRenderer>().materials[0].color = _colors[1];
            }
        
            
       
       
        var isLocalPlayer = Runner.LocalPlayer == Object.HasInputAuthority;

        if (isLocalPlayer)
        {
            if (Object.HasInputAuthority)
            {
                EventLibrary.DebugMessage.Invoke("Playerteam: " + PlayerInfo.PlayerTeam + " PlayerName: " + PlayerInfo.PlayerName + " PlayerOperator: " + PlayerInfo.PlayerName + "PlayerDieCount: " +PlayerInfo.PlayerDieCount + "PlayerKillCount: " +PlayerInfo.PlayerKillCount);
                EventLibrary.OnTeamsUpdate?.Invoke(PlayerInfo.PlayerName);
            }

        }
    }

    private void Check()
    {
        EventLibrary.DebugMessage.Invoke("Playerteam: " + PlayerInfo.PlayerTeam + " PlayerName: " + PlayerInfo.PlayerName + " PlayerOperator: " + PlayerInfo.PlayerName + " PlayerDieCount: " + PlayerInfo.PlayerDieCount + " PlayerKillCount: " + PlayerInfo.PlayerKillCount);
    }

   
    


  

}
