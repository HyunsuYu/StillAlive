using CommonUtilLib.ThreadSafe;
using UnityEngine;

public class PopupManager : SingleTonForGameObject<PopupManager>
{
    [SerializeField] private GameObject m_ESC;
    [SerializeField] private GameObject m_INVEN;
    [SerializeField] private GameObject m_RADIO;
    [SerializeField] private GameObject m_CONVERSATION;
    [SerializeField] private GameObject m_INFOPENAL;

    private static bool bisOtherPopupOpen = false;
    private static bool bisESCOpen = false;
    private static bool bisINVENOpen = false;
    private static bool bisRADIOOpen = false;
    private static bool bisCONVERSATIONOpen = false;
    private static bool bisINFOPENALOpen = false;

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

    internal bool m_bisRADIOOpen
    {
        get { return bisRADIOOpen; }
        set { bisRADIOOpen = value; }
    }

    internal bool m_bisCONVERSATIONOpen
    {
        get { return bisCONVERSATIONOpen; }
        set { bisCONVERSATIONOpen = value; }
    }

    internal bool m_bisINFOPENALOpen
    {
        get { return bisINFOPENALOpen; }
        set { bisINFOPENALOpen = value; }
    }

    internal bool m_bisOtherPopupOpen
    {
        get { return bisOtherPopupOpen; }
        set { bisOtherPopupOpen = value; }
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

        if(m_INFOPENAL != null)
        {
            m_INFOPENAL.SetActive(false);
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

    internal void INFOPENALOpen()
    {
        m_INFOPENAL.SetActive(true);
    }

    internal void INFOPENALClose()
    {
        m_INFOPENAL.SetActive(false);
    }    
    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
