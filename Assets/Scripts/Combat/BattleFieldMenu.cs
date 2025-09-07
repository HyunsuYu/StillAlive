using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleFieldMenu : MonoBehaviour
{
    [SerializeField] private Button m_battleStartBT;
    [SerializeField] private Button m_infoBT;
    [SerializeField] private Button m_invenBT;

    [SerializeField] private TMP_Text m_coinText;
    [SerializeField] private TMP_Text m_dayText;

    [Header("팀 상태창")]
    [SerializeField] private Transform m_teamStatusParent;     // 팀 상태창 부모 오브젝트
    [SerializeField] private GameObject m_teamMemberUIPrefab;  // 팀원 UI 프리팹 (초상화 + HP바)
    [SerializeField] private float m_teamUISpacing;

    private List<GameObject> m_teamMemberUIs;// 생성된 팀원 UI들

    private void Start()
    {
        m_dayText.text = $"D - {SaveDataBuffer.Instance.Data.DPlusDay}";
        m_coinText.text = SaveDataBuffer.Instance.Data.Money.ToString();

        m_teamMemberUIs = new List<GameObject>();
    }

    public void ConsumeCoin(int _amount)
    {

    }

    /// <summary>
    /// 팀 전체 상태창을 초기화 합니다.
    /// </summary>
    /// <param name="teamCards">팀 카드 리스트</param>
    public void InitTeamStatus(List<BattleCard> teamCards)
    {
        if (teamCards == null || m_teamStatusParent == null) return;

        int memberCount = teamCards.Count;

        // 각 팀원에 대해 UI 생성 및 위치 설정
        for (int i = 0; i < memberCount; i++)
        {
            if (teamCards[i] != null)
            {
                // m_teamStatusParent의 중심에서 시작해서 오른쪽으로 간격만큼 배치
                Vector3 memberUIPosition = new Vector3(i * m_teamUISpacing, 0, 0);

                // 팀원 UI 인스턴스 생성
                GameObject memberUI = Instantiate(m_teamMemberUIPrefab, m_teamStatusParent);
                m_teamMemberUIs.Add(memberUI);

                memberUI.transform.localPosition = memberUIPosition;

                // 초상화 생성
                CreateTeamMemberPortrait(teamCards[i], memberUI);

                // HP바와 설정
                UpdateTeamMemberStatus(teamCards[i], i);
            }
        }
    }

    /// <summary>
    /// 특정 팀원의 상태를 업데이트 현재는 Hp바
    /// </summary>
    /// <param name="teamCard">업데이트할 팀 카드</param>
    /// <param name="index">팀에서의 인덱스</param>
    public void UpdateTeamMemberStatus(BattleCard teamCard, int index, bool isRemove = false)
    {
        if (teamCard == null || index < 0 || index >= m_teamMemberUIs.Count) return;

        GameObject memberUI = m_teamMemberUIs[index];
        if (memberUI == null)
            return;

        if (isRemove)
        {
            Destroy(memberUI);
            return;
        }

        // HP바 업데이트
        Slider hpBar = memberUI.GetComponentInChildren<Slider>();

        float hpRatio = (float)teamCard.GetCurrentHP() / teamCard.GetMaxHP();
        if (hpRatio <= 0f)
        {
            hpBar.value = 0f;
        }
        hpBar.value = hpRatio;


        //// HP 텍스트 업데이트
        //TMP_Text hpText = memberUI.GetComponentInChildren<TMP_Text>();
        //if (hpText != null)
        //{
        //    hpText.text = $"{teamCard.GetCurrentHP()}/{teamCard.GetMaxHP()}";
        //}        
    }

    /// <summary>
    /// 팀원의 초상화를 생성, 원본 카드의 초상화 클론 사용
    /// </summary>
    /// <param name="teamCard">팀 카드 오브젝트</param>
    /// <param name="memberUI">팀원 UI 오브젝트</param>
    private void CreateTeamMemberPortrait(BattleCard teamCard, GameObject memberUI)
    {
        Transform portraitParent = memberUI.transform;
        if (portraitParent == null)
            return;

        // 원본 카드의 초상화 인스턴스 가져오기
        NPCPortrait originalPortrait = teamCard.GetPortraitInstance();
        if (originalPortrait != null)
        {
            // 원본 초상화를 클론하여 UI에 배치
            GameObject portraitClone = Instantiate(originalPortrait.gameObject, portraitParent);
            portraitClone.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
