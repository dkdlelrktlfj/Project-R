using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadingController : LazySingleton<SceneLoadingController>
{
    public delegate void SceneLoaded(SceneLoadingController sceneLoadingController);
    public static void LoadScene(string _sceneName, SceneLoaded _onCompliteCallback)
    {
        
    }
}
