using UnityEngine;

public class ESC : MonoBehaviour
{
    public void Update()
    {
        if (PopupControl.Instance.m_bisOtherPopupOpened) return;
        ESCPopup();
    }

    private void ESCPopup()
    {
        if(!ESCControl.Instance.m_bisESCOpened)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (InvenControl.Instance.m_bisInvenOpened)
                {
                    InvenControl.Instance.CloseInven();
                    InvenControl.Instance.m_bisInvenOpened = false;
                }

                ESCControl.Instance.OpenESC();
                ESCControl.Instance.m_bisESCOpened = true;
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                ESCControl.Instance.CloseESC();
                ESCControl.Instance.m_bisESCOpened = false;
            }
        }
    }
}
