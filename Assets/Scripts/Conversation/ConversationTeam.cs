using System.Collections.Generic;
using UnityEngine;

public class ConversationTeam : MonoBehaviour
{
    [Header("팀 플레이어 카드 설정")]
    [SerializeField] private Transform m_teamFieldAnchor;     // 팀 플레이어 카드 배치 앵커 
    [SerializeField] private float m_teamCardSpacing = 1.5f;  // 카드 간격 설정

    private List<CharacterPortraitHelper.PortraitData> m_teamMembers = new List<CharacterPortraitHelper.PortraitData>();

    /// <summary>
    /// 팀 멤버들을 초기화합니다.
    /// </summary>
    /// <param name="cardDataList">팀 멤버들의 카드 데이터 리스트</param>
    public void Init(List<CardData> cardDataList)
    {
        ClearTeamMembers();

        if (cardDataList == null || m_teamFieldAnchor == null) return;

        for (int i = 0; i < cardDataList.Count; i++)
        {
            CardData cardData = cardDataList[i];
            
            // Portrait 생성
            GameObject portraitObj = CharacterPortraitHelper.CreatePortrait(cardData, m_teamFieldAnchor);
            if (portraitObj != null)
            {
                NPCPortrait portrait = portraitObj.GetComponent<NPCPortrait>();
                if (portrait != null)
                {
                    // 위치 설정
                    Vector3 position = m_teamFieldAnchor.position + Vector3.right * (i * m_teamCardSpacing);
                    portraitObj.transform.position = position;

                    // PortraitData 생성 및 추가
                    CharacterPortraitHelper.PortraitData portraitData = new CharacterPortraitHelper.PortraitData(
                        portrait, 
                        cardData, 
                        $"TeamMember_{i}"
                    );
                    m_teamMembers.Add(portraitData);
                }
            }
        }
    }

    /// <summary>
    /// 특정 팀 멤버의 부위 설명을 가져옵니다.
    /// </summary>
    /// <param name="memberIndex">팀 멤버 인덱스</param>
    /// <param name="partType">부위 타입</param>
    /// <returns>해당 부위의 Description</returns>
    public string GetMemberPartDescription(int memberIndex, CardData.NPCLookPartType partType)
    {
        if (memberIndex < 0 || memberIndex >= m_teamMembers.Count) return null;
        
        CharacterPortraitHelper.PortraitData memberData = m_teamMembers[memberIndex];
        return CharacterPortraitHelper.GetPartDescription(memberData.Portrait, memberData.CardData, partType);
    }

    /// <summary>
    /// 특정 팀 멤버의 모든 부위 설명을 가져옵니다.
    /// </summary>
    /// <param name="memberIndex">팀 멤버 인덱스</param>
    /// <returns>부위별 Description Dictionary</returns>
    public Dictionary<CardData.NPCLookPartType, string> GetMemberAllPartDescriptions(int memberIndex)
    {
        if (memberIndex < 0 || memberIndex >= m_teamMembers.Count) 
            return new Dictionary<CardData.NPCLookPartType, string>();
        
        CharacterPortraitHelper.PortraitData memberData = m_teamMembers[memberIndex];
        return CharacterPortraitHelper.GetAllPartDescriptions(memberData.Portrait, memberData.CardData);
    }

    /// <summary>
    /// 팀 멤버의 PortraitData를 가져옵니다.
    /// </summary>
    /// <param name="memberIndex">팀 멤버 인덱스</param>
    /// <returns>PortraitData, 없으면 null</returns>
    public CharacterPortraitHelper.PortraitData? GetMemberData(int memberIndex)
    {
        if (memberIndex < 0 || memberIndex >= m_teamMembers.Count) return null;
        return m_teamMembers[memberIndex];
    }

    /// <summary>
    /// 팀 멤버 수를 반환합니다.
    /// </summary>
    public int GetMemberCount()
    {
        return m_teamMembers.Count;
    }

    /// <summary>
    /// 기존 팀 멤버들을 모두 제거합니다.
    /// </summary>
    private void ClearTeamMembers()
    {
        foreach (var memberData in m_teamMembers)
        {
            if (memberData.Portrait != null && memberData.Portrait.gameObject != null)
            {
                Destroy(memberData.Portrait.gameObject);
            }
        }
        m_teamMembers.Clear();
    }

    private void OnDestroy()
    {
        ClearTeamMembers();
    }
}