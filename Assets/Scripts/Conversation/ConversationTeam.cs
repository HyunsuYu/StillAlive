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
    [SerializeField] private Transform m_teamPortraitsAnchor;     // �÷��̾� ���� ī�� �߽� ��Ŀ�� 
    [SerializeField] private float m_teamPortraitsSpacing = 1.5f;  // ī�� ���� ����

    private List<NPCPortrait> teamPortraits;

    public List<NPCPortrait> Init(List<CardData> cardDatas)
    {
        int memberCount = cardDatas.Count;
        List<NPCPortrait> returnPortraits = new List<NPCPortrait>();

        float totalWidth = (memberCount - 1) * m_teamPortraitsSpacing;
        Vector3 startPosition = m_teamPortraitsAnchor.position - new Vector3(totalWidth / 2f, 0, 0);
        // �� ������ ���� UI ���� �� ��ġ ����
        for (int i = 0; i < memberCount; i++)
        {
            Vector3 cardPosition = startPosition + new Vector3(i * m_teamPortraitsSpacing, 0, 0);

            // CreatePortrait�� parent�� �����Ͽ� ����
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
