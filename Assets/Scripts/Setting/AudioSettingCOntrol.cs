using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class AudioSettingCOntrol : SingleTonForGameObject<AudioSettingCOntrol>
{
    public void Awake()
    {
        SetInstance(this);
    }

    internal void Render()
    {

    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}