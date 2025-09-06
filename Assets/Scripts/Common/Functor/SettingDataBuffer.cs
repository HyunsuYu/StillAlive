using System;
using System.IO;

using UnityEngine;

using CommonUtilLib.ThreadSafe;

using Newtonsoft.Json;


public sealed class SettingDataBuffer : SingleTonForGameObject<SettingDataBuffer>
{
    private SettingData? m_settingData;


    public void Awake()
    {
        SetInstance(this);

        TryLoadData();
    }

    internal SettingData SettingData
    {
        get
        {
            if(!m_settingData.HasValue)
            {
                throw new Exception("Setting Data is Empty");
            }

            return m_settingData.Value;
        }
        set
        {
            m_settingData = value;
        }
    }

    private string SettingDataPath
    {
        get
        {
            return Path.Combine(Application.streamingAssetsPath, "SettingData.json");
        }
    }

    internal bool TryLoadData()
    {
        if (!File.Exists(SettingDataPath))
        {
            m_settingData = new SettingData()
            {
                Screen = new SettingData.ScreenSetting()
                {
                    Resolution = SettingData.ScreenSetting.ResolutionType.FHD_1920x1080,
                    BIsFullScreen = true,
                    BIsVSync = true,

                    TextSize = 25.0f,
                    TextBoxOpacity = 0.8f,

                    BIsTextEffect = true,
                    TextSpeed = 3
                },
                Audio = new SettingData.AudioSetting()
                {
                    MasterVolume = 1.0f,
                    BGMVolume = 1.0f,
                    SFXVolume = 1.0f,
                    BIsMute = false
                }
            };
            TrySaveData();
            return true;
        }

        try
        {
            string jsonData = File.ReadAllText(SettingDataPath);
            m_settingData = JsonConvert.DeserializeObject<SettingData>(jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }

        return true;
    }
    internal bool TrySaveData()
    {
        if (m_settingData == null)
        {
            Debug.LogError("Cur Save Data is Empty");
            return false;
        }

        try
        {
            string jsonData = JsonConvert.SerializeObject(m_settingData);

            string directory = Path.GetDirectoryName(SettingDataPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(SettingDataPath, jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }

        return true;
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}