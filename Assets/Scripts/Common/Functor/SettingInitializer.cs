using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class SettingInitializer : SingleTonForGameObject<SettingInitializer>
{
    [Header("Screen Setting")]
    [SerializeField] private RectTransform[] m_rectTransform_ResolutionLayout;

    [Header("Audio Setting")]
    [SerializeField] private AudioSource[] m_audioSources_BGMs;
    [SerializeField] private AudioSource[] m_audioSources_SFXs;


    public void Awake()
    {
        SetInstance(this);
    }

    internal void Initialize()
    {
        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;

        #region Screen Setting
        #region Resolution
        switch (curSettingData.Screen.Resolution)
        {
            case SettingData.ScreenSetting.ResolutionType.FHD_1920x1080:
                Screen.SetResolution(1920, 1080, curSettingData.Screen.BIsFullScreen);
                foreach (RectTransform rectTransform in m_rectTransform_ResolutionLayout)
                {
                    rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                break;

            case SettingData.ScreenSetting.ResolutionType.QHD_2560x1440:
                Screen.SetResolution(2560, 1440, curSettingData.Screen.BIsFullScreen);
                foreach (RectTransform rectTransform in m_rectTransform_ResolutionLayout)
                {
                    rectTransform.localScale = new Vector3(1.333333f, 1.333333f, 1.0f);
                }
                break;

            case SettingData.ScreenSetting.ResolutionType.UHD_3840x2160:
                Screen.SetResolution(3840, 2160, curSettingData.Screen.BIsFullScreen);
                foreach (RectTransform rectTransform in m_rectTransform_ResolutionLayout)
                {
                    rectTransform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
                }
                break;
        }
        #endregion

        #region V-Sync
        QualitySettings.vSyncCount = curSettingData.Screen.BIsVSync ? 1 : 0;
        #endregion
        #endregion

        #region Audio Setting
        if (curSettingData.Audio.BIsMute)
        {
            foreach (AudioSource audioSource in m_audioSources_BGMs)
            {
                audioSource.volume = 0.0f;
            }
            foreach (AudioSource audioSource in m_audioSources_SFXs)
            {
                audioSource.volume = 0.0f;
            }
        }
        else
        {
            foreach (AudioSource audioSource in m_audioSources_BGMs)
            {
                audioSource.volume = curSettingData.Audio.BGMVolume * curSettingData.Audio.MasterVolume;
            }
            foreach (AudioSource audioSource in m_audioSources_SFXs)
            {
                audioSource.volume = curSettingData.Audio.SFXVolume * curSettingData.Audio.MasterVolume;
            }
        }
        #endregion
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}