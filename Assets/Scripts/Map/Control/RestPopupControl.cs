using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;
using UnityEngine.SceneManagement;


public sealed class RestPopupControl : SingleTonForGameObject<RestPopupControl>
{
    [SerializeField] private GameObject m_gameobject_Popup;

    [SerializeField] private RestCharacterItem[] m_restCharacterItems;


    public void Awake()
    {
        SetInstance(this);
    }

    #region Unity Callbacks
    public void OepnPopup()
    {
        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;
        List<int> alivedCards = new List<int>();
        for(int index = 0; index < cardDatas.Count; index++)
        {
            if (cardDatas[index].Status.CurHP >= 1)
            {
                alivedCards.Add(index);
            }
        }

        for (int index = 0; index < m_restCharacterItems.Length; index++)
        {
            m_restCharacterItems[index].gameObject.SetActive(false);
            if(index < alivedCards.Count)
            {
                m_restCharacterItems[index].gameObject.SetActive(true);
                m_restCharacterItems[index].Init(alivedCards[index]);
            }
        }

        m_gameobject_Popup.SetActive(true);
    }
    public void ClosePopup()
    {
        m_gameobject_Popup.SetActive(false);

        SceneManager.LoadScene("Map");
    }
    #endregion

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}