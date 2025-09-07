using CommonUtilLib.ThreadSafe;
using UnityEngine;

public class ESCControl : SingleTonForGameObject<ESCControl>
{
    [SerializeField] private GameObject m_ESC;

    private static bool m_bisESCOpen = false;

    internal bool m_bisESCOpened
    {
        get { return m_bisESCOpen; }
        set { m_bisESCOpen = value; }
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
        if (m_ESC != null)
        {
            m_ESC.SetActive(false);
        }
    }

    internal void OpenESC()
    {
        m_ESC.SetActive(true);
    }

    internal void CloseESC()
    {
        m_ESC.SetActive(false);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
