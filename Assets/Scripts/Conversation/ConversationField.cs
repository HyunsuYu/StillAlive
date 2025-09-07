using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대화창을 관리하는 클래스
/// </summary>
public class ConversationField : MonoBehaviour
{
    [SerializeField] private ConversationMenu m_conversationMenu;
    [SerializeField] private ConversationTeam m_conversationField;
    
    

    public void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();

        // 나중에 필터링 할 예정 --> 
        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;

        List<NPCPortrait> npcPortraits = m_conversationMenu.InitTeamStatus(cardDatas);



    }

}
