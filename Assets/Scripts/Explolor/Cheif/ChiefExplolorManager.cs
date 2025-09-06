using CommonUtilLib.ThreadSafe;
using UnityEngine;

public class ChiefExplolorManager : SingleTonForGameObject<ChiefExplolorManager>
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
        Explolor.Instance.Render();
    }
    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
