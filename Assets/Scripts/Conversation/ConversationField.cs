using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 대화 씬의 모든 로직을 관리하고, LLM API를 통해 대화를 출력하는 총괄 클래스.
/// </summary>
public class ConversationField : MonoBehaviour
{
    private enum ConversationState { Inactive, GeneratingDialogue, DialogueTyping, DialogueFinished, Voting }
    private ConversationState m_currentState;

    [Header("핵심 컴포넌트")]
    [SerializeField] private ConversationMenu m_conversationMenu;
    [SerializeField] private ConversationTeam m_conversationTeam;

    [Header("대화창 UI")]
    [SerializeField] private GameObject m_contextBG;
    [SerializeField] private GameObject m_contextPanel;
    [SerializeField] private TMP_Text m_dialogueText;
    [SerializeField] private Button m_dialogueScreenButton; // 대화창 전체를 덮는 투명 버튼

    [Header("투표 UI")]
    [SerializeField] private Button m_kickOutButton;
    [SerializeField] private Button m_skipButton;

    [Header("테스트 설정")]
    [SerializeField] private int testCardCount;

    private Coroutine m_typingCoroutine;

    private void OnEnable()
    {
        m_conversationTeam.OnDialogueStartRequested += OnDialogueStartRequested;
        m_conversationTeam.OnDialogueEnded += CloseDialogue;

        m_dialogueScreenButton.onClick.AddListener(OnScreenClicked);
        m_kickOutButton.onClick.AddListener(OnKickOutButtonClicked);
        m_skipButton.onClick.AddListener(OnSkipButtonClicked);
    }

    private void OnDisable()
    {
        m_conversationTeam.OnDialogueStartRequested -= OnDialogueStartRequested;
        m_conversationTeam.OnDialogueEnded -= CloseDialogue;

        m_dialogueScreenButton.onClick.RemoveListener(OnScreenClicked);
        m_kickOutButton.onClick.RemoveListener(OnKickOutButtonClicked);
        m_skipButton.onClick.RemoveListener(OnSkipButtonClicked);
    }

    private void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();
        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas.FindAll(h => h.Status.CurHP > 0);

        List<CardData> useCardDatas = new List<CardData>();
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

                if (i == 1)
                    cd.BIsTraitor = true;

                cardDatas.Add(cd);

                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Face] = Random.Range(0, 3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Eye] = Random.Range(0, 3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Glasses] = 0;
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.Top] = Random.Range(0, 3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.FrontHair] = Random.Range(0, 3);
                cardDatas[i].NPCLookTable[CardData.NPCLookPartType.BackHair] = Random.Range(0, 3);

            }
        }
        else
        {
            for(int i=0; i<cardDatas.Count; i++)
            {
                if (cardDatas[i].BIsPlayer)
                {
                    continue;
                }
                useCardDatas.Add(cardDatas[i]);
            }
        }

        m_conversationTeam.Init(useCardDatas);
        m_conversationMenu.InitTeamStatus(useCardDatas);

        SwitchState(ConversationState.Inactive);
    }

    /// <summary>
    /// 상태를 전환하고 관련 UI를 제어하는 중앙 함수
    /// </summary>
    private void SwitchState(ConversationState newState)
    {
        m_currentState = newState;

        bool isDialogueActive = (newState == ConversationState.GeneratingDialogue || newState == ConversationState.DialogueTyping || newState == ConversationState.DialogueFinished);
        m_contextPanel.SetActive(isDialogueActive);
        m_contextBG.SetActive(isDialogueActive);
        m_dialogueScreenButton.gameObject.SetActive(isDialogueActive);

        bool isVotingActive = (newState == ConversationState.Voting);
        m_kickOutButton.gameObject.SetActive(isVotingActive);
        m_skipButton.gameObject.SetActive(isVotingActive);

        if (newState == ConversationState.Inactive)
            m_conversationTeam.SwitchState(ConversationTeam.TeamState.Selection);
        else if (newState == ConversationState.Voting)
            m_conversationTeam.SwitchState(ConversationTeam.TeamState.Voting);
    }

    private void OnDialogueStartRequested(CardData cardData)
    {
        SwitchState(ConversationState.GeneratingDialogue);
        m_dialogueText.text = "생각 중...";
        ReplicateInterface.Instance.TryGetSpeakText(cardData);
    }

    public void OnLLMResponseArrived()
    {
        if (m_currentState != ConversationState.GeneratingDialogue) return;

        SwitchState(ConversationState.DialogueTyping);
        if (m_typingCoroutine != null) StopCoroutine(m_typingCoroutine);
        m_typingCoroutine = StartCoroutine(TypeText(ReplicateInterface.Instance.Output));
    }

    private IEnumerator TypeText(string fullText)
    {
        m_dialogueText.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            m_dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        m_typingCoroutine = null;
        OnDialogueFinished();
    }

    private void OnDialogueFinished()
    {
        SwitchState(ConversationState.DialogueFinished);
    }

    private void OnScreenClicked()
    {
        if (m_currentState == ConversationState.DialogueTyping)
        {
            if (m_typingCoroutine != null) StopCoroutine(m_typingCoroutine);
            m_typingCoroutine = null;
            m_dialogueText.text = ReplicateInterface.Instance.Output;
            OnDialogueFinished();
        }
        else if (m_currentState == ConversationState.DialogueFinished)
        {
            SwitchState(ConversationState.Voting);
        }
    }

    public void CloseDialogue()
    {  
        m_contextPanel.SetActive(false);
        m_contextBG.SetActive(false);
        m_dialogueText.text = "";

        if (m_currentState != ConversationState.Voting)
        {
            SwitchState(ConversationState.Inactive);
        }
    }

    private void OnKickOutButtonClicked()
    {
        List<NPCPortrait> kickedOutPortraits = m_conversationTeam.GetKickoutSelection();
        if (kickedOutPortraits.Count == 0)
        {
            return;
        }
        List<CardData> allPlayerDatas = SaveDataBuffer.Instance.Data.CardDatas;
        
        foreach (NPCPortrait portrait in kickedOutPortraits)
        {
            CardData kickedOutCardData = m_conversationTeam.GetCardDataByPortrait(portrait);
            int indexToUpdate = allPlayerDatas.FindIndex(card => card.Equals(kickedOutCardData));

            if (indexToUpdate != -1)
            {
                CardData dataToUpdate = allPlayerDatas[indexToUpdate];
                dataToUpdate.Status.CurHP = 0; 
                allPlayerDatas[indexToUpdate] = dataToUpdate; 
            }
        }

        SaveData currentSave = SaveDataBuffer.Instance.Data;
        currentSave.CardDatas = allPlayerDatas;
        SaveDataBuffer.Instance.TrySetData(currentSave);
        SaveDataBuffer.Instance.TrySaveData();

        Debug.Log("퇴출된 동료 정보가 저장되었습니다.");

        EndScene();
    }

    private void OnSkipButtonClicked()
    {
        Debug.Log("퇴출 없이 다음으로 넘어갑니다.");
        EndScene();
    }

    private void EndScene()
    {
        Debug.Log("대화 씬을 종료하고 다음 씬으로 넘어갑니다.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
    }
}