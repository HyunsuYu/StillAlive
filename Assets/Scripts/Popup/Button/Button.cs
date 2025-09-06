using UnityEngine;

public class Button : MonoBehaviour
{
    #region INVEN
    public void Inven()
    {
        if(!PopupManager.Instance.m_bisINVENOpen)
        {
            PopupManager.Instance.INVENOpen();

            PopupManager.Instance.m_bisINVENOpen = true;
        }
        else
        {
            PopupManager.Instance.INVENClose();

            PopupManager.Instance.m_bisINVENOpen = false;
        }
    }
    #endregion
}
