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

    #region INFO
    public void InfoPenal()
    {
        if(!PopupManager.Instance.m_bisINFOPENALOpen)
        {
            if(PopupManager.Instance.m_bisINVENOpen)
            {
                PopupManager.Instance.INVENClose();
                PopupManager.Instance.m_bisINVENOpen = false;
            }

            PopupManager.Instance.INFOPENALOpen();

            PopupManager.Instance.m_bisINFOPENALOpen = true;
            PopupManager.Instance.m_bisOtherPopupOpen = true;
        }
        else
        {
            PopupManager.Instance.INFOPENALClose();

            PopupManager.Instance.m_bisINFOPENALOpen = false;
            PopupManager.Instance.m_bisOtherPopupOpen = false;
        }
    }

    public void Radio()
    {
        if(PopupManager.Instance.m_bisCONVERSATIONOpen)
        {
            PopupManager.Instance.CONVERSATIONClose();
            PopupManager.Instance.m_bisCONVERSATIONOpen = false;

            PopupManager.Instance.RADIOOpen();
            PopupManager.Instance.m_bisRADIOOpen = true;
        }
        else
        {
            PopupManager.Instance.RADIOOpen();
            PopupManager.Instance.m_bisRADIOOpen = true;
        }
    }

    public void Conversation()
    {
        if(PopupManager.Instance.m_bisRADIOOpen)
        {
            PopupManager.Instance.RADIOClose();
            PopupManager.Instance.m_bisRADIOOpen = false;

            PopupManager.Instance.CONVERSATIONOpen();
            PopupManager.Instance.m_bisCONVERSATIONOpen = true;
        }
        else
        {
            PopupManager.Instance.CONVERSATIONOpen();
            PopupManager.Instance.m_bisCONVERSATIONOpen = true;
        }
    }
    #endregion
}
