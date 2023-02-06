using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioConfig")]
public class AudioConfig : ScriptableObject
{
    public int configID = 0;
    public string configName;

    public BaseAudioController baseController;

    [Range(0f, 1f)]
    public float baseVolume = 1f;

    public float minDistance = 0f;
    public float maxDistance = 10f;

    public float minAudioRange = 1f;
    public float maxAudioRange = 500f;

    public AudioConstraints constraints;

    public virtual bool AudioConstraints(Vector3 _localPlayerPosition, Vector3 _audioPlayPosition)
    {
        if (constraints == null)
            return true;

        return constraints.InRange(_localPlayerPosition, _audioPlayPosition, minDistance, maxDistance);
    }
}
