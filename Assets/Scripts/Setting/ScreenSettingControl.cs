using CommonUtilLib.ThreadSafe;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public sealed class ScreenSettingControl : SingleTonForGameObject<ScreenSettingControl>
{
    [SerializeField] private UI_Displayer m_displayer_Resolution;
    [SerializeField] private UI_Toggle m_toggle_BIsFullScreen;
    [SerializeField] private UI_Toggle m_toggle_VSync;
    [SerializeField] private Slider m_slider_TextSize;
    [SerializeField] private Slider m_slider_TextBoxOpacity;
    [SerializeField] private UI_Toggle m_toggle_BIsTextEffect;
    [SerializeField] private Slider m_slider_TextSpeed;



    public void Awake()
    {
        SetInstance(this);
    }

    #region Unity Callbacks
    public void TrySetResolution()
    {
        if(!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        curSettingData.Screen.Resolution = (SettingData.ScreenSetting.ResolutionType)m_displayer_Resolution.Value;
        SettingDataBuffer.Instance.SettingData = curSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        SettingInitializer.Instance.Initialize();
    }
    public void TrySetFullScreen()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        curSettingData.Screen.BIsFullScreen = m_toggle_BIsFullScreen.Value;
        SettingDataBuffer.Instance.SettingData = curSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        SettingInitializer.Instance.Initialize();
    }
    public void TrySetVSync()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        curSettingData.Screen.BIsVSync = m_toggle_VSync.Value;
        SettingDataBuffer.Instance.SettingData = curSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        SettingInitializer.Instance.Initialize();
    }
    public void TrySetTextSize()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        curSettingData.Screen.TextSize = m_slider_TextSize.value;
        SettingDataBuffer.Instance.SettingData = curSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        SettingInitializer.Instance.Initialize();
    }
    public void TrySetTextBoxOpacity()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        curSettingData.Screen.TextBoxOpacity = m_slider_TextBoxOpacity.value;
        SettingDataBuffer.Instance.SettingData = curSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        SettingInitializer.Instance.Initialize();
    }
    public void TrySetBTextEffect()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        curSettingData.Screen.BIsTextEffect = m_toggle_BIsTextEffect.Value;
        SettingDataBuffer.Instance.SettingData = curSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        SettingInitializer.Instance.Initialize();
    }
    public void TrySetTextSpeed()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        curSettingData.Screen.TextSpeed = (int)m_slider_TextSpeed.value;
        SettingDataBuffer.Instance.SettingData = curSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        SettingInitializer.Instance.Initialize();
    }
    #endregion

    internal void Render()
    {
        SettingData.ScreenSetting screenSetting = SettingDataBuffer.Instance.SettingData.Screen;

        Resolution[] resolutions = Screen.resolutions;
        m_displayer_Resolution.ClearOptions();
        Dictionary<int, string> resolutionOptions = new Dictionary<int, string>();
        foreach (Resolution resolution in resolutions)
        {
            if (resolution.width == 1920 && resolution.height == 1080 && !resolutionOptions.ContainsKey(resolution.width * resolution.height))
            {
                resolutionOptions.Add(resolution.width * resolution.height, "FHD 1920x1080");
            }
            else if (resolution.width == 2560 && resolution.height == 1440 && !resolutionOptions.ContainsKey(resolution.width * resolution.height))
            {
                resolutionOptions.Add(resolution.width * resolution.height, "QHD 2560x1440");
            }
            else if (resolution.width == 3840 && resolution.height == 2160 && !resolutionOptions.ContainsKey(resolution.width * resolution.height))
            {
                resolutionOptions.Add(resolution.width * resolution.height, "UHD 3840x2160");
            }
        }
        m_displayer_Resolution.AddOptions(resolutionOptions.Values.ToList());
        m_displayer_Resolution.SetValueWithoutNotify((int)screenSetting.Resolution);

        m_toggle_BIsFullScreen.SetValueWithoutNotify(screenSetting.BIsFullScreen);
        m_toggle_VSync.SetValueWithoutNotify(screenSetting.BIsVSync);

        m_slider_TextSize.SetValueWithoutNotify(screenSetting.TextSize);
        m_slider_TextBoxOpacity.SetValueWithoutNotify(screenSetting.TextBoxOpacity);

        m_toggle_BIsTextEffect.SetValueWithoutNotify(screenSetting.BIsTextEffect);
        m_slider_TextSpeed.SetValueWithoutNotify(screenSetting.TextSpeed);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}