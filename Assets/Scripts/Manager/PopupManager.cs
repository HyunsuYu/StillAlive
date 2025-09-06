using CommonUtilLib.ThreadSafe;
using UnityEngine;

public class PopupManager : SingleTonForGameObject<PopupManager>
{
    [SerializeField] private GameObject m_ESC;
    [SerializeField] private GameObject m_INVEN;
    [SerializeField] private GameObject m_RADIO;
    [SerializeField] private GameObject m_CONVERSATION;

    private static bool bisESCOpen = false;
    private static bool bisINVENOpen = false;

    internal bool m_bisESCOpen
    {
        get { return bisESCOpen; }
        set { bisESCOpen = value; }
    }

    internal bool m_bisINVENOpen
    {
        get { return bisINVENOpen; }
        set { bisINVENOpen = value; }
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
        if(m_ESC != null)
        {
            m_ESC.SetActive(false);
        }

        if(m_INVEN != null)
        {
            m_INVEN.SetActive(false);
        }

        if(m_RADIO != null)
        {
            m_RADIO.SetActive(false);
        }    

        if(m_CONVERSATION != null)
        {
            m_CONVERSATION.SetActive(false);
        }
    }

    internal void ESCOpen()
    {
        m_ESC.SetActive(true);
    }

    internal void ESCClose()
    {
        m_ESC.SetActive(false);
    }

    internal void INVENOpen()
    {
        m_INVEN.SetActive(true);
    }

    internal void INVENClose()
    {
        m_INVEN.SetActive(false);
    }

    internal void RADIOOpen()
    {
        m_RADIO.SetActive(true);
    }

    internal void RADIOClose()
    {
        m_RADIO.SetActive(false);
    }

    internal void CONVERSATIONOpen()
    {
        m_CONVERSATION.SetActive(true);
    }

    internal void CONVERSATIONClose()
    {
        m_CONVERSATION.SetActive(false);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
