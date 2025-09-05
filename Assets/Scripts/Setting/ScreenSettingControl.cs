using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class ScreenSettingControl : SingleTonForGameObject<ScreenSettingControl>
{
    [SerializeField] private UI_Displayer m_displayer_Resolution;


    public void Awake()
    {
        SetInstance(this);
    }

    #region Unity Callbacks
    public void TrySetResolution()
    {

    }
    public void TrySetFullScreen()
    {

    }
    public void TrySetVSync()
    {

    }
    public void TrySetTextSize()
    {

    }
    public void TrySetTextBoxOpacity()
    {

    }
    public void TrySetBTextEffect()
    {

    }
    public void TrySetTextSpeed()
    {

    }
    #endregion

    internal void Render()
    {

    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}