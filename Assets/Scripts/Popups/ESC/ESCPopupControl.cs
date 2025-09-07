using UnityEngine;
using UnityEngine.SceneManagement;


public sealed class ESCPopupControl : MonoBehaviour
{
    [SerializeField] private GameObject m_layout_ESCPopup;

    [SerializeField] private ChiefSettingManager.FromPageType m_fromPageType;


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            m_layout_ESCPopup.SetActive(!m_layout_ESCPopup.activeSelf);
        }
    }

    #region Unity Callbacks
    public void OpenSetting()
    {
        ChiefSettingManager.FromPage = m_fromPageType;
        SceneManager.LoadScene("Setting");
    }
    public void OpenTitle()
    {
        SceneManager.LoadScene("Title");
    }
    #endregion
}