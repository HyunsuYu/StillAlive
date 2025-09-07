using CommonUtilLib.ThreadSafe;
using UnityEngine;

public class InfoControl : SingleTonForGameObject<InfoControl>
{
    [SerializeField] private GameObject m_Info;

    private static bool m_bisInfoOpen = false;

    internal bool m_bisInfoOpened
    {
        get { return m_bisInfoOpen; }
        set { m_bisInfoOpen = value; }
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
        if(m_Info != null)
        {
            m_Info.SetActive(false);
        }
    }

    internal void OpenInfo()
    {
        m_Info.SetActive(true);
    }

    internal void CloseInfo()
    {
        m_Info.SetActive(false);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
