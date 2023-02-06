using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazySingleton<T> where T : class
{
    private static readonly Lazy<T> instance = new Lazy<T>(CreateInstance, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
    public static T Instance => instance.Value;
    

    private static T CreateInstance()
    {
        return Activator.CreateInstance(typeof(T), true) as T;
    }

    public LazySingleton()
    {

    }

    ~LazySingleton()
    {

    }
}
