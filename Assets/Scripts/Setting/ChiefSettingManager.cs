using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class ChiefSettingManager : SingleTonForGameObject<ChiefSettingManager>
{
    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        RenderAll();
    }

    internal void RenderAll()
    {
        ScreenSettingControl.Instance.Render();
        AudioSettingCOntrol.Instance.Render();
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}