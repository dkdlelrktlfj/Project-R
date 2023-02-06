using UnityEngine;

public interface IAudioConstraints
{
    bool InRange(Vector3 _localPlayerPosition, Vector3 _audioPlayPosition, float _minDist, float _maxDist);
}