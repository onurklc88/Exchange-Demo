using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerStatsManager : NetworkBehaviour
{
    public static PlayerStatsManager Instance;
    [Networked, Capacity(10)]
    public NetworkLinkedList<PlayerInfo> PlayerStatsList { get; } = new NetworkLinkedList<PlayerInfo>();
    public PlayerInfo PlayerInfo { get; set; }
   
   

    public override void Spawned()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {

            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetPlayerInfo(PlayerInfo playerinfo)
    {
        PlayerInfo = playerinfo;
        PlayerStatsList.Add(PlayerInfo);
    }
}
