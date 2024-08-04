using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class ObjectPoolManager : MonoBehaviour, INetworkObjectPool
{
    private Dictionary<NetworkObject, List<NetworkObject>> _pooledObjects = new();

   



    public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
    {
        NetworkObject networkObject = null;
        NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out var prefab);
        _pooledObjects.TryGetValue(prefab, out var networkObjects);
        bool foundMatch = false;
        if(networkObjects?.Count > 0)
        {
            foreach(var item in networkObjects)
            {
                if(item != null && item.gameObject.activeSelf == false)
                {
                    networkObject = item;
                    foundMatch = true;
                    break;
                }
            }
        }


        if (foundMatch == false)
        {
            networkObject = CreateObjectInstance(prefab);
        }

        return networkObject;
    
    }



    private NetworkObject CreateObjectInstance(NetworkObject prefab)
    {
        var obj = Instantiate(prefab);
        if(_pooledObjects.TryGetValue(prefab, out var instaceData))
        {
            instaceData.Add(obj);
        }
        else
        {
            var list = new List<NetworkObject> { obj };
            _pooledObjects.Add(prefab, list);
        }

        return obj;
    }

    public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
    {
        instance.gameObject.SetActive(false);
    }

    
}
