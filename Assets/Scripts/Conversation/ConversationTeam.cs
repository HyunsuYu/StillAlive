using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static NPCLookPart;

/// <summary>
/// 대화 플레이어 관련 대사나, 초상화 존재
/// </summary>
public class ConversationTeam : MonoBehaviour
{
    [Header("플레이어 진영 설정")]
    [SerializeField] private Transform m_teamPortraitsAnchor;     // 플레이어 진영 카드 중심 앵커값 
    [SerializeField] private float m_teamPortraitsSpacing = 1.5f;  // 카드 범위 넓이

    private List<NPCPortrait> teamPortraits;

    public List<NPCPortrait> Init(List<CardData> cardDatas)
    {
        int memberCount = cardDatas.Count;
        List<NPCPortrait> returnPortraits = new List<NPCPortrait>();

        float totalWidth = (memberCount - 1) * m_teamPortraitsSpacing;
        Vector3 startPosition = m_teamPortraitsAnchor.position - new Vector3(totalWidth / 2f, 0, 0);
        // 각 팀원에 대해 UI 생성 및 위치 설정
        for (int i = 0; i < memberCount; i++)
        {
            Vector3 cardPosition = startPosition + new Vector3(i * m_teamPortraitsSpacing, 0, 0);

            // CreatePortrait에 parent를 전달하여 생성
            GameObject portraitObj = CharacterPortraitHelper.CreatePortrait(cardDatas[i], m_teamPortraitsAnchor);
            if (portraitObj != null)
            {
                NPCPortrait portrait = portraitObj.GetComponent<NPCPortrait>();
                portrait.transform.position = cardPosition;
                returnPortraits.Add(portrait);
                teamPortraits.Add(portrait);
            }
        }
        return returnPortraits;
    }
}
