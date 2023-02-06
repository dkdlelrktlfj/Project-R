using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;

    public static T CreateInstance()
    {
        GameObject go = new GameObject($"@{typeof(T).ToString()}");
        go.transform.position = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        T result = go.AddComponent<T>();

        return result;
    }

    protected virtual void Awake()
    {
        if(instance != null)
        {
            GameObject.Destroy(instance.gameObject);
        }

        instance = this as T;
        GameObject.DontDestroyOnLoad(instance.gameObject);
    }
}
