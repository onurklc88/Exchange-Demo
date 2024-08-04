using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerCameraController : NetworkBehaviour
{
    
    [SerializeField] private Camera _fpsArmCamera;
    [SerializeField] private Camera _characterCamera;
    [SerializeField] private Camera _uýCamera;
    [SerializeField] private GameObject[] _fpsArmObjects;
    [SerializeField] private GameObject _characterObject;
    private Dictionary<int, int> _gunLayerDictionary = new Dictionary<int, int>();
    private int _playerCount = 0;

    public override void Spawned()
    {
        AddDictionaryValues();

        
        if (Object.HasInputAuthority)
        {
            Utils.SetRenderLayerInChildren(_characterObject.transform, 6);
            SetPlayerGunLayer(Runner.SessionInfo.PlayerCount);
           
        }
        else
        {
            _fpsArmCamera.enabled = false;
            _characterCamera.enabled = false;
        }

       

    }

    private void AddDictionaryValues()
    {

        for(int i = 0; i < 10; i++)
        {
            _playerCount++;
            _gunLayerDictionary.Add(_playerCount, i + 20);
        }

     }


    private void SetPlayerGunLayer(int key)
    {
        int layerToUse = _gunLayerDictionary[key];
        _fpsArmCamera.cullingMask &= ~(1 << 7);
        _fpsArmCamera.cullingMask |= 1 << layerToUse;
        Utils.SetRenderLayerInChildren(_fpsArmObjects[0].transform, layerToUse);
        Utils.SetRenderLayerInChildren(_fpsArmObjects[1].transform, layerToUse);
    }
}
