using CommonUtilLib.ThreadSafe;
using UnityEngine;

public class PopupControl : SingleTonForGameObject<PopupControl>
{
    private static bool m_bisOtherPopupOpen;

    internal bool m_bisOtherPopupOpened
    {
        get {  return m_bisOtherPopupOpen; }
        set { m_bisOtherPopupOpen = value; }
    }

    public void Awake()
    {
        SetInstance(this);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
