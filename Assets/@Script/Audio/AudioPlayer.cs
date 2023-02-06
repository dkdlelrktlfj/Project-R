using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer
{
    private const int DISABLE = 0;
    private const int ENABLE = 1;

    private readonly string RESOURCE_PATH = "Prefabs/Audio/AudioController";
    private readonly int TO_MILLISECONDS = 1000;
    private BaseAudioController audioPrefab;

    //Current Playing SFX Callback
    private event System.Action<float> OnChangeSFXVolume;

    private Dictionary<int, AudioConfig> audioConfigs = new Dictionary<int, AudioConfig>();

    //Audio Controller Object Pool
    private Dictionary<AudioConfig, Dictionary<BaseAudioController, BaseAudioController>> activePresetAudioControllers = new Dictionary<AudioConfig, Dictionary<BaseAudioController, BaseAudioController>>();
    private Dictionary<System.Type, Stack<IAudioPlayer>> deactiveAudioControllers = new Dictionary<System.Type, Stack<IAudioPlayer>>();

    private bool baseMute = false;
    public bool IsMute => baseMute;

    private bool bgmMute = false;
    public bool IsBGMMute => bgmMute;

    private bool sfxMute = false;
    public bool SFXMute => sfxMute;

    private float baseVolume = 1f;
    public float BaseVolume => baseVolume;
    private float backgroundVolume = 1f;
    public float BGMVolume => backgroundVolume;
    private float sfxVolume = 1f;
    public float SFXVolume => sfxVolume;

    private IAudioPlayer bgmPlayer;

    public AudioPlayer()
    {
        baseVolume = PlayerPrefs.GetFloat("BaseVolume", 1f);
        backgroundVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        baseMute = PlayerPrefs.GetInt("BaseMute", DISABLE) == DISABLE ? false : true;
        bgmMute = PlayerPrefs.GetInt("BGMMute", DISABLE) == DISABLE ? false : true;
        sfxMute = PlayerPrefs.GetInt("SFXMute", DISABLE) == DISABLE ? false : true;

        audioPrefab = Resources.Load<BaseAudioController>(RESOURCE_PATH);

        AudioConfig[] configs = Resources.LoadAll<AudioConfig>("AudioSettings/");
        for (int index = 0; index < configs.Length; index++)
        {
            var config = configs[index];
            if (audioConfigs.ContainsKey(config.configID) == false)
            {
                audioConfigs.Add(config.configID, config);
                activePresetAudioControllers.Add(config, new Dictionary<BaseAudioController, BaseAudioController>());

                System.Type type = audioPrefab.GetType();
                if (config.baseController != null)
                    type = config.baseController.GetType();

                deactiveAudioControllers[type] = new Stack<IAudioPlayer>();
            }
        }

        bgmPlayer = Instantiate(audioPrefab);
        var bgmController = (BaseAudioController)bgmPlayer;
        bgmController.SetPlayer(audioConfigs[(int)EAudioType.Config_2D], true, baseVolume * backgroundVolume);
        bgmController.SetMute(bgmMute);

        GameObject.DontDestroyOnLoad(bgmController);
    }

    public void ClearAudioPlayer()
    {
        bgmPlayer.Stop();
        var activeAudios = activePresetAudioControllers.Values.GetEnumerator();
        while (activeAudios.MoveNext() == true)
        {
            activeAudios.Current.Clear();
        }

        var deActiveAudios = deactiveAudioControllers.Values.GetEnumerator();
        while (deActiveAudios.MoveNext() == true)
        {
            deActiveAudios.Current.Clear();
        }
        OnChangeSFXVolume = null;
    }

    #region Set Mute

    #endregion

    #region Set Optional Volume
    public void SetBaseVolume(float _volume)
    {
        this.baseVolume = _volume;

        bgmPlayer.SetVolume(baseVolume * this.backgroundVolume);
        OnChangeSFXVolume?.Invoke(baseVolume * this.sfxVolume);
        PlayerPrefs.SetFloat("BaseVolume", baseVolume);
    }

    public void SetBGMVolume(float _volume)
    {
        this.backgroundVolume = _volume;
        bgmPlayer.SetVolume(baseVolume * this.backgroundVolume);
        PlayerPrefs.SetFloat("BGMVolume", backgroundVolume);
    }

    public void SetSFXVolume(float _volume)
    {
        this.sfxVolume = _volume;
        OnChangeSFXVolume?.Invoke(baseVolume * this.sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
    #endregion
    #region BGM Controls

    public async void SetVolume(float _targetVolume, float _durationTime = 0f)
    {
        float currentVolume = bgmPlayer.GetVolume();
        float volumeOffset = _targetVolume - currentVolume;

        if (_durationTime > 0f)
        {
            float currentTime = 0f;
            while (currentTime < _durationTime)
            {
                //최종 볼륨 = BaseVolume * 시작볼륨 + (목표 볼륨까지의 차 * (시간 진행도));
                float volume = baseVolume * (currentVolume + (volumeOffset * (currentTime / _durationTime)));
                bgmPlayer.SetVolume(volume);

                currentTime += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        bgmPlayer.SetVolume(baseVolume * _targetVolume);
    }

    public async UniTask<bool> SetVolumeAsync(float _targetVolume, float _durationTime = 0f)
    {
        float currentVolume = bgmPlayer.GetVolume();
        float volumeOffset = _targetVolume - currentVolume;

        if (_durationTime > 0f)
        {
            float currentTime = 0f;
            while (currentTime < _durationTime)
            {
                //최종 볼륨 = BaseVolume * 시작볼륨 + (목표 볼륨까지의 차 * (시간 진행도));
                float volume = baseVolume * (currentVolume + (volumeOffset * (currentTime / _durationTime)));
                bgmPlayer.SetVolume(volume);

                currentTime += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        bgmPlayer.SetVolume(baseVolume * _targetVolume);

        return true;
    }

    public void PlayBGM(AudioClip _clip)
    {
        bgmPlayer.Play(_clip, true);
    }

    public void ResumeBGM()
    {
        bgmPlayer.Resume();
    }

    public void PauseBGM()
    {
        bgmPlayer.Pause();
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void SwitchBGM(AudioClip _toClip)
    {
        bgmPlayer.Play(_toClip, true);
    }

    public async void SwitchBGMWithDuration(AudioClip _toClip, float _startDuration, float _endDuration)
    {
        float currentVolume = bgmPlayer.GetVolume();

        if (_startDuration > 0f)
            await SetVolumeAsync(0f, _startDuration);

        bgmPlayer.Play(_toClip, true);

        if (_endDuration > 0f)
            await SetVolumeAsync(currentVolume, _endDuration);
    }
    #endregion

    public IAudioPlayer Play(AudioClip _clip, bool _loop = false, bool _stopToAutoRelease = true, bool _playOnAwake = true)
    {
        if (_clip == null)
            return null;

        BaseAudioController audioController = GetAudioPlayer((int)EAudioType.Config_2D);

        OnChangeSFXVolume += audioController.SetVolume;
        audioController.SetPlayer(audioConfigs[(int)EAudioType.Config_2D], _loop, baseVolume * sfxVolume);
        audioController.SetMute(baseMute || sfxMute);
        audioController.Play(_clip, _playOnAwake);

        if (_stopToAutoRelease)
            Release(audioController, Mathf.RoundToInt(_clip.length * TO_MILLISECONDS));

        return audioController;
    }

    //Audio Play With Force
    public IAudioPlayer Play3D(AudioClip _clip, Vector3 _playPostiion, bool _loop = false, float _minDist = 1f, float _maxDist = 500f, bool _stopToAutoRelease = true, bool _playOnAwake = true)
    {
        if (_clip == null)
            return null;

        BaseAudioController audioController = GetAudioPlayer((int)EAudioType.Config_3D);

        audioController.transform.localPosition = _playPostiion;

        OnChangeSFXVolume += audioController.SetVolume;
        audioController.SetPlayer(audioConfigs[(int)EAudioType.Config_3D], _loop, baseVolume * sfxVolume, 1f, _minDist, _maxDist);
        audioController.SetMute(baseMute || sfxMute);
        audioController.Play(_clip, _playOnAwake);

        if (_stopToAutoRelease)
            Release(audioController, Mathf.RoundToInt(_clip.length * TO_MILLISECONDS));

        return audioController;
    }

    public IAudioPlayer Play3DPreset(AudioClip _clip, Vector3 _playPostiion, int _preset, bool _loop = false, bool _stopToAutoRelease = true, bool _playOnAwake = true)
    {
        if (_clip == null)
            return null;

        AudioConfig preset = audioConfigs[_preset];

        BaseAudioController audioController = GetAudioPlayer(preset);

        audioController.transform.localPosition = _playPostiion;

        OnChangeSFXVolume += audioController.SetVolume;
        audioController.SetPlayer(preset, _loop, baseVolume * sfxVolume, 1f, preset.minAudioRange, preset.maxAudioRange);
        audioController.SetMute(baseMute || sfxMute);
        audioController.Play(_clip, _playOnAwake);

        if (_stopToAutoRelease)
            Release(audioController, Mathf.RoundToInt(_clip.length * TO_MILLISECONDS));

        return audioController;
    }

    public void SetBaseVolumeMute(bool _mute)
    {
        baseMute = _mute;
        SetBGMVolumeMute(_mute);
        SetSFXVolumeMute(_mute);

        PlayerPrefs.SetInt("BaseMute", (_mute == false ? DISABLE : ENABLE));
    }

    public void SetBGMVolumeMute(bool _mute)
    {
        bgmMute = _mute;
        bgmPlayer.SetMute(baseMute || bgmMute);
        PlayerPrefs.SetInt("BGMMute", (_mute == false ? DISABLE : ENABLE));
    }

    public void SetSFXVolumeMute(bool _mute)
    {
        sfxMute = _mute;

        var presetEnumerator = activePresetAudioControllers.Values.GetEnumerator();
        while (presetEnumerator.MoveNext() == true)
        {
            var currentPreset = presetEnumerator.Current.GetEnumerator();
            while (currentPreset.MoveNext() == true)
            {
                currentPreset.Current.Value.SetMute(baseMute || sfxMute);
            }
        }
        PlayerPrefs.SetInt("SFXMute", (_mute == false ? DISABLE : ENABLE));
    }

    public async void Release(BaseAudioController _controller, int _time)
    {
        await UniTask.Delay(_time);
        Release(_controller);
    }

    public void Release(BaseAudioController _controller)
    {
        if (_controller == null)
        {
            return;
        }

        AudioConfig config = audioConfigs[_controller.Config.configID];
        activePresetAudioControllers[config].Remove(_controller);

        _controller.transform.SetParent(null);

        _controller.Stop();
        _controller.gameObject.SetActive(false);
        OnChangeSFXVolume -= _controller.SetVolume;
        deactiveAudioControllers[_controller.GetType()].Push(_controller);
    }

    private BaseAudioController GetAudioPlayer(int _preset = (int)EAudioType.Config_2D)
    {
        BaseAudioController player = null;
        AudioConfig config = audioConfigs[_preset];

        System.Type type = audioPrefab.GetType();
        if (config.baseController != null)
            type = config.baseController.GetType();

        if (deactiveAudioControllers[type].Count <= 0)
        {
            if (config.baseController == null)
                player = Instantiate(audioPrefab);
            else
                player = Instantiate(config.baseController);
        }
        else
        {
            player = (BaseAudioController)deactiveAudioControllers[type].Pop();
        }

        player.gameObject.SetActive(true);
        activePresetAudioControllers[audioConfigs[_preset]].Add(player, player);

        return player;
    }

    private BaseAudioController GetAudioPlayer(AudioConfig _preset)
    {
        BaseAudioController player = null;

        System.Type type = audioPrefab.GetType();
        if (_preset.baseController != null)
            type = _preset.baseController.GetType();

        if (deactiveAudioControllers[type].Count <= 0)
        {
            if (_preset.baseController == null)
                player = Instantiate(audioPrefab);
            else
                player = Instantiate(_preset.baseController);
        }
        else
        {
            player = (BaseAudioController)deactiveAudioControllers[type].Pop();
        }

        player.gameObject.SetActive(true);
        activePresetAudioControllers[_preset].Add(player, player);

        return player;
    }

    private BaseAudioController Instantiate(BaseAudioController _prefab)
    {
        return GameObject.Instantiate(_prefab);
    }
}
