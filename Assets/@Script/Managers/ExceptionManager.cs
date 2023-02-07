using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExceptionManager
{
    public static void SetCriticalException(System.Exception _exception)
    {
        GameManager.TimeScale = 0f;
        Utility.Log(_exception.Message);
        //Todo 게임 정상진행이 어려운 상황, 게임 재시작
    }
}
