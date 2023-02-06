using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioConstraints : ScriptableObject, IAudioConstraints

{
    public abstract bool InRange(Vector3 _localPlayerPosition, Vector3 _audioPlayPosition, float _minDist, float _maxDist);
}
