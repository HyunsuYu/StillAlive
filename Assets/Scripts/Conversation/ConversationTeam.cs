using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 대화 씬에서 팀원 초상화들의 상호작용(호버, 클릭, 선택)을 관리하는 클래스.
/// 중앙 관리 함수(UpdateAllPortraitAppearances)를 통해 상태를 명확하게 제어합니다.
/// </summary>
public class ConversationTeam : MonoBehaviour
{
    [Header("UI 및 프리팹 설정")]
    [Tooltip("초상화들이 생성될 부모 Canvas")]
    [SerializeField] private Transform m_teamPortraitsContainer;
    [SerializeField] private float m_portraitSpacing = 200f;

    [Tooltip("초상화 선택 해제를 위한 전체 화면 투명 버튼")]
    [SerializeField] private Button m_deselectButton;

    [Header("연출 설정")]
    [Tooltip("마우스를 올렸을 때의 크기 배율")]
    [SerializeField] private float m_hoverScale = 1.1f;
    [Tooltip("평상시 크기 배율")]
    [SerializeField] private float m_normalScale = 1.0f;
    [Tooltip("마우스를 올렸을 때의 Tint 색상")]
    [SerializeField] private Color m_hoverColor = Color.white;
    [Tooltip("선택되었을 때의 Tint 색상")]
    [SerializeField] private Color m_normalColor = Color.white;
    [Tooltip("선택/호버되지 않았을 때의 Tint 색상")]
    [SerializeField] private Color m_darkColor = new Color(0.5f, 0.5f, 0.5f, 1f);


    [SerializeField] private float m_configPortraitsScale = 6f; 

    // 내부 상태 변수
    private List<(NPCPortrait portrait, CardData cardData)> m_teamMembers;

    private NPCPortrait m_currentHoveredPortrait;
    private NPCPortrait m_currentSelectedPortrait;

    public event Action<CardData> OnPortraitSelected;
    public event Action OnPortraitDeselected;

    /// <summary>
    /// 카드 데이터 리스트를 기반으로 팀 초상화들을 초기화합니다.
    /// </summary>
    public void Init(List<CardData> cardDatas)
    {
        int memberCount = cardDatas.Count;
        m_teamMembers = new List<(NPCPortrait portrait, CardData cardData)>();
        
        // 카드 데이터만큼 초상화 생성
        for (int i = 0; i < cardDatas.Count; i++)
        {           
            GameObject portraitObj = CharacterPortraitHelper.CreatePortrait(cardDatas[i], m_teamPortraitsContainer);
            if (portraitObj != null)
            {
                NPCPortrait portrait = portraitObj.GetComponent<NPCPortrait>();
 
                portrait.transform.localScale = Vector3.one * m_configPortraitsScale;

                SetupPortraitEvents(portrait);
                m_teamMembers.Add((portrait, cardDatas[i]));
            }
        }

        // 선택 해제 버튼 이벤트 등록
        if (m_deselectButton != null)
        {
            m_deselectButton.onClick.AddListener(OnDeselect);
        }

        // 모든 초상화의 초기 상태(어둡게)를 업데이트
        UpdateAllPortraitAppearances();

    }

    // 이벤트 핸들러: 상태 변수 변경 후 전체 업데이트 호출

    private void OnPortraitHoverEnter(NPCPortrait portrait)
    {
        m_currentHoveredPortrait = portrait;
        UpdateAllPortraitAppearances();
    }

    private void OnPortraitHoverExit(NPCPortrait portrait)
    {
        if (m_currentHoveredPortrait == portrait)
        {
            m_currentHoveredPortrait = null;
        }
        UpdateAllPortraitAppearances();
    }

    private void OnPortraitClick(NPCPortrait portrait)
    {
        // 이미 선택된 것을 다시 클릭하면 선택 해제, 아니면 새로 선택
        m_currentSelectedPortrait = (m_currentSelectedPortrait == portrait) ? null : portrait;
        UpdateAllPortraitAppearances();

        if (m_currentSelectedPortrait != null)
        {
            // 선택된 캐릭터의 CardData와 함께 '선택됨' 이벤트를 방송
            OnPortraitSelected?.Invoke(GetSelectedCardData());
        }
        else
        {
            // '선택 해제됨' 이벤트를 방송
            OnPortraitDeselected?.Invoke();
        }
    }

    /// <summary>
    /// 배경 클릭 시 모든 선택을 해제합니다.
    /// </summary>
    private void OnDeselect()
    {
        m_currentSelectedPortrait = null;
        UpdateAllPortraitAppearances();

        OnPortraitDeselected?.Invoke();
    }

    /// <summary>
    /// 모든 초상화의 현재 상태(선택, 호버, 기본)를 기준으로 시각적 표현을 한 번에 업데이트합니다.
    /// </summary>
    private void UpdateAllPortraitAppearances()
    {
        foreach (var member in m_teamMembers)
        {
            if (member.portrait == null) continue;

            if (member.portrait == m_currentSelectedPortrait)
            {
                // 1. 선택된 포트레이트: 크고 밝게
                SetPortraitAppearance(member.portrait, m_hoverScale, m_normalColor);
            }
            else if (member.portrait == m_currentHoveredPortrait)
            {
                // 2. 호버된 포트레이트: 크고 밝게 (호버 색상)
                SetPortraitAppearance(member.portrait, m_hoverScale, m_hoverColor);
            }
            else
            {
                // 3. 둘 다 아닌 기본 상태: 작고 어둡게
                SetPortraitAppearance(member.portrait, m_normalScale, m_darkColor);
            }
        }
    }

    /// <summary>
    /// 지정된 포트레이트의 크기와 색상을 설정하는 Helper 함수.
    /// </summary>
    private void SetPortraitAppearance(NPCPortrait portrait, float scale, Color color)
    {
        portrait.transform.localScale = Vector3.one * scale * m_configPortraitsScale;

        // NPCPortrait의 모든 Image 자식들을 순회하며 Tint 색상(_Color) 변경
        Image[] images = portrait.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            // 머티리얼이 동적으로 생성되었는지, _Color 프로퍼티가 있는지 확인
            if (img.material != null && img.material.HasProperty("_Color"))
            {
                img.material.SetColor("_Color", color);
            }
        }
    }

    // --- 이벤트 트리거 설정 및 데이터 조회 ---

    private void SetupPortraitEvents(NPCPortrait portrait)
    {
        EventTrigger eventTrigger = portrait.gameObject.GetComponent<EventTrigger>() ?? portrait.gameObject.AddComponent<EventTrigger>();
        eventTrigger.triggers.Clear();

        // Pointer Enter (호버 시작)
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => OnPortraitHoverEnter(portrait));
        eventTrigger.triggers.Add(pointerEnter);

        // Pointer Exit (호버 종료)
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => OnPortraitHoverExit(portrait));
        eventTrigger.triggers.Add(pointerExit);

        // Pointer Click (클릭)
        EventTrigger.Entry pointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        pointerClick.callback.AddListener((data) => OnPortraitClick(portrait));
        eventTrigger.triggers.Add(pointerClick);
    }

    public CardData GetSelectedCardData()
    {
        if (m_currentSelectedPortrait != null)
        {
            var selectedMember = m_teamMembers.Find(member => member.portrait == m_currentSelectedPortrait);
            // CardData는 struct이므로 null 체크 대신 portrait 존재 여부로 확인
            if (selectedMember.portrait != null)
            {
                return selectedMember.cardData;
            }
        }
        return default; // CardData가 struct이므로 default를 반환
    }
}