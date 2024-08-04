using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class VFXManager : NetworkBehaviour
{
    [SerializeField] private ParticleSystem _bulletVFX;
    [SerializeField] private List<NetworkPrefabRef> _impactVFX;
   
  
   
    private void OnEnable()
    {
      //Ak74m.OnBulletHit += RPC_PlayBulletParticle;
        Weapon.OnBulletHit += RPC_PlayBulletParticle;
    }

    private void OnDisable()
    {
       //Ak74m.OnBulletHit -= RPC_PlayBulletParticle;
        Weapon.OnBulletHit -= RPC_PlayBulletParticle;
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PlayBulletParticle(Vector3 pos, string objectTag)
    {
      
       var index = 0;
        switch (objectTag)
        {
            case "Floor":
                 index = 0;
                break;
             case "Metal":
                index = 1;
                break;
            case "Player":
                index = 2;
                break;
            case "Wood":
                index = 3;
                break;
        }
       
        Runner.Spawn(_impactVFX[index], pos);
    }


 
}
