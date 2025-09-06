using CommonUtilLib.ThreadSafe;
using UnityEngine;

public class PopupManager : SingleTonForGameObject<PopupManager>
{
    [SerializeField] private GameObject m_ESC;
    [SerializeField] private GameObject m_INVEN;
    [SerializeField] private GameObject m_SETTING;

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

        if(m_SETTING != null)
        {
            m_SETTING.SetActive(false);
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
    internal void SETTINGOpen()
    {
        m_SETTING.SetActive(true);
    }

    internal void SETTINGClose()
    {
        m_SETTING.SetActive(false);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
