using UnityEngine;

public class Button : MonoBehaviour
{
    #region INVEN
    public void Inven()
    {
        if(!InvenControl.Instance.m_bisInvenOpened)
        {
            InvenControl.Instance.OpenInven();

            InvenControl.Instance.m_bisInvenOpened = true;
        }
        else
        {
            InvenControl.Instance.CloseInven();

            InvenControl.Instance.m_bisInvenOpened = false;
        }
    }
    #endregion

    #region INFO
    public void Info()
    {
        if(!InfoControl.Instance.m_bisInfoOpened)
        {
            if(InvenControl.Instance.m_bisInvenOpened)
            {
                InvenControl.Instance.CloseInven();
                InvenControl.Instance.m_bisInvenOpened = false;
            }

            InfoControl.Instance.OpenInfo();
            RadioInfoControl.Instance.OpenRadioInfo();

            InfoControl.Instance.m_bisInfoOpened = true;
            RadioInfoControl.Instance.m_bisRadioInfoOpened = true;
            PopupControl.Instance.m_bisOtherPopupOpened = true;
        }
        else
        {
            if(RadioInfoControl.Instance.m_bisRadioInfoOpened)
            {
                RadioInfoControl.Instance.CloseRadioInfo();
                RadioInfoControl.Instance.m_bisRadioInfoOpened = false;
            }
            else if(ConversationInfoControl.Instance.m_bisConversationInfoOpened)
            {
                ConversationInfoControl.Instance.CloseConversationInfo();
                ConversationInfoControl.Instance.m_bisConversationInfoOpened = false;
            }

            InfoControl.Instance.CloseInfo();
            
            InfoControl.Instance.m_bisInfoOpened = false;
            PopupControl.Instance.m_bisOtherPopupOpened = false;
        }
    }

    public void Radio()
    {
        if(ConversationInfoControl.Instance.m_bisConversationInfoOpened)
        {
            ConversationInfoControl.Instance.CloseConversationInfo();
            ConversationInfoControl.Instance.m_bisConversationInfoOpened = false;

            RadioInfoControl.Instance.OpenRadioInfo();
            RadioInfoControl.Instance.m_bisRadioInfoOpened = true;
        }
        else
        {
            RadioInfoControl.Instance.OpenRadioInfo();
            RadioInfoControl.Instance.m_bisRadioInfoOpened = true;
        }
    }

    public void Conversation()
    {
        if (RadioInfoControl.Instance.m_bisRadioInfoOpened)
        {
            RadioInfoControl.Instance.CloseRadioInfo();
            RadioInfoControl.Instance.m_bisRadioInfoOpened = false;

            ConversationInfoControl.Instance.OpenConversationInfo();
            ConversationInfoControl.Instance.m_bisConversationInfoOpened = true;
        }
        else
        {
            ConversationInfoControl.Instance.OpenConversationInfo();
            ConversationInfoControl.Instance.m_bisConversationInfoOpened = true;
        }
    }
    #endregion
}
