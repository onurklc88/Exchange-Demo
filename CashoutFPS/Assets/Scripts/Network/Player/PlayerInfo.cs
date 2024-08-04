using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public struct PlayerInfo : INetworkStruct
{
    public NetworkString<_16> PlayerName { get; set; }
    public PlayerStats.Team PlayerTeam;
    public OperatorProperties.OperatorType PlayerOperator;
    public int PlayerKillCount;
    public int PlayerDieCount;
    public float CollectedTotalMoney;
}
