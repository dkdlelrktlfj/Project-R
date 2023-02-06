using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private float timeScale = 1f;
    public static float TimeScale
    {
        get
        {
            return Instance.timeScale;
        }

        set
        {
            Instance.timeScale = value;
        }
    }

    public static float GameDeltaTime => Time.deltaTime * TimeScale;

    protected override void Awake()
    {
        base.Awake();
    }
}
