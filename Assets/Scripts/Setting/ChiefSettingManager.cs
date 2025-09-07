using UnityEngine;

using CommonUtilLib.ThreadSafe;
using UnityEngine.SceneManagement;


public sealed class ChiefSettingManager : SingleTonForGameObject<ChiefSettingManager>
{
    public enum FromPageType
    {
        Title,
        Map,
        Combat,
        Conversation,
        Explolor
    }


    [SerializeField] private GameObject m_layout_ScreenTab;
    [SerializeField] private GameObject m_layout_AudioTab;

    private static FromPageType m_fromPageType;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        RenderAll();

        OpenScreenTab();
    }

    internal static FromPageType FromPage
    {
        set
        {
            m_fromPageType = value;
        }
    }

    #region Unity Callbacks
    public void Back2Page()
    {
        switch(m_fromPageType)
        {
            case FromPageType.Title:
                SceneManager.LoadScene("Title");
                break;

            case FromPageType.Map:
                SceneManager.LoadScene("Map");
                break;

            case FromPageType.Combat:
                SceneManager.LoadScene("Combat");
                break;

            case FromPageType.Conversation:
                SceneManager.LoadScene("Conversation");
                break;

            case FromPageType.Explolor:
                SceneManager.LoadScene("Explolor");
                break;
        }
    }

    public void OpenScreenTab()
    {
        m_layout_ScreenTab.SetActive(true);
        m_layout_AudioTab.SetActive(false);
    }
    public void OpenAudioTab()
    {
        m_layout_ScreenTab.SetActive(false);
        m_layout_AudioTab.SetActive(true);
    }
    #endregion

    internal void RenderAll()
    {
        ScreenSettingControl.Instance.Render();
        AudioSettingCOntrol.Instance.Render();

        SettingInitializer.Instance.Initialize();
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}