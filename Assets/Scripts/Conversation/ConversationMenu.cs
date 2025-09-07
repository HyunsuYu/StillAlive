using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationMenu : MonoBehaviour
{
    [SerializeField] private Button m_infoBT;
    [SerializeField] private Button m_invenBT;

    //[SerializeField] private TMP_Text m_coinText;
    [SerializeField] private TMP_Text m_dayText;

    [Header("팀 상태창")]
    [SerializeField] private Transform m_teamStatusParent;     // 팀 상태창 부모 오브젝트
    [SerializeField] private GameObject m_teamMemberUIPrefab;  // 팀원 UI 프리팹 (초상화 + HP바)
    [SerializeField] private float m_teamUISpacing;

    private List<GameObject> m_teamMemberUIs;// 생성된 팀원 UI들

    private void Start()
    {
        m_dayText.text = $"D - {SaveDataBuffer.Instance.Data.DPlusDay}";
        //m_coinText.text = SaveDataBuffer.Instance.Data.Money.ToString();

        m_teamMemberUIs = new List<GameObject>();
    }

    public void ConsumeCoin(int _amount)
    {

    }

    /// <summary>
    /// 팀 전체 상태창을 초기화 세팅을 합니다.
    /// </summary>
    /// <param name="teamPortrait">팀 카드 리스트</param>
    public void InitTeamStatus(List<NPCPortrait> portraits, List<CardData> datas)
    {
        int memberCount = portraits.Count;
     
        // 각 팀원에 대해 UI 생성 및 위치 설정
        for (int i = 0; i < memberCount; i++)
        {

            // m_teamStatusParent의 중심에서 시작해서 오른쪽으로 간격만큼 배치
            Vector3 memberUIPosition = new Vector3(i * m_teamUISpacing, 0, 0);

            // 팀원 UI 인스턴스 생성
            GameObject memberUI = Instantiate(m_teamMemberUIPrefab, m_teamStatusParent);
            m_teamMemberUIs.Add(memberUI);

            memberUI.transform.localPosition = memberUIPosition;

            // 초상화 생성
            Transform portraitParent = memberUI.transform;

          
            if (portraits[i] != null)
            {
                // 원본 초상화를 클론하여 UI에 배치
                Instantiate(portraits[i].gameObject, portraitParent);

                // HP바 업데이트
                Slider hpBar = memberUI.GetComponentInChildren<Slider>();

                float hpRatio = (float)datas[i].Status.CurHP / datas[i].Status.MaxHP;
                if (hpRatio <= 0f)
                {
                    hpBar.value = 0f;
                }
                hpBar.value = hpRatio;
            }
            else
            {
                Debug.LogError("Portrait 비어있음");
            }
        }
      

    }

}
