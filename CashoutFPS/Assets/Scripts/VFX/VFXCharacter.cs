using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class VFXCharacter : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _moneyBag;
    [SerializeField] private ParticleSystem _ps;
    [SerializeField] private GameObject[] _mesh;
    public float spawnEffectTime = 5;
    public float pause = 1;
    public AnimationCurve fadeIn;
    private float _timer = 0;
    private Renderer _renderer;
    private int _shaderProperty;
    private bool _hasRunOnce;
  
   
    private void OnEnable()
    {
       EventLibrary.OnPlayerDiedNetwork.AddListener(RPC_ExecuteEvents);
       //PlayerHealth.OnPlayerDiedOnNetwork += RPC_ExecuteEvents;
    }

    private void OnDisable()
    {
        EventLibrary.OnPlayerDiedNetwork.RemoveListener(RPC_ExecuteEvents);
        //PlayerHealth.OnPlayerDiedOnNetwork -= RPC_ExecuteEvents;
    }
    public override void Spawned()
    {
        _shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponent<Renderer>();
    }

    
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_ExecuteEvents()
    {
         _ps.Play();
       
        for (int i = 0; i < _mesh.Length; i++)
            _mesh[i].SetActive(false);

        StartCoroutine(PlayDissolveEffect());
    }

    private IEnumerator PlayDissolveEffect()
    {
        while (!_hasRunOnce)
        {
            if (_timer < spawnEffectTime + pause)
            {
                _timer += Time.deltaTime;
            }
            else
            {
               _hasRunOnce = true; 
            }

            _renderer.material.SetFloat(_shaderProperty, fadeIn.Evaluate(Mathf.Lerp(0, spawnEffectTime, _timer)));

            yield return null;
        }
    }
}


    
   

