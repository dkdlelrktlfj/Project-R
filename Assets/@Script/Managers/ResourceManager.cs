using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Resource
{
    public string name;
    public UnityEngine.Object asset;
    protected int referenceCounter = 0;

    public Resource(string _name)
    {

    }

    public Resource(string _name, UnityEngine.Object _asset)
    {
        this.name = _name;
        this.asset = _asset;
        referenceCounter++;
    }

    public virtual bool Release()
    {
        referenceCounter--;
        return referenceCounter <= 0;
    }

    //Todo Load Count
    //Resources, Addressable Exception
    //Wait Addressable Handler
}

public class AddressableResource : Resource
{
    public AsyncOperationHandle resourceHandle;

    public AddressableResource(string _name, AsyncOperationHandle _handler) : base(_name)
    {
        this.name = _name;
        this.resourceHandle = _handler;
    }
}

public sealed class ResourceManager : LazySingleton<ResourceManager>
{
    private Dictionary<string, Resource> loadedResource = new Dictionary<string, Resource>();

    public T LoadResourceFromResources<T>(string _path) where T : UnityEngine.Object
    {
        if (loadedResource.ContainsKey(_path) == true)
        {
            return loadedResource[_path].asset as T;
        }

        T resource = Resources.Load<T>(_path);

        Resource res = new Resource(_path);
        res.asset = resource;
        
        loadedResource[_path] = 

        return 
    }

    public async UniTask<T> LoadAsyncResourceFromAddressables<T>(string _path) where T : UnityEngine.Object
    {
        if (loadedResource.ContainsKey(_path) == true)
            return loadedResource[_path] as T;

        resourceLoadHandler

        T result = await Addressables.LoadAssetAsync<T>(_path);
        return result;
    }
}
