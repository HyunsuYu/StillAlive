using UnityEngine;


public struct SettingData
{
    public struct ScreenSetting
    {
        public enum ResolutionType
        {
            FHD_1920x1080,
            QHD_2560x1440,
            UHD_3840x2160
        }


        public ResolutionType Resolution;
        public bool BIsFullScreen;
        public bool BIsVSync;

        public float TextSize;
        public float TextBoxOpacity;

        public bool BIsTextEffect;
        public int TextSpeed;
    }
    public struct AudioSetting
    {
        public float MasterVolume;
        public float BGMVolume;
        public float SFXVolume;

        public bool BIsMute;
    }


    public ScreenSetting Screen;
    public AudioSetting Audio;
}