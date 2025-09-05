using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class RestPopupControl : SingleTonForGameObject<RestPopupControl>
{
    [SerializeField] private GameObject m_gameobject_Popup;


    public void Awake()
    {
        SetInstance(this);
    }

    internal void OepnPopup()
    {


        m_gameobject_Popup.SetActive(true);
    }
    internal void ClosePopup()
    {
        m_gameobject_Popup.SetActive(false);


    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}