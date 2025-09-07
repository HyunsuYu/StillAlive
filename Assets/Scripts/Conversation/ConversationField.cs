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

    [SerializeField] private int testCardCount;
    private void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();
    
        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;

        if (cardDatas.Count == 0)
        {
            for (int i = 0; i < testCardCount; i++)
            {
                // 테스트
                CardData cd = new CardData();
                cd.NPCLookTable = new Dictionary<CardData.NPCLookPartType, int>();
                cd.ColorPalleteIndex = 0;
                cd.Status = CardData.DefaultStatus();
                cd.Status.MaxHP += 1;
                cd.Status.CurHP += 1;

                cardDatas.Add(cd);

                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Face] = Random.Range(0, 3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Eye] = Random.Range(0, 3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Glasses] = 0;
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Top] = Random.Range(0,3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.FrontHair] = Random.Range(0,3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.BackHair] = Random.Range(0,3);

            }
        }

        List<NPCPortrait> npcPortraits = m_conversationTeam.Init(cardDatas);

        m_conversationMenu.InitTeamStatus(npcPortraits, cardDatas);
    }
}