using UnityEngine;
using UnityEngine.SceneManagement;


public sealed class GameEndPopupControl : MonoBehaviour
{
    [SerializeField] private GameObject m_layout_GameEndPopup;


    public void FixedUpdate()
    {
        bool bisPlayerAlive = false;
        var aliveCards = SaveDataInterface.GetAliveCardInfos();
        foreach(var card in aliveCards)
        {
            if(card.BIsPlayer)
            {
                bisPlayerAlive = true;
                break;
            }
        }

        if(!bisPlayerAlive)
        {
            m_layout_GameEndPopup.SetActive(true);
        }
    }

    #region Unity Ccallbacks
    public void StartNewGame()
    {
        SaveDataBuffer.Instance.ClearSaveData();
        SceneManager.LoadScene("Map");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion
}