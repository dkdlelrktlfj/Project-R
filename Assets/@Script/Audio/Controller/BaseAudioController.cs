using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class BaseAudioController : MonoBehaviour, IAudioPlayer
{
    protected AudioConfig config = null;
    public AudioConfig Config { get { return config; } }

    [SerializeField] protected AudioSource audioBase;

    public virtual void Play(AudioClip _clip, bool _playOnAwake)
    {
        audioBase.clip = _clip;

        if (_playOnAwake)
            audioBase.Play();
    }

    public virtual void Pause()
    {
        audioBase.Pause();
    }

    public virtual void Resume()
    {
        audioBase.Play();
    }

    public virtual void SetMute(bool _mute)
    {
        audioBase.mute = _mute;
    }


    //Stop 이후 수동으로 controller를 Release 해주어야함
    public virtual void Stop()
    {
        audioBase.Stop();
    }

    public virtual float GetVolume()
    {
        return audioBase.volume;
    }

    public virtual void SetVolume(float _volume)
    {
        audioBase.volume = _volume * config.baseVolume;
    }

    public virtual void SetPlayer(AudioConfig config, bool _loop = false, float _volume = 1f, float _spatialBlend = 0f, float _minDist = 1f, float _maxDist = 500)
    {
        this.config = config;
        audioBase.loop = _loop;
        audioBase.volume = _volume * config.baseVolume;
        audioBase.spatialBlend = _spatialBlend;
        audioBase.minDistance = _minDist;
        audioBase.maxDistance = _maxDist;
    }
}
