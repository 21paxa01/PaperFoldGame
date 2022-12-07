using UnityEngine;
using UnityEngine.Audio;

public class AudioSwitch : BaseSwitch
{
    private const float MIN_VOLUME_VALUE = -80f;

    [Header("Audio Switch")]
    [SerializeField] private AudioMixerGroup _audioGroup;
    [SerializeField] private string _audioMixerGroupParameterName;
    [SerializeField] private float _maxVolumeValue;


    protected override void Awake()
    {
        base.Awake();

        int audioSwitchValue;
        if (PlayerPrefs.HasKey(_audioMixerGroupParameterName))
            audioSwitchValue = PlayerPrefs.GetInt(_audioMixerGroupParameterName);
        else
            audioSwitchValue = IsOn ? 1 : 0;

        if (audioSwitchValue == 1)
            SwitchState(true);
        else
            SwitchState(false);
    }

    private void SaveAudioSwitchState(bool isAudioSwitchOn)
    {
        if (isAudioSwitchOn)
            PlayerPrefs.SetInt(_audioMixerGroupParameterName, 1);
        else
            PlayerPrefs.SetInt(_audioMixerGroupParameterName, 0);

        PlayerPrefs.Save();
    }


    protected override void AddEventListeners()
    {
        base.AddEventListeners();
        ValueChanged.AddListener(SwitchAudioState);
    }

    protected override void RemoveEventListeners()
    {
        base.RemoveEventListeners();
        ValueChanged.RemoveListener(SwitchAudioState);
    }


    public void SwitchAudioState(bool isOn)
    {
        if (isOn)
            SetAudioGroupVolume(_maxVolumeValue);
        else
            SetAudioGroupVolume(MIN_VOLUME_VALUE);

        SaveAudioSwitchState(isOn);
    }

    public void SetAudioGroupVolume(float volume)
    {
        _audioGroup.audioMixer.SetFloat(_audioMixerGroupParameterName, volume);
    }
}
