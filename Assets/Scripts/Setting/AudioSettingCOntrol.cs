using UnityEngine;
using UnityEngine.UI;

using CommonUtilLib.ThreadSafe;


public sealed class AudioSettingCOntrol : SingleTonForGameObject<AudioSettingCOntrol>
{
    [SerializeField] private Slider m_slider_MasterVolime;
    [SerializeField] private Slider m_slider_BGMVolume;
    [SerializeField] private Slider m_slider_SFXVolume;

    [SerializeField] private UI_Toggle m_toggle_BIsMute;


    public void Awake()
    {
        SetInstance(this);
    }

    #region Unity Callbacks
    public void TrySetMasterVolume()
    {
        if(!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        curSettingData.Audio.MasterVolume = m_slider_MasterVolime.value;
        SettingDataBuffer.Instance.SettingData = curSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        SettingInitializer.Instance.Initialize();
    }
    public void TrySetBGMVolume()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        curSettingData.Audio.BGMVolume = m_slider_BGMVolume.value;
        SettingDataBuffer.Instance.SettingData = curSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        SettingInitializer.Instance.Initialize();
    }
    public void TrySetSFXVolume()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        curSettingData.Audio.SFXVolume = m_slider_SFXVolume.value;
        SettingDataBuffer.Instance.SettingData = curSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        SettingInitializer.Instance.Initialize();
    }
    public void TrySetBIsMute()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        curSettingData.Audio.BIsMute = m_toggle_BIsMute.Value;
        SettingDataBuffer.Instance.SettingData = curSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        SettingInitializer.Instance.Initialize();
    }
    #endregion

    internal void Render()
    {
        SettingData.AudioSetting audioSetting = SettingDataBuffer.Instance.SettingData.Audio;

        m_slider_MasterVolime.SetValueWithoutNotify(audioSetting.MasterVolume);
        m_slider_BGMVolume.SetValueWithoutNotify(audioSetting.BGMVolume);
        m_slider_SFXVolume.SetValueWithoutNotify(audioSetting.SFXVolume);

        m_toggle_BIsMute.SetValueWithoutNotify(audioSetting.BIsMute);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}