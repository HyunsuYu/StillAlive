using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ��ȭ ������ ���� �ʻ�ȭ���� ��ȣ�ۿ�(ȣ��, Ŭ��, ����)�� �����ϴ� Ŭ����.
/// �߾� ���� �Լ�(UpdateAllPortraitAppearances)�� ���� ���¸� ��Ȯ�ϰ� �����մϴ�.
/// </summary>
public class ConversationTeam : MonoBehaviour
{
    [Header("UI �� ������ ����")]
    [Tooltip("�ʻ�ȭ���� ������ �θ� Canvas")]
    [SerializeField] private Transform m_teamPortraitsContainer;
    [SerializeField] private float m_portraitSpacing = 200f;

    [Tooltip("�ʻ�ȭ ���� ������ ���� ��ü ȭ�� ���� ��ư")]
    [SerializeField] private Button m_deselectButton;

    [Header("���� ����")]
    [Tooltip("���콺�� �÷��� ���� ũ�� ����")]
    [SerializeField] private float m_hoverScale = 1.1f;
    [Tooltip("���� ũ�� ����")]
    [SerializeField] private float m_normalScale = 1.0f;
    [Tooltip("���콺�� �÷��� ���� Tint ����")]
    [SerializeField] private Color m_hoverColor = Color.white;
    [Tooltip("���õǾ��� ���� Tint ����")]
    [SerializeField] private Color m_normalColor = Color.white;
    [Tooltip("����/ȣ������ �ʾ��� ���� Tint ����")]
    [SerializeField] private Color m_darkColor = new Color(0.5f, 0.5f, 0.5f, 1f);


    [SerializeField] private float m_configPortraitsScale = 6f; 

    // ���� ���� ����
    private List<(NPCPortrait portrait, CardData cardData)> m_teamMembers;

    private NPCPortrait m_currentHoveredPortrait;
    private NPCPortrait m_currentSelectedPortrait;

    public event Action<CardData> OnPortraitSelected;
    public event Action OnPortraitDeselected;

    /// <summary>
    /// ī�� ������ ����Ʈ�� ������� �� �ʻ�ȭ���� �ʱ�ȭ�մϴ�.
    /// </summary>
    public void Init(List<CardData> cardDatas)
    {
        int memberCount = cardDatas.Count;
        m_teamMembers = new List<(NPCPortrait portrait, CardData cardData)>();
        
        // ī�� �����͸�ŭ �ʻ�ȭ ����
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

        // ���� ���� ��ư �̺�Ʈ ���
        if (m_deselectButton != null)
        {
            m_deselectButton.onClick.AddListener(OnDeselect);
        }

        // ��� �ʻ�ȭ�� �ʱ� ����(��Ӱ�)�� ������Ʈ
        UpdateAllPortraitAppearances();

    }

    // �̺�Ʈ �ڵ鷯: ���� ���� ���� �� ��ü ������Ʈ ȣ��

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
        // �̹� ���õ� ���� �ٽ� Ŭ���ϸ� ���� ����, �ƴϸ� ���� ����
        m_currentSelectedPortrait = (m_currentSelectedPortrait == portrait) ? null : portrait;
        UpdateAllPortraitAppearances();

        if (m_currentSelectedPortrait != null)
        {
            // ���õ� ĳ������ CardData�� �Բ� '���õ�' �̺�Ʈ�� ���
            OnPortraitSelected?.Invoke(GetSelectedCardData());
        }
        else
        {
            // '���� ������' �̺�Ʈ�� ���
            OnPortraitDeselected?.Invoke();
        }
    }

    /// <summary>
    /// ��� Ŭ�� �� ��� ������ �����մϴ�.
    /// </summary>
    private void OnDeselect()
    {
        m_currentSelectedPortrait = null;
        UpdateAllPortraitAppearances();

        OnPortraitDeselected?.Invoke();
    }

    /// <summary>
    /// ��� �ʻ�ȭ�� ���� ����(����, ȣ��, �⺻)�� �������� �ð��� ǥ���� �� ���� ������Ʈ�մϴ�.
    /// </summary>
    private void UpdateAllPortraitAppearances()
    {
        foreach (var member in m_teamMembers)
        {
            if (member.portrait == null) continue;

            if (member.portrait == m_currentSelectedPortrait)
            {
                // 1. ���õ� ��Ʈ����Ʈ: ũ�� ���
                SetPortraitAppearance(member.portrait, m_hoverScale, m_normalColor);
            }
            else if (member.portrait == m_currentHoveredPortrait)
            {
                // 2. ȣ���� ��Ʈ����Ʈ: ũ�� ��� (ȣ�� ����)
                SetPortraitAppearance(member.portrait, m_hoverScale, m_hoverColor);
            }
            else
            {
                // 3. �� �� �ƴ� �⺻ ����: �۰� ��Ӱ�
                SetPortraitAppearance(member.portrait, m_normalScale, m_darkColor);
            }
        }
    }

    /// <summary>
    /// ������ ��Ʈ����Ʈ�� ũ��� ������ �����ϴ� Helper �Լ�.
    /// </summary>
    private void SetPortraitAppearance(NPCPortrait portrait, float scale, Color color)
    {
        portrait.transform.localScale = Vector3.one * scale * m_configPortraitsScale;

        // NPCPortrait�� ��� Image �ڽĵ��� ��ȸ�ϸ� Tint ����(_Color) ����
        Image[] images = portrait.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            // ��Ƽ������ �������� �����Ǿ�����, _Color ������Ƽ�� �ִ��� Ȯ��
            if (img.material != null && img.material.HasProperty("_Color"))
            {
                img.material.SetColor("_Color", color);
            }
        }
    }

    // --- �̺�Ʈ Ʈ���� ���� �� ������ ��ȸ ---

    private void SetupPortraitEvents(NPCPortrait portrait)
    {
        EventTrigger eventTrigger = portrait.gameObject.GetComponent<EventTrigger>() ?? portrait.gameObject.AddComponent<EventTrigger>();
        eventTrigger.triggers.Clear();

        // Pointer Enter (ȣ�� ����)
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => OnPortraitHoverEnter(portrait));
        eventTrigger.triggers.Add(pointerEnter);

        // Pointer Exit (ȣ�� ����)
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => OnPortraitHoverExit(portrait));
        eventTrigger.triggers.Add(pointerExit);

        // Pointer Click (Ŭ��)
        EventTrigger.Entry pointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        pointerClick.callback.AddListener((data) => OnPortraitClick(portrait));
        eventTrigger.triggers.Add(pointerClick);
    }

    public CardData GetSelectedCardData()
    {
        if (m_currentSelectedPortrait != null)
        {
            var selectedMember = m_teamMembers.Find(member => member.portrait == m_currentSelectedPortrait);
            // CardData�� struct�̹Ƿ� null üũ ��� portrait ���� ���η� Ȯ��
            if (selectedMember.portrait != null)
            {
                return selectedMember.cardData;
            }
        }
        return default; // CardData�� struct�̹Ƿ� default�� ��ȯ
    }
}