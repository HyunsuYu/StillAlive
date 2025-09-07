using CommonUtilLib.ThreadSafe;
using UnityEngine;

public class InvenControl : SingleTonForGameObject<InvenControl>
{
    [SerializeField] private GameObject m_Inven;

    private static bool m_bisInvenOpen = false;

    internal bool m_bisInvenOpened
    {
        get { return m_bisInvenOpen; }
        set { m_bisInvenOpen = value; }
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
        if(m_Inven != null)
        {
            m_Inven.SetActive(false);
        }
    }

    internal void OpenInven()
    {
        m_Inven.SetActive(true);
    }

    internal void CloseInven()
    {
        m_Inven.SetActive(false);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
