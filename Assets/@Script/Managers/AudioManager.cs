using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AudioManager : LazySingleton<AudioManager>
{
    private const string AUDIO_RESOURCE_PATH = "Assets/Resources_moved/Sound/";
    private Dictionary<string, AudioClip> sfxs = new Dictionary<string, AudioClip>();

    private AudioPlayer player = null;

    private AudioManager() : base()
    {
        player = new AudioPlayer();
    }

    #region Get Volume
    public float GetBaseVolume()
    {
        return player.BaseVolume;
    }

    public float GetBGMVolume()
    {
        return player.BGMVolume;
    }

    public float GetSFXVolume()
    {
        return player.SFXVolume;
    }
    #endregion

    #region Mute
    public void SetBaseVolumeMute(bool _mute)
    {
        player.SetBaseVolumeMute(_mute);
    }

    public void SetBGMVolumeMute(bool _mute)
    {
        player.SetBGMVolumeMute(_mute);
    }

    public void SetSFXVolumeMute(bool _mute)
    {
        player.SetSFXVolumeMute(_mute);
    }

    public bool IsBaseVolumeMute()
    {
        return player.IsMute;
    }

    public bool IsBGMVolumeMute()
    {
        return player.IsBGMMute;
    }

    public bool IsSFXVolumeMute()
    {
        return player.SFXMute;
    }

    #endregion

    #region Set Optional Volume
    public void SetBaseVolume(float _volume)
    {
        player.SetBaseVolume(_volume);
    }

    public void SetBGMVolume(float _volume)
    {
        player.SetBGMVolume(_volume);
    }

    public void SetSFXVolume(float _volume)
    {
        player.SetSFXVolume(_volume);
    }
    #endregion

    #region BGM Controls
    public void SetBGMAudioVolume(float _targetVolume, float _durationTime = 0f)
    {
        player.SetVolume(_targetVolume, _durationTime);
    }

    public async void PlayBGM(string _clipName)
    {
        AudioClip clip = await Load(_clipName);
        player.PlayBGM(clip);
    }

    public void ResumeBGM()
    {
        player.ResumeBGM();
    }

    public void PauseBGM()
    {
        player.PauseBGM();
    }

    public void StopBGM()
    {
        player.StopBGM();
    }

    public async void SwitchBGMWithDuration(string _clipName, float _startDuration = 0f, float _endDuration = 0f)
    {
        AudioClip clip = await Load(_clipName);
        player.SwitchBGMWithDuration(clip, _startDuration, _endDuration);
    }
    #endregion

    #region Audio Controls
    public async UniTask<IAudioPlayer> Play(string _clipName, bool _loop = false, bool _stopToAutoRelease = true, bool _playOnAwake = true)
    {
        AudioClip clip = await Load(_clipName);
        return player.Play(clip, _loop, _stopToAutoRelease, _playOnAwake);
    }

    public async UniTask<IAudioPlayer> Play3D(string _clipName, Vector3 _playPostiion, bool _loop = false, float _minDist = 1f, float _maxDist = 500f, bool _stopToAutoRelease = true, bool _playOnAwake = true)
    {
        AudioClip clip = await Load(_clipName);
        return player.Play3D(clip, _playPostiion, _loop, _minDist, _maxDist, _stopToAutoRelease, _playOnAwake);
    }

    public async UniTask<IAudioPlayer> Play3DPreset(string _clipName, Vector3 _playPostiion, int _presetID, bool _loop = false, bool _stopToAutoRelease = true, bool _playOnAwake = true)
    {
        AudioClip clip = await Load(_clipName);
        return player.Play3DPreset(clip, _playPostiion, _presetID, _loop, _stopToAutoRelease, _playOnAwake);
    }
    #endregion

    public void ClearAll()
    {
        player.ClearAudioPlayer();

        var enumerator = sfxs.Keys.GetEnumerator();
        while (enumerator.MoveNext() == true)
        {
            Release(enumerator.Current);
        }

        sfxs.Clear();
    }

    public void ClearAudioPlayer()
    {
        player.ClearAudioPlayer();
    }

    //Auto Release가 아닌 사용자가 임의로 Stop 후 Release할 때 사용
    public void ReleaseAudioController(IAudioPlayer _player)
    {
        player.Release((BaseAudioController)_player);
    }

    public async UniTask<AudioClip> Load(string _audioName)
    {
        if (sfxs.TryGetValue(_audioName, out AudioClip clip))
        {
            return clip;
        }

        return await LoadAsync(_audioName);
    }

    public async UniTask<AudioClip[]> LoadAll(params string[] _audioNames)
    {
        int length = _audioNames.Length;
        AudioClip[] clips = new AudioClip[length];

        for (int index = 0; index < length; index++)
        {
            if (sfxs.TryGetValue(_audioNames[index], out AudioClip clip) == true)
            {
                clips[index] = clip;
                continue;
            }

            clips[index] = await LoadAsync(_audioNames[index]);
        }

        return clips;
    }

    //AudioClip을 Release할 때 호출
    public void Release(string _audioName)
    {
        ReleaseAudioAsset(_audioName);
    }

    public void ReleaseAll(params string[] _audioName)
    {
        for (int index = 0; index < _audioName.Length; index++)
        {
            ReleaseAudioAsset(_audioName[index]);
        }
    }

    private async UniTask<AudioClip> LoadAsync(string _audioName)
    {
        var clip = await ResourceManager.Instance.LoadAsyncResourceFromAddressables<AudioClip>($"{AUDIO_RESOURCE_PATH}{_audioName}");
#if UNITY_EDITOR
        if (clip == null)
        {
            Debug.LogError($"Audio Not Found : {AUDIO_RESOURCE_PATH}{_audioName}");
            throw new System.Exception($"Audio Not Found : {AUDIO_RESOURCE_PATH}{_audioName}");
        }
#endif
        sfxs.Add(_audioName, clip);
        return clip;
    }

    private void ReleaseAudioAsset(string _name)
    {
        if (sfxs.ContainsKey(_name) == true)
            sfxs.Remove(_name);


        //ResourceManager.Instance.Release($"{AUDIO_RESOURCE_PATH}{_name}");
    }
}