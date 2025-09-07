using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static NPCLookPart;

/// <summary>
/// ��ȭ �÷��̾� ���� ��糪, �ʻ�ȭ ����
/// </summary>
public class ConversationTeam : MonoBehaviour
{
    [Header("�÷��̾� ���� ����")]
    [SerializeField] private Canvas m_teamPortraitsDrawCanvas;

    private List<NPCPortrait> teamPortraits;

    public List<NPCPortrait> Init(List<CardData> cardDatas)
    {
        int memberCount = cardDatas.Count;
        teamPortraits = new List<NPCPortrait>();

        // �� ������ ���� UI ���� �� ��ġ ����
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
