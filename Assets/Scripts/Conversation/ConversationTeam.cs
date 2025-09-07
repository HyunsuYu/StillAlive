using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ��ȭ ������ ���� �ʻ�ȭ���� ��ȣ�ۿ��� '����'�� ���� �����ϴ� ���� ����.
/// (Selection, InDialogue, Voting)
/// </summary>
public class ConversationTeam : MonoBehaviour
{
    public enum TeamState { Selection, InDialogue, Voting }
    private TeamState m_currentState;

    // �̺�Ʈ 
    public event Action<CardData> OnDialogueStartRequested; // ��ȭ ���� ��û
    public event Action OnDialogueEnded;                    // ��ȭ ����

    [Header("UI �� ������ ����")]
    [SerializeField] private Transform m_teamPortraitsContainer;

    [Header("���� ����")]
    [SerializeField] private float m_hoverScale = 1.1f;
    [SerializeField] private float m_normalScale = 1.0f;
    [SerializeField] private float m_configPortraitsScale = 1f;
    [SerializeField] private Color m_hoverColor = Color.white;
    [SerializeField] private Color m_selectedColor = Color.white;
    [SerializeField] private Color m_votingSelectedColor = new Color(1f, 0.6f, 0.6f, 1f);
    [SerializeField] private Color m_darkColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private List<(NPCPortrait portrait, CardData cardData)> m_teamMembers;
    private NPCPortrait m_currentHoveredPortrait;
    private NPCPortrait m_currentDialogueSelection;

    private List<NPCPortrait> m_kickoutSelection;

    /// <summary>
    /// ī�� ������ ����Ʈ�� ������� �� �ʻ�ȭ���� �ʱ�ȭ�մϴ�.
    /// </summary>
    public void Init(List<CardData> cardDatas)
    {
        // ���� �ʻ�ȭ ����
        if (m_teamMembers != null)
        {
            foreach (var member in m_teamMembers)
            {
                if (member.portrait != null) Destroy(member.portrait.gameObject);
            }
        }

        m_teamMembers = new List<(NPCPortrait portrait, CardData cardData)>();
        m_kickoutSelection = new List<NPCPortrait>();

        for (int i = 0; i < cardDatas.Count; i++)
        {
            GameObject portraitObj = CharacterPortraitHelper.CreatePortrait(cardDatas[i], m_teamPortraitsContainer);
            if (portraitObj != null)
            {
                NPCPortrait portrait = portraitObj.GetComponent<NPCPortrait>();
                portrait.transform.SetParent(m_teamPortraitsContainer, false);
                portrait.transform.localScale = Vector3.one * m_configPortraitsScale;
                SetupPortraitEvents(portrait);
                m_teamMembers.Add((portrait, cardDatas[i]));
            }
        }

        SwitchState(TeamState.Selection); 
    }

    public void SwitchState(TeamState newState)
    {
        m_currentState = newState;
        m_currentDialogueSelection = null;
        m_kickoutSelection.Clear();
        UpdateAllPortraitAppearances();
    }

    // �̺�Ʈ �ڵ鷯��

    private void OnPortraitHoverEnter(NPCPortrait portrait)
    {
        m_currentHoveredPortrait = portrait;
        UpdateAllPortraitAppearances();
    }

    private void OnPortraitHoverExit(NPCPortrait portrait)
    {
        if (m_currentHoveredPortrait == portrait) m_currentHoveredPortrait = null;
        UpdateAllPortraitAppearances();
    }

    private void OnPortraitClick(NPCPortrait portrait)
    {
        switch (m_currentState)
        {
            case TeamState.Selection:
                m_currentDialogueSelection = portrait;
                OnDialogueStartRequested?.Invoke(GetCardDataByPortrait(portrait));
                SwitchState(TeamState.InDialogue);
                break;

            case TeamState.Voting:
                if (m_kickoutSelection.Contains(portrait))
                    m_kickoutSelection.Remove(portrait);
                else
                    m_kickoutSelection.Add(portrait);
                UpdateAllPortraitAppearances();
                break;

            case TeamState.InDialogue:
                break;
        }
    }

    private void UpdateAllPortraitAppearances()
    {
        foreach (var member in m_teamMembers)
        {
            if (member.portrait == null) continue;

            switch (m_currentState)
            {
                case TeamState.Selection:
                    if (member.portrait == m_currentHoveredPortrait)
                        SetPortraitAppearance(member.portrait, m_hoverScale, m_hoverColor);
                    else
                        SetPortraitAppearance(member.portrait, m_normalScale, m_darkColor);
                    break;

                case TeamState.InDialogue:
                    if (member.portrait == m_currentDialogueSelection)
                        SetPortraitAppearance(member.portrait, m_hoverScale, m_selectedColor);
                    else
                        SetPortraitAppearance(member.portrait, m_normalScale, m_darkColor);
                    break;

                case TeamState.Voting:
                    if (m_kickoutSelection.Contains(member.portrait))
                        SetPortraitAppearance(member.portrait, m_hoverScale, m_votingSelectedColor);
                    else if (member.portrait == m_currentHoveredPortrait)
                        SetPortraitAppearance(member.portrait, m_hoverScale, m_hoverColor);
                    else
                        SetPortraitAppearance(member.portrait, m_normalScale, m_selectedColor);
                    break;
            }
        }
    }

    private void SetPortraitAppearance(NPCPortrait portrait, float scaleMultiplier, Color color)
    {
        portrait.transform.localScale = Vector3.one * m_configPortraitsScale * scaleMultiplier;
        Image[] images = portrait.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            if (img.material != null && img.material.HasProperty("_Color"))
                img.material.SetColor("_Color", color);
        }
    }

    private void SetupPortraitEvents(NPCPortrait portrait)
    {
        EventTrigger eventTrigger = portrait.gameObject.GetComponent<EventTrigger>() ?? portrait.gameObject.AddComponent<EventTrigger>();
        eventTrigger.triggers.Clear();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => OnPortraitHoverEnter(portrait));
        eventTrigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => OnPortraitHoverExit(portrait));
        eventTrigger.triggers.Add(pointerExit);

        EventTrigger.Entry pointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        pointerClick.callback.AddListener((data) => OnPortraitClick(portrait));
        eventTrigger.triggers.Add(pointerClick);
    }

    public List<NPCPortrait> GetKickoutSelection() => m_kickoutSelection;
    public CardData GetCardDataByPortrait(NPCPortrait portrait) => m_teamMembers.Find(m => m.portrait == portrait).cardData;
}