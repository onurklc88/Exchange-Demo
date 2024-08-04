using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cashout;
using DG.Tweening;
public class MoneyBag : NetworkBehaviour, IInteractable
{
   
    [SerializeField] private Outline _outline;
    [Networked] public float LootMoneyAmount { get; set; }



    public override void Spawned()
    {
       StartCoroutine(StopDelay());
    }


    public void HighlightObject(bool condition)
    {
        _outline.enabled = condition;
    }


    private IEnumerator StopDelay()
    {
        yield return new WaitForSeconds(2f);
        transform.GetComponent<NetworkRigidbody>().Rigidbody.isKinematic = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_MoveMoneyBag(Vector3 playerPos) 
    {
        
        transform.GetComponent<Collider>().isTrigger = true;
        transform.DOMove(playerPos, 0.3f).OnComplete(() =>
        {
            Runner.Despawn(GetComponent<NetworkObject>());
        }); ;

        
    }


}
