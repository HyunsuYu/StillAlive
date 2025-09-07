using CommonUtilLib.ThreadSafe;
using UnityEngine;

public class RadioInfoControl : SingleTonForGameObject<RadioInfoControl>
{
    [SerializeField] private GameObject m_RadioInfo;

    private static bool m_bisRadioInfoOpen;

    internal bool m_bisRadioInfoOpened
    {
        get { return m_bisRadioInfoOpen; }
        set { m_bisRadioInfoOpen = value; }
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
        if(m_RadioInfo != null)
        {
            m_RadioInfo.SetActive(false);
        }
    }

    internal void OpenRadioInfo()
    {
        m_RadioInfo.SetActive(true);
    }

    internal void CloseRadioInfo()
    {
        m_RadioInfo.SetActive(false);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
