using UnityEngine;


public sealed class InternetWarningControl : MonoBehaviour
{
    [SerializeField] private GameObject m_layout_WarnPopup;


    public void Update()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            m_layout_WarnPopup.SetActive(true);
        }
        else
        {
            m_layout_WarnPopup.SetActive(false);
        }
    }

    #region Unity Callbacks
    public void QuitGameWithForce()
    {
        Application.Quit();
    }
    #endregion
}