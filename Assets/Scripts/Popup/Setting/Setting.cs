using UnityEngine;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
    [SerializeField] private ChiefSettingManager.FromPageType m_CurPage;

    public void Awake()
    {
        ChiefSettingManager.FromPage = m_CurPage;
    }

    public void OpenSetting()
    {
        SceneManager.LoadScene("Setting");
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
