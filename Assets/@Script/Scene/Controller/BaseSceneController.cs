using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSceneController : MonoBehaviour
{
    protected virtual void Awake()
    {
        InitScene();
    }

    protected virtual void InitScene()
    {

    }
}
