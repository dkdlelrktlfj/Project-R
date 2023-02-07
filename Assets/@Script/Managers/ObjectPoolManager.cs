using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : LazySingleton<ObjectPoolManager>
{
    protected Dictionary<System.Type, object> managedPools = new Dictionary<System.Type, object>();

    public ObjectPool<T> GetObjectPool<T>() where T : IPoolable, new()
    {
        System.Type type = typeof(T);
        if (managedPools.ContainsKey(type) == true)
        {
            return (ObjectPool<T>)managedPools[type];
        }

        ObjectPool<T> newPool = CreatePool<T>();
        managedPools.Add(type, newPool);
        return newPool;
    }

    /// <summary>
    /// 사용되지 않는 오브젝트 풀 해제용
    /// :: 반환용이 아님 ::
    /// </summary>
    public void ReleaseObjectPool<T>()
    {
        System.Type type = typeof(T);
        if (managedPools.ContainsKey(type) == true)
        {
            managedPools.Remove(type);
        }

        Utility.Log($"Not Found Pool Type : {type}", Color.red);
    }

    private ObjectPool<T> CreatePool<T>() where T : IPoolable, new()
    {
        var result = new ObjectPool<T>();
        return result;
    }
}
