using CommonUtilLib.ThreadSafe;
using UnityEngine;

public class ConversationInfoControl : SingleTonForGameObject<ConversationInfoControl>
{
    [SerializeField] private GameObject m_ConversationInfo;

    private static bool m_bisConversationInfoOpen;

    internal bool m_bisConversationInfoOpened
    {
        get { return m_bisConversationInfoOpen; }
        set { m_bisConversationInfoOpen = value; }
    }

    public void Awake()
    {
        SetInstance(this);
    }

    public void Start()
    {
        Init();
    }

    private void Init()
    {
        if(m_ConversationInfo != null)
        {
            m_ConversationInfo.SetActive(false);
        }
    }

    internal void OpenConversationInfo()
    {
        m_ConversationInfo.SetActive(true);
    }

    internal void CloseConversationInfo()
    {
        m_ConversationInfo.SetActive(false);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
