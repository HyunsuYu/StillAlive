using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대화관련된 로직을 관리하는 클래스
/// </summary>
public class ConversationField : MonoBehaviour
{
    [SerializeField] private ConversationMenu m_conversationMenu;
    [SerializeField] private ConversationTeam m_conversationTeam;
    
    private void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();
    
        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;
        
        if(cardDatas == null)
        {
            Debug.LogError("CardDatas is null");
            return;
        }

        List<NPCPortrait> npcPortraits = m_conversationTeam.Init(cardDatas);

        m_conversationMenu.InitTeamStatus(npcPortraits, cardDatas);
    }
}