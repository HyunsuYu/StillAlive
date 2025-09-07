using UnityEngine;


public sealed class IntelInfosPopupControl : MonoBehaviour
{
    [SerializeField] private GameObject m_layout_IntelInfosPopup;

    [SerializeField] private GameObject m_layout_RadioIntelParent;
    [SerializeField] private GameObject m_layout_ConversationIntelParent;

    [Header("Related with Button")]
    [SerializeField] private Transform m_transform_UpperBtnParent;
    [SerializeField] private Transform m_transfomr_LowerBtnParent;

    [SerializeField] private GameObject m_gameObject_OpenRadioTabBtn;
    [SerializeField] private GameObject m_gameObject_OepnConversationTabBtn;

    [Header("For Radio")]
    [SerializeField] private GameObject m_prefab_RadioIntelItem;
    [SerializeField] private GameObject m_prefab_Line;
    [SerializeField] private Transform m_transform_RadioIntelItemParent;

    [Header("For Conversation")]
    [SerializeField] private GameObject m_prefab_ConversationIntelItem;
    [SerializeField] private Transform m_transform_ConversationIntelItemParent;


    public void Start()
    {
        OpenRadioIntelTab();
    }

    #region Unity Callbacks
    public void OpenAndClosePopup()
    {
        m_layout_IntelInfosPopup.SetActive(!m_layout_IntelInfosPopup.activeSelf);
    }

    public void OpenRadioIntelTab()
    {
        m_gameObject_OpenRadioTabBtn.transform.SetParent(m_transform_UpperBtnParent);
        m_gameObject_OepnConversationTabBtn.transform.SetParent(m_transfomr_LowerBtnParent);

        m_layout_RadioIntelParent.SetActive(true);
        m_layout_ConversationIntelParent.SetActive(false);

        #region Render Radio
        int childCount = m_transform_RadioIntelItemParent.childCount;
        for(int index = childCount - 1; index >= 0; index--)
        {
            Destroy(m_transform_RadioIntelItemParent.GetChild(index));
        }


        #endregion
    }
    public void OpenConversationIntelTab()
    {
        m_gameObject_OpenRadioTabBtn.transform.SetParent(m_transfomr_LowerBtnParent);
        m_gameObject_OepnConversationTabBtn.transform.SetParent(m_transform_UpperBtnParent);

        m_layout_RadioIntelParent.SetActive(false);
        m_layout_ConversationIntelParent.SetActive(true);
    }
    #endregion
}