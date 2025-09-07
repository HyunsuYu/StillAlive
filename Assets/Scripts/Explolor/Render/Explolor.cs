using CommonUtilLib.ThreadSafe;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Explolor : SingleTonForGameObject<Explolor>
{
    [Header("Windows")]
    [SerializeField] private GameObject SelecterWindow; // 선택 창
    [SerializeField] private GameObject CheckWindow; // 확인 창
    [SerializeField] private GameObject EventWindow; // 이벤트 창
    [SerializeField] private GameObject ItemWindow;
    [SerializeField] private GameObject ColleagueWindow;

    [Header("Objects")]
    [SerializeField] private GameObject[] ItemBox;
    [SerializeField] private GameObject[] ColleagueBox;
    [SerializeField] private GameObject[] MyTeamBox;

    [Header("Text")]
    [SerializeField] private TMP_Text m_text_DPlusDay;
    [SerializeField] private TMP_Text Durability;
    [SerializeField] private TMP_Text Wreath;
    [SerializeField] private TMP_Text DurabilityNum;
    [SerializeField] private TMP_Text WreathNum;
    [SerializeField] private TMP_Text Explain;


    [SerializeField] private ItemTypes m_itemType;

    public List<CardData> ColleaugueCards;

    public List<int> ItemId;

    public enum ExplolorState
    {
        None = 0,
        Selecter = 1,
        ItemEvent = 2,
        ColleagueEvent = 3
    }


    public ExplolorState exState = ExplolorState.None;
    static bool curCompleteWork = true;

    void Awake()
    {
        SetInstance(this);
    }

    public void OnItemButtonClick() //아이템 탐사 버튼을 눌렀을 때
    {
        exState = ExplolorState.ItemEvent;
        ItemCatch();
        CheckWindow.SetActive(false); // 확인 창 비활성화
        SelecterWindow.SetActive(false); // 선택 창 비활성화
        exState = ExplolorState.None; // 선택된 버튼 초기화
    }

    public void OnColleagueButtonClick() //동료 탐사 버튼을 눌렀을 때
    {
        exState = ExplolorState.ColleagueEvent;
        colleagueCatch();
        CheckWindow.SetActive(false); // 확인 창 비활성화
        SelecterWindow.SetActive(false); // 선택 창 비활성화
        exState = ExplolorState.None; // 선택된 버튼 초기화
    }


    public void OnCancelButtonClick()  //취소 버튼을 눌렀을 때
    {
        CheckWindow.SetActive(false); // 확인 창 비활성화
        exState = ExplolorState.None; // 선택된 버튼 초기화
    }

    public void ItemCatch() //아이템 발견 함수
    {
        if(exState != ExplolorState.ItemEvent)
        {
            Debug.LogWarning("ItemCatch 호출 오류: 현재 상태가 ItemEvent가 아닙니다.");

            return;
        }
        int itemCount = ProbabilityUtillity.GetCount(SaveDataBuffer.Instance.Data.DPlusDay);

        if (itemCount > 0)
        {
            Debug.Log($"아이템 {itemCount}개 발견!");
            for (int i = 0; i < itemCount; i++)
            {
                int randomItemID = UnityEngine.Random.Range(0, ItemTypes.ItemCount);
                if(randomItemID == 0 && SaveDataBuffer.Instance.Data.ItemAmountTable[0] > 0)
                {
                    randomItemID = UnityEngine.Random.Range(1, ItemTypes.ItemCount);
                }

                ItemId.Add(randomItemID);
                
                ItemBox[i].GetComponent<Image>().sprite = m_itemType.ItemDatas[randomItemID].ItemSprite;
                ItemBox[i].SetActive(true);
            }

            EventWindow.SetActive(true); // 이벤트 창 활성화
            ItemWindow.SetActive(true);

        }
        else
        {
            Debug.Log("아이템 발견 못함");
        }
    }

    public void colleagueCatch() //동료 발견 함수
    {
        if (exState != ExplolorState.ColleagueEvent)
        {
            Debug.LogWarning("colleagueCatch 호출 오류: 현재 상태가 ColleagueEvent가 아닙니다.");
            return;
        }
        int ColleagueCount = ProbabilityUtillity.GetCount(SaveDataBuffer.Instance.Data.DPlusDay);
        SpawnCards(ColleagueCount);
    }

    private void SpawnCards(int count)
    {
        // 모든 카드 UI를 미리 비활성화
        foreach (var box in ColleagueBox) box.SetActive(false);
        foreach (var box in MyTeamBox) box.SetActive(false);

        if (count > 0)
        {
            Debug.Log($"동료 {count}명 발견!");
            // 1. 새로 발견한 동료 (임시 데이터) 표시
            for (int i = 0; i < count; i++)
            {
                CardData newColleague = new CardData();
                newColleague.Status = CardData.DefaultStatus();
                newColleague.NPCLookTable = new Dictionary<CardData.NPCLookPartType, int>();
                
                // To-Do: 외형 데이터도 여기서 생성하면 좋습니다.
                // newColleague.NPCLookTable = ...;

                GameObject colleagueGO = ColleagueBox[i];
                GameObject cardObj = Resources.Load<GameObject>("Prefabs/BattleScene/BattleCard_UI");
                BattleCard card  = Instantiate(cardObj.gameObject, colleagueGO.transform)?.GetComponent<BattleCard>();
                if (card != null)
                {
                    card.Init(newColleague, true);
                    colleagueGO.SetActive(true);
                    card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // 위치 초기화
                }
            }
        }
        else
        {
            Debug.Log("동료 발견 못함");
        }

        // 2. 현재 내 팀 (영구 데이터) 표시


        var currentTeam = SaveDataInterface.GetAliveCardInfos(); // 살아있는 카드 데이터들을 가져옴
        for (int j = 0; j < currentTeam.Count && j < MyTeamBox.Length; j++)
        {
            GameObject myTeamGO = MyTeamBox[j];
            BattleCard card = myTeamGO.GetComponent<BattleCard>();
            if (card != null)
            {
                card.Init(currentTeam[j],true);
                myTeamGO.SetActive(true);
            }
        }

        EventWindow.SetActive(true);
        ColleagueWindow.SetActive(true);
        exState = ExplolorState.None; // 상태 초기화
    }

    internal void Render()
    {
        m_text_DPlusDay.text = "D+" + SaveDataBuffer.Instance.Data.DPlusDay.ToString();
        if (curCompleteWork)
        {
            CheckWindow.SetActive(false); // 시작 시 확인 창 비활성화
            SelecterWindow.SetActive(true); // 시작 시 이벤트 창 활성화
            EventWindow.SetActive(false); // 시작 시 이벤트 창 비활성화
            ItemWindow.SetActive(false);
            ColleagueWindow.SetActive(false);
            for (int i = 0; i < ItemBox.Length; i++)
            {
                ItemBox[i].SetActive(false);
            }
            for (int i = 0; i < ColleagueBox.Length; i++)
            {
                ColleagueBox[i].SetActive(false);
            }
            for (int i = 0; i < MyTeamBox.Length; i++)
            {
                MyTeamBox[i].SetActive(false);
            }
            exState = ExplolorState.None; // 상태 초기화

            Durability.GameObject().SetActive(false);
            Wreath.GameObject().SetActive(false);
            DurabilityNum.GameObject().SetActive(false);
            WreathNum.GameObject().SetActive(false);
            Explain.gameObject.SetActive(false);

        }
    }


    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}

// 아이템 드롭 확률을 계산하는 유틸리티 클래스
public static class ProbabilityUtillity
{
    // --- 확률 모델 설정값 ---

    // 확률 변화가 일어나는 총 기간을 90일로 수정
    private const float PROBABILITY_CHANGE_DURATION = 90.0f;

    // 시작 (Day 0) 지점의 경계선 Y값
    private const float startBoundary_1_0 = 0.05f; // 0개와 1개 경계
    private const float startBoundary_2_1 = 0.2f; // 1개와 2개 경계
    private const float startBoundary_3_2 = 0.4f; // 2개와 3개 경계

    // 마지막 (Day 90) 지점의 경계선 Y값
    private const float endBoundary_1_0 = 0.55f;
    private const float endBoundary_2_1 = 0.8f;
    private const float endBoundary_3_2 = 0.95f;


    // 특정 날짜를 기준으로 확률을 정한다.
    public static int GetCount(int currentDay)
    {
        // 1. 현재 날짜가 전체 기간 중 어디쯤인지 계산 (0.0 ~ 1.0 사이의 값)
        // currentDay가 90을 넘으면 progress는 1.0으로 고정됨
        float progress = Mathf.Clamp01(currentDay / PROBABILITY_CHANGE_DURATION);

        // 2. Lerp 함수로 '오늘'의 경계선 Y값들을 계산
        float todayBoundary_3_2 = Mathf.Lerp(startBoundary_3_2, endBoundary_3_2, progress);
        float todayBoundary_2_1 = Mathf.Lerp(startBoundary_2_1, endBoundary_2_1, progress);
        float todayBoundary_1_0 = Mathf.Lerp(startBoundary_1_0, endBoundary_1_0, progress);

        // 3. 0.0과 1.0 사이의 난수를 생성
        float randomValue = UnityEngine.Random.value;

        // 4. 난수가 어느 영역에 속하는지 위에서부터 확인
        if (randomValue >= todayBoundary_3_2)
        {
            return 3;
        }
        else if (randomValue >= todayBoundary_2_1)
        {
            return 2;
        }
        else if (randomValue >= todayBoundary_1_0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
