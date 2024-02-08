using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour, INetworkObjectPool
{
    // Key : Value (actual game object that we're being instantiated during the run time)
    // the Key is prefabs and value is the actual network gameobjects that had been spawned
    
    private Dictionary<NetworkObject, List<NetworkObject>> prefabsThatHadBeenInstantiated = new();

    private void Start()
    {
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.ObjectPoolingManager = this;
        }
    }


    public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
    {
        // Debug.Log("AcquireInstance");
        // This function will be called every time Runner.spawned is being called
        // Also, It will run on all of peers (other clients)
        NetworkObject networkObejct = null; 
        NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out var prefab);

        prefabsThatHadBeenInstantiated.TryGetValue(prefab, out var networkObjects);

        bool foundMatch = false;
        if (networkObjects?.Count > 0)
        {
            foreach (var item in networkObjects)
            {
                if (item != null && item.gameObject.activeSelf == false)
                {
                    networkObejct = item;

                    foundMatch = true;
                    break;
                }
            }
        }

        // stays false when a complete new data that is not in our dic OR
        //When the function is getting called too fast and no object is ready to be recycled
        if (foundMatch == false)
        {
            networkObejct = CreateObjectInstance(prefab);
        }

        return networkObejct;
    }

    private NetworkObject CreateObjectInstance(NetworkObject prefab)
    {
        // Debug.Log("CreateObjectInstance");
        var obj = Instantiate(prefab);
        if (prefabsThatHadBeenInstantiated.TryGetValue(prefab, out var instanceData))
        {
            instanceData.Add(obj);
        }
        else
        {
            var list = new List<NetworkObject> {obj};
            list.Add(obj);
            prefabsThatHadBeenInstantiated.Add(prefab, list);
        }
        
        return obj;
    }

    // called once runner.despawn is called
    public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
    {
        // Debug.Log("ReleaseInstance");
        instance.gameObject.SetActive(false);
    }

    public void RemoveNetworkObejctFromDic(NetworkObject obj)
    {
        Debug.Log("RemoveNetworkObejctFromDic");
        if (prefabsThatHadBeenInstantiated.Count > 0)
        {
            foreach (var item in prefabsThatHadBeenInstantiated)
            {
                foreach (var networkObject in item.Value)
                {
                    if (networkObject == obj)
                    {
                        item.Value.Remove(networkObject);
                        break;
                    }
                }
            }
        }
    }
}
