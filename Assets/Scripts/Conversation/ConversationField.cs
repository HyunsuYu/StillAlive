using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ȭâ�� �����ϴ� Ŭ����
/// </summary>
public class ConversationField : MonoBehaviour
{
    [SerializeField] private ConversationMenu m_conversationMenu;
    [SerializeField] private ConversationTeam m_conversationField;
    
    

    public void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();

        // ���߿� ���͸� �� ���� --> 
        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;

        List<NPCPortrait> npcPortraits = m_conversationMenu.InitTeamStatus(cardDatas);



    }

}
