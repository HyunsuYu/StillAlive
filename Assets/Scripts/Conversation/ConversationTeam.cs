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
    [SerializeField] private Canvas m_teamPortraitsDrawCanvas;

    private List<NPCPortrait> teamPortraits;

    public List<NPCPortrait> Init(List<CardData> cardDatas)
    {
        int memberCount = cardDatas.Count;
        teamPortraits = new List<NPCPortrait>();

        // 각 팀원에 대해 UI 생성 및 위치 설정
        for (int i = 0; i < memberCount; i++)
        {

            GameObject portraitObj = CharacterPortraitHelper.CreatePortrait(cardDatas[i]);
            if (portraitObj != null)
            {
                NPCPortrait portrait = portraitObj.GetComponent<NPCPortrait>();
                portrait.transform.SetParent(m_teamPortraitsDrawCanvas.transform);
                portrait.transform.localScale = new Vector3(7f, 7f, 7f);
                teamPortraits.Add(portrait);
            }
        }
        return teamPortraits;
    }
}
