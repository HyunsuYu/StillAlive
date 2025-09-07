using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대화창을 관리하는 클래스
/// </summary>
public class ConversationField : MonoBehaviour
{
    [SerializeField] private ConversationMenu m_conversationMenu;
    
    List<NPCPortrait> npcPortraits;
    public void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();

        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;

        npcPortraits = m_conversationMenu.InitTeamStatus(cardDatas);



    }

}
