using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public sealed partial class ResourceManager
{
    protected class ResourceHandler
    {
        public string name;
        protected UnityEngine.Object res;
        protected int referenceCount = 0;
        public int Count => referenceCount;

        public ResourceHandler(string _name)
        {
            this.name = _name;
        }

        public ResourceHandler(string _name, UnityEngine.Object _resource)
        {
            this.name = _name;
            this.res = _resource;
        }

        public virtual T GetResource<T>() where T : UnityEngine.Object
        {
            referenceCount++;
            return this.res as T;
        }

        public virtual int Release()
        {
            if (referenceCount > 0)
            {
                referenceCount--;
            }

            return referenceCount;
        }
    }

    protected class AddressableResourceHandler : ResourceHandler
    {
        protected AsyncOperationHandle handler;

        public AddressableResourceHandler(string _name) : base(_name)
        {
            this.name = _name;
        }

        public AddressableResourceHandler(string _name, AsyncOperationHandle _resourceHandler) : base(_name)
        {
            this.name = _name;
            this.handler = _resourceHandler;
        }

        public bool IsDone()
        {
            return handler.IsDone;
        }

        public async UniTask<T> GetAwaitResource<T>() where T : UnityEngine.Object
        {
            int tryCount = 0;
            while (true)
            {
                if (handler.IsDone == true)
                {
                    res = handler.Result as T;
                    break;
                }

                if(tryCount > 100)
                {
                    return null;
                }

                await UniTask.Delay(1000);
            }

            return GetResource<T>();
        }

        public override int Release()
        {
            referenceCount--;
            return referenceCount;
        }

        public void ReleaseHandler()
        {
            referenceCount = 0;
            ResourceManager.Instance.Release(this.name);
        }

        public static implicit operator AsyncOperationHandle(AddressableResourceHandler handler) => handler.handler;
    }
}

public sealed partial class ResourceManager : LazySingleton<ResourceManager>
{
    private const int GC_RELEASE_TIME = 6000;

    private Dictionary<string, ResourceHandler> loadedResource = new Dictionary<string, ResourceHandler>();
    private Dictionary<string, ResourceHandler> releaseDelayResources = new Dictionary<string, ResourceHandler>();

    /// Resources 폴더로부터 로드
    public T LoadResourceFromResources<T>(string _path) where T : UnityEngine.Object
    {
        if (loadedResource.ContainsKey(_path) == true)
        {
            return loadedResource[_path].GetResource<T>();
        }

        T resource = Resources.Load<T>(_path);

        ResourceHandler res = new ResourceHandler(_path, resource);
        loadedResource[_path] = res;

        return resource;
    }

    /// 어드레서블로부터 비동기 로드 후 결과 리턴
    public async UniTask<T> LoadAsyncResourceFromAddressables<T>(string _path) where T : UnityEngine.Object
    {
        do
        {
            if (loadedResource.ContainsKey(_path) == false)
            {
                break;
            }

            AddressableResourceHandler handler = (AddressableResourceHandler)loadedResource[_path];
            T result = await handler.GetAwaitResource<T>();

            return result;
        }
        while (false);

        AsyncOperationHandle handle = Addressables.LoadAssetAsync<T>(_path);
        AddressableResourceHandler res = new AddressableResourceHandler(_path, handle);

        loadedResource[_path] = res;

        return await res.GetAwaitResource<T>();
    }

    /// 어드레서블로부터 로드 후 콜백
    public async UniTaskVoid LoadAsyncResourceFromAddressables<T>(string _path, System.Action<T> _onLoad) where T : UnityEngine.Object
    {
        do
        {
            if (loadedResource.ContainsKey(_path) == false)
            {
                break;
            }

            AddressableResourceHandler handler = (AddressableResourceHandler)loadedResource[_path];
            T result = await handler.GetAwaitResource<T>();

            _onLoad?.Invoke(result);
        }
        while (false);

        AsyncOperationHandle handle = Addressables.LoadAssetAsync<T>(_path);
        AddressableResourceHandler res = new AddressableResourceHandler(_path, handle);

        loadedResource[_path] = res;

        _onLoad?.Invoke(await res.GetAwaitResource<T>());
    }

    // 에셋 해제
    public void Release(string _path)
    {
        if (loadedResource.TryGetValue(_path, out ResourceHandler _reference))
        {
            int refCounter = _reference.Release();
            if (refCounter <= 0 && releaseDelayResources.ContainsKey(_path) == false)
            {
                releaseDelayResources.Add(_path, _reference);
                Release(GC_RELEASE_TIME, _path, _reference).Forget();
            }
        }
    }

    // 60초의 시간 대기 후 참조가 없는 애셋 해제
    private async UniTaskVoid Release(int _milliseconds, string _name, ResourceHandler _handler)
    {
        await UniTask.Delay(_milliseconds);
        if (_handler.Count <= 0)
        {
            releaseDelayResources.Remove(_name);
            loadedResource.Remove(_name);

            if (_handler is AddressableResourceHandler)
                Addressables.Release((AsyncOperationHandle)(_handler as AddressableResourceHandler));
            else
                Resources.UnloadAsset(_handler.GetResource<UnityEngine.Object>());
#if UNITY_EDITOR
            Debug.LogWarning($"BundleMessage \r\n<color=yellow>Relase Asset {_name}</color>");
#endif
        }
    }
}
