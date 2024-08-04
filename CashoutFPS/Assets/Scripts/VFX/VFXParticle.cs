using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class VFXParticle : NetworkBehaviour
{
    private float _destroyTime = 1f;
    public override void Spawned()
    {
        StartCoroutine(DespawnDelay());
    }

    private IEnumerator DespawnDelay()
    {
        yield return new WaitForSeconds(_destroyTime);
        Runner.Despawn(gameObject.GetComponent<NetworkObject>());
    }
}
