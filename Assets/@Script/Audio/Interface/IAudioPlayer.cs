using UnityEngine;

public interface IAudioPlayer
{
    void Play(AudioClip _clip, bool _playOnAwake);
    void Stop();
    void Pause();
    void Resume();

    void SetMute(bool _mute);
    float GetVolume();
    void SetVolume(float _volume);
}
