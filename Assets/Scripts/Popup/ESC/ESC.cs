using UnityEngine;

public class ESC : MonoBehaviour
{
    public void Update()
    {
        ESCPopup();
    }

    private void ESCPopup()
    {
        if(!PopupManager.Instance.m_bisESCOpen)
        {
            PopupManager.Instance.ESCOpen();

            PopupManager.Instance.m_bisESCOpen = true;
        }
        else
        {
            PopupManager.Instance.ESCClose();

            PopupManager.Instance.m_bisESCOpen = false;
        }
    }
}
