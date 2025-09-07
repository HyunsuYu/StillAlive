using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class InventoryPopupControl : SingleTonForGameObject<InventoryPopupControl>
{
    [SerializeField] private GameObject m_layout_InventoryPopup;


    public void Awake()
    {
        SetInstance(this);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}