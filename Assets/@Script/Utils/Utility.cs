using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Utility
{
    [Conditional("ENABLE_LOG")]
    public static void Log(object _message, Color _color = default(Color))
    {
#if UNITY_EDITOR || DEV
        if(_color == default(Color))
        {
            _color = Color.white;
        }

        string htmlColor = ColorUtility.ToHtmlStringRGBA(_color);
        UnityEngine.Debug.Log($"<color=#{htmlColor}>{_message}</color>");
#endif
    }
}
