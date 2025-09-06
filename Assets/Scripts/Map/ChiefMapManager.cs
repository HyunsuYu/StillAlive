using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class ChiefMapManager : SingleTonForGameObject<ChiefMapManager>
{
    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        // Test
        if (!SaveDataBuffer.Instance.BHasValue)
        {
            SaveDataBuffer.Instance.ClearSaveData();
        }

        RenderAll();
    }

    internal void RenderAll()
    {
        MapRenderControl.Instance.Render();
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}