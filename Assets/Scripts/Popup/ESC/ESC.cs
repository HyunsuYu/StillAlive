using UnityEngine;

public class ESC : MonoBehaviour
{
    public void Update()
    {
        if (PopupManager.Instance.m_bisOtherPopupOpen) return;
        ESCPopup();
    }

    private void ESCPopup()
    {
        if(!PopupManager.Instance.m_bisESCOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (PopupManager.Instance.m_bisINVENOpen)
                {
                    PopupManager.Instance.INVENClose();
                    PopupManager.Instance.m_bisINVENOpen = false;
                }

                PopupManager.Instance.ESCOpen();

                PopupManager.Instance.m_bisESCOpen = true;
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                PopupManager.Instance.ESCClose();

                PopupManager.Instance.m_bisESCOpen = false;
            }
        }
    }
}
