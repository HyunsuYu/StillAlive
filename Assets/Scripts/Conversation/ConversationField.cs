using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 대화관련된 씬 로직을 관리, LLM API를 가져와, 대화를 출력하는 클래스
/// </summary>
public class ConversationField : MonoBehaviour
{
    [SerializeField] private ConversationMenu m_conversationMenu;
    [SerializeField] private ConversationTeam m_conversationTeam;

    [Header("대화창 UI")]
    [SerializeField] private GameObject m_contextPanel; // 대화창 패널
    [SerializeField] private TMP_Text m_dialogueText;   // 대사가 표시될 Text
    [SerializeField] private Button m_closeButton;      // 대화창 닫기 버튼

    [SerializeField] private Button m_kickOutButton;    // 팀 퇴출 버튼
    [SerializeField] private Button m_skipButton;       // 팀 퇴출 건너뛰기 버튼


    [SerializeField] private int testCardCount;

    List<CardData> cardDatas;

    [SerializeField] private TMP_Text testTMP;

    private void OnEnable()
    {
        // ConversationTeam에서 방송하는 이벤트를 구독
        m_conversationTeam.OnPortraitSelected += HandlePortraitSelected;
        m_conversationTeam.OnPortraitDeselected += CloseDialogue;

        if (m_closeButton != null) m_closeButton.onClick.AddListener(CloseDialogue);
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화될 때 안전하게 구독 취소
        m_conversationTeam.OnPortraitSelected -= HandlePortraitSelected;
        m_conversationTeam.OnPortraitDeselected -= CloseDialogue;

        if (m_closeButton != null) m_closeButton.onClick.RemoveListener(CloseDialogue);
    }

    private void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();
    
       cardDatas = SaveDataBuffer.Instance.Data.CardDatas;

        if (cardDatas.Count == 0)
        {
            for (int i = 0; i < testCardCount; i++)
            {
                // 테스트
                CardData cd = new CardData();
                cd.NPCLookTable = new Dictionary<CardData.NPCLookPartType, int>();
                cd.ColorPalleteIndex = 0;
                cd.Status = CardData.DefaultStatus();
                cd.Status.MaxHP += 1;
                cd.Status.CurHP += 1;

                if(i == 1)
                    cd.BIsTraitor = true;

                cardDatas.Add(cd);

                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Face] = Random.Range(0, 3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Eye] = Random.Range(0, 3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Glasses] = 0;
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Top] = Random.Range(0,3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.FrontHair] = Random.Range(0,3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.BackHair] = Random.Range(0,3);

            }
        }

        m_conversationTeam.Init(cardDatas);
        m_conversationMenu.InitTeamStatus(cardDatas);

        m_contextPanel.SetActive(false);
    }

    private void HandlePortraitSelected(CardData cardData)
    {
        m_contextPanel.SetActive(true);
        m_dialogueText.text = "생각 중..."; 
        ReplicateInterface.Instance.TryGetSpeakText(in cardData);
    }
    
    // LLM에서부터 텍스트를 받아, 해당 인터페이스에 이벤트로 구독함
    public void OutputTextByCharacter()
    {
        m_dialogueText.text = ReplicateInterface.Instance.Output;
    }

    public void CloseDialogue()
    {
        m_contextPanel.SetActive(false);
        m_dialogueText.text = ""; 
    }

    public void TestSample()
    {
        ReplicateInterface.Instance.TryGetSpeakText(cardDatas[0]);
        Debug.Log("Clicked");
    }

    public void TestSampleResult()
    {
        testTMP.text = ReplicateInterface.Instance.Output;
    }
}